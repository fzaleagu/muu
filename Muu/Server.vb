﻿Imports System.Collections.ObjectModel
Imports System.IO
Imports System.Net
Imports System.Net.Sockets

Public Class Server
    Private files As Collection(Of File)
    Private listener As TcpListener
    Private lock As New Object

    Public Delegate Sub ServerEnabledDelegate()
    Public ServerEnabled As ServerEnabledDelegate

    Public Delegate Sub ServerDisabledDelegate()
    Public ServerDisabled As ServerDisabledDelegate

    Public Delegate Sub ServerLoggerDelegate(message As String)
    Public ServerLogger As ServerLoggerDelegate

    Public Sub New(files As Collection(Of File))
        Me.files = files
    End Sub

    Public Sub Enable(port As Integer)
        SyncLock lock
            If Not listener Is Nothing Then
                Throw New InvalidOperationException("Server is already enabled")
            End If
            Try
                listener = New TcpListener(IPAddress.IPv6Any, port)
                listener.Server.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, False)
                listener.Start()
            Catch ex As Exception
                listener = Nothing
                Throw New InvalidOperationException("Failed to enable server", ex)
            End Try
        End SyncLock

        AcceptClients()

        If Not ServerEnabled Is Nothing Then
            Log("Server enabled")
            ServerEnabled()
        End If
    End Sub

    Public Sub Disable()
        SyncLock lock
            If listener Is Nothing Then
                Throw New InvalidOperationException("Server is already disabled")
            End If
            listener.Stop()
            listener = Nothing
        End SyncLock

        If Not ServerDisabled Is Nothing Then
            Log("Server disabled")
            ServerDisabled()
        End If
    End Sub

    Private Async Sub AcceptClients()
        While listener IsNot Nothing
            Dim handler As Socket = Nothing
            Try
                handler = Await listener.AcceptSocketAsync()
            Catch ex As ObjectDisposedException
            End Try
            If handler IsNot Nothing Then
                Dim state As New State(handler)
                HandleClient(state)
            End If
        End While
    End Sub

    Private Async Sub HandleClient(state As State)
        Dim request = Await state.ReceiveRequest()
        If request Is Nothing Then
            Return
        End If

        Await HandleRequest(state, request)
        state.Close()
    End Sub

    Private Async Function HandleRequest(state As State, request As Request) As Task
        Dim file = GetFile(request.FileName)
        If file Is Nothing Then
            If request.FileName = "" Then
                Await SendListing(state)
            Else
                Await Send404(state, request)
            End If
        Else
            Log(String.Format("Sending {0}", file.FileName))
            Using Stream = New FileStream(file.FilePath, FileMode.Open)
                Dim header = New Header(200)
                header.ContentLength = file.FileSize
                header.ContentType = file.ContentType
                Await state.SendAsync(header.GetData())

                Dim buffer() = New [Byte](4095) {}
                Dim read As Integer
                Do
                    read = Await Stream.ReadAsync(buffer, 0, buffer.Length)
                    If read > 0 Then
                        Await state.SendAsync(buffer, read)
                    End If
                Loop While read > 0
            End Using
        End If
    End Function

    Private Async Function Send404(state As State, request As Request) As Task
        Dim header = New Header(404)
        header.ContentType = "text/plain"
        Await state.SendAsync(header.GetData())

        Dim body = "404 Not Found"
        Await state.SendAsync(Text.Encoding.ASCII.GetBytes(body))
    End Function

    Private Async Function SendListing(state As State) As Task
        Dim header = New Header(200)
        header.ContentType = "text/plain"
        Await state.SendAsync(header.GetData())

        For Each file In files
            Dim line = file.FileName + ControlChars.CrLf
            Await state.SendAsync(Text.Encoding.ASCII.GetBytes(line))
        Next
    End Function

    Private Function MakeDebugResponse(request As Request) As Byte()
        Dim header = New Header(200)
        header.ContentType = "text/plain"
        Dim headerData = header.GetData()
        Dim bodyData() = request.Data
        Dim response() As Byte = New [Byte](headerData.Length + bodyData.Length - 1) {}
        Buffer.BlockCopy(headerData, 0, response, 0, headerData.Length)
        Buffer.BlockCopy(bodyData, 0, response, headerData.Length, bodyData.Length)
        Return response
    End Function

    Private Function GetFile(fileName As String) As File
        For Each file In files
            If file.FileName().Equals(fileName) Then
                Return file
            End If
        Next
        Return Nothing
    End Function

    Private Sub Log(message As String)
        If ServerLogger IsNot Nothing Then
            ServerLogger(message)
        End If
    End Sub
End Class

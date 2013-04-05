Imports System.Collections.ObjectModel
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

    Public Sub New(files As Collection(Of File))
        Me.files = files
    End Sub

    Public Sub Enable(port As Integer)
        SyncLock lock
            If Not listener Is Nothing Then
                Throw New InvalidOperationException("Server is already enabled")
            End If
            listener = New TcpListener(IPAddress.IPv6Any, port)
            listener.Server.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, False)
            listener.Start()
        End SyncLock

        AcceptMore()

        If Not ServerEnabled Is Nothing Then
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
            ServerDisabled()
        End If
    End Sub

    Private Sub AcceptMore()
        SyncLock lock
            If Not listener Is Nothing Then
                listener.BeginAcceptSocket(New AsyncCallback(AddressOf AcceptCallback), Nothing)
            End If
        End SyncLock
    End Sub

    Private Sub HandleRequest(state As State, request As Request)
        Dim file = GetFile(request.GetFileName())
        If file Is Nothing Then
            state.Close()
        Else
            Dim stream = New FileStream(file.Path, FileMode.Open)
            Dim header =
                "HTTP/1.1 200 OK" + ControlChars.CrLf +
                "Content-Type: text/plain" + ControlChars.CrLf +
                ControlChars.CrLf
            Dim headerData() = Text.Encoding.ASCII.GetBytes(header)
            state.Send(headerData,
                       Sub()
                           SendResponse(state, stream)
                       End Sub)
        End If
    End Sub

    Private Async Sub SendResponse(state As State, stream As FileStream)
        Dim buffer() = New [Byte](4095) {}
        Dim read = Await stream.ReadAsync(buffer, 0, buffer.Length)
        If read > 0 Then
            state.Send(buffer, read,
                       Sub()
                           SendResponse(state, stream)
                       End Sub)
        Else
            stream.Dispose()
            state.Close()
        End If
    End Sub

    Private Sub AcceptCallback(ar As IAsyncResult)
        Dim handler As Socket

        SyncLock lock
            If listener Is Nothing Then
                Return
            End If
            handler = listener.EndAcceptSocket(ar)
        End SyncLock

        AcceptMore()

        Dim state As New State(handler)
        state.ReceiveRequest(
            Sub(request As Request)
                HandleRequest(state, request)
            End Sub)
    End Sub

    Private Function MakeDebugResponse(request As Request) As Byte()
        Dim header =
            "HTTP/1.1 200 OK" + ControlChars.CrLf +
            "Content-Type: text/plain" + ControlChars.CrLf +
            ControlChars.CrLf
        Dim headerData() = Text.Encoding.ASCII.GetBytes(header)
        Dim bodyData() = request.GetData()
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
End Class

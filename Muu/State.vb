Imports System.Net
Imports System.Net.Sockets

Public Class State
    Private handler As Socket
    Private readBuffer() As Byte = New [Byte](1023) {}
    Private requestBuilder As RequestBuilder = New RequestBuilder()
    Private stream As NetworkStream

    Public Sub New(handler As Socket)
        Me.handler = handler
        stream = New NetworkStream(handler)
    End Sub

    Public Async Function ReceiveRequest() As Task(Of Request)
        Try
            While True
                Dim bytesRead = Await stream.ReadAsync(readBuffer, 0, readBuffer.Count)
                If bytesRead > 0 Then
                    requestBuilder.AppendData(readBuffer, bytesRead)
                    If requestBuilder.IsComplete() Then
                        Return requestBuilder.GetRequest()
                    End If
                Else
                    Return Nothing
                End If
            End While
        Catch ex As SocketException
            Close()
        End Try
        Return Nothing
    End Function

    Public Sub Send(data() As Byte, size As Integer, callback As Action)
        handler.BeginSend(data, 0, size, 0, New AsyncCallback(AddressOf SendCallback), callback)
    End Sub

    Public Sub Send(data() As Byte, callback As Action)
        Send(data, data.Length, callback)
    End Sub

    Public Sub Close()
        handler.Shutdown(SocketShutdown.Both)
        handler.Close()
        handler = Nothing
    End Sub

    Private Sub SendCallback(ar As IAsyncResult)
        Dim bytesSent = handler.EndReceive(ar)
        Dim callback As Action = ar.AsyncState
        If callback <> Nothing Then
            callback()
        End If
    End Sub
End Class

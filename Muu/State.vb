Imports System.Net
Imports System.Net.Sockets

Public Class State
    Private handler As Socket
    Private readBuffer() As Byte = New [Byte](1023) {}
    Private requestBuilder As RequestBuilder = New RequestBuilder()

    Public Sub New(handler As Socket)
        Me.handler = handler
    End Sub

    Public Sub ReceiveRequest(callback As Action(Of Request))
        ReadMore(callback)
    End Sub

    Public Sub Send(data() As Byte, callback As Action)
        handler.BeginSend(data, 0, data.Length, 0, New AsyncCallback(AddressOf SendCallback), callback)
    End Sub

    Public Sub Close()
        handler.Shutdown(SocketShutdown.Both)
        handler.Close()
        handler = Nothing
    End Sub

    Private Sub ReadMore(callback As Action(Of Request))
        handler.BeginReceive(readBuffer, 0, readBuffer.Length, 0, New AsyncCallback(AddressOf ReadCallback), callback)
    End Sub

    Private Sub ReadCallback(ar As IAsyncResult)
        Dim bytesRead = handler.EndReceive(ar)
        Dim callback As Action(Of Request) = ar.AsyncState
        If bytesRead > 0 Then
            requestBuilder.AppendData(readBuffer, bytesRead)
            If requestBuilder.IsComplete() Then
                Dim request = requestBuilder.GetRequest()
                If callback <> Nothing Then
                    callback(request)
                End If
            Else
                ReadMore(callback)
            End If
        End If
    End Sub

    Private Sub SendCallback(ar As IAsyncResult)
        Dim bytesSent = handler.EndReceive(ar)
        Dim callback As Action = ar.AsyncState
        If callback <> Nothing Then
            callback()
        End If
    End Sub
End Class

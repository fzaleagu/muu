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

    Public Async Function SendAsync(data() As Byte, size As Integer) As Task
        Await stream.WriteAsync(data, 0, size)
    End Function

    Public Sub Close()
        stream.Dispose()
        handler.Shutdown(SocketShutdown.Both)
        handler.Close()
        handler = Nothing
    End Sub
End Class

Imports System.Net
Imports System.Net.Sockets

Public Class Server
    Private listener As Socket
    Private lock As New Object

    Public Delegate Sub ServerEnabledDelegate()
    Public ServerEnabled As ServerEnabledDelegate

    Public Delegate Sub ServerDisabledDelegate()
    Public ServerDisabled As ServerDisabledDelegate

    Public Sub Enable(port As Integer)
        SyncLock lock
            If Not listener Is Nothing Then
                Throw New InvalidOperationException("Server is already enabled")
            End If
            Dim localEndPoint As New IPEndPoint(IPAddress.IPv6Any, port)
            listener = New Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp)
            listener.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, 0)
            listener.Bind(localEndPoint)
            listener.Listen(100)
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
            listener.Close()
            listener = Nothing
        End SyncLock

        If Not ServerDisabled Is Nothing Then
            ServerDisabled()
        End If
    End Sub

    Private Sub AcceptMore()
        SyncLock lock
            If Not listener Is Nothing Then
                listener.BeginAccept(New AsyncCallback(AddressOf AcceptCallback), Nothing)
            End If
        End SyncLock
    End Sub

    Private Sub HandleRequest(state As State, request As Request)
        state.Send(MakeDebugResponse(request),
                   Sub()
                       state.Close()
                   End Sub)
    End Sub

    Private Sub AcceptCallback(ar As IAsyncResult)
        Dim handler As Socket

        SyncLock lock
            If listener Is Nothing Then
                Return
            End If
            handler = listener.EndAccept(ar)
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
End Class

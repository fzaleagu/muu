Public Class RequestBuilder
    Private buffer() As Byte = New [Byte](-1) {}

    Public Sub AppendData(buffer() As Byte, Optional length As Integer = -1)
        Dim sourceLength = If(length = -1, buffer.Length, length)
        Dim sourceIndex = 0
        Dim targetLength = Me.buffer.Length + sourceLength
        Dim targetIndex = Me.buffer.Length
        ReDim Preserve Me.buffer(targetLength - 1)
        Array.Copy(buffer, sourceIndex, Me.buffer, targetIndex, sourceLength)
    End Sub

    Public Function IsComplete()
        If (buffer.Length < 4) Then
            Return False
        End If
        For index = 0 To buffer.Length - 4
            If buffer(index) = 13 And buffer(index + 1) = 10 And buffer(index + 2) = 13 And buffer(index + 3) = 10 Then
                Return True
            End If
        Next
        Return False
    End Function

    Public Function GetRequest() As Request
        Return New Request(buffer)
    End Function
End Class

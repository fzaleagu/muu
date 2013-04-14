Public Class Header
    Private status As String

    Public Property ContentLength As Long?
    Public Property ContentType As String

    Public Sub New(statusCode As Integer)
        Select Case statusCode
            Case 200
                status = String.Format("{0} OK", statusCode)
            Case 404
                status = String.Format("{0} Not Found", statusCode)
            Case Else
                Throw New ArgumentException("Invalid status code")
        End Select
    End Sub

    Public Function GetData() As Byte()
        Dim header = "HTTP/1.1 " + status + ControlChars.CrLf

        If ContentLength IsNot Nothing Then
            header += "Content-Length: " + ContentLength.Value.ToString + ControlChars.CrLf
        End If

        If ContentType IsNot Nothing Then
            header += "Content-Type: " + ContentType + ControlChars.CrLf
        End If

        header += ControlChars.CrLf
        Return Text.Encoding.ASCII.GetBytes(header)
    End Function
End Class

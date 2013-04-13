Public Class File
    Private _Path As String
    Private _ContentType As String

    Sub New(path As String)
        _Path = path

        Select Case IO.Path.GetExtension(_Path)
            Case ".txt"
                _ContentType = "text/plain"
            Case Else
                _ContentType = "application/octet-stream"
        End Select
    End Sub

    Public ReadOnly Property Path As String
        Get
            Return _Path
        End Get
    End Property

    Public ReadOnly Property FileName
        Get
            Return System.IO.Path.GetFileName(Path)
        End Get
    End Property

    Public ReadOnly Property ContentType
        Get
            Return _ContentType
        End Get
    End Property

    Public Overloads Overrides Function Equals(obj As Object) As Boolean
        If obj Is Nothing OrElse Not Me.GetType() Is obj.GetType() Then
            Return False
        End If

        Dim otherFile As File = CType(obj, File)
        Return Me.FileName().Equals(otherFile.FileName())
    End Function
End Class

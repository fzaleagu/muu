Public Class File
    Public Property Path As String

    Public ReadOnly Property FileName
        Get
            Return System.IO.Path.GetFileName(Path)
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

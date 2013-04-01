Public Class File
    Public Property Path As String

    Public ReadOnly Property FileName
        Get
            Return System.IO.Path.GetFileName(Path)
        End Get
    End Property
End Class

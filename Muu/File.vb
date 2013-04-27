Imports System.IO

Public Class File
    Private _FilePath As String
    Private _ContentType As String

    Sub New(filePath As String)
        _FilePath = filePath

        Select Case Path.GetExtension(_FilePath)
            Case ".gif"
                _ContentType = "image/gif"
            Case ".jpg"
                _ContentType = "image/jpeg"
            Case ".png"
                _ContentType = "image/png"
            Case ".txt"
                _ContentType = "text/plain"
            Case Else
                _ContentType = "application/octet-stream"
        End Select
    End Sub

    Public ReadOnly Property FilePath As String
        Get
            Return _FilePath
        End Get
    End Property

    Public ReadOnly Property FileName
        Get
            Return Path.GetFileName(FilePath)
        End Get
    End Property

    Public ReadOnly Property FileSize As Long?
        Get
            Try
                Dim fileInfo As New FileInfo(_FilePath)
                Return fileInfo.Length
            Catch ex As FileNotFoundException
                Return Nothing
            End Try
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

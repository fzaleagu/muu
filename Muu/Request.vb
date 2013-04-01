Imports System.Text

Public Class Request
    Private data As Byte()
    Private dataIndex As New List(Of Tuple(Of Integer, Integer))
    Private requestLine As String
    Private headers As New Dictionary(Of String, String)
    Private method As String
    Private requestURI As String
    Private httpVersion As String

    Public Sub New(data As Byte())
        Me.data = data
        MakeIndex()
        MakeHeaders()
        ParseRequest()
    End Sub

    Public Function GetData() As Byte()
        Return data
    End Function

    Public Function GetMethod() As String
        Return method
    End Function

    Public Function GetRequestURI() As String
        Return requestURI
    End Function

    Public Function GetHttpVersion() As String
        Return httpVersion
    End Function

    Public Function GetFileName() As String
        Dim parts = requestURI.Split(New [Char]() {"/"c}, StringSplitOptions.RemoveEmptyEntries)
        If parts.Count = 0 Then
            Return Nothing
        End If
        Return parts(parts.GetUpperBound(0))
    End Function

    Private Sub MakeIndex()
        Dim begin = 0
        For index = 0 To data.Length - 2
            If data(index) = 13 And data(index + 1) = 10 Then
                Dim length = index - begin
                If length = 0 Then
                    Return
                End If
                dataIndex.Add(New Tuple(Of Integer, Integer)(begin, length))
                begin = index + 2
            End If
        Next
    End Sub

    Private Sub MakeHeaders()
        For Each item In dataIndex
            Dim line = Encoding.ASCII.GetString(data, item.Item1, item.Item2)
            If item.Item1 = 0 Then
                requestLine = line
            Else
                Dim parts() As String = line.Split(New Char() {":"}, 2)
                If parts.Length = 2 Then
                    headers.Add(parts(0).Trim(), parts(1).Trim())
                End If
            End If
        Next
    End Sub

    Private Sub ParseRequest()
        Dim parts() As String = requestLine.Split(New Char() {" "}, 3)
        If parts.Length = 3 Then
            method = parts(0)
            requestURI = parts(1)
            httpVersion = parts(2)
        End If
    End Sub
End Class

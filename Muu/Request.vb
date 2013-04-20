Imports System.Text

Public Class Request
    Private _data As Byte()
    Private _method As String
    Private _requestURI As String
    Private _httpVersion As String

    Private dataIndex As New List(Of Tuple(Of Integer, Integer))
    Private requestLine As String
    Private headers As New Dictionary(Of String, String)

    Public Sub New(data As Byte())
        Me._data = data
        MakeIndex()
        MakeHeaders()
        ParseRequest()
    End Sub

    Public ReadOnly Property Data As Byte()
        Get
            Return _data
        End Get
    End Property

    Public ReadOnly Property Method As String
        Get
            Return _method
        End Get
    End Property

    Public ReadOnly Property RequestURI As String
        Get
            Return _requestURI
        End Get
    End Property

    Public ReadOnly Property HttpVersion As String
        Get
            Return _httpVersion
        End Get
    End Property

    Public ReadOnly Property FileName As String
        Get
            Dim parts = _requestURI.Split(New [Char]() {"/"c}, StringSplitOptions.RemoveEmptyEntries)
            If parts.Count = 0 Then
                Return Nothing
            End If
            Dim raw As String = parts(parts.GetUpperBound(0))
            Return raw.Replace("%20", " ")
        End Get
    End Property

    Private Sub MakeIndex()
        Dim begin = 0
        For index = 0 To _data.Length - 2
            If _data(index) = 13 And _data(index + 1) = 10 Then
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
            Dim line = Encoding.ASCII.GetString(_data, item.Item1, item.Item2)
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
            _method = parts(0)
            _requestURI = parts(1)
            _httpVersion = parts(2)
        End If
    End Sub
End Class

Class MainWindow
    Private server As Server
    Private port As Integer = 8080

    Public Sub New()
        InitializeComponent()

        server = New Server()
        server.ServerEnabled = AddressOf ServerEnabled
        server.ServerDisabled = AddressOf ServerDisabled
    End Sub

    Private Sub ServerEnabled()
        Log("Server enabled")
    End Sub

    Private Sub ServerDisabled()
        Log("Server disabled")
    End Sub

    Private Sub EnableButton_Click(sender As Object, e As RoutedEventArgs)
        Try
            server.Enable(port)
        Catch ex As InvalidOperationException
            Log(ex.Message)
        End Try
    End Sub

    Private Sub DisableButton_Click(sender As Object, e As RoutedEventArgs)
        Try
            server.Disable()
        Catch ex As InvalidOperationException
            Log(ex.Message)
        End Try
    End Sub

    Private Sub LogBox_TextChanged(sender As Object, e As TextChangedEventArgs)
        LogViewer.ScrollToEnd()
    End Sub

    Private Sub Log(message As String)
        If (LogBox.Text.Length > 0) Then
            LogBox.AppendText(Environment.NewLine)
        End If
        LogBox.AppendText(message)
    End Sub
End Class

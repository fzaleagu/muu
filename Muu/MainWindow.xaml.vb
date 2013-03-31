Class MainWindow
    Private server As Server
    Private port As Integer = 8080

    Public Sub New()
        InitializeComponent()

        PortBox.Text = port
        SetControlState(False)

        server = New Server()
        server.ServerEnabled = AddressOf ServerEnabled
        server.ServerDisabled = AddressOf ServerDisabled
    End Sub

    Private Sub ServerEnabled()
        Log("Server enabled")
        SetControlState(True)
    End Sub

    Private Sub ServerDisabled()
        Log("Server disabled")
        SetControlState(False)
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

    Private Sub PortBox_TextChanged(sender As Object, e As TextChangedEventArgs) Handles PortBox.TextChanged
        Try
            port = CInt(PortBox.Text)
            PortBox.Background = Nothing
        Catch ex As InvalidCastException
            PortBox.Background = New SolidColorBrush(Colors.Red)
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

    Private Sub SetControlState(serverEnabled As Boolean)
        If serverEnabled Then
            EnableButton.IsEnabled = False
            DisableButton.IsEnabled = True
            PortBox.IsEnabled = False
        Else
            EnableButton.IsEnabled = True
            DisableButton.IsEnabled = False
            PortBox.IsEnabled = True
        End If
    End Sub
End Class

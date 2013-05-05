Imports System.Collections.ObjectModel

Class MainWindow
    Private server As Server
    Private port As Integer = 8080
    Private files As ObservableCollection(Of File) = New ObservableCollection(Of File)()

    Public Sub New()
        InitializeComponent()

        PortBox.Text = port
        SetControlState(False)

        server = New Server(files) With {
            .ServerDisabled = AddressOf ServerDisabled,
            .ServerEnabled = AddressOf ServerEnabled,
            .ServerLogger = AddressOf Log
        }
    End Sub

    Private Sub ServerEnabled()
        SetControlState(True)
    End Sub

    Private Sub ServerDisabled()
        SetControlState(False)
    End Sub

    Private Sub EnableButton_Click(sender As Object, e As RoutedEventArgs) Handles EnableButton.Click
        Try
            server.Enable(port)
        Catch ex As InvalidOperationException
            Log(ex.Message)
        End Try
    End Sub

    Private Sub DisableButton_Click(sender As Object, e As RoutedEventArgs) Handles DisableButton.Click
        Try
            server.Disable()
        Catch ex As InvalidOperationException
            Log(ex.Message)
        End Try
    End Sub

    Private Sub PortBox_TextChanged(sender As Object, e As TextChangedEventArgs) Handles PortBox.TextChanged
        Try
            port = Integer.Parse(PortBox.Text)
            PortBox.ClearValue(TextBox.BackgroundProperty)
        Catch ex As Exception
            PortBox.Background = New SolidColorBrush(Colors.Red)
        End Try
    End Sub

    Private Sub LogBox_TextChanged(sender As Object, e As TextChangedEventArgs) Handles LogBox.TextChanged
        LogViewer.ScrollToEnd()
    End Sub

    Private Async Sub SaveButton_Click(sender As Object, e As RoutedEventArgs) Handles SaveButton.Click
        Dim dialog = New Forms.SaveFileDialog()
        Dim result = dialog.ShowDialog()
        If result = Forms.DialogResult.OK Then
            Dim path = dialog.FileName
            Using writer As New IO.StreamWriter(path)
                Using reader As New IO.StringReader(LogBox.Text)
                    Dim line As String = Nothing
                    Do
                        line = Await reader.ReadLineAsync()
                        If line IsNot Nothing Then
                            Await writer.WriteLineAsync(line)
                        End If
                    Loop Until line Is Nothing
                End Using
            End Using
        End If
    End Sub

    Private Sub AddButton_Click(sender As Object, e As RoutedEventArgs) Handles AddButton.Click
        Dim dialog = New Forms.OpenFileDialog() With {
            .Multiselect = True
        }
        Dim result = dialog.ShowDialog()
        If result = Forms.DialogResult.OK Then
            Dim notAdded = New List(Of File)
            For Each path In dialog.FileNames
                Dim file = New File(path)
                If Not files.Contains(file) Then
                    files.Add(file)
                Else
                    notAdded.Add(file)
                End If
            Next

            If notAdded.Count > 0 Then
                MessageBox.Show(MainWindow, "Some filenames were already added")
            End If
        End If
    End Sub

    Private Sub RemoveButton_Click(sender As Object, e As RoutedEventArgs) Handles RemoveButton.Click
        Dim selected = New List(Of File)
        For Each file In SharedFilesView.SelectedItems
            selected.Add(file)
        Next
        For Each file In selected
            files.Remove(file)
        Next
    End Sub

    Private Sub Log(message As String)
        If (LogBox.Text.Length > 0) Then
            LogBox.AppendText(Environment.NewLine)
        End If
        LogBox.AppendText(String.Format("{0}: {1}", Date.Now, message))
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

    Public ReadOnly Property SharedFiles() As ObservableCollection(Of File)
        Get
            Return files
        End Get
    End Property
End Class

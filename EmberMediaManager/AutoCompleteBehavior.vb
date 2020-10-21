Public Class AutoCompleteBehavior(Of T)

    Private ReadOnly comboBox As ComboBox
    Private ReadOnly itemProvider As Func(Of String, List(Of T))
    Private ReadOnly displayMember As String
    Private ReadOnly valueMember As String

    Private lastText As String

    Public Sub New(comboBox As ComboBox, itemProvider As Func(Of String, List(Of T)), displayMember As String, valueMember As String)

        Me.comboBox = comboBox
        Me.displayMember = displayMember
        Me.valueMember = valueMember

        ' crucial otherwise exceptions occur when the user types in text which Is Not found in the autocompletion list
        Me.comboBox.AutoCompleteMode = AutoCompleteMode.Suggest

        Me.itemProvider = itemProvider

        AddHandler Me.comboBox.KeyPress, New KeyPressEventHandler(AddressOf Me.OnKeyPress)
        AddHandler Me.comboBox.Leave, New EventHandler(AddressOf Me.OnLeave)
        AddHandler Me.comboBox.SelectionChangeCommitted, New EventHandler(AddressOf Me.OnSelectionChangeCommitted)
    End Sub

    Private Sub OnLeave(sender As Object, e As EventArgs)
        HandleLeaveOrEnter()
    End Sub

    Private Sub OnSelectionChangeCommitted(sender As Object, e As EventArgs)
        HandleLeaveOrEnter()
    End Sub

    Private Sub HandleLeaveOrEnter()
        If comboBox.SelectedIndex < 0 Then
            'we have to handle the case, when the user didn't selected from the list (first item would be selected)

            'we have to clear the items to be able to set the Text of the ComboBox 
            comboBox.DataSource = Nothing
            comboBox.DisplayMember = ""
            comboBox.ValueMember = ""

            'But... we cannot use empty list, so it's a workaround for the System.ArgumentOutOfRangeException with empty Item list
            'workaround for the System.ArgumentOutOfRangeException with empty Item list
            comboBox.Items.Add("")

            'After the list has been closed we have to clear the dummy element
            comboBox.BeginInvoke(New Action(AddressOf Me.ClearDummyItems))

            comboBox.Text = Me.lastText
            Me.lastText = Nothing

            'SelectAll is the expected behavior (ComboBox.AutoCompleteMode)
            comboBox.SelectAll()
        End If
    End Sub


    Private Sub OnKeyDown(sender As Object, e As KeyEventArgs)
        If (e.KeyCode = Keys.Enter) Then
            e.Handled = True
            Return
        End If

    End Sub

    Private Sub OnKeyPress(sender As Object, e As KeyPressEventArgs)
        'Its crucial that we use begininvoke because we need the changes to sink into the textfield  
        'Omitting begininvoke would cause the searchterm to lag behind by one character
        'That Is the last character that got typed in
        comboBox.BeginInvoke(New Action(AddressOf Me.ReevaluateCompletionList))
    End Sub

    Private Sub ReevaluateCompletionList()
        Dim currentSearchterm = comboBox.Text
        If currentSearchterm Is Nothing Then
            currentSearchterm = ""
        End If

        'we want to call the itemProvider with empty String (for example for last used values), so the Me.lastText = Nothing default value is expected
        If currentSearchterm = Me.lastText Then
            Return
        End If

        Me.lastText = currentSearchterm
        Dim selectionStart = comboBox.SelectionStart
        Dim selectionLength = comboBox.SelectionLength

        Try
            comboBox.SuspendLayout()

            'Nothing is accepted here
            Dim items = Me.itemProvider(currentSearchterm)

            comboBox.DataSource = items
            comboBox.DisplayMember = Me.displayMember
            comboBox.ValueMember = Me.valueMember

            If (currentSearchterm.Length >= 1 AndAlso comboBox.Items.Count > 0 AndAlso Not comboBox.DroppedDown) Then
                'If the current searchterm Is empty we leave the dropdown list To whatever state it already had
                comboBox.DroppedDown = True
                comboBox.SelectedIndex = -1

                'workaround For the fact the cursor disappears due To droppeddown=True  This Is a known bu.g plaguing combobox which microsoft denies To fix For years now
                Cursor.Current = Cursors.Default
            ElseIf (comboBox.Items.Count = 0 AndAlso comboBox.DroppedDown) Then
                comboBox.DataSource = Nothing
                comboBox.DisplayMember = ""
                comboBox.ValueMember = ""

                'workaround for the System.ArgumentOutOfRangeException with empty Item list
                comboBox.Items.Add("")

                comboBox.SelectedIndex = -1
                comboBox.DroppedDown = False
                Me.lastText = Nothing

                'After the list has been closed we have to clear the dummy element
                comboBox.BeginInvoke(New Action(AddressOf Me.ClearDummyItems))
            End If

        Finally
            'Another workaround For a glitch which causes all text To be selected When there Is a matching entry which starts With the exact text being typed In
            comboBox.Text = currentSearchterm
            comboBox.Select(Math.Min(selectionStart, currentSearchterm.Length), Math.Min(selectionLength, currentSearchterm.Length - selectionStart))
            comboBox.ResumeLayout(True)
        End Try

    End Sub

    Private Sub ClearDummyItems()
        'Dim currentSearchterm = comboBox.Text
        'If currentSearchterm Is Nothing Then
        '    currentSearchterm = ""
        'End If
        'Dim selectionStart = comboBox.SelectionStart
        'Dim selectionLength = comboBox.SelectionLength

        If comboBox.DataSource Is Nothing Then
            'it means the Items contains only the dummy element 
            'clear list by loop through it otherwise the cursor would move to the beginning of the textbox
            While comboBox.Items.Count > 0
                comboBox.Items.RemoveAt(0)
            End While
        End If

        'comboBox.Text = currentSearchterm
        'comboBox.Select(Math.Min(selectionStart, currentSearchterm.Length), Math.Min(selectionLength, currentSearchterm.Length - selectionStart))
    End Sub

End Class

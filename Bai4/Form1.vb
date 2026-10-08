Public Class Form1

    Private Const SlotAvailable As String = "Available"
    Private Const SlotSelected As String = "Selected"
    Private Const SlotBooked As String = "Booked"

    Private ReadOnly slotButtons As New List(Of Button)()
    Private slotsCountLabel As Label
    Private subtotalLabel As Label
    Private timeSlotComboBox As ComboBox

    Public Sub New()
        InitializeComponent()
        BuildInterface()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        BuildInterface()
    End Sub

    Private Sub BuildInterface()
        For index As Integer = Controls.Count - 1 To 0 Step -1
            Controls(index).Dispose()
        Next
        slotButtons.Clear()

        Text = "Đặt vị trí / Đặt bàn hẹn giờ"
        MinimumSize = New Size(640, 540)
        StartPosition = FormStartPosition.CenterScreen

        Dim layout As New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .Padding = New Padding(16),
            .ColumnCount = 1,
            .RowCount = 3
        }
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 48))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 100))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 132))

        Dim title As New Label With {
            .Text = "SƠ ĐỒ CHỌN VỊ TRÍ",
            .Dock = DockStyle.Fill,
            .TextAlign = ContentAlignment.MiddleCenter,
            .Font = New Font(Font.FontFamily, 16, FontStyle.Bold)
        }
        layout.Controls.Add(title, 0, 0)

        Dim slotGrid As New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 4,
            .RowCount = 5,
            .Padding = New Padding(8)
        }
        For column As Integer = 0 To 3
            slotGrid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25))
        Next
        For row As Integer = 0 To 4
            slotGrid.RowStyles.Add(New RowStyle(SizeType.Percent, 20))
        Next

        For index As Integer = 1 To 20
            Dim slotButton As New Button With {
                .Text = $"Vị trí {index}",
                .Dock = DockStyle.Fill,
                .Margin = New Padding(6),
                .Tag = SlotAvailable,
                .BackColor = Color.WhiteSmoke,
                .UseVisualStyleBackColor = False,
                .Font = New Font(Font.FontFamily, 10, FontStyle.Bold)
            }
            AddHandler slotButton.Click, AddressOf SlotButton_Click
            slotButtons.Add(slotButton)
            slotGrid.Controls.Add(slotButton, (index - 1) Mod 4, (index - 1) \ 4)
        Next
        layout.Controls.Add(slotGrid, 0, 1)

        Dim summaryPanel As New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 2,
            .RowCount = 3,
            .Padding = New Padding(8)
        }
        summaryPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50))
        summaryPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50))
        summaryPanel.RowStyles.Add(New RowStyle(SizeType.Percent, 33.33F))
        summaryPanel.RowStyles.Add(New RowStyle(SizeType.Percent, 33.33F))
        summaryPanel.RowStyles.Add(New RowStyle(SizeType.Percent, 33.34F))

        timeSlotComboBox = New ComboBox With {
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .Dock = DockStyle.Fill,
            .Font = New Font(Font.FontFamily, 10)
        }
        timeSlotComboBox.Items.AddRange(New Object() {"Sáng - 100.000đ", "Tối - 150.000đ"})
        timeSlotComboBox.SelectedIndex = 0
        AddHandler timeSlotComboBox.SelectedIndexChanged, AddressOf TimeSlotComboBox_SelectedIndexChanged
        summaryPanel.Controls.Add(timeSlotComboBox, 0, 0)

        slotsCountLabel = New Label With {
            .Text = "Số vị trí đang chọn: 0",
            .Dock = DockStyle.Fill,
            .TextAlign = ContentAlignment.MiddleLeft,
            .Font = New Font(Font.FontFamily, 10, FontStyle.Bold)
        }
        summaryPanel.Controls.Add(slotsCountLabel, 1, 0)

        subtotalLabel = New Label With {
            .Text = "Tạm tính tiền: 0đ",
            .Dock = DockStyle.Fill,
            .TextAlign = ContentAlignment.MiddleLeft,
            .Font = New Font(Font.FontFamily, 10, FontStyle.Bold)
        }
        summaryPanel.Controls.Add(subtotalLabel, 0, 1)

        Dim clearButton As New Button With {
            .Text = "Hủy chọn tất cả",
            .Dock = DockStyle.Fill,
            .Margin = New Padding(6),
            .Font = New Font(Font.FontFamily, 9, FontStyle.Bold)
        }
        AddHandler clearButton.Click, AddressOf ClearButton_Click
        summaryPanel.Controls.Add(clearButton, 1, 1)

        Dim confirmButton As New Button With {
            .Text = "Xác nhận đặt",
            .Dock = DockStyle.Fill,
            .Margin = New Padding(6),
            .BackColor = Color.SeaGreen,
            .ForeColor = Color.White,
            .UseVisualStyleBackColor = False,
            .Font = New Font(Font.FontFamily, 10, FontStyle.Bold)
        }
        AddHandler confirmButton.Click, AddressOf ConfirmButton_Click
        summaryPanel.Controls.Add(confirmButton, 0, 2)
        summaryPanel.SetColumnSpan(confirmButton, 2)

        layout.Controls.Add(summaryPanel, 0, 2)
        Controls.Add(layout)
    End Sub

    Private Sub SlotButton_Click(sender As Object, e As EventArgs)
        Dim slotButton = DirectCast(sender, Button)
        If CStr(slotButton.Tag) = SlotBooked Then Return

        If CStr(slotButton.Tag) = SlotSelected Then
            SetSlotState(slotButton, SlotAvailable)
        Else
            SetSlotState(slotButton, SlotSelected)
        End If

        UpdateSummary()
    End Sub

    Private Sub TimeSlotComboBox_SelectedIndexChanged(sender As Object, e As EventArgs)
        UpdateSummary()
    End Sub

    Private Sub ClearButton_Click(sender As Object, e As EventArgs)
        For Each slotButton In slotButtons
            If CStr(slotButton.Tag) = SlotSelected Then
                SetSlotState(slotButton, SlotAvailable)
            End If
        Next

        UpdateSummary()
    End Sub

    Private Sub ConfirmButton_Click(sender As Object, e As EventArgs)
        Dim selectedCount = GetSelectedSlotCount()
        If selectedCount = 0 Then
            MessageBox.Show("Vui lòng chọn ít nhất một vị trí.", "Chưa chọn vị trí", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        For Each slotButton In slotButtons
            If CStr(slotButton.Tag) = SlotSelected Then
                SetSlotState(slotButton, SlotBooked)
            End If
        Next

        UpdateSummary()
        MessageBox.Show($"Đã xác nhận đặt {selectedCount} vị trí.", "Đặt thành công", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub SetSlotState(slotButton As Button, state As String)
        slotButton.Tag = state
        Select Case state
            Case SlotAvailable
                slotButton.BackColor = Color.WhiteSmoke
                slotButton.ForeColor = SystemColors.ControlText
                slotButton.Enabled = True
            Case SlotSelected
                slotButton.BackColor = Color.LightGreen
                slotButton.ForeColor = SystemColors.ControlText
                slotButton.Enabled = True
            Case SlotBooked
                slotButton.BackColor = Color.IndianRed
                slotButton.ForeColor = Color.White
                slotButton.Enabled = False
        End Select
    End Sub

    Private Sub UpdateSummary()
        If slotsCountLabel Is Nothing OrElse subtotalLabel Is Nothing OrElse timeSlotComboBox Is Nothing Then Return

        Dim selectedCount = GetSelectedSlotCount()
        Dim pricePerSlot As Integer = If(timeSlotComboBox.SelectedIndex = 1, 150000, 100000)
        slotsCountLabel.Text = $"Số vị trí đang chọn: {selectedCount}"
        subtotalLabel.Text = $"Tạm tính tiền: {(selectedCount * pricePerSlot).ToString("N0")}đ"
    End Sub

    Private Function GetSelectedSlotCount() As Integer
        Dim count As Integer = 0
        For Each slotButton In slotButtons
            If CStr(slotButton.Tag) = SlotSelected Then count += 1
        Next
        Return count
    End Function

End Class

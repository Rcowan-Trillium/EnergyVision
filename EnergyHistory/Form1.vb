
Imports System.IO
Imports System.Text
Imports MySql.Data.MySqlClient
Imports OxyPlot
Imports OxyPlot.Annotations
Imports OxyPlot.Axes
Imports OxyPlot.Series
Imports OxyPlot.WindowsForms
Imports OxyPlot.ImageSharp

Public Class Form1

    Dim names() As String = {"CW_Supply", "CW_PreFiltPres", "CW_PostFiltPres",
                "CW_FiltDif", "HW_PreFiltPres", "HW_PostFiltPres", "HW_FiltDif", "HW_Temp",
                "HW_Flow", "ST_FeedWaterPres", "ST_HeadPres", "ST_LowPres", "ST_MedPres",
                "ST_LowDem", "ST_MedDem", "ST_Flow", "EL_NorthA", "EL_NorthB", "EL_NorthC",
                "EL_SouthA", "EL_SouthB", "EL_SouthC", "AR_LinePres"}
    Private Sub EnerygyVision_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        With DateTimePicker1
            .Format = DateTimePickerFormat.Custom
            .CustomFormat = "yyyy-MM-dd hh:mm:ss tt"
            .ShowUpDown = False
        End With
        With DateTimePicker2
            .Format = DateTimePickerFormat.Custom
            .CustomFormat = "yyyy-MM-dd hh:mm:ss tt"
            .ShowUpDown = False
        End With
        DateTimePicker2.Value = Now
        DateTimePicker1.Value = Now.AddHours(-1)

        ChartPanel.HorizontalScroll.Enabled = True

    End Sub
    Public Sub ExportPlotToPng(model As PlotModel, filePath As String, Optional width As Integer = 1920, Optional height As Integer = 1080)
        ' Using stream As New FileStream(filePath, FileMode.Create)
        ' Render the PlotModel to a PNG file
        ImageSharp.PngExporter.Export(model, filePath, width, height, 96)
        ' End Using
    End Sub
    Private Sub seriesColorChange(sender As Object, e As MouseEventArgs) Handles CW_Supply_Col_BTN.Click, CW_PreFiltPres_Col_BTN.Click, CW_PostFiltPres_Col_BTN.Click,
                CW_FiltDif_Col_BTN.Click, HW_PreFiltPres_Col_BTN.Click, HW_PostFiltPres_Col_BTN.Click, HW_FiltDif_Col_BTN.Click, HW_Temp_Col_BTN.Click,
                HW_Flow_Col_BTN.Click, ST_FeedWaterPres_Col_BTN.Click, ST_HeadPres_Col_BTN.Click, ST_LowPres_Col_BTN.Click, ST_MedPres_Col_BTN.Click,
                ST_LowDem_Col_BTN.Click, ST_MedDem_Col_BTN.Click, ST_Flow_Col_BTN.Click, EL_NorthA_Col_BTN.Click, EL_NorthB_Col_BTN.Click, EL_NorthC_Col_BTN.Click,
                EL_SouthA_Col_BTN.Click, EL_SouthB_Col_BTN.Click, EL_SouthC_Col_BTN.Click, AR_LinePres_Col_BTN.Click

        Dim CPicker As New ColorDialog
        CPicker.ShowDialog()
        Dim cnt As Integer = 0
        For Each i As LineSeries In PlotView.Model.Series
            If i.Title = sender.name.split("_Col_")(0) Then
                i.Color = ColorToOxyColor(CPicker.Color)
                PlotView.Invalidate()
                SeriesColorList(cnt) = ColorToOxyColor(CPicker.Color)
            End If
            cnt = +1
        Next
        ' PlotView.Model.Axes(sender.name.split("_Col_")(0)).AxislineColor = ColorToOxyColor(CPicker.Color)
        sender.backcolor = CPicker.Color
        'ChartView.Update()
    End Sub

    Dim AlarmsInList As Integer = 250
    Private Sub GetNewData(StartPoint As DateTime, EndPoint As DateTime)
        Dim SQL As String = My.Settings.SQL_ConString
        Dim query As String = "SELECT Date_Time AS Date_Time, "
        Dim SerCNT As Integer = 0
        Dim CheckSeries As New List(Of String)

        'Build query by looping though all of the series visibility checkboxes
        For Each i As CheckBox In {CW_Supply_Vis_CB, CW_PreFiltPres_Vis_CB, CW_PostFiltPres_Vis_CB,
                CW_FiltDif_Vis_CB, HW_PreFiltPres_Vis_CB, HW_PostFiltPres_Vis_CB, HW_FiltDif_Vis_CB, HW_Temp_Vis_CB,
                HW_Flow_Vis_CB, ST_FeedWaterPres_Vis_CB, ST_HeadPres_Vis_CB, ST_LowPres_Vis_CB, ST_MedPres_Vis_CB,
                ST_LowDem_Vis_CB, ST_MedDem_Vis_CB, ST_Flow_Vis_CB, EL_NorthA_Vis_CB, EL_NorthB_Vis_CB, EL_NorthC_Vis_CB,
                EL_SouthA_Vis_CB, EL_SouthB_Vis_CB, EL_SouthC_Vis_CB, AR_LinePres_Vis_CB}
            'for each of the checkboxes in the data selection panel,
            'add the series to the list of names to pull from and
            'build the body of the query string
            If i.Checked = True Then
                SerCNT += 1
                CheckSeries.Add(i.Text)
                query += i.Text & " AS " & i.Text & ", "

            End If

        Next
        query = query.Remove(query.Count - 2)
        query += " FROM a_vals WHERE Date_Time BETWEEN @startDate And @endDate;"

        Dim totalcount As Integer = 0
        Using connection As New MySqlConnection(SQL)
            'Get Values using start and end date and create a data table
            Dim cmd1 As New MySqlCommand(query, connection)
            cmd1.Parameters.AddWithValue("@startDate", StartPoint)
            cmd1.Parameters.AddWithValue("@endDate", EndPoint)
            Dim adapter As New MySqlDataAdapter(cmd1)
            Dim table As New DataTable()
            adapter.Fill(table)
            'Populate GridView with new table data
            GridView.DataSource = table
        End Using
        Dim Data_cellstyle As New DataGridViewCellStyle
        With Data_cellstyle
            .BackColor = Color.FromArgb(10, 10, 10)
            .ForeColor = Color.Silver
            .Format = "0.00"
            .Alignment = DataGridViewContentAlignment.MiddleCenter
            .Font = SUM_Count_LBL.Font
            .SelectionBackColor = Color.DarkGray
            .SelectionForeColor = Color.Black
        End With
        With GridView
            .RowHeadersVisible = True
            .RowHeadersDefaultCellStyle = Data_cellstyle
            .ColumnHeadersDefaultCellStyle = Data_cellstyle
            For i As Integer = 1 To SerCNT
                .Columns(i).DefaultCellStyle.ApplyStyle(Data_cellstyle)
            Next
            Data_cellstyle.Format = ("MM/dd/yyyy hh:mm:ss tt")
            .Columns(0).DefaultCellStyle.ApplyStyle(Data_cellstyle)
        End With
    End Sub



    Private Sub SpanChanged(sender As Object, e As EventArgs) Handles TextBox7.TextChanged
        If IsNumeric(sender.text) Then
            Dim NewTimeSpan As New TimeSpan
            If RadioButton1.Checked = True Then
                NewTimeSpan = TimeSpan.FromHours(TextBox7.Text)
            Else
                NewTimeSpan = TimeSpan.FromMinutes(TextBox7.Text)
            End If
            ' DateTimePicker2.Value = DateTimePicker1.Value.Add(NewTimeSpan)
            ZoomPlot(PlotView.Model, NewTimeSpan)
        Else
            MsgBox("Entry must be numeric")
        End If
    End Sub

    Dim SeriesNames_x2 As New List(Of String) From {"CW_Supply", "CW_PreFiltPres", "CW_PostFiltPres",
                "CW_FiltDif", "HW_PreFiltPres", "HW_PostFiltPres", "HW_FiltDif", "HW_Temp",
                "HW_Flow", "ST_FeedWaterPres", "ST_HeadPres", "ST_LowPres", "ST_MedPres",
                "ST_LowDem", "ST_MedDem", "ST_Flow", "EL_NorthA", "EL_NorthB", "EL_NorthC",
                "EL_SouthA", "EL_SouthB", "EL_SouthC", "AR_LinePres"}

    Private Sub SUM_Name_LBL_CLICK(sender As Object, e As MouseEventArgs) Handles SUM_Name_LBL0.Click,
        SUM_Name_LBL1.Click, SUM_Name_LBL2.Click, SUM_Name_LBL3.Click, SUM_Name_LBL4.Click, SUM_Name_LBL5.Click,
        SUM_Name_LBL6.Click, SUM_Name_LBL7.Click, SUM_Name_LBL8.Click, SUM_Name_LBL9.Click, SUM_Name_LBL10.Click,
        SUM_Name_LBL11.Click, SUM_Name_LBL12.Click, SUM_Name_LBL13.Click, SUM_Name_LBL14.Click, SUM_Name_LBL15.Click,
        SUM_Name_LBL16.Click, SUM_Name_LBL17.Click, SUM_Name_LBL18.Click, SUM_Name_LBL19.Click, SUM_Name_LBL20.Click,
        SUM_Name_LBL21.Click, SUM_Name_LBL22.Click

        GetMoreInfo(DateTimePicker1.Value, DateTimePicker2.Value, sender.text)
    End Sub
    Private Sub GetMoreInfo(StartPoint As DateTime, EndPoint As DateTime, Series As String)
        Dim query As String = "SELECT Date_Time, " & Series & " FROM a_vals WHERE Date_Time BETWEEN @startDate AND @endDate ORDER BY " & Series & " DESC LIMIT 1;"

        Dim SQL As String = My.Settings.SQL_ConString
        Using connection As New MySqlConnection(SQL)
            Dim cmd As New MySqlCommand(query, connection)
            cmd.Parameters.AddWithValue("@startDate", StartPoint)
            cmd.Parameters.AddWithValue("@endDate", EndPoint)
            connection.Open()
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.Read() Then

                    Label7.Text = reader("Date_Time")
                End If
            End Using
        End Using

    End Sub
    Private Sub GetSummary(StartPoint As DateTime, EndPoint As DateTime)
        Dim query As String = "SELECT COUNT(*) AS total_rows, " &
                      "MIN(Date_Time) AS start_date, " &
                      "MAX(Date_Time) AS end_date, " &
                      "MIN(CW_supply) AS CW_Supply_MIN, " &
                      "MAX(CW_Supply) AS CW_Supply_MAX, " &
                      "AVG(CW_Supply) AS CW_Supply_AVG, " &
                      "MIN(CW_PreFiltPres) AS CW_PreFiltPres_MIN, " &
                      "MAX(CW_PreFiltPres) AS CW_PreFiltPres_MAX, " &
                      "AVG(CW_PreFiltPres) AS CW_PreFiltPres_AVG, " &
                      "MIN(CW_PostFiltPres) AS CW_PostFiltPres_MIN, " &
                      "MAX(CW_PostFiltPres) AS CW_PostFiltPres_MAX, " &
                      "AVG(CW_PostFiltPres) AS CW_PostFiltPres_AVG, " &
                      "MIN(CW_FiltDif) AS CW_FiltDif_MIN, " &
                      "MAX(CW_FiltDif) AS CW_FiltDif_MAX, " &
                      "AVG(CW_FiltDif) AS CW_FiltDif_AVG, " &
                      "MIN(HW_PreFiltPres) AS HW_PreFiltPres_MIN, " &
                      "MAX(HW_PreFiltPres) AS HW_PreFiltPres_MAX, " &
                      "AVG(HW_PreFiltPres) AS HW_PreFiltPres_AVG, " &
                      "MIN(HW_PostFiltPres) AS HW_PostFiltPres_MIN, " &
                      "MAX(HW_PostFiltPres) AS HW_PostFiltPres_MAX, " &
                      "AVG(HW_PostFiltPres) AS HW_PostFiltPres_AVG, " &
                      "MIN(HW_FiltDif) AS HW_FiltDif_MIN, " &
                      "MAX(HW_FiltDif) AS HW_FiltDif_MAX, " &
                      "AVG(HW_FiltDif) AS HW_FiltDif_AVG, " &
                      "MIN(HW_Temp) AS HW_Temp_MIN, " &
                      "MAX(HW_Temp) AS HW_Temp_MAX, " &
                      "AVG(HW_Temp) AS HW_Temp_AVG, " &
                      "MIN(HW_Flow) AS HW_Flow_MAX, " &
                      "MAX(HW_Flow) AS HW_Flow_MIN, " &
                      "AVG(HW_Flow) AS HW_Flow_AVG, " &
                      "MIN(ST_FeedWaterPres) AS ST_FeedWaterPres_MIN, " &
                      "MAX(ST_FeedWaterPres) AS ST_FeedWaterPres_MAX, " &
                      "AVG(ST_FeedWaterPres) AS ST_FeedWaterPres_AVG, " &
                      "MIN(ST_HeadPres) AS ST_HeadPres_MIN, " &
                      "MAX(ST_HeadPres) AS ST_HeadPres_MAX, " &
                      "AVG(ST_HeadPres) AS ST_HeadPres_AVG, " &
                      "MIN(ST_LowPres) AS ST_LowPres_MIN, " &
                      "MAX(ST_LowPres) AS ST_LowPres_MAX, " &
                      "AVG(ST_LowPres) AS ST_LowPres_AVG, " &
                      "MIN(ST_MedPres) AS ST_MedPres_MIN, " &
                      "MAX(ST_MedPres) AS ST_MedPres_MAX, " &
                      "AVG(ST_MedPres) AS ST_MedPres_AVG, " &
                      "MIN(ST_LowDem) AS ST_LowDem_MIN, " &
                      "MAX(ST_LowDem) AS ST_LowDem_MAX, " &
                      "AVG(ST_LowDem) AS ST_LowDem_AVG, " &
                      "MIN(ST_MedDem) AS ST_MedDem_MIN, " &
                      "MAX(ST_MedDem) AS ST_MedDem_MAX, " &
                      "AVG(ST_MedDem) AS ST_MedDem_AVG, " &
                      "MIN(ST_Flow) AS ST_Flow_MIN, " &
                      "MAX(ST_Flow) AS ST_Flow_MAX, " &
                      "AVG(ST_Flow) AS ST_Flow_AVG, " &
                      "MIN(EL_NorthA) AS EL_NorthA_MIN, " &
                      "MAX(EL_NorthA) AS EL_NorthA_MAX, " &
                      "AVG(EL_NorthA) AS EL_NorthA_AVG, " &
                      "MIN(EL_NorthB) AS EL_NorthB_MIN, " &
                      "MAX(EL_NorthB) AS EL_NorthB_MAX, " &
                      "AVG(EL_NorthB) AS EL_NorthB_AVG, " &
                      "MIN(EL_NorthC) AS EL_NorthC_MIN, " &
                      "MAX(EL_NorthC) AS EL_NorthC_MAX, " &
                      "AVG(EL_NorthC) AS EL_NorthC_AVG, " &
                      "MIN(EL_SouthA) AS EL_SouthA_MIN, " &
                      "MAX(EL_SouthA) AS EL_SouthA_MAX, " &
                      "AVG(EL_SouthA) AS EL_SouthA_AVG, " &
                      "MIN(EL_SouthB) AS EL_SouthB_MIN, " &
                      "MAX(EL_SouthB) AS EL_SouthB_MAX, " &
                      "AVG(EL_SouthB) AS EL_SouthB_AVG, " &
                      "MIN(EL_SouthC) AS EL_SouthC_MIN, " &
                      "MAX(EL_SouthC) AS EL_SouthC_MAX, " &
                      "AVG(EL_SouthC) AS EL_SouthC_AVG, " &
                      "MIN(AR_LinePres) AS AR_LinePres_MIN, " &
                      "MAX(AR_LinePres) AS AR_LinePres_MAX, " &
                      "AVG(AR_LinePres) AS AR_LinePres_AVG " &
                     "FROM a_vals " & "WHERE Date_Time BETWEEN @startDate And @endDate;"
        Dim SQL As String = My.Settings.SQL_ConString
        Using connection As New MySqlConnection(SQL)
            Dim cmd As New MySqlCommand(query, connection)
            cmd.Parameters.AddWithValue("@startDate", StartPoint)
            cmd.Parameters.AddWithValue("@endDate", EndPoint)

            Dim totalCount As Integer = 0
            Dim minDate As DateTime
            Dim maxDate As DateTime

            Dim minValue(25) As Double
            Dim maxValue(25) As Double
            Dim avgValue(25) As Double

            connection.Open()
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    '#####  Info    #####
                    totalCount = Convert.ToInt32(reader("total_rows"))
                    If Not IsDBNull(reader("start_date")) Then minDate = Convert.ToDateTime(reader("start_date"))
                    If Not IsDBNull(reader("end_date")) Then maxDate = Convert.ToDateTime(reader("end_date"))
                    '#####  CW_Supply   #####
                    If Not IsDBNull(reader("CW_Supply_MAX")) Then maxValue(0) = Convert.ToDouble(reader("CW_Supply_MAX"))
                    If Not IsDBNull(reader("CW_Supply_MIN")) Then minValue(0) = Convert.ToDouble(reader("CW_Supply_MIN"))
                    If Not IsDBNull(reader("CW_Supply_AVG")) Then avgValue(0) = Convert.ToDouble(reader("CW_Supply_AVG"))
                    '#####  CW_PreFiltPres   #####
                    If Not IsDBNull(reader("CW_PreFiltPres_MAX")) Then maxValue(1) = Convert.ToDouble(reader("CW_PreFiltPres_MAX"))
                    If Not IsDBNull(reader("CW_PreFiltPres_MIN")) Then minValue(1) = Convert.ToDouble(reader("CW_PreFiltPres_MIN"))
                    If Not IsDBNull(reader("CW_PreFiltPres_AVG")) Then avgValue(1) = Convert.ToDouble(reader("CW_PreFiltPres_AVG"))
                    '#####  CW_PostFiltPres   #####
                    If Not IsDBNull(reader("CW_PostFiltPres_MAX")) Then maxValue(2) = Convert.ToDouble(reader("CW_PostFiltPres_MAX"))
                    If Not IsDBNull(reader("CW_PostFiltPres_MIN")) Then minValue(2) = Convert.ToDouble(reader("CW_PostFiltPres_MIN"))
                    If Not IsDBNull(reader("CW_PostFiltPres_AVG")) Then avgValue(2) = Convert.ToDouble(reader("CW_PostFiltPres_AVG"))
                    '#####  CW_FiltDif   #####
                    If Not IsDBNull(reader("CW_FiltDif_MAX")) Then maxValue(3) = Convert.ToDouble(reader("CW_FiltDif_MAX"))
                    If Not IsDBNull(reader("CW_FiltDif_MIN")) Then minValue(3) = Convert.ToDouble(reader("CW_FiltDif_MIN"))
                    If Not IsDBNull(reader("CW_FiltDif_AVG")) Then avgValue(3) = Convert.ToDouble(reader("CW_FiltDif_AVG"))
                    '#####  HW_PreFiltPres   #####
                    If Not IsDBNull(reader("HW_PreFiltPres_MAX")) Then maxValue(4) = Convert.ToDouble(reader("HW_PreFiltPres_MAX"))
                    If Not IsDBNull(reader("HW_PreFiltPres_MIN")) Then minValue(4) = Convert.ToDouble(reader("HW_PreFiltPres_MIN"))
                    If Not IsDBNull(reader("HW_PreFiltPres_AVG")) Then avgValue(4) = Convert.ToDouble(reader("HW_PreFiltPres_AVG"))
                    '#####  HW_PostFiltPres   #####
                    If Not IsDBNull(reader("HW_PostFiltPres_MAX")) Then maxValue(5) = Convert.ToDouble(reader("HW_PostFiltPres_MAX"))
                    If Not IsDBNull(reader("HW_PostFiltPres_MIN")) Then minValue(5) = Convert.ToDouble(reader("HW_PostFiltPres_MIN"))
                    If Not IsDBNull(reader("HW_PostFiltPres_AVG")) Then avgValue(5) = Convert.ToDouble(reader("HW_PostFiltPres_AVG"))
                    '#####  HW_FiltDif   #####
                    If Not IsDBNull(reader("HW_FiltDif_MAX")) Then maxValue(6) = Convert.ToDouble(reader("HW_FiltDif_MAX"))
                    If Not IsDBNull(reader("HW_FiltDif_MIN")) Then minValue(6) = Convert.ToDouble(reader("HW_FiltDif_MIN"))
                    If Not IsDBNull(reader("HW_FiltDif_AVG")) Then avgValue(6) = Convert.ToDouble(reader("HW_FiltDif_AVG"))
                    '#####  HW_Temp   #####
                    If Not IsDBNull(reader("HW_Temp_MAX")) Then maxValue(7) = Convert.ToDouble(reader("HW_Temp_MAX"))
                    If Not IsDBNull(reader("HW_Temp_MIN")) Then minValue(7) = Convert.ToDouble(reader("HW_Temp_MIN"))
                    If Not IsDBNull(reader("HW_Temp_AVG")) Then avgValue(7) = Convert.ToDouble(reader("HW_Temp_AVG"))
                    '#####  HW_Flow   #####
                    If Not IsDBNull(reader("HW_Flow_MAX")) Then maxValue(8) = Convert.ToDouble(reader("HW_Flow_MAX"))
                    If Not IsDBNull(reader("HW_Flow_MIN")) Then minValue(8) = Convert.ToDouble(reader("HW_Flow_MIN"))
                    If Not IsDBNull(reader("HW_Flow_AVG")) Then avgValue(8) = Convert.ToDouble(reader("HW_Flow_AVG"))
                    '#####  ST_FeedWaterPres   #####
                    If Not IsDBNull(reader("ST_FeedWaterPres_MAX")) Then maxValue(9) = Convert.ToDouble(reader("ST_FeedWaterPres_MAX"))
                    If Not IsDBNull(reader("ST_FeedWaterPres_MIN")) Then minValue(9) = Convert.ToDouble(reader("ST_FeedWaterPres_MIN"))
                    If Not IsDBNull(reader("ST_FeedWaterPres_AVG")) Then avgValue(9) = Convert.ToDouble(reader("ST_FeedWaterPres_AVG"))
                    '#####  ST_HeadPres   #####
                    If Not IsDBNull(reader("ST_HeadPres_MAX")) Then maxValue(10) = Convert.ToDouble(reader("ST_HeadPres_MAX"))
                    If Not IsDBNull(reader("ST_HeadPres_MIN")) Then minValue(10) = Convert.ToDouble(reader("ST_HeadPres_MIN"))
                    If Not IsDBNull(reader("ST_HeadPres_AVG")) Then avgValue(10) = Convert.ToDouble(reader("ST_HeadPres_AVG"))
                    '#####  ST_LowPres   #####
                    If Not IsDBNull(reader("ST_LowPres_MAX")) Then maxValue(11) = Convert.ToDouble(reader("ST_LowPres_MAX"))
                    If Not IsDBNull(reader("ST_LowPres_MIN")) Then minValue(11) = Convert.ToDouble(reader("ST_LowPres_MIN"))
                    If Not IsDBNull(reader("ST_LowPres_AVG")) Then avgValue(11) = Convert.ToDouble(reader("ST_LowPres_AVG"))
                    '#####  ST_MedPres   #####
                    If Not IsDBNull(reader("ST_MedPres_MAX")) Then maxValue(12) = Convert.ToDouble(reader("ST_MedPres_MAX"))
                    If Not IsDBNull(reader("ST_MedPres_MIN")) Then minValue(12) = Convert.ToDouble(reader("ST_MedPres_MIN"))
                    If Not IsDBNull(reader("ST_MedPres_AVG")) Then avgValue(12) = Convert.ToDouble(reader("ST_MedPres_AVG"))
                    '#####  ST_LowPresDem   #####
                    If Not IsDBNull(reader("ST_LowDem_MAX")) Then maxValue(13) = Convert.ToDouble(reader("ST_LowDem_MAX"))
                    If Not IsDBNull(reader("ST_LowDem_MIN")) Then minValue(13) = Convert.ToDouble(reader("ST_LowDem_MIN"))
                    If Not IsDBNull(reader("ST_LowDem_AVG")) Then avgValue(13) = Convert.ToDouble(reader("ST_LowDem_AVG"))
                    '#####  ST_MedPresDem   #####
                    If Not IsDBNull(reader("ST_MedDem_MAX")) Then maxValue(14) = Convert.ToDouble(reader("ST_MedDem_MAX"))
                    If Not IsDBNull(reader("ST_MedDem_MIN")) Then minValue(14) = Convert.ToDouble(reader("ST_MedDem_MIN"))
                    If Not IsDBNull(reader("ST_MedDem_AVG")) Then avgValue(14) = Convert.ToDouble(reader("ST_MedDem_AVG"))
                    '#####  ST_Flow   #####
                    If Not IsDBNull(reader("ST_Flow_MAX")) Then maxValue(15) = Convert.ToDouble(reader("ST_Flow_MAX"))
                    If Not IsDBNull(reader("ST_Flow_MIN")) Then minValue(15) = Convert.ToDouble(reader("ST_Flow_MIN"))
                    If Not IsDBNull(reader("ST_Flow_AVG")) Then avgValue(15) = Convert.ToDouble(reader("ST_Flow_AVG"))
                    '#####  EL_NorthA   #####
                    If Not IsDBNull(reader("EL_NorthA_MAX")) Then maxValue(16) = Convert.ToDouble(reader("EL_NorthA_MAX"))
                    If Not IsDBNull(reader("EL_NorthA_MIN")) Then minValue(16) = Convert.ToDouble(reader("EL_NorthA_MIN"))
                    If Not IsDBNull(reader("EL_NorthA_AVG")) Then avgValue(16) = Convert.ToDouble(reader("EL_NorthA_AVG"))
                    '#####  EL_NorthB   #####
                    If Not IsDBNull(reader("EL_NorthB_MAX")) Then maxValue(17) = Convert.ToDouble(reader("EL_NorthB_MAX"))
                    If Not IsDBNull(reader("EL_NorthB_MIN")) Then minValue(17) = Convert.ToDouble(reader("EL_NorthB_MIN"))
                    If Not IsDBNull(reader("EL_NorthB_AVG")) Then avgValue(17) = Convert.ToDouble(reader("EL_NorthB_AVG"))
                    '#####  EL_NorthC   #####
                    If Not IsDBNull(reader("EL_NorthC_MAX")) Then maxValue(18) = Convert.ToDouble(reader("EL_NorthC_MAX"))
                    If Not IsDBNull(reader("EL_NorthC_MIN")) Then minValue(18) = Convert.ToDouble(reader("EL_NorthC_MIN"))
                    If Not IsDBNull(reader("EL_NorthC_AVG")) Then avgValue(18) = Convert.ToDouble(reader("EL_NorthC_AVG"))
                    '#####  EL_SouthA   #####
                    If Not IsDBNull(reader("EL_SouthA_MAX")) Then maxValue(19) = Convert.ToDouble(reader("EL_SouthA_MAX"))
                    If Not IsDBNull(reader("EL_SouthA_MIN")) Then minValue(19) = Convert.ToDouble(reader("EL_SouthA_MIN"))
                    If Not IsDBNull(reader("EL_SouthA_AVG")) Then avgValue(19) = Convert.ToDouble(reader("EL_SouthA_AVG"))
                    '#####  EL_SouthB   #####
                    If Not IsDBNull(reader("EL_SouthB_MAX")) Then maxValue(20) = Convert.ToDouble(reader("EL_SouthB_MAX"))
                    If Not IsDBNull(reader("EL_SouthB_MIN")) Then minValue(20) = Convert.ToDouble(reader("EL_SouthB_MIN"))
                    If Not IsDBNull(reader("EL_SouthB_AVG")) Then avgValue(20) = Convert.ToDouble(reader("EL_SouthB_AVG"))
                    '#####  EL_SouthC   #####
                    If Not IsDBNull(reader("EL_SouthC_MAX")) Then maxValue(21) = Convert.ToDouble(reader("EL_SouthC_MAX"))
                    If Not IsDBNull(reader("EL_SouthC_MIN")) Then minValue(21) = Convert.ToDouble(reader("EL_SouthC_MIN"))
                    If Not IsDBNull(reader("EL_SouthC_AVG")) Then avgValue(21) = Convert.ToDouble(reader("EL_SouthC_AVG"))
                    '#####  AR_LinePres   #####
                    If Not IsDBNull(reader("AR_LinePres_MAX")) Then maxValue(22) = Convert.ToDouble(reader("AR_LinePres_MAX"))
                    If Not IsDBNull(reader("AR_LinePres_MIN")) Then minValue(22) = Convert.ToDouble(reader("AR_LinePres_MIN"))
                    If Not IsDBNull(reader("AR_LinePres_AVG")) Then avgValue(22) = Convert.ToDouble(reader("AR_LinePres_AVG"))
                    '#####  Spare   #####
                    maxValue(23) = 0
                    minValue(23) = 0
                    avgValue(23) = 0
                    '#####  Spare   #####
                    maxValue(24) = 0
                    minValue(24) = 0
                    avgValue(24) = 0
                End If
            End Using
            For i As Integer = 0 To 22
                SetControlPropertyByName("SUM_Name_LBL" & i, "Text", names(i))
                SetControlPropertyByName("SUM_Min_LBL" & i, "Text", minValue(i).ToString("F2"))
                SetControlPropertyByName("SUM_Max_LBL" & i, "Text", maxValue(i).ToString("F2"))
                SetControlPropertyByName("SUM_Avg_LBL" & i, "Text", avgValue(i).ToString("F2"))
            Next
            SetControlPropertyByName("SUM_Count_LBL", "Text", totalCount & " rows")
            'SetControlPropertyByName("SUM_Start_Time", "Text", minDate)
            'SetControlPropertyByName("SUM_End_Time", "Text", maxDate)
        End Using
    End Sub
    Public Function FindControlByName(parent As Control, name As String) As Control
        For Each ctrl As Control In parent.Controls
            If ctrl.Name = name Then
                Return ctrl
            Else
                Dim found = FindControlByName(ctrl, name)
                If found IsNot Nothing Then Return found
            End If
        Next
        Return Nothing
    End Function
    Public Sub SetControlPropertyByName(controlName As String, propertyName As String, newValue As Object)
        ' Find the control by name (recursive if needed)
        Dim ctrl As Control = FindControlByName(Me, controlName)
        If ctrl IsNot Nothing Then
            ' Use reflection to get the property
            Dim prop = ctrl.GetType().GetProperty(propertyName)
            If prop IsNot Nothing AndAlso prop.CanWrite Then
                ' Convert value if needed
                Dim convertedValue = Convert.ChangeType(newValue, prop.PropertyType)
                prop.SetValue(ctrl, convertedValue)
            Else
                MessageBox.Show($"Property '{propertyName}' not found or not writable on control '{controlName}'.")
            End If
        Else
            MessageBox.Show($"Control '{controlName}' not found.")
        End If
    End Sub
    Private Sub Confirm_And_Pull_Button(sender As Object, e As EventArgs) Handles Button13.Click

        Dim Start_DateTime = DateTimePicker1.Value
        Dim End_DateTime = DateTimePicker2.Value
        If Start_DateTime < End_DateTime Then
            If Pull_Grid_Selector.Checked Then GetNewData(Start_DateTime, End_DateTime)
            If Pull_Chart_Selector.Checked Then PopulatePlotView(Start_DateTime, End_DateTime)
        Else
            MsgBox("The Start time must come before the end time", MessageBoxButtons.OK)
        End If

        ' LoadChartData_test(Start_DateTime, End_DateTime)
    End Sub
    Private cursorLine As LineAnnotation
    Private Sub PlotView_MouseDown(sender As Object, e As OxyMouseDownEventArgs)
        If e.ChangedButton = OxyMouseButton.Left Then
            ' Convert screen position → X data coordinate
            Dim xAxis = PlotView.Model.Axes.OfType(Of DateTimeAxis)().First()
            Dim xVal = xAxis.InverseTransform(e.Position.X)

            ' Move cursor
            cursorLine.X = xVal

            ' Clear previous annotation text
            cursorLine.Text = ""

            ' Collect Y-values at cursor X
            Dim textLines As New List(Of String)

            For Each s In PlotView.Model.Series.OfType(Of LineSeries)()
                Dim nearest = s.Points.OrderBy(Function(p) Math.Abs(p.X - xVal)).FirstOrDefault()
                ' If nearest IsNot vbEmpty Then
                Dim yVal = nearest.Y
                Dim seriesName = s.Title
                textLines.Add($"{seriesName}: {yVal:F2}")
                ' End If
            Next

            ' Build label text
            cursorLine.Text = String.Join(Environment.NewLine, textLines)

            ' Optional: position label
            cursorLine.TextOrientation = AnnotationTextOrientation.Vertical
            cursorLine.TextVerticalAlignment = VerticalAlignment.Top

            ' Update plot
            PlotView.Model.InvalidatePlot(False)
            e.Handled = True
        End If

    End Sub

    Sub OpenNewPage(Page As Object)
        Summary_Page.Hide()
        ChartPanel.Hide()
        Grid_Page.Hide()
        Settings_Page.Hide()
        Page.show()
        Page.bringtofront()
    End Sub

    Private Sub Open_Summary_Page(sender As Object, e As EventArgs) Handles Button7.Click
        OpenNewPage(Summary_Page)
    End Sub
    Private Sub Open_Grid_Page(sender As Object, e As EventArgs) Handles Button8.Click
        OpenNewPage(Grid_Page)
    End Sub
    Private Sub Open_Chart_Page(sender As Object, e As EventArgs) Handles Button9.Click
        OpenNewPage(ChartPanel)
    End Sub
    Private Sub Open_Settings_Page(sender As Object, e As EventArgs) Handles Button4.Click
        OpenNewPage(Settings_Page)
    End Sub
    Dim DTP1_FirstChange As Boolean = False
    Dim DTP2_FirstChange As Boolean = False
    Private Sub DateTimePicker1_ValueChanged(sender As Object, e As EventArgs) Handles DateTimePicker1.ValueChanged, DateTimePicker2.ValueChanged
        If DTP1_FirstChange AndAlso DTP2_FirstChange Then
            GetSummary(DateTimePicker1.Value, DateTimePicker2.Value)
        Else

        End If
        If sender.name.ToString.Contains("1") AndAlso DTP1_FirstChange = False Then DTP1_FirstChange = True
        If sender.name.ToString.Contains("2") AndAlso DTP2_FirstChange = False Then DTP2_FirstChange = True


    End Sub

    Private Sub Edit_SQL_BTN(sender As Object, e As EventArgs) Handles Button10.Click
        If sender.text = "Edit" Then
            SQL_Server_TB.Enabled = True
            SQL_User_TB.Enabled = True
            SQL_Pass_TB.Enabled = True
            SQL_Database_TB.Enabled = True
            sender.text = "Save"
        Else
            SQL_Server_TB.Enabled = False
            SQL_User_TB.Enabled = False
            SQL_Pass_TB.Enabled = False
            SQL_Database_TB.Enabled = False
            sender.text = "Edit"
            My.Settings.SQL_ConString = "server=" & SQL_Server_TB.Text & ";user id=" & SQL_User_TB.Text & ";password=" & SQL_Pass_TB.Text & ";database=" & SQL_Database_TB.Text
            MsgBox("Connection string saved to: " & vbCrLf & My.Settings.SQL_ConString)
        End If

    End Sub

    Private Sub Form1_DoubleClick(sender As Object, e As EventArgs) Handles Me.DoubleClick
        PopulatePlotView(DateTimePicker1.Value, DateTimePicker2.Value)
    End Sub

    'Private Sub HScrollBar1_ValueChanged(sender As Object, e As EventArgs) Handles HScrollBar1.ValueChanged
    '    For Each i As ChartArea In ChartView.ChartAreas
    '        i.AxisX.ScaleView.Position = ChartView.Series(0).Points(0).XValue + sender.value * (((1 / 24) / 60) / 60)
    '    Next

    'End Sub

    Dim VisEdit As Boolean = False
    Private Sub Vis_Check_Changed(sender As CheckBox, e As EventArgs) Handles CW_Supply_Vis_CB.Click, CW_PreFiltPres_Vis_CB.Click, CW_PostFiltPres_Vis_CB.Click,
                CW_FiltDif_Vis_CB.Click, HW_PreFiltPres_Vis_CB.Click, HW_PostFiltPres_Vis_CB.Click, HW_FiltDif_Vis_CB.Click, HW_Temp_Vis_CB.Click,
                HW_Flow_Vis_CB.Click, ST_FeedWaterPres_Vis_CB.Click, ST_HeadPres_Vis_CB.Click, ST_LowPres_Vis_CB.Click, ST_MedPres_Vis_CB.Click,
                ST_LowDem_Vis_CB.Click, ST_MedDem_Vis_CB.Click, ST_Flow_Vis_CB.Click, EL_NorthA_Vis_CB.Click, EL_NorthB_Vis_CB.Click, EL_NorthC_Vis_CB.Click,
                EL_SouthA_Vis_CB.Click, EL_SouthB_Vis_CB.Click, EL_SouthC_Vis_CB.Click, AR_LinePres_Vis_CB.Click
        Dim Check_Checked As Boolean = True

        Dim sercnt As Integer = 0
        If sender.Checked = True Then
            sender.Font = New Font(sender.Font.FontFamily, sender.Font.Size, FontStyle.Bold)
            For Each i As CheckBox In {CW_Supply_Vis_CB, CW_PreFiltPres_Vis_CB, CW_PostFiltPres_Vis_CB,
                CW_FiltDif_Vis_CB, HW_PreFiltPres_Vis_CB, HW_PostFiltPres_Vis_CB, HW_FiltDif_Vis_CB, HW_Temp_Vis_CB,
                HW_Flow_Vis_CB, ST_FeedWaterPres_Vis_CB, ST_HeadPres_Vis_CB, ST_LowPres_Vis_CB, ST_MedPres_Vis_CB,
                ST_LowDem_Vis_CB, ST_MedDem_Vis_CB, ST_Flow_Vis_CB, EL_NorthA_Vis_CB, EL_NorthB_Vis_CB, EL_NorthC_Vis_CB,
                EL_SouthA_Vis_CB, EL_SouthB_Vis_CB, EL_SouthC_Vis_CB, AR_LinePres_Vis_CB}
                If i.Checked = False Then
                    Check_Checked = False
                Else sercnt += 1
                End If
            Next
            Calc_DataPoints(sercnt)
            All_Vis_CB.Checked = Check_Checked
        Else
            All_Vis_CB.Checked = False
            sender.Font = New Font(sender.Font.FontFamily, sender.Font.Size, FontStyle.Regular)
        End If

    End Sub
    Public Sub Calc_DataPoints(cnt As Integer)
        Dim RowCount As Integer = Convert.ToInt32(SUM_Count_LBL.Text.Split(" ")(0))
        If RowCount > 0 Then
            DataCountLBL.Text = RowCount * cnt & " Points"
        End If
    End Sub
    Private Sub Vis_SelectALL_CheckedChanged(sender As CheckBox, e As EventArgs) Handles All_Vis_CB.Click
        For Each i As CheckBox In {CW_Supply_Vis_CB, CW_PreFiltPres_Vis_CB, CW_PostFiltPres_Vis_CB,
                CW_FiltDif_Vis_CB, HW_PreFiltPres_Vis_CB, HW_PostFiltPres_Vis_CB, HW_FiltDif_Vis_CB, HW_Temp_Vis_CB,
                HW_Flow_Vis_CB, ST_FeedWaterPres_Vis_CB, ST_HeadPres_Vis_CB, ST_LowPres_Vis_CB, ST_MedPres_Vis_CB,
                ST_LowDem_Vis_CB, ST_MedDem_Vis_CB, ST_Flow_Vis_CB, EL_NorthA_Vis_CB, EL_NorthB_Vis_CB, EL_NorthC_Vis_CB,
                EL_SouthA_Vis_CB, EL_SouthB_Vis_CB, EL_SouthC_Vis_CB, AR_LinePres_Vis_CB}
            If sender.Checked = True Then
                i.Font = New Font(sender.Font.FontFamily, sender.Font.Size, FontStyle.Bold)
                i.Checked = True
            Else
                i.Font = New Font(sender.Font.FontFamily, sender.Font.Size, FontStyle.Regular)
                i.Checked = False
            End If
        Next
    End Sub

    Sub PopulatePlotView(StartTime As DateTime, EndTime As DateTime)
        '*******************************************
        'Clear Plot data and color selection panels.
        '*******************************************
        PlotView.Model = Nothing
        For Each Col_panel As Panel In {CW_Supply_Col_BTN, CW_PreFiltPres_Col_BTN, CW_PostFiltPres_Col_BTN,
                CW_FiltDif_Col_BTN, HW_PreFiltPres_Col_BTN, HW_PostFiltPres_Col_BTN, HW_FiltDif_Col_BTN, HW_Temp_Col_BTN,
                HW_Flow_Col_BTN, ST_FeedWaterPres_Col_BTN, ST_HeadPres_Col_BTN, ST_LowPres_Col_BTN, ST_MedPres_Col_BTN,
                ST_LowDem_Col_BTN, ST_MedDem_Col_BTN, ST_Flow_Col_BTN, EL_NorthA_Col_BTN, EL_NorthB_Col_BTN, EL_NorthC_Col_BTN,
                EL_SouthA_Col_BTN, EL_SouthB_Col_BTN, EL_SouthC_Col_BTN, AR_LinePres_Col_BTN}
            Col_panel.BackColor = Color.Transparent
        Next

        Dim SQL As String = My.Settings.SQL_ConString
        Dim query As String = "SELECT Date_Time AS Date_Time, "
        Dim CheckCount As Integer = 0
        Dim CheckSeries As New List(Of String)
        Dim CheckedIndexs As New List(Of Integer)
        Dim CheckIDX As Integer = 0
        Dim cursorLine As OxyPlot.Annotations.LineAnnotation
        Dim infoAnnotation As OxyPlot.Annotations.TextAnnotation

        '*******************************************
        'Build query by looping though all of the series visibility checkboxes
        '*******************************************
        For Each i As CheckBox In {CW_Supply_Vis_CB, CW_PreFiltPres_Vis_CB, CW_PostFiltPres_Vis_CB,
                CW_FiltDif_Vis_CB, HW_PreFiltPres_Vis_CB, HW_PostFiltPres_Vis_CB, HW_FiltDif_Vis_CB, HW_Temp_Vis_CB,
                HW_Flow_Vis_CB, ST_FeedWaterPres_Vis_CB, ST_HeadPres_Vis_CB, ST_LowPres_Vis_CB, ST_MedPres_Vis_CB,
                ST_LowDem_Vis_CB, ST_MedDem_Vis_CB, ST_Flow_Vis_CB, EL_NorthA_Vis_CB, EL_NorthB_Vis_CB, EL_NorthC_Vis_CB,
                EL_SouthA_Vis_CB, EL_SouthB_Vis_CB, EL_SouthC_Vis_CB, AR_LinePres_Vis_CB}
            'for each of the checkboxes in the data selection panel,
            'add the series to the list of names to pull from and
            'build the body of the query string
            If i.Checked = True Then
                CheckCount += 1
                CheckSeries.Add(i.Text)
                CheckedIndexs.Add(CheckIDX)
                query += i.Text & " AS " & i.Text & ", "
            End If
            CheckIDX += 1
        Next
        'Trim end of string to remove ", " and add range string
        query = query.Remove(query.Count - 2)
        query += " FROM a_vals WHERE Date_Time BETWEEN @startDate And @endDate;"

        '*******************************************
        ' Create Plot Model with styling
        '*******************************************

        Dim PlotModel As New OxyPlot.PlotModel With {.Title = "Process Data",
            .IsLegendVisible = True,
            .PlotAreaBackground = OxyColor.FromRgb(13, 13, 13),
            .Background = OxyPlot.OxyColor.FromRgb(10, 10, 10),     ' Plot background
            .TextColor = OxyColors.White,                    ' Axis/Legend text
            .PlotAreaBorderColor = OxyColors.Silver,           ' Border around plot area
            .PlotAreaBorderThickness = New OxyThickness(2)}

        ' --- Common X Axis (Datetime) ---
        PlotModel.Axes.Add(New DateTimeAxis With {
            .Position = AxisPosition.Bottom,
            .StringFormat = "MM/dd/yy" & vbCrLf & "hh:mm:ss",
            .Title = "Time",
            .IntervalType = OxyPlot.Axes.DateTimeIntervalType.Minutes,
            .MajorGridlineStyle = LineStyle.Solid,
            .MajorGridlineColor = OxyColor.FromRgb(64, 64, 64),
            .MinorGridlineStyle = LineStyle.Dot,
            .MinorGridlineColor = OxyColor.FromRgb(32, 32, 32)})
        '*******************************************
        'Connect to the SQL Database
        '*******************************************
        Using conn As New MySqlConnection(SQL)
            conn.Open()
            Using cmd As New MySqlCommand(query, conn)
                'Add start and end time parameter for que
                cmd.Parameters.AddWithValue("@startDate", StartTime)
                cmd.Parameters.AddWithValue("@endDate", EndTime)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    ' --- Prepare list of LineSeries (one per checked series) ---
                    Dim seriesList As New List(Of LineSeries)
                    Dim AxisCnt As Integer = 0
                    For Each name As String In CheckSeries
                        'Create Series with name, Y axis Key, line thickness, and select the color form the preset color list.
                        Dim ser As New LineSeries With {
                            .Title = name,
                            .YAxisKey = name,
                            .StrokeThickness = 1.5,
                            .Color = SeriesColorList(CheckedIndexs(AxisCnt))
                        }
                        seriesList.Add(ser)

                        'create isolation between the series in the plot area
                        Dim iso As Double = 0.25 / CheckCount
                        AxisCnt += 1
                        Dim strpnt As Double = ((AxisCnt - 1) / CheckCount) + (iso / 2)
                        Dim endpnt As Double = (AxisCnt / CheckCount) - (iso / 2)

                        'Create and assign a new Y axis the plot area to link to the series
                        Dim axis As New LinearAxis With {
                            .Position = AxisPosition.Left,
                            .Key = name,
                            .Title = name,
                            .StartPosition = strpnt,
                            .EndPosition = endpnt,
                            .IsZoomEnabled = False,
                            .IsPanEnabled = False,
                            .MajorGridlineStyle = LineStyle.Solid,
                            .MajorGridlineColor = OxyColor.FromRgb(32, 32, 32),
                            .MinorGridlineStyle = LineStyle.Dot,
                            .MinorGridlineColor = OxyColor.FromRgb(32, 32, 32),
                            .AxislineThickness = 3}
                        ' add it to the plot view
                        PlotModel.Axes.Add(axis)

                    Next
                    '*******************************************
                    ' --- Read each line from SQL to populate the series in the plot
                    '*******************************************
                    While reader.Read()
                        'get time and date of datapoint
                        Dim t As Double = DateTimeAxis.ToDouble(reader.GetDateTime("Date_Time"))
                        Dim serCNT As Integer = 0

                        For Each name As String In CheckSeries
                            If Not IsDBNull(reader(name)) Then
                                ' add all data points from this line to their respective series
                                Dim y As Double = Convert.ToDouble(reader(name))
                                seriesList(serCNT).Points.Add(New OxyPlot.DataPoint(t, y))
                                serCNT += 1
                            End If
                        Next
                    End While
                    ' add al the populated series to the plotModel
                    For Each ser As LineSeries In seriesList
                        PlotModel.Series.Add(ser)
                    Next

                End Using
            End Using
        End Using
        '*******************************************
        'Add Cursor object to the plotModel
        '*******************************************
        cursorLine = New OxyPlot.Annotations.LineAnnotation With {
            .Type = LineAnnotationType.Vertical,
            .Color = OxyColors.Yellow,
            .LineStyle = LineStyle.Solid,
            .StrokeThickness = 1.5,
            .X = DateTimeAxis.ToDouble(DateTime.Now),
            .Layer = AnnotationLayer.AboveSeries
        }
        PlotModel.Annotations.Add(cursorLine)
        '*******************************************
        'Add The plot model to the Plot View
        '*******************************************
        PlotView.Model = PlotModel

        For Each ax As LineSeries In PlotView.Model.Series
            If ax IsNot Nothing AndAlso ax.Title IsNot "" Then
                SetControlPropertyByName(ax.Title & "_Col_BTN", "BackColor", OxyColorToColor(ax.Color))
            End If

        Next



    End Sub
    Dim FirstZoomChange As Boolean = False
    Public Sub ZoomPlot(plotModel As PlotModel, span As TimeSpan)
        ' Find the DateTime X axis (bottom axis)
        If FirstZoomChange Then


            Dim xAxis = plotModel.Axes.FirstOrDefault(Function(a) a.Position = AxisPosition.Bottom)
            If xAxis Is Nothing OrElse Not TypeOf xAxis Is DateTimeAxis Then Exit Sub

            ' Convert current axis range to DateTime
            Dim currentStart As DateTime = DateTimeAxis.ToDateTime(xAxis.ActualMinimum)
            Dim newEnd As DateTime = currentStart.Add(span)

            ' Convert back to OxyPlot coordinates
            Dim newMin As Double = DateTimeAxis.ToDouble(currentStart)
            Dim newMax As Double = DateTimeAxis.ToDouble(newEnd)

            ' Apply the zoom to the X axis
            xAxis.Zoom(newMin, newMax)
            plotModel.InvalidatePlot(False)
        Else firstzoomchange = True
        End If
    End Sub

    ' -----------------------------
    ' ORDERED LIST (Index-Based)
    ' -----------------------------
    Public SeriesColorList As New List(Of OxyColor) From {
    OxyColors.DodgerBlue,         ' City Water Pressure
    OxyColors.SteelBlue,          ' Cold Water Pre Filter Pressure
    OxyColors.LightSkyBlue,       ' Cold Water Post Filter Pressure
    OxyColors.OrangeRed,          ' Cold Water Filter Differential
    OxyColors.IndianRed,          ' Hot Water Pre Filter Pressure
    OxyColors.Tomato,             ' Hot Water Post Filter Pressure
    OxyColors.DarkOrange,         ' Hot Water Filter Differential
    OxyColors.Goldenrod,          ' Hot Water Flow Rate
    OxyColors.Red,                ' Hot Water Temperature
    OxyColors.SlateBlue,          ' Boiler Feed Water Pressure
    OxyColors.MediumPurple,       ' Steam Header Pressure
    OxyColors.Orchid,             ' Low Pressure Steam Pressure
    OxyColors.Violet,             ' Medium Pressure Steam Pressure
    OxyColors.Plum,                ' Low Steam Demand  ← NEW
    OxyColors.MediumVioletRed,     ' Medium Steam Demand  ← NEW
    OxyColors.MediumOrchid,       ' Steam Header Flow Rate
    OxyColors.Yellow,             ' North A Phase Voltage To Ground
    OxyColors.Chartreuse,         ' North B Phase Voltage To Ground
    OxyColors.LightGoldenrodYellow, ' North C Phase Voltage To Ground
    OxyColors.Gold,               ' South A Phase Voltage To Ground
    OxyColors.LawnGreen,          ' South B Phase Voltage To Ground
    OxyColors.Khaki,              ' South C Phase Voltage To Ground
    OxyColors.DeepSkyBlue          ' Compressed Air Pressure
   }

    ' -----------------------------
    ' NAME → COLOR DICTIONARY
    ' -----------------------------
    Public SeriesColorDict As New Dictionary(Of String, OxyColor) From {
    {"City Water Pressure", OxyColors.DodgerBlue},
    {"Cold Water Pre Filter Pressure", OxyColors.SteelBlue},
    {"Cold Water Post Filter Pressure", OxyColors.LightSkyBlue},
    {"Cold Water Filter Differential", OxyColors.OrangeRed},
    {"Hot Water Pre Filter Pressure", OxyColors.IndianRed},
    {"Hot Water Post Filter Pressure", OxyColors.Tomato},
    {"Hot Water Filter Differential", OxyColors.DarkOrange},
    {"Hot Water Flow Rate", OxyColors.Goldenrod},
    {"Hot Water Temperature", OxyColors.Red},
    {"Boiler Feed Water Pressure", OxyColors.SlateBlue},
    {"Steam Header Pressure", OxyColors.MediumPurple},
    {"Low Pressure Steam Pressure", OxyColors.Orchid},
    {"Medium Pressure Steam Pressure", OxyColors.Violet},
        {"Low Steam Demand", OxyColors.Plum},                ' ← NEW
    {"Medium Steam Demand", OxyColors.MediumVioletRed},  ' ← NEW
    {"Steam Header Flow Rate", OxyColors.MediumOrchid},
    {"North A Phase Voltage To Ground", OxyColors.Yellow},
    {"North B Phase Voltage To Ground", OxyColors.Chartreuse},
    {"North C Phase Voltage To Ground", OxyColors.LightGoldenrodYellow},
    {"South A Phase Voltage To Ground", OxyColors.Gold},
    {"South B Phase Voltage To Ground", OxyColors.LawnGreen},
    {"South C Phase Voltage To Ground", OxyColors.Khaki},
    {"Compressed Air Pressure", OxyColors.DeepSkyBlue}
}
    'Public Sub MatchSeriesColorsToPanelsByName(plotModel As PlotModel, container As Control)
    '    ' Dim autoColors As OxyColor() = OxyPlot.DefaultColors.Automatic

    '    Dim seriesIndex As Integer = 0

    '    For Each s As LineSeries In plotModel.Series
    '        Dim seriesName As String = s.Title
    '        If String.IsNullOrEmpty(seriesName) Then Continue For

    '        ' Find the matching panel
    '        Dim matchingPanels As Control() = container.Controls.Find(seriesName, True)
    '        If matchingPanels.Length = 0 Then Continue For

    '        Dim pnl As Panel = TryCast(matchingPanels(0), Panel)
    '        If pnl Is Nothing Then Continue For

    '        ' Determine the series color
    '        Dim oxyColor As OxyColor = OxyColors.Transparent

    '        If TypeOf s Is LineSeries Then
    '            Dim ls = DirectCast(s, LineSeries)
    '            oxyColor = If(ls.Color.IsVisible(), ls.Color, autoColors(seriesIndex Mod autoColors.Length))

    '        End If

    '        ' Convert to System.Drawing.Color
    '        Dim panelColor As Color = Color.FromArgb(oxyColor.A, oxyColor.R, oxyColor.G, oxyColor.B)

    '        ' Apply background color
    '        pnl.BackColor = panelColor

    '        seriesIndex += 1
    '    Next
    'End Sub

    Private Sub Chart_Page_Paint(sender As Object, e As PaintEventArgs) Handles Chart_Page.Paint

    End Sub




    Public Sub ExportPlotToSvg(model As PlotModel, filePath As String, Optional width As Integer = 1920, Optional height As Integer = 1080)
        Using stream As New FileStream(filePath, FileMode.Create)
            Dim exporter As New OxyPlot.SvgExporter With {
            .Width = width,
            .Height = height,
            .IsDocument = True
        }
            exporter.Export(model, stream)
        End Using
    End Sub
    Private Sub SaveToFile_BTN_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim FolderLoc As String = "C:/"
        Dim FBD As New FolderBrowserDialog With {
            .AddToRecent = True,
           .ShowNewFolderButton = True,
           .InitialDirectory = FolderLoc}

        Dim FBD_Result As DialogResult = FBD.ShowDialog()
        If FBD_Result = DialogResult.OK Then
            FolderLoc = FBD.SelectedPath
        End If
        ExportPlotToPng(PlotView.Model, FBD.SelectedPath & "/EXPORT.png")
    End Sub

    Private Sub Pull_Grid_Selector_CheckedChanged(sender As Object, e As EventArgs) Handles Pull_Grid_Selector.CheckedChanged
        If sender.checked = True Then
            Dim result As DialogResult = MessageBox.Show("Pulling large amounts of data to a grid view can take several minutes to process" & vbCrLf & "Would you like to proceed?", "Grid Selection", MessageBoxButtons.YesNo, MessageBoxIcon.Hand)
            If result = DialogResult.No Then Pull_Grid_Selector.Checked = False
        End If
    End Sub
End Class

Public Module ColorConverters
    Public Function OxyColorToColor(c As OxyPlot.OxyColor) As System.Drawing.Color
        Return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B)
    End Function

    Public Function ColorToOxyColor(c As System.Drawing.Color) As OxyPlot.OxyColor
        Return OxyPlot.OxyColor.FromArgb(c.A, c.R, c.G, c.B)
    End Function
End Module
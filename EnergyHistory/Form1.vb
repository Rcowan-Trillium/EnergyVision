
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
    Dim DebugMSGs As Boolean = False
    Dim names() As String = {"CW_Supply", "CW_PreFiltPres", "CW_PostFiltPres",
                "CW_FiltDif", "HW_PreFiltPres", "HW_PostFiltPres", "HW_FiltDif", "HW_Temp",
                "HW_Flow", "ST_FeedWaterPres", "ST_HeadPres", "ST_LowPres", "ST_MedPres",
                "ST_LowDem", "ST_MedDem", "ST_Flow", "EL_NorthA", "EL_NorthB", "EL_NorthC",
                "EL_SouthA", "EL_SouthB", "EL_SouthC", "AR_LinePres"}

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


    Private Sub GetNewData(StartPoint As DateTime, EndPoint As DateTime, query As List(Of String))
        Dim SQL As String = My.Settings.SQL_ConString
        Dim SQLquery As String = "SELECT Date_Time AS Date_Time, "
        Dim SerCNT As Integer = 0


        '*******************************************
        'Build query by looping though all of the selected series
        '*******************************************
        For Each i As String In query
            SerCNT += 1
            SQLquery += i & " AS " & i & ", "
        Next
        SQLquery = SQLquery.Remove(SQLquery.Count - 2)
        SQLquery += " FROM a_vals WHERE Date_Time BETWEEN @startDate And @endDate;"

        '*******************************************
        'Get data from the SQL 
        '*******************************************
        Dim totalcount As Integer = 0
        Using connection As New MySqlConnection(SQL)
            'Get Values using start and end date and create a data table
            Dim cmd1 As New MySqlCommand(SQLquery, connection)
            cmd1.Parameters.AddWithValue("@startDate", StartPoint)
            cmd1.Parameters.AddWithValue("@endDate", EndPoint)
            Dim adapter As New MySqlDataAdapter(cmd1)
            Dim table As New DataTable()
            adapter.Fill(table)
            'Populate GridView with new table data
            GridView.VirtualMode = True
            GridView.DataSource = table
        End Using
        '*******************************************
        'Style all cell columns to match the dark theme
        '*******************************************
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

        'GetMoreInfo(DateTimePicker1.Value, DateTimePicker2.Value, sender.text)
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
    Private Function GetRowCountInRange(startpoint As DateTime, endpoint As DateTime)
        Dim totalCount As Integer = 0
        Dim query As String = "SELECT COUNT(*) AS total_rows FROM a_vals WHERE Date_Time BETWEEN @startDate And @endDate;"
        Dim SQL As String = My.Settings.SQL_ConString
        Using connection As New MySqlConnection(SQL)
            Dim cmd As New MySqlCommand(query, connection)
            cmd.Parameters.AddWithValue("@startDate", startpoint)
            cmd.Parameters.AddWithValue("@endDate", endpoint)

            connection.Open()
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    '#####  Info    #####
                    totalCount = Convert.ToInt32(reader("total_rows"))
                End If
            End Using
        End Using
        Return totalCount
    End Function
    Private Function GetSummary(StartPoint As DateTime, EndPoint As DateTime) As Integer
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
            Return totalCount
            'SetControlPropertyByName("SUM_Start_Time", "Text", minDate)
            'SetControlPropertyByName("SUM_End_Time", "Text", maxDate)
        End Using
    End Function
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
    Private Sub Open_Chart_Page(sender As Object, e As EventArgs) Handles Button9.Click, Button3.Click
        OpenNewPage(ChartPanel)
    End Sub
    Private Sub Open_Settings_Page(sender As Object, e As EventArgs) Handles Button4.Click
        OpenNewPage(Settings_Page)
    End Sub
    Dim DTP1_FirstChange As Boolean = False
    Dim DTP2_FirstChange As Boolean = False
    Private Sub DateTimePicker1_ValueChanged(sender As Object, e As EventArgs) Handles DateTimePicker1.ValueChanged, DateTimePicker2.ValueChanged
        If DTP1_FirstChange AndAlso DTP2_FirstChange Then
            Dim TS As New Long
            TS = DateDiff(DateInterval.Second, DateTimePicker1.Value, DateTimePicker2.Value)
            Dim Span As TimeSpan = TimeSpan.FromSeconds(TS)
            ZoomPlot(PlotView.Model, Span)
            If RadioButton1.Checked Then
                TextBox7.Text = Span.TotalHours
            ElseIf RadioButton2.Checked Then
                TextBox7.Text = Span.TotalMinutes
            End If
            'GetSummary(DateTimePicker1.Value, DateTimePicker2.Value)
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

    Sub PopulatePlotView(StartTime As DateTime, EndTime As DateTime, query As List(Of String))
        Dim SQL As String = My.Settings.SQL_ConString
        Dim CheckCount As Integer = query.Count

        '*******************************************
        'Build query by looping though all of the selected series
        '*******************************************
        Dim SQLquery As String = "SELECT Date_Time AS Date_Time, "
        Dim queryBot As String = " FROM a_vals WHERE Date_Time BETWEEN @startDate And @endDate;"
        For Each i As String In query
            SQLquery += i & " AS " & i & ", "
        Next
        SQLquery = SQLquery.Remove(SQLquery.Length - 2) & queryBot
        If DebugMSGs Then MsgBox(SQLquery)
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
        '*******************************************
        'check all of the visibility checkbox's agains the selected series 
        '*******************************************
        Dim CheckIDX As Integer = 0
        Dim CheckedIndexs As New List(Of Integer)
        For Each i As CheckBox In {CW_Supply_Vis_CB, CW_PreFiltPres_Vis_CB, CW_PostFiltPres_Vis_CB,
                CW_FiltDif_Vis_CB, HW_PreFiltPres_Vis_CB, HW_PostFiltPres_Vis_CB, HW_FiltDif_Vis_CB, HW_Temp_Vis_CB,
                HW_Flow_Vis_CB, ST_FeedWaterPres_Vis_CB, ST_HeadPres_Vis_CB, ST_LowPres_Vis_CB, ST_MedPres_Vis_CB,
                ST_LowDem_Vis_CB, ST_MedDem_Vis_CB, ST_Flow_Vis_CB, EL_NorthA_Vis_CB, EL_NorthB_Vis_CB, EL_NorthC_Vis_CB,
                EL_SouthA_Vis_CB, EL_SouthB_Vis_CB, EL_SouthC_Vis_CB, AR_LinePres_Vis_CB}
            Dim found As Boolean = False
            For Each q As String In query
                'check selected series' againts the visiblity checkbox's
                If i.Text.Contains(q) Then
                    CheckedIndexs.Add(CheckIDX)
                    found = True
                    Exit For
                End If
            Next
            'if a series was found that matched the checkbox then enbaled it and check it, otherwise, disbale it and uncheck it
            If found Then
                i.Enabled = True
                i.Checked = True
            Else
                i.Enabled = False
                i.Checked = False
            End If
            CheckIDX += 1
        Next


        '*******************************************
        ' Create Plot Model with styling
        '*******************************************

        Dim PlotModel As New OxyPlot.PlotModel With {.Title = "Found Data",
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
        Dim RowCount As Integer = GetRowCountInRange(StartTime, EndTime)
        Dim pointCount As Integer = RowCount * (query.Count + 1)
        Dim progressPanel As New DataProgressPanel(Me, RowCount, pointCount)
        Using conn As New MySqlConnection(SQL)
            conn.Open()
            Using cmd As New MySqlCommand(SQLquery, conn)
                'Add start and end time parameter for que
                cmd.Parameters.AddWithValue("@startDate", StartTime)
                cmd.Parameters.AddWithValue("@endDate", EndTime)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    ' --- Prepare list of LineSeries (one per checked series) ---
                    Dim seriesList As New List(Of LineSeries)
                    Dim AxisCnt As Integer = 0
                    For Each name As String In query
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
                    Dim CurrentRow As Integer = 0
                    Dim CurrentPoint As Integer = 0
                    Dim cnt10 As Integer = 0
                    Dim cntTo As Integer = RowCount / 100
                    While reader.Read()
                        CurrentRow += 1
                        cnt10 += 1
                        'get time and date of datapoint
                        Dim t As Double = DateTimeAxis.ToDouble(reader.GetDateTime("Date_Time"))
                        Dim serCNT As Integer = 0

                        For Each name As String In query

                            If Not IsDBNull(reader(name)) Then
                                CurrentPoint += 1
                                ' add all data points from this line to their respective series
                                Dim y As Double = Convert.ToDouble(reader(name))
                                seriesList(serCNT).Points.Add(New OxyPlot.DataPoint(t, y))
                                serCNT += 1

                            End If

                            If progressPanel.CancelRequested Then
                                MessageBox.Show("Data pull canceled.")
                                Exit For
                            End If
                        Next

                        If cnt10 >= cntTo Then
                            progressPanel.UpdateProgress(CurrentRow, CurrentPoint, False)
                            cnt10 = 0
                        End If
                    End While
                    progressPanel.UpdateProgress(RowCount, pointCount, True)

                    ' add al the populated series to the plotModel
                    For Each ser As LineSeries In seriesList
                        PlotModel.Series.Add(ser)
                    Next

                End Using
            End Using
        End Using

        '*******************************************
        'Add The plot model to the Plot View
        '*******************************************
        PlotView.Model = PlotModel

        For Each ax As LineSeries In PlotView.Model.Series
            If ax IsNot Nothing AndAlso ax.Title IsNot "" Then
                SetControlPropertyByName(ax.Title & "_Col_BTN", "BackColor", OxyColorToColor(ax.Color))
            End If
        Next
        FitPlotToData(PlotView.Model)
        'Dim xAxis = PlotView.Model.Axes.FirstOrDefault(Function(a) a.Position = AxisPosition.Bottom)
        'If xAxis IsNot Nothing Then
        '    xAxis.Zoom(xAxis.ActualMinimum, xAxis.ActualMaximum)
        'End If
        progressPanel.Close(Me)
    End Sub

    Private Sub FitPlotToData(plotModel As PlotModel)
        If plotModel Is Nothing Then Exit Sub

        ' Find the X and Y ranges based on all series data
        Dim minX As Double = Double.MaxValue
        Dim maxX As Double = Double.MinValue


        For Each s In plotModel.Series.OfType(Of LineSeries)()
            If s.Points.Count > 0 Then
                Dim seriesMinX = s.Points.Min(Function(p) p.X)
                Dim seriesMaxX = s.Points.Max(Function(p) p.X)


                minX = Math.Min(minX, seriesMinX)
                maxX = Math.Max(maxX, seriesMaxX)

            End If
        Next

        ' Apply the X range to your DateTimeAxis
        Dim xAxis = plotModel.Axes.FirstOrDefault(Function(a) a.Position = AxisPosition.Bottom)
        If xAxis IsNot Nothing Then
            If maxX > 43200 Then
                xAxis.Zoom(minX, minX + 43200)
            Else
                xAxis.Zoom(minX, maxX)
            End If

        End If
        ' Refresh
        plotModel.InvalidatePlot(False)
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
        Else FirstZoomChange = True
        End If
    End Sub

    ' -----------------------------
    ' ORDERED LIST (Index-Based)
    ' -----------------------------
    Public SeriesColorList As New List(Of OxyColor) From {
    OxyColors.DodgerBlue,         ' City Water Pressure
    OxyColors.SteelBlue,          ' Cold Water Pre Filter Pressure
    OxyColors.LightSkyBlue,       ' Cold Water Post Filter Pressure
    OxyColors.AliceBlue,          ' Cold Water Filter Differential
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
    {"Cold Water Filter Differential", OxyColors.AliceBlue},
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
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim result As DialogResult = MsgBox("Clear all data from chart and select new range?", MsgBoxStyle.OkCancel, "Clear Data")
        If result = DialogResult.OK Then
            '*******************************************
            'Show Data Selection page and wait for user to enter data.
            '*******************************************
            Dim Selection As SeriesSelectorResult = SeriesSelector.ShowSeriesSelector(Me, SeriesNames_x2)
            'If data is not empty then perform data pull for selected data
            If Selection IsNot Nothing Then
                'If grid was selected run pull the selected data to the data grid
                If Selection.ShowGrid Then
                    GetNewData(Selection.StartDate, Selection.EndDate, Selection.SelectedSeries)
                End If
                'if Chart was sleected run pull the selected data to the chart
                If Selection.ShowChart Then
                    PopulatePlotView(Selection.StartDate, Selection.EndDate, Selection.SelectedSeries)
                End If
                'set min and max dates on the date and time pickers for selecting range.
                DateTimePicker1.MinDate = Selection.StartDate
                DateTimePicker1.MaxDate = Selection.EndDate
                DateTimePicker1.Value = Selection.StartDate
                DateTimePicker2.MinDate = Selection.StartDate
                DateTimePicker2.MaxDate = Selection.EndDate
                DateTimePicker2.Value = Selection.EndDate
            End If

        End If
    End Sub
End Class

Public Class SeriesSelectorResult
    Public Property SelectedSeries As List(Of String)
    Public Property StartDate As DateTime
    Public Property EndDate As DateTime
    Public Property ShowChart As Boolean
    Public Property ShowGrid As Boolean
End Class

Public Class SeriesSelector
    Public Shared Function ShowSeriesSelector(parent As Form, series As List(Of String)) As SeriesSelectorResult
        ' === Create panel ===
        Dim pnl As New Panel With {
            .Size = New Size(350, 600),
            .BackColor = Color.FromArgb(40, 40, 40),
            .BorderStyle = BorderStyle.FixedSingle
        }

        pnl.Location = New Point((parent.ClientSize.Width - pnl.Width) \ 2,
                                 (parent.ClientSize.Height - pnl.Height) \ 2)

        ' === "Select All" Checkbox ===
        Dim chkSelectAll As New CheckBox With {
            .Text = "Select All",
            .ForeColor = Color.White,
            .BackColor = Color.FromArgb(40, 40, 40),
            .AutoSize = True,
            .Location = New Point(10, 10),
            .ThreeState = True,
            .CheckState = CheckState.Checked
        }
        pnl.Controls.Add(chkSelectAll)

        ' === CheckedListBox for series ===
        Dim clb As New CheckedListBox With {
            .Location = New Point(10, 35),
            .Size = New Size(330, 300),
            .CheckOnClick = True,
            .BackColor = Color.FromArgb(55, 55, 55),
            .ForeColor = Color.White,
            .BorderStyle = BorderStyle.FixedSingle
        }
        For Each s In series
            clb.Items.Add(s, True)
        Next
        pnl.Controls.Add(clb)

        ' === Tri-State Logic ===
        Dim updatingSelectAll As Boolean = False
        AddHandler chkSelectAll.CheckStateChanged, Sub()
                                                       If updatingSelectAll Then Return
                                                       updatingSelectAll = True
                                                       Select Case chkSelectAll.CheckState
                                                           Case CheckState.Checked
                                                               For i As Integer = 0 To clb.Items.Count - 1
                                                                   clb.SetItemChecked(i, True)
                                                               Next
                                                           Case CheckState.Unchecked
                                                               For i As Integer = 0 To clb.Items.Count - 1
                                                                   clb.SetItemChecked(i, False)
                                                               Next
                                                       End Select
                                                       updatingSelectAll = False
                                                   End Sub

        AddHandler clb.ItemCheck, Sub(sender, e)
                                      parent.BeginInvoke(CType(Sub()
                                                                   If updatingSelectAll Then Return
                                                                   updatingSelectAll = True
                                                                   Dim total = clb.Items.Count
                                                                   Dim checkedCount = clb.CheckedItems.Count
                                                                   If e.NewValue = CheckState.Checked Then
                                                                       checkedCount += 1
                                                                   ElseIf e.NewValue = CheckState.Unchecked Then
                                                                       checkedCount -= 1
                                                                   End If

                                                                   If checkedCount = total Then
                                                                       chkSelectAll.CheckState = CheckState.Checked
                                                                   ElseIf checkedCount = 0 Then
                                                                       chkSelectAll.CheckState = CheckState.Unchecked
                                                                   Else
                                                                       chkSelectAll.CheckState = CheckState.Indeterminate
                                                                   End If
                                                                   updatingSelectAll = False
                                                               End Sub, Action))
                                  End Sub

        ' === Labels ===
        Dim lblStart As New Label With {
            .Text = "Start Date/Time:",
            .ForeColor = Color.White,
            .Location = New Point(10, 345),
            .AutoSize = True
        }
        Dim lblEnd As New Label With {
            .Text = "End Date/Time:",
            .ForeColor = Color.White,
            .Location = New Point(10, 405),
            .AutoSize = True
        }
        pnl.Controls.Add(lblStart)
        pnl.Controls.Add(lblEnd)

        ' === DateTimePickers ===
        Dim dtpStart As New DateTimePicker With {
            .Format = DateTimePickerFormat.Custom,
            .CustomFormat = "MM/dd/yyyy HH:mm:ss",
            .Location = New Point(10, 365),
            .Width = 330,
            .Value = DateTime.Now.AddHours(-1)
        }
        Dim dtpEnd As New DateTimePicker With {
            .Format = DateTimePickerFormat.Custom,
            .CustomFormat = "MM/dd/yyyy HH:mm:ss",
            .Location = New Point(10, 425),
            .Width = 330,
            .Value = DateTime.Now
        }
        pnl.Controls.Add(dtpStart)
        pnl.Controls.Add(dtpEnd)

        ' === Output Options ===
        Dim lblOutput As New Label With {
            .Text = "Display Options:",
            .ForeColor = Color.White,
            .Location = New Point(10, 465),
            .AutoSize = True
        }
        pnl.Controls.Add(lblOutput)

        Dim chkShowChart As New CheckBox With {
            .Text = "Show in Chart",
            .ForeColor = Color.White,
            .BackColor = Color.FromArgb(40, 40, 40),
            .AutoSize = True,
            .Location = New Point(20, 485),
            .Checked = True
        }
        pnl.Controls.Add(chkShowChart)

        Dim chkShowGrid As New CheckBox With {
            .Text = "Show in Grid",
            .ForeColor = Color.White,
            .BackColor = Color.FromArgb(40, 40, 40),
            .AutoSize = True,
            .Location = New Point(150, 485)
        }
        pnl.Controls.Add(chkShowGrid)

        ' === OK and Cancel buttons ===
        Dim btnOk As New Button With {
            .Text = "OK",
            .Size = New Size(120, 35),
            .Location = New Point(40, 530),
            .BackColor = Color.FromArgb(70, 70, 70),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat
        }

        Dim btnCancel As New Button With {
            .Text = "Cancel",
            .Size = New Size(120, 35),
            .Location = New Point(180, 530),
            .BackColor = Color.FromArgb(70, 70, 70),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat
        }
        pnl.Controls.Add(btnOk)
        pnl.Controls.Add(btnCancel)

        ' === Add and show ===
        parent.Controls.Add(pnl)
        pnl.BringToFront()
        pnl.Visible = True

        ' === State handling ===
        Dim result As SeriesSelectorResult = Nothing
        Dim done As Boolean = False

        AddHandler btnOk.Click,
            Sub()
                result = New SeriesSelectorResult With {
                    .SelectedSeries = clb.CheckedItems.Cast(Of String)().ToList(),
                    .StartDate = dtpStart.Value,
                    .EndDate = dtpEnd.Value,
                    .ShowChart = chkShowChart.Checked,
                    .ShowGrid = chkShowGrid.Checked
                }
                done = True
            End Sub

        AddHandler btnCancel.Click,
            Sub()
                result = Nothing
                done = True
            End Sub

        ' === Wait loop ===
        Do While Not done
            Application.DoEvents()
            Threading.Thread.Sleep(10)
        Loop

        parent.Controls.Remove(pnl)
        pnl.Dispose()

        Return result
    End Function
End Class

Public Class DataProgressPanel
    Private pnl As Panel
    Private lblTitle As Label
    Private lblRows As Label
    Private lblPoints As Label
    Private progress As ProgressBar
    Private btnCancel As Button

    Private totalRows As Integer
    Private totalPoints As Integer

    Public Property CancelRequested As Boolean = False

    Public Sub New(parent As Form, totalRows As Integer, totalPoints As Integer)
        Me.totalRows = totalRows
        Me.totalPoints = totalPoints

        ' === Panel ===
        pnl = New Panel With {
            .Size = New Size(400, 200),
            .BackColor = Color.FromArgb(40, 40, 40),
            .BorderStyle = BorderStyle.FixedSingle
        }
        pnl.Location = New Point((parent.ClientSize.Width - pnl.Width) \ 2,
                                 (parent.ClientSize.Height - pnl.Height) \ 2)

        ' === Title ===
        lblTitle = New Label With {
            .Text = "Retrieving Data...",
            .Font = New Font("Segoe UI", 12, FontStyle.Bold),
            .ForeColor = Color.White,
            .AutoSize = False,
            .TextAlign = ContentAlignment.MiddleCenter,
            .Dock = DockStyle.Top,
            .Height = 40
        }
        pnl.Controls.Add(lblTitle)

        ' === Progress Bar ===
        progress = New ProgressBar With {
            .Location = New Point(30, 60),
            .Size = New Size(340, 25),
            .Style = ProgressBarStyle.Continuous
        }
        pnl.Controls.Add(progress)

        ' === Row Label ===
        lblRows = New Label With {
            .Text = $"Rows: 0 / {totalRows}",
            .ForeColor = Color.White,
            .Location = New Point(30, 100),
            .AutoSize = True
        }
        pnl.Controls.Add(lblRows)

        ' === Data Points Label ===
        lblPoints = New Label With {
            .Text = $"Data Points: 0 / {totalPoints}",
            .ForeColor = Color.White,
            .Location = New Point(30, 125),
            .AutoSize = True
        }
        pnl.Controls.Add(lblPoints)

        ' === Cancel Button ===
        btnCancel = New Button With {
            .Text = "Cancel",
            .Size = New Size(100, 30),
            .Location = New Point((pnl.Width - 100) \ 2, 160),
            .BackColor = Color.FromArgb(70, 70, 70),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat
        }
        AddHandler btnCancel.Click, Sub() CancelRequested = True
        pnl.Controls.Add(btnCancel)

        parent.Controls.Add(pnl)
        pnl.BringToFront()
        pnl.Visible = True
    End Sub

    ' === Update progress ===
    Public Sub UpdateProgress(currentRow As Integer, currentPoint As Integer, complete As Boolean)
        Dim percent As Integer = CInt((currentRow / totalRows) * 100)
        If complete Then
            lblRows.Text = $"Rows: {totalRows} / {totalRows}"
            lblRows.Invalidate()
            lblPoints.Text = $"Data Points: {totalPoints} / {totalPoints}"
            lblPoints.Invalidate()
            progress.Value = 100
            progress.Invalidate()
            lblTitle.Text = "Load Data to Chart Please Wait..."
            lblTitle.Invalidate()

        Else
            If percent > 100 Then percent = 100
            lblRows.Text = $"Rows: {currentRow} / {totalRows}"
            lblPoints.Text = $"Data Points: {currentPoint} / {totalPoints}"
            progress.Value = percent
        End If

        Application.DoEvents()
    End Sub

    ' === Close ===
    Public Sub Close(parent As Form)
        parent.Controls.Remove(pnl)
        pnl.Dispose()
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
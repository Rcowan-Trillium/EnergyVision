Imports System.Data.SqlClient
Imports System.Math
Imports System.Net
Imports System.Text
Imports System.Windows.Forms.DataVisualization.Charting
Imports MadMilkman.Ini
Imports OxyPlot
Imports Microsoft.Win32
Imports MySql.Data.MySqlClient


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
    Private Sub seriesColorChange(sender As Object, e As MouseEventArgs) Handles CW_Supply_Col_BTN.Click, CW_PreFiltPres_Col_BTN.Click, CW_PostFiltPres_Col_BTN.Click,
                CW_FiltDif_Col_BTN.Click, HW_PreFiltPres_Col_BTN.Click, HW_PostFiltPres_Col_BTN.Click, HW_FiltDif_Col_BTN.Click, HW_Temp_Col_BTN.Click,
                HW_Flow_Col_BTN.Click, ST_FeedWaterPres_Col_BTN.Click, ST_HeadPres_Col_BTN.Click, ST_LowPres_Col_BTN.Click, ST_MedPres_Col_BTN.Click,
                ST_LowDem_Col_BTN.Click, ST_MedDem_Col_BTN.Click, ST_Flow_Col_BTN.Click, EL_NorthA_Col_BTN.Click, EL_NorthB_Col_BTN.Click, EL_NorthC_Col_BTN.Click,
                EL_SouthA_Col_BTN.click, EL_SouthB_Col_BTN.Click, EL_SouthC_Col_BTN.Click, AR_LinePres_Col_BTN.Click

        Dim CPicker As New ColorDialog
        CPicker.ShowDialog()
        ChartView.Series(sender.name.split("_Col_")(0)).Color = CPicker.Color
        sender.backcolor = CPicker.Color
        ChartView.Update()
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

    Private Sub Chart1_MouseClick(sender As Object, e As MouseEventArgs) Handles ChartView.MouseClick
        Dim ca As ChartArea = ChartView.ChartAreas(0)
        Dim xVal As Double = ca.AxisX.PixelPositionToValue(e.X)
        Dim clickedTime As DateTime = DateTime.FromOADate(xVal)
        Label42.Text = clickedTime
        For Each ca_i As ChartArea In ChartView.ChartAreas
            ca_i.CursorX.Position = xVal
            ca_i.CursorX.LineColor = Color.Lime
            ca_i.CursorX.LineWidth = 1
            ca_i.CursorX.IsUserEnabled = True
            ca_i.CursorX.IsUserSelectionEnabled = False

            For Each s As Series In ChartView.Series
                Dim closestPoint As DataPoint = Nothing
                Dim minDiff As Double = Double.MaxValue

                ' Find nearest point in this series
                For Each pt As DataPoint In s.Points
                    Dim diff = Math.Abs(pt.XValue - xVal)
                    If diff < minDiff Then
                        minDiff = diff
                        closestPoint = pt
                    End If
                Next
                'set the cursor label as the closet value
                If closestPoint IsNot Nothing Then

                    Dim yV As Double = closestPoint.YValues(0)
                    SetControlPropertyByName(s.Name & "_Cursor_LBL", "Text", yV.ToString("0.00"))
                End If
            Next
        Next
    End Sub

    Private Sub LoadChartData_test(StartPoint As DateTime, EndPoint As DateTime)
        'used to place chart areas in the chart
        Dim lastPos As New Point(0, 0)
        'get saved connection string
        Dim SQL As String = My.Settings.SQL_ConString
        'build the start or the query string 
        Dim query As String = "SELECT Date_Time AS Date_Time, "

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
                CheckSeries.Add(i.Text)
                query += i.Text & " AS " & i.Text & ", "
            End If

        Next
        query = query.Remove(query.Count - 2)
        query += " FROM a_vals WHERE Date_Time BETWEEN @startDate And @endDate;"

        'start new SQL connection and get data using the specified query
        Using conn As New MySqlConnection(SQL)
            Try
                conn.Open()
                Dim cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@startDate", StartPoint)
                cmd.Parameters.AddWithValue("@endDate", EndPoint)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                'Clear all Existing Chart Series and Chart Areas before creating new ones.
                ChartView.Series.Clear()
                ChartView.ChartAreas.Clear()

                ' for each of the series in the list of series names; .
                For Each i As String In CheckSeries


                    'create a new chart area from the template using the series name
                    Dim ca As New ChartArea(i)
                    'Area Template
                    ca.BackColor = Color.fromargb(10,10,10)
                    ca.IsSameFontSizeForAllAxes = True
                    'XAxis Template
                    ca.AxisX.MajorTickMark.Enabled = False
                    ca.AxisX.LabelStyle.Angle = 45
                    ca.AxisX.LabelStyle.Enabled = False
                    ca.AxisX.LabelStyle.Interval = 15
                    ca.AxisX.LabelStyle.ForeColor = Color.Silver
                    ca.AxisX.LineColor = Color.FromArgb(64, 64, 64)
                    ca.AxisX.MajorGrid.LineColor = Color.FromArgb(48, 48, 48)
                    ca.AxisX.IsMarginVisible = False
                    ca.AxisX.MajorTickMark.Enabled = False
                    ca.CursorX.IsUserSelectionEnabled = True
                    ca.CursorX.IsUserEnabled = True
                    ca.CursorX.LineColor = Color.Red
                    ca.CursorX.LineWidth = 1
                    ca.CursorX.LineDashStyle = DataVisualization.Charting.ChartDashStyle.Dash
                    'YAxis Template
                    ca.AxisY.TitleForeColor = Color.Silver
                    ca.AxisY.Title = i
                    ca.AxisY.LabelStyle.Font = New Font("Segoe UI", 8)
                    ca.AxisY.LabelStyle.Format = "0"
                    ca.AxisY.LabelStyle.ForeColor = Color.White
                    ca.AxisY.LineColor = Color.FromArgb(64, 64, 64)
                    ca.AxisY.MajorGrid.LineColor = Color.FromArgb(48, 48, 48)
                    ca.AxisY.MajorGrid.Interval = 50
                    ca.AxisY.MinorGrid.Enabled = True
                    ca.AxisY.MinorGrid.LineColor = Color.FromArgb(20, 20, 20)
                    ca.AxisY.IsMarginVisible = False
                    ca.AxisY.IntervalAutoMode = False
                    ca.AxisY.MajorTickMark.Enabled = False
                    ca.CursorY.IsUserSelectionEnabled = True
                    ca.CursorY.IsUserEnabled = False
                    ca.CursorY.LineColor = Color.Lime
                    ca.CursorY.LineWidth = 1
                    ca.CursorY.LineDashStyle = DataVisualization.Charting.ChartDashStyle.Dash
                    'Calculate position of the next area
                    Dim hgt As Single = 100 / CheckSeries.Count
                    If CheckSeries.Count > 6 Then hgt = 100 / CheckSeries.Count Else hgt = 100 / 12

                    ca.Position = New ElementPosition(lastPos.X, lastPos.Y, 99, hgt)
                    lastPos = New Point(lastPos.X, (lastPos.Y + hgt))
                    'Aligne all chart areas with the first chart area
                    If i IsNot CheckSeries(0) Then
                        ca.AlignWithChartArea = CheckSeries(0)
                        ca.AlignmentOrientation = AreaAlignmentOrientations.Vertical
                        ca.AlignmentStyle = AreaAlignmentStyles.PlotPosition
                    End If
                    'Add the new chart area to the chart
                    ChartView.ChartAreas.Add(ca)
                    'Add the series for the chart area to the chart area
                    Dim sr As New Series(i)
                    sr.ChartType = SeriesChartType.Line
                    sr.ChartArea = i
                    sr.BorderWidth = 1
                    'add the series to the chart area
                    ChartView.Series.Add(sr)

                Next

                'Read each row in the data; Populate series with data 
                While reader.Read()
                    'Get all data from each column 

                    Dim points As New List(Of Double)
                    For Each i As String In CheckSeries
                        points.Add(reader(i))
                    Next

                    Dim serCNT As Integer = 0
                    For Each i As String In CheckSeries
                        ChartView.Series(i).Points.AddXY(reader("Date_Time"), points(serCNT))
                        serCNT += 1
                    Next

                End While
                For Each i As ChartArea In ChartView.ChartAreas
                    Dim max As Double = Math.Round(ChartView.Series(i.Name).Points.FindMaxByValue().YValues(0), 1) + 2
                    Dim min As Double = Math.Round(ChartView.Series(i.Name).Points.FindMinByValue().YValues(0), 1) - 2
                    i.AxisY.Maximum = max
                    i.AxisY.Minimum = min
                    i.AxisY.LabelStyle.Interval = (max - min) / 5
                    i.AxisY.MajorGrid.Interval = (max - min) / 5
                    i.AxisY.MinorGrid.Interval = ((max - min) / 5) / 2
                    i.AxisY.MinorGrid.Enabled = True
                Next



            Catch ex As Exception
                MessageBox.Show("Error:  " & ex.Message)
            End Try
        End Using
    End Sub
    Private Sub LoadChartData(StartPoint As DateTime, EndPoint As DateTime)


        'SQL Database query string to get all data from the tables
        Dim query As String = "SELECT Date_Time AS Date_Time, " &
            "CW_Supply AS CW_Supply, CW_PreFiltPres AS CW_PreFiltPres, CW_PostFiltPres AS CW_PostFiltPres, CW_FiltDif AS CW_FiltDif, " &
            "HW_PreFiltPres AS HW_PreFiltPres, HW_PostFiltPres AS HW_PostFiltPres, HW_FiltDif AS HW_FiltDif, HW_Temp AS HW_Temp, HW_Flow AS HW_Flow, " &
            "ST_FeedWaterPres AS ST_FeedWaterPres, ST_HeadPres AS ST_HeadPres, ST_LowPres AS ST_LowPres, ST_MedPres AS ST_MedPres, ST_LowDem AS ST_LowDem, ST_MedDem AS ST_MedDem, ST_Flow AS ST_Flow, " &
            "EL_NorthA AS EL_NorthA, EL_NorthB AS EL_NorthB, EL_NorthC AS EL_NorthC, EL_SouthA AS EL_SouthA, EL_SouthB AS EL_SouthB, EL_SouthC AS EL_SouthC, " &
            "AR_LinePres AS AR_LinePres FROM a_vals WHERE Date_Time BETWEEN @startDate And @endDate;"


        'start new SQL connection and get datausing the specified query
        Dim SQL As String = My.Settings.SQL_ConString
        Using conn As New MySqlConnection(SQL)
            Try
                conn.Open()
                Dim cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@startDate", StartPoint)
                cmd.Parameters.AddWithValue("@endDate", EndPoint)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()


                'Clear all Existing Chart Series and Chart Areas before creating new ones.
                ChartView.Series.Clear()
                ChartView.ChartAreas.Clear()

                Dim SeriesNames() As String = {"CW_Supply", "CW_PreFiltPres", "CW_PostFiltPres",
                "CW_FiltDif", "HW_PreFiltPres", "HW_PostFiltPres", "HW_FiltDif", "HW_Temp",
                "HW_Flow", "ST_FeedWaterPres", "ST_HeadPres", "ST_LowPres", "ST_MedPres",
                "ST_LowDem", "ST_MedDem", "ST_Flow", "EL_NorthA", "EL_NorthB", "EL_NorthC",
                "EL_SouthA", "EL_SouthB", "EL_SouthC", "AR_LinePres"}


                Dim lastPos As New Point(0, 0)

                ' for each of the series in the list of series names; .
                For Each i As String In SeriesNames
                    'add series to the checked list box for visiblity selection.
                    ' CheckedListBox1.Items.Add(i, True)
                    'create a new chart area from the template using the series name
                    Dim ca As New ChartArea(i)
                    'Area Template
                    ca.BackColor = Color.Black
                    ca.IsSameFontSizeForAllAxes = True
                    ' ca.AxisX.ScaleView.Zoomable = True
                    'ca.AxisX.ScrollBar.Enabled = True
                    'XAxis Template
                    ca.AxisX.MajorTickMark.Enabled = False
                    ca.AxisX.LabelStyle.Angle = 45
                    ca.AxisX.LabelStyle.Enabled = False
                    ca.AxisX.LabelStyle.Interval = 15
                    ca.AxisX.LabelStyle.ForeColor = Color.Silver
                    ca.AxisX.LineColor = Color.FromArgb(64, 64, 64)
                    ca.AxisX.MajorGrid.LineColor = Color.FromArgb(48, 48, 48)
                    ca.AxisX.IsMarginVisible = False
                    ca.AxisX.MajorTickMark.Enabled = False
                    ca.CursorX.IsUserSelectionEnabled = False
                    ca.CursorX.IsUserEnabled = True
                    ca.CursorX.LineColor = Color.Red
                    ca.CursorX.LineWidth = 1
                    ca.CursorX.LineDashStyle = DataVisualization.Charting.ChartDashStyle.Dash
                    'YAxis Template
                    ca.AxisY.TitleForeColor = Color.Silver
                    ca.AxisY.Title = i
                    ca.AxisY.LabelStyle.Font = New Font("Segoe UI", 8)
                    ca.AxisY.LabelStyle.Format = "0"
                    ca.AxisY.LabelStyle.ForeColor = Color.White
                    ca.AxisY.LineColor = Color.FromArgb(64, 64, 64)
                    ca.AxisY.MajorGrid.LineColor = Color.FromArgb(48, 48, 48)
                    ca.AxisY.MajorGrid.Interval = 50
                    ca.AxisY.MinorGrid.Enabled = True
                    ca.AxisY.MinorGrid.LineColor = Color.FromArgb(20, 20, 20)
                    ca.AxisY.IsMarginVisible = False
                    ca.AxisY.IntervalAutoMode = False
                    ca.AxisY.MajorTickMark.Enabled = False
                    ca.CursorY.IsUserSelectionEnabled = False
                    ca.CursorY.IsUserEnabled = False
                    ca.CursorY.LineColor = Color.Lime
                    ca.CursorY.LineWidth = 1
                    ca.CursorY.LineDashStyle = DataVisualization.Charting.ChartDashStyle.Dash

                    'Calculate position of the next area
                    Dim hgt As Single = 100 / 23
                    ca.Position = New ElementPosition(lastPos.X, lastPos.Y, 99, hgt)
                    lastPos = New Point(lastPos.X, (lastPos.Y + hgt))
                    'Aligne all chart areas with the first chart area
                    If i IsNot "CW_Supply" Then
                        ca.AlignWithChartArea = "CW_Supply"
                        ca.AlignmentOrientation = AreaAlignmentOrientations.Vertical
                        ca.AlignmentStyle = AreaAlignmentStyles.PlotPosition
                    End If
                    'Add the new chart area to the chart
                    ChartView.ChartAreas.Add(ca)
                    'Add the series for the chart area to the chart area
                    Dim sr As New Series(i)

                    sr.ChartType = SeriesChartType.Line
                    sr.ChartArea = i
                    sr.BorderWidth = 1
                    ChartView.Series.Add(sr)

                Next
                '' Read each row in the data; Populate series with data 
                While reader.Read()
                    'Get all data from each column 
                    Dim points As Double() = {reader("CW_Supply"), reader("CW_PreFiltPres"),
                        reader("CW_PostFiltPres"), reader("CW_FiltDif"), reader("HW_PreFiltPres"),
                        reader("HW_PostFiltPres"), reader("HW_FiltDif"), reader("HW_Temp"),
                        reader("HW_Flow"), reader("ST_FeedWaterPres"), reader("ST_HeadPres"),
                        reader("ST_LowPres"), reader("ST_MedPres"), reader("ST_LowDem"),
                        reader("ST_MedDem"), reader("ST_Flow"), reader("EL_NorthA"),
                        reader("EL_NorthB"), reader("EL_NorthC"), reader("EL_SouthA"),
                        reader("EL_SouthB"), reader("EL_SouthC"), reader("AR_LinePres")}

                    Dim serCNT As Integer = 0
                    For Each i As String In {"CW_Supply", "CW_PreFiltPres",
                        "CW_PostFiltPres", "CW_FiltDif", "HW_PreFiltPres",
                        "HW_PostFiltPres", "HW_FiltDif", "HW_Temp",
                        "HW_Flow", "ST_FeedWaterPres", "ST_HeadPres",
                        "ST_LowPres", "ST_MedPres", "ST_LowDem",
                        "ST_MedDem", "ST_Flow", "EL_NorthA",
                        "EL_NorthB", "EL_NorthC", "EL_SouthA",
                        "EL_SouthB", "EL_SouthC", "AR_LinePres"}
                        ChartView.Series(i).Points.AddXY(reader("Date_Time"), points(serCNT))
                        serCNT += 1

                    Next

                End While
                For Each i As ChartArea In ChartView.ChartAreas
                    Dim max As Double = Math.Round(ChartView.Series(i.Name).Points.FindMaxByValue().YValues(0), 1) + 2
                    Dim min As Double = Math.Round(ChartView.Series(i.Name).Points.FindMinByValue().YValues(0), 1) - 2
                    i.AxisY.Maximum = max
                    i.AxisY.Minimum = min
                    i.AxisY.LabelStyle.Interval = (max - min) / 5
                    i.AxisY.MajorGrid.Interval = (max - min) / 5
                    i.AxisY.MinorGrid.Interval = ((max - min) / 5) / 2
                    i.AxisY.MinorGrid.Enabled = True
                Next



            Catch ex As Exception
                MessageBox.Show("Error:  " & ex.Message)
            End Try
        End Using
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
            SetControlPropertyByName("SUM_Start_Time", "Text", minDate)
            SetControlPropertyByName("SUM_End_Time", "Text", maxDate)
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
        Dim Start_DateTime As Date = DateTimePicker1.Value '.Month & "/" & DateTimePicker1.Value.Day & "/" & DateTimePicker1.Value.Year & " " & Start_Hour.Value & ":" & Start_Minute.Value.ToString("00") & ":" & "00"
        Dim End_DateTime As Date = DateTimePicker2.Value '.Month & "/" & DateTimePicker2.Value.Day & "/" & DateTimePicker2.Value.Year & " " & End_Hour.Value & ":" & End_Minute.Value.ToString("00") & ":" & "00"
        GetNewData(Start_DateTime, End_DateTime)
        LoadChartData_test(Start_DateTime, End_DateTime)
    End Sub
    Sub OpenNewPage(Page As Object)
        Summary_Page.Hide()
        Chart_Page.Hide()
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
        OpenNewPage(Chart_Page)
    End Sub
    Private Sub Open_Settings_Page(sender As Object, e As EventArgs) Handles Button4.Click
        OpenNewPage(Settings_Page)
    End Sub
    Private Sub DateTimePicker1_ValueChanged(sender As Object, e As EventArgs) Handles DateTimePicker1.ValueChanged, DateTimePicker2.ValueChanged
        GetSummary(DateTimePicker1.Value, DateTimePicker2.Value)
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
        Dim query As String = "SELECT Date_Time AS Date_Time, "
        Dim CheckSeries As New List(Of String)
        For Each i As CheckBox In {CW_Supply_Vis_CB, CW_PreFiltPres_Vis_CB, CW_PostFiltPres_Vis_CB,
                CW_FiltDif_Vis_CB, HW_PreFiltPres_Vis_CB, HW_PostFiltPres_Vis_CB, HW_FiltDif_Vis_CB, HW_Temp_Vis_CB,
                HW_Flow_Vis_CB, ST_FeedWaterPres_Vis_CB, ST_HeadPres_Vis_CB, ST_LowPres_Vis_CB, ST_MedPres_Vis_CB,
                ST_LowDem_Vis_CB, ST_MedDem_Vis_CB, ST_Flow_Vis_CB, EL_NorthA_Vis_CB, EL_NorthB_Vis_CB, EL_NorthC_Vis_CB,
                EL_SouthA_Vis_CB, EL_SouthB_Vis_CB, EL_SouthC_Vis_CB, AR_LinePres_Vis_CB}

            If i.Checked = True Then
                CheckSeries.Add(i.Text)
                query += i.Text & " AS " & i.Text & ", "
            End If

        Next
        query = query.Remove(query.Count - 2)
        query += " FROM a_vals WHERE Date_Time BETWEEN @startDate And @endDate;"
        MsgBox(query)
    End Sub

    Dim VisEdit As Boolean = False

End Class

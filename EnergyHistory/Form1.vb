Imports System.Data.SqlClient
Imports System.Net
Imports System.Text
Imports System.Windows.Forms.DataVisualization.Charting

Imports MadMilkman.Ini
Imports Microsoft.Win32
Imports MySql.Data.MySqlClient
Imports Mysqlx

Public Class Form1

    Private Sub EnerygyVision_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
    Dim SQL_ConString As String = "server=localhost;user id=System;password=System;database=facilitylog"
    Dim AlarmsInList As Integer = 250


    Private Function GetNewData(StartPoint As DateTime, EndPoint As DateTime) As Integer

        Dim query1 As String = "SELECT * FROM a_vals " &
                      "WHERE Date_Time BETWEEN @startDate AND @endDate " &
                      "ORDER BY Date_Time DESC;"



        Dim totalcount As Integer = 0
        Using connection As New MySqlConnection(SQL_ConString)
            Dim cmd1 As New MySqlCommand(query1, connection)
            cmd1.Parameters.AddWithValue("@startDate", StartPoint)
            cmd1.Parameters.AddWithValue("@endDate", EndPoint)
            Dim adapter As New MySqlDataAdapter(cmd1)
            Dim table As New DataTable()
            adapter.Fill(table)

            GridView.DataSource = table
            ChartView.DataSource = table

        End Using

        Return totalcount

        GridView.Columns(1).DefaultCellStyle.Format = "MM/dd/yyyy hh:mm:ss tt"

    End Function

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
                     "FROM a_vals " & "WHERE Date_Time BETWEEN @startDate AND @endDate;"

        Using connection As New MySqlConnection(SQL_ConString)
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

                    If Not IsDBNull(reader("start_date")) Then
                        minDate = Convert.ToDateTime(reader("start_date"))
                    End If

                    If Not IsDBNull(reader("end_date")) Then
                        maxDate = Convert.ToDateTime(reader("end_date"))
                    End If
                    '#####  CW_Supply   #####
                    If Not IsDBNull(reader("CW_Supply_MAX")) Then
                        maxValue(0) = Convert.ToDouble(reader("CW_Supply_MAX"))
                    End If

                    If Not IsDBNull(reader("CW_Supply_MIN")) Then
                        minValue(0) = Convert.ToDouble(reader("CW_Supply_MIN"))
                    End If

                    If Not IsDBNull(reader("CW_Supply_AVG")) Then
                        avgValue(0) = Convert.ToDouble(reader("CW_Supply_AVG"))
                    End If
                    '#####  CW_PreFiltPres   #####
                    If Not IsDBNull(reader("CW_PreFiltPres_MAX")) Then
                        maxValue(1) = Convert.ToDouble(reader("CW_PreFiltPres_MAX"))
                    End If

                    If Not IsDBNull(reader("CW_PreFiltPres_MIN")) Then
                        minValue(1) = Convert.ToDouble(reader("CW_PreFiltPres_MIN"))
                    End If

                    If Not IsDBNull(reader("CW_PreFiltPres_AVG")) Then
                        avgValue(1) = Convert.ToDouble(reader("CW_PreFiltPres_AVG"))
                    End If
                    '#####  CW_PostFiltPres   #####
                    If Not IsDBNull(reader("CW_PostFiltPres_MAX")) Then
                        maxValue(2) = Convert.ToDouble(reader("CW_PostFiltPres_MAX"))
                    End If

                    If Not IsDBNull(reader("CW_PostFiltPres_MIN")) Then
                        minValue(2) = Convert.ToDouble(reader("CW_PostFiltPres_MIN"))
                    End If

                    If Not IsDBNull(reader("CW_PostFiltPres_AVG")) Then
                        avgValue(2) = Convert.ToDouble(reader("CW_PostFiltPres_AVG"))
                    End If
                    '#####  CW_FiltDif   #####
                    If Not IsDBNull(reader("CW_FiltDif_MAX")) Then
                        maxValue(3) = Convert.ToDouble(reader("CW_FiltDif_MAX"))
                    End If

                    If Not IsDBNull(reader("CW_FiltDif_MIN")) Then
                        minValue(3) = Convert.ToDouble(reader("CW_FiltDif_MIN"))
                    End If

                    If Not IsDBNull(reader("CW_FiltDif_AVG")) Then
                        avgValue(3) = Convert.ToDouble(reader("CW_FiltDif_AVG"))
                    End If
                    '#####  HW_PreFiltPres   #####
                    If Not IsDBNull(reader("HW_PreFiltPres_MAX")) Then
                        maxValue(4) = Convert.ToDouble(reader("HW_PreFiltPres_MAX"))
                    End If

                    If Not IsDBNull(reader("HW_PreFiltPres_MIN")) Then
                        minValue(4) = Convert.ToDouble(reader("HW_PreFiltPres_MIN"))
                    End If

                    If Not IsDBNull(reader("HW_PreFiltPres_AVG")) Then
                        avgValue(4) = Convert.ToDouble(reader("HW_PreFiltPres_AVG"))
                    End If
                    '#####  HW_PostFiltPres   #####
                    If Not IsDBNull(reader("HW_PostFiltPres_MAX")) Then
                        maxValue(5) = Convert.ToDouble(reader("HW_PostFiltPres_MAX"))
                    End If

                    If Not IsDBNull(reader("HW_PostFiltPres_MIN")) Then
                        minValue(5) = Convert.ToDouble(reader("HW_PostFiltPres_MIN"))
                    End If

                    If Not IsDBNull(reader("HW_PostFiltPres_AVG")) Then
                        avgValue(5) = Convert.ToDouble(reader("HW_PostFiltPres_AVG"))
                    End If
                    '#####  HW_FiltDif   #####
                    If Not IsDBNull(reader("HW_FiltDif_MAX")) Then
                        maxValue(6) = Convert.ToDouble(reader("HW_FiltDif_MAX"))
                    End If

                    If Not IsDBNull(reader("HW_FiltDif_MIN")) Then
                        minValue(6) = Convert.ToDouble(reader("HW_FiltDif_MIN"))
                    End If

                    If Not IsDBNull(reader("HW_FiltDif_AVG")) Then
                        avgValue(6) = Convert.ToDouble(reader("HW_FiltDif_AVG"))
                    End If
                    '#####  HW_Temp   #####
                    If Not IsDBNull(reader("HW_Temp_MAX")) Then
                        maxValue(7) = Convert.ToDouble(reader("HW_Temp_MAX"))
                    End If

                    If Not IsDBNull(reader("HW_Temp_MIN")) Then
                        minValue(7) = Convert.ToDouble(reader("HW_Temp_MIN"))
                    End If

                    If Not IsDBNull(reader("HW_Temp_AVG")) Then
                        avgValue(7) = Convert.ToDouble(reader("HW_Temp_AVG"))
                    End If
                    '#####  HW_Flow   #####
                    If Not IsDBNull(reader("HW_Flow_MAX")) Then
                        maxValue(8) = Convert.ToDouble(reader("HW_Flow_MAX"))
                    End If

                    If Not IsDBNull(reader("HW_Flow_MIN")) Then
                        minValue(8) = Convert.ToDouble(reader("HW_Flow_MIN"))
                    End If

                    If Not IsDBNull(reader("HW_Flow_AVG")) Then
                        avgValue(8) = Convert.ToDouble(reader("HW_Flow_AVG"))
                    End If
                    '#####  ST_FeedWaterPres   #####
                    If Not IsDBNull(reader("ST_FeedWaterPres_MAX")) Then
                        maxValue(9) = Convert.ToDouble(reader("ST_FeedWaterPres_MAX"))
                    End If

                    If Not IsDBNull(reader("ST_FeedWaterPres_MIN")) Then
                        minValue(9) = Convert.ToDouble(reader("ST_FeedWaterPres_MIN"))
                    End If

                    If Not IsDBNull(reader("ST_FeedWaterPres_AVG")) Then
                        avgValue(9) = Convert.ToDouble(reader("ST_FeedWaterPres_AVG"))
                    End If
                    '#####  ST_HeadPres   #####
                    If Not IsDBNull(reader("ST_HeadPres_MAX")) Then
                        maxValue(10) = Convert.ToDouble(reader("ST_HeadPres_MAX"))
                    End If

                    If Not IsDBNull(reader("ST_HeadPres_MIN")) Then
                        minValue(10) = Convert.ToDouble(reader("ST_HeadPres_MIN"))
                    End If

                    If Not IsDBNull(reader("ST_HeadPres_AVG")) Then
                        avgValue(10) = Convert.ToDouble(reader("ST_HeadPres_AVG"))
                    End If
                    '#####  ST_LowPres   #####
                    If Not IsDBNull(reader("ST_LowPres_MAX")) Then
                        maxValue(11) = Convert.ToDouble(reader("ST_LowPres_MAX"))
                    End If

                    If Not IsDBNull(reader("ST_LowPres_MIN")) Then
                        minValue(11) = Convert.ToDouble(reader("ST_LowPres_MIN"))
                    End If

                    If Not IsDBNull(reader("ST_LowPres_AVG")) Then
                        avgValue(11) = Convert.ToDouble(reader("ST_LowPres_AVG"))
                    End If
                    '#####  ST_MedPres   #####
                    If Not IsDBNull(reader("ST_MedPres_MAX")) Then
                        maxValue(12) = Convert.ToDouble(reader("ST_MedPres_MAX"))
                    End If

                    If Not IsDBNull(reader("ST_MedPres_MIN")) Then
                        minValue(12) = Convert.ToDouble(reader("ST_MedPres_MIN"))
                    End If

                    If Not IsDBNull(reader("ST_MedPres_AVG")) Then
                        avgValue(12) = Convert.ToDouble(reader("ST_MedPres_AVG"))
                    End If
                    '#####  ST_LowPresDem   #####
                    If Not IsDBNull(reader("ST_LowDem_MAX")) Then
                        maxValue(13) = Convert.ToDouble(reader("ST_LowDem_MAX"))
                    End If

                    If Not IsDBNull(reader("ST_LowDem_MIN")) Then
                        minValue(13) = Convert.ToDouble(reader("ST_LowDem_MIN"))
                    End If

                    If Not IsDBNull(reader("ST_LowDem_AVG")) Then
                        avgValue(13) = Convert.ToDouble(reader("ST_LowDem_AVG"))
                    End If
                    '#####  ST_MedPresDem   #####
                    If Not IsDBNull(reader("ST_MedDem_MAX")) Then
                        maxValue(14) = Convert.ToDouble(reader("ST_MedDem_MAX"))
                    End If

                    If Not IsDBNull(reader("ST_MedDem_MIN")) Then
                        minValue(14) = Convert.ToDouble(reader("ST_MedDem_MIN"))
                    End If

                    If Not IsDBNull(reader("ST_MedDem_AVG")) Then
                        avgValue(14) = Convert.ToDouble(reader("ST_MedDem_AVG"))
                    End If
                    '#####  ST_Flow   #####
                    If Not IsDBNull(reader("ST_Flow_MAX")) Then
                        maxValue(15) = Convert.ToDouble(reader("ST_Flow_MAX"))
                    End If

                    If Not IsDBNull(reader("ST_Flow_MIN")) Then
                        minValue(15) = Convert.ToDouble(reader("ST_Flow_MIN"))
                    End If

                    If Not IsDBNull(reader("ST_Flow_AVG")) Then
                        avgValue(15) = Convert.ToDouble(reader("ST_Flow_AVG"))
                    End If
                    '#####  EL_NorthA   #####
                    If Not IsDBNull(reader("EL_NorthA_MAX")) Then
                        maxValue(16) = Convert.ToDouble(reader("EL_NorthA_MAX"))
                    End If

                    If Not IsDBNull(reader("EL_NorthA_MIN")) Then
                        minValue(16) = Convert.ToDouble(reader("EL_NorthA_MIN"))
                    End If

                    If Not IsDBNull(reader("EL_NorthA_AVG")) Then
                        avgValue(16) = Convert.ToDouble(reader("EL_NorthA_AVG"))
                    End If
                    '#####  EL_NorthB   #####
                    If Not IsDBNull(reader("EL_NorthB_MAX")) Then
                        maxValue(17) = Convert.ToDouble(reader("EL_NorthB_MAX"))
                    End If

                    If Not IsDBNull(reader("EL_NorthB_MIN")) Then
                        minValue(17) = Convert.ToDouble(reader("EL_NorthB_MIN"))
                    End If

                    If Not IsDBNull(reader("EL_NorthB_AVG")) Then
                        avgValue(17) = Convert.ToDouble(reader("EL_NorthB_AVG"))
                    End If
                    '#####  EL_NorthC   #####
                    If Not IsDBNull(reader("EL_NorthC_MAX")) Then
                        maxValue(18) = Convert.ToDouble(reader("EL_NorthC_MAX"))
                    End If

                    If Not IsDBNull(reader("EL_NorthC_MIN")) Then
                        minValue(18) = Convert.ToDouble(reader("EL_NorthC_MIN"))
                    End If

                    If Not IsDBNull(reader("EL_NorthC_AVG")) Then
                        avgValue(18) = Convert.ToDouble(reader("EL_NorthC_AVG"))
                    End If
                    '#####  EL_SouthA   #####
                    If Not IsDBNull(reader("EL_SouthA_MAX")) Then
                        maxValue(19) = Convert.ToDouble(reader("EL_SouthA_MAX"))
                    End If

                    If Not IsDBNull(reader("EL_SouthA_MIN")) Then
                        minValue(19) = Convert.ToDouble(reader("EL_SouthA_MIN"))
                    End If

                    If Not IsDBNull(reader("EL_SouthA_AVG")) Then
                        avgValue(19) = Convert.ToDouble(reader("EL_SouthA_AVG"))
                    End If
                    '#####  EL_SouthB   #####
                    If Not IsDBNull(reader("EL_SouthB_MAX")) Then
                        maxValue(20) = Convert.ToDouble(reader("EL_SouthB_MAX"))
                    End If

                    If Not IsDBNull(reader("EL_SouthB_MIN")) Then
                        minValue(20) = Convert.ToDouble(reader("EL_SouthB_MIN"))
                    End If

                    If Not IsDBNull(reader("EL_SouthB_AVG")) Then
                        avgValue(20) = Convert.ToDouble(reader("EL_SouthB_AVG"))
                    End If
                    '#####  EL_SouthC   #####
                    If Not IsDBNull(reader("EL_SouthC_MAX")) Then
                        maxValue(21) = Convert.ToDouble(reader("EL_SouthC_MAX"))
                    End If

                    If Not IsDBNull(reader("EL_SouthC_MIN")) Then
                        minValue(21) = Convert.ToDouble(reader("EL_SouthC_MIN"))
                    End If

                    If Not IsDBNull(reader("EL_SouthC_AVG")) Then
                        avgValue(21) = Convert.ToDouble(reader("EL_SouthC_AVG"))
                    End If
                    '#####  AR_LinePres   #####
                    If Not IsDBNull(reader("AR_LinePres_MAX")) Then
                        maxValue(22) = Convert.ToDouble(reader("AR_LinePres_MAX"))
                    End If

                    If Not IsDBNull(reader("AR_LinePres_MIN")) Then
                        minValue(22) = Convert.ToDouble(reader("AR_LinePres_MIN"))
                    End If

                    If Not IsDBNull(reader("AR_LinePres_AVG")) Then
                        avgValue(22) = Convert.ToDouble(reader("AR_LinePres_AVG"))
                    End If
                    ''#####  CW_Supply   #####
                    'If Not IsDBNull(reader("CW_Supply_MAX")) Then
                    '    maxValue(0) = Convert.ToDouble(reader("CW_Supply_MAX"))
                    'End If

                    'If Not IsDBNull(reader("CW_Supply_MIN")) Then
                    '    minValue(0) = Convert.ToDouble(reader("CW_Supply_MIN"))
                    'End If

                    'If Not IsDBNull(reader("CW_Supply_AVG")) Then
                    '    avgValue(0) = Convert.ToDouble(reader("CW_Supply_AVG"))
                    'End If
                    ''#####  CW_Supply   #####
                    'If Not IsDBNull(reader("CW_Supply_MAX")) Then
                    '    maxValue(0) = Convert.ToDouble(reader("CW_Supply_MAX"))
                    'End If

                    'If Not IsDBNull(reader("CW_Supply_MIN")) Then
                    '    minValue(0) = Convert.ToDouble(reader("CW_Supply_MIN"))
                    'End If

                    'If Not IsDBNull(reader("CW_Supply_AVG")) Then
                    '    avgValue(0) = Convert.ToDouble(reader("CW_Supply_AVG"))
                    'End If
                    ''#####  CW_Supply   #####
                    'If Not IsDBNull(reader("CW_Supply_MAX")) Then
                    '    maxValue(0) = Convert.ToDouble(reader("CW_Supply_MAX"))
                    'End If

                    'If Not IsDBNull(reader("CW_Supply_MIN")) Then
                    '    minValue(0) = Convert.ToDouble(reader("CW_Supply_MIN"))
                    'End If

                    'If Not IsDBNull(reader("CW_Supply_AVG")) Then
                    '    avgValue(0) = Convert.ToDouble(reader("CW_Supply_AVG"))
                    'End If

                End If
            End Using



            Dim names() As String = {"CW_Supply", "CW_PreFiltPres", "CW_PostFiltPres",
                "CW_FiltDif", "HW_PreFiltPres", "HW_PostFiltPres", "HW_FiltDif", "HW_Temp",
                "HW_Flow", "ST_FeedWaterPres", "ST_HeadPres", "ST_LowPres", "ST_MedPres",
                "ST_LowDem", "ST_MedDem", "ST_Flow", "EL_NorthA", "EL_NorthB", "EL_NorthC",
                "EL_SouthA", "EL_SouthB", "EL_SouthC", "AR_LinePres"}
            CheckedListBox1.Items.Clear()

            For Each i As String In names
                CheckedListBox1.Items.Add(i)
            Next
            For i As Integer = 0 To 22
                SetControlPropertyByName("SUM_Name_LBL" & i, "Text", names(i))
                SetControlPropertyByName("SUM_Min_LBL" & i, "Text", minValue(i).ToString("F2"))
                SetControlPropertyByName("SUM_Max_LBL" & i, "Text", maxValue(i).ToString("F2"))
                SetControlPropertyByName("SUM_Avg_LBL" & i, "Text", avgValue(i).ToString("F2"))
            Next
            SetControlPropertyByName("SUM_Count_LBL", "Text", totalCount)
            SetControlPropertyByName("SUM_Start_Time", "Text", minDate)
            SetControlPropertyByName("SUM_End_Time", "Text", maxDate)
            ' Example: show results in labels

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
    Private Sub RefreshAlarmTable()
        Dim query As String = "SELECT * FROM a_vals " &
                      "WHERE Date_Time BETWEEN @startDate AND @endDate " &
                      "ORDER BY Date_Time DESC"
        Using connection As New MySqlConnection(SQL_ConString)
            Dim cmd As New MySqlCommand(query, connection)
            cmd.Parameters.AddWithValue("@startDate", DateTimePicker1.Value)
            cmd.Parameters.AddWithValue("@endDate", DateTimePicker2.Value)

            Dim adapter As New MySqlDataAdapter(cmd)
            Dim table As New DataTable()
            adapter.Fill(table)

            GridView.DataSource = table
            ChartView.DataSource = table
        End Using
        GridView.Columns(0).DefaultCellStyle.Format = "MM/dd/yyyy hh:mm:ss tt"

    End Sub

    Private Sub Button13_Click(sender As Object, e As EventArgs) Handles Button13.Click


        Dim Start_DateTime As DateTime = (DateTimePicker1.Value.Month & "/" & DateTimePicker1.Value.Day & "/" & DateTimePicker1.Value.Year & " " & NumericUpDown1.Value & ":" & NumericUpDown3.Value.ToString("00") & ":" & "00")
        Dim End_DateTime As DateTime = (DateTimePicker2.Value.Month & "/" & DateTimePicker2.Value.Day & "/" & DateTimePicker2.Value.Year & " " & NumericUpDown2.Value & ":" & NumericUpDown4.Value.ToString("00") & ":" & "00")
        GetNewData(Start_DateTime, End_DateTime)

    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        DataChart_Page.Show()
        DataChart_Page.BringToFront()
        GetSummary(DateTimePicker1.Value, DateTimePicker2.Value)

    End Sub
    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        DataSum_Page.Show()
        DataSum_Page.BringToFront()
    End Sub
    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        DataGrid_Page.Show()
        DataGrid_Page.BringToFront()
    End Sub

    Private Sub TableLayoutPanel1_Paint(sender As Object, e As PaintEventArgs) Handles TableLayoutPanel1.Paint

    End Sub
End Class

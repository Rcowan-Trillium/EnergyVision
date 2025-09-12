#Region "FACILITY MONITOR - TRILLIUM HEALTH CARE PRODUCTS"
'»«»«»«»«»«»«»«»«»«»«»«»«»«»«»«»«»«»«»«»«»«»«»«»«»«»«»«»«»«»«»
'»«  FACILITY MONITOR FOR TRILLIUM HEALTH CARE MANUFACTURING  «»
'«»@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@«»
'«»@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@«»
'«»@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@«»
'«»@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@&%&@@@@@@@@@@@@@@@«»
'«»@@@@@@@@@@@@@@@@&%%%%%%%%%%%%%%%&@@@@@%*,..*#@@@@@@@@@@@@@@«»
'«»@@@@@@@@@@@@@@@&&&%%%%%%%&&&&&&&&%&&(,..,,./&@@@@@@@@@@@@@@«»
'«»@@@@@@@@@@@@@@&%%%%%%&&&&&&&&&&&&&@#*.,,,.,#@@@@@@@@@@@@@@@«»
'«»@@@@@@@@@@@@@@&&&&&&&&&&&&&@@@@@@@@@%/,,,*#@@@@@@@@@@@@@@@@«»
'«»@@@@@@@@@@@@@@@&&%&&&&&&&&&&@@@@@@&&%#(/#%&&&&@&&&&@@@@@@@@«»
'«»@@@@@@@@@@@@@&&%#((#%%%&&&&&&@&&%%%%&&&&&&&&&&&&&&%&&&@@@@@«»
'«»@@@@@@@@&#//*********/#%%%%%%%#((//((###%%%%%%%%%%&&&&%&&@@«»
'«»@@@@@@@@%#(/**,,,,,,,,,.*(#%%##(((((############%%%%%%%##&@«»
'«»@@@@@@@@@@@@@@@@@@@@@@&%%%%#%#####%%%%%####(((######((#%&@@«»
'«»@@@@@@@@@@@@@@@@@@@@@&%%%%%%############//#&&%#((((#%&@@@@@«»
'«»@@@@@@@@@@@@@@@@@@@@&%%%%%%#############(*/(&@@@@@@@@@@@@@@«»
'«»@@@@@@@@@@@@@@@@@@@@&%%%%%%%%%#########((**/(@@@@@@@@@@@@@@«»
'«»@@@@@@@@@@@@@@@@@@@@&%#%%&&%%%%%%%%#%#%%#///#&@@@@@@@@@@@@@«»
'«»@@@@@@@@@@@@@@@@@@@@@@&%%%%%%%%%%###%&@@@@@@@@@@@@@@@@@@@@@«»
'«»@@@@@@@@@@@@@@@@@@@@@@@@&%######%&&@@@@@@@@@@@@@@@@@@@@@@@@«»
'«»@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@«»
'«»@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@«»
'»«»«»«»«»«»«»«»«»«»«»«»«»«»«»«»«»«»«»«»«»«»«»«»«»«»«»«»«»«»«»«»
#End Region
'Imports System.Collections.Specialized.BitVector32
Imports System.Data.SqlClient
Imports System.Net
Imports System.Text
Imports System.Windows.Forms.DataVisualization.Charting
'Imports Google.Protobuf.WellKnownTypes
'Imports MadMilkman
Imports MadMilkman.Ini
Imports Microsoft.Win32
Imports MySql.Data.MySqlClient
'Imports Mysqlx.Expect.Open.Types.Condition.Types
Imports Newtonsoft.Json

Public Class Dashboard_Page
    Inherits Form
    Private myConn As SqlConnection
    Private myCmd As SqlCommand
    Private myReader As SqlDataReader
#Region "Global Variable Declaration"


    '╔══════════════════════════════════════════════════════════════════════════════════════════════════════════════╗
    '║                                        [Global Variable Declaration]                                         ║
    '║                                                Section Start                                                 ║
    '╠══════════════════════════════════════════════════════════════════════════════════════════════════════════════╣
    '║                                                                                                              ║
    'USER CONTROL VARIABLES
    'SYSTEM CONTROL VARIABLES
    '
    Dim CurrentTime As String = "12:59:59 PM"
    Dim CurrentDate As String = Now.Month & "-" & Now.Day & "-" & Now.Year

    Public CurrentUserLevel As Integer = 0
    Public CurrentUser As String = "System"
    Dim LoginTimeRemaining As Integer = 0

    Dim Firsttick As Boolean = False
    Dim FirstTick_EN As Boolean = False
    Dim First5Count As Integer = 0
    Dim FirstTickDone As Boolean = False
    Dim HeartBeatError As Boolean = False
    Dim lastHBTime As New DateTime
    Dim lastHBTime_5s As New DateTime
    Dim temphour As Integer = 0
    Dim TempMinute As Integer = 99

    Dim RunSQL As Boolean = False
    Dim RunLog As Boolean = True
    Dim SQL_ConString As String = "server=localhost;user id=System;password=System;database=facilitylog"
    Dim MinMaxLogSaved As Boolean = False

    Dim E_Als, A_Als, S_Als, W_Als, L_Als, C_Als As Boolean
    Dim PLCConInAlarm As Boolean = True
    Dim PLCTimeSyncError As Boolean = False



    '║                                                                                                              ║
    '╠══════════════════════════════════════════════════════════════════════════════════════════════════════════════╣
    '║                                        [Global Variable Declaration]                                         ║
    '║                                                 Section End                                                  ║
    '╚══════════════════════════════════════════════════════════════════════════════════════════════════════════════╝
#End Region
#Region "Functions"
    '╔════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════╗
    '║     dMMMMb  dMMMMb  .aMMMb  .aMMMMP dMMMMb  .aMMMb  dMMMMMMMMb         dMMMMMP dMP dMP dMMMMb  .aMMMb dMMMMMMP dMP .aMMMb  dMMMMb  .dMMMb  ║
    '║    dMP.dMP dMP.dMP dMP"dMP dMP"    dMP.dMP dMP"dMP dMP"dMP"dMP        dMP     dMP dMP dMP dMP dMP"VMP   dMP   amr dMP"dMP dMP dMP dMP" VP  ║
    '║   dMMMMP" dMMMMK" dMP dMP dMP MMP"dMMMMK" dMMMMMP dMP dMP dMP        dMMMP   dMP dMP dMP dMP dMP       dMP   dMP dMP dMP dMP dMP  VMMMb    ║
    '║  dMP     dMP"AMF dMP.aMP dMP.dMP dMP"AMF dMP dMP dMP dMP dMP        dMP     dMP.aMP dMP dMP dMP.aMP   dMP   dMP dMP.aMP dMP dMP dP .dMP    ║
    '║ dMP     dMP dMP  VMMMP"  VMMMP" dMP dMP dMP dMP dMP dMP dMP        dMP      VMMMP" dMP dMP  VMMMP"   dMP   dMP  VMMMP" dMP dMP  VMMMP"     ║
    '╠════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════╣
    '║                                                                                                                                            ║
    Public Sub PropertyBind()
        'All values from the Setpoint screen will bind their properties
        ' to the displays on other screens to minimize traffic

        'INSTRUCTIONS:
        '[DestinationControl].DataBindings.Add([DestinationProperty], [SourceControl], [SourceProperty])

        ' ______________________________________________________________________________________________________
        '/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/
        '   _______ __           _       __      __               _____                   __     
        '  / ____(_) /___  __   | |     / /___ _/ /____  _____   / ___/__  ______  ____  / /_  __
        ' / /   / / __/ / / /   | | /| / / __ `/ __/ _ \/ ___/   \__ \/ / / / __ \/ __ \/ / / / /
        '/ /___/ / /_/ /_/ /    | |/ |/ / /_/ / /_/  __/ /      ___/ / /_/ / /_/ / /_/ / / /_/ / 
        '\____/_/\__/\__, /     |__/|__/\__,_/\__/\___/_/      /____/\__,_/ .___/ .___/_/\__, /  
        '           /____/                                               /_/   /_/      /____/   


        '-Real Values
        Water_Sup_City_G1.DataBindings.Add("value", CWSUP_Actual, "value") 'Dashboard Page
        Water_Sup_City_V1.DataBindings.Add("value", CWSUP_Actual, "value") 'Dashboard Page
        Water_Sup_City_G2.DataBindings.Add("value", CWSUP_Actual, "value") 'Water Page
        Water_Sup_City_V2.DataBindings.Add("value", CWSUP_Actual, "value") 'Water Page
        '-Max Actual Values
        Water_Sup_City_MAX1.DataBindings.Add("value", CWSUP_High, "value") 'Dashboard Page
        Water_Sup_City_MAX2.DataBindings.Add("value", CWSUP_High, "value") 'Water Page
        '-Min Actual Values
        Water_Sup_City_MIN1.DataBindings.Add("value", CWSUP_Low, "value") 'Dashboard Page
        Water_Sup_City_MIN2.DataBindings.Add("value", CWSUP_Low, "value") 'Water Page
        '-Visual Min Values
        Water_Sup_City_G1.DataBindings.Add("Band2StartValue", CWSUP_Min, "value") 'Dashboard Page
        Water_Sup_City_G2.DataBindings.Add("Band2StartValue", CWSUP_Min, "value") 'Water Page
        Water_Sup_City_V1.DataBindings.Add("ValueLimitLower", CWSUP_Min, "value") 'Dashboard Page
        Water_Sup_City_V2.DataBindings.Add("ValueLimitLower", CWSUP_Min, "value") 'Water Page
        '-Visual Max Values
        Water_Sup_City_G1.DataBindings.Add("Band2EndValue", CWSUP_Max, "value") 'Dashboard Page
        Water_Sup_City_G2.DataBindings.Add("Band2EndValue", CWSUP_Max, "value") 'Water Page
        Water_Sup_City_V1.DataBindings.Add("ValueLimitUpper", CWSUP_Max, "value") 'Dashboard Page
        Water_Sup_City_V2.DataBindings.Add("ValueLimitUpper", CWSUP_Max, "value") 'Water Page
        '-Unit Display
        Water_Sup_City_UN1.DataBindings.Add("text", CWSUP_Unit, "text") 'Dashboard Page
        Water_Sup_City_UN2.DataBindings.Add("text", CWSUP_Unit, "text") 'Water Page


        '    ____                   __           __   _______ __           ____                                    
        '   / __ )____  ____  _____/ /____  ____/ /  / ____(_) /___  __   / __ \________  ____________  __________ 
        '  / __  / __ \/ __ \/ ___/ __/ _ \/ __  /  / /   / / __/ / / /  / /_/ / ___/ _ \/ ___/ ___/ / / / ___/ _ \
        ' / /_/ / /_/ / /_/ (__  ) /_/  __/ /_/ /  / /___/ / /_/ /_/ /  / ____/ /  /  __(__  |__  ) /_/ / /  /  __/
        '/_____/\____/\____/____/\__/\___/\__,_/   \____/_/\__/\__, /  /_/   /_/   \___/____/____/\__,_/_/   \___/ 
        '                                                        /_/ 

        '-Real Values
        'Water_PR_ColdPre_G1.DataBindings.Add("value", CWPRP_Actual, "value") 'Dashboard Page
        ' Water_PR_ColdPre_V1.DataBindings.Add("value", CWPRP_Actual, "value") 'Dashboard Page
        Water_Pr_ColdPre_G2.DataBindings.Add("value", CWPRP_Actual, "value") 'Water Page
        Water_Pr_ColdPre_V2.DataBindings.Add("value", CWPRP_Actual, "value") 'Water Page
        '-Max Actual Values
        ' Water_PR_ColdPre_MAX1.DataBindings.Add("value", CWPRP_High, "value") 'Dashboard Page
        Water_Pr_ColdPre_MAX2.DataBindings.Add("value", CWPRP_High, "value") 'Water Page
        '-Min Actual Values
        'Water_PR_ColdPre_MIN1.DataBindings.Add("value", CWPRP_Low, "value") 'Dashboard Page
        Water_Pr_ColdPre_MIN2.DataBindings.Add("value", CWPRP_Low, "value") 'Water Page
        '-Visual Min Values
        ' Water_PR_ColdPre_G1.DataBindings.Add("Band2StartValue", CWPRP_Min, "value") 'Dashboard Page
        Water_Pr_ColdPre_G2.DataBindings.Add("Band2StartValue", CWPRP_Min, "value") 'Water Page
        ' Water_PR_ColdPre_V1.DataBindings.Add("ValueLimitLower", CWPRP_Min, "value") 'Dashboard Page
        Water_Pr_ColdPre_V2.DataBindings.Add("ValueLimitLower", CWPRP_Min, "value") 'Water Page
        '-Visual Max Values
        'Water_PR_ColdPre_G1.DataBindings.Add("Band2EndValue", CWPRP_Max, "value") 'Dashboard Page
        Water_Pr_ColdPre_G2.DataBindings.Add("Band2EndValue", CWPRP_Max, "value") 'Water Page
        'Water_PR_ColdPre_V1.DataBindings.Add("ValueLimitUpper", CWPRP_Max, "value") 'Dashboard Page
        Water_Pr_ColdPre_V2.DataBindings.Add("ValueLimitUpper", CWPRP_Max, "value") 'Water Page
        '-Unit Display
        ' Water_PR_ColdPre_UN1.DataBindings.Add("text", CWPRP_Unit, "text") 'Dashboard Page
        Water_Pr_ColdPre_UN2.DataBindings.Add("text", CWPRP_Unit, "text") 'Water Page
        '   ______      __    __   _       __      __               ____                                    
        '  / ____/___  / /___/ /  | |     / /___ _/ /____  _____   / __ \________  ____________  __________ 
        ' / /   / __ \/ / __  /   | | /| / / __ `/ __/ _ \/ ___/  / /_/ / ___/ _ \/ ___/ ___/ / / / ___/ _ \
        '/ /___/ /_/ / / /_/ /    | |/ |/ / /_/ / /_/  __/ /     / ____/ /  /  __(__  |__  ) /_/ / /  /  __/
        '\____/\____/_/\__,_/     |__/|__/\__,_/\__/\___/_/     /_/   /_/   \___/____/____/\__,_/_/   \___/ 
        '-Real Values
        Water_Pr_Cold_G1.DataBindings.Add("value", CWPOP_Actual, "value") 'Dashboard Page
        Water_Pr_Cold_V1.DataBindings.Add("value", CWPOP_Actual, "value") 'Dashboard Page
        Water_Pr_Cold_G2.DataBindings.Add("value", CWPOP_Actual, "value") 'Water Page
        Water_Pr_Cold_V2.DataBindings.Add("value", CWPOP_Actual, "value") 'Water Page
        '-Max Actual Values
        Water_Pr_Cold_MAX1.DataBindings.Add("value", CWPOP_High, "value") 'Dashboard Page
        Water_Pr_Cold_MAX2.DataBindings.Add("value", CWPOP_High, "value") 'Water Page
        '-Min Actual Values
        Water_Pr_Cold_MIN1.DataBindings.Add("value", CWPOP_Low, "value") 'Dashboard Page
        Water_Pr_Cold_MIN2.DataBindings.Add("value", CWPOP_Low, "value") 'Water Page
        '-Visual Min Values
        Water_Pr_Cold_G1.DataBindings.Add("Band2StartValue", CWPOP_Min, "value") 'Dashboard Page
        Water_Pr_Cold_G2.DataBindings.Add("Band2StartValue", CWPOP_Min, "value") 'Water Page
        Water_Pr_Cold_V1.DataBindings.Add("ValueLimitLower", CWPOP_Min, "value") 'Dashboard Page
        Water_Pr_Cold_V2.DataBindings.Add("ValueLimitLower", CWPOP_Min, "value") 'Water Page
        '-Visual Max Values
        Water_Pr_Cold_G1.DataBindings.Add("Band2EndValue", CWPOP_Max, "value") 'Dashboard Page
        Water_Pr_Cold_G2.DataBindings.Add("Band2EndValue", CWPOP_Max, "value") 'Water Page
        Water_Pr_Cold_V1.DataBindings.Add("ValueLimitUpper", CWPOP_Max, "value") 'Dashboard Page
        Water_Pr_Cold_V2.DataBindings.Add("ValueLimitUpper", CWPOP_Max, "value") 'Water Page
        '-Unit Display
        Water_Pr_Cold_UN1.DataBindings.Add("text", CWPOP_Unit, "text") 'Dashboard Page
        Water_Pr_Cold_UN2.DataBindings.Add("text", CWPOP_Unit, "text") 'Water Page


        '    __  __      __     _       __      __               ____                           ____                                    
        '   / / / /___  / /_   | |     / /___ _/ /____  _____   / __ \__  ______ ___  ____     / __ \________  ____________  __________ 
        '  / /_/ / __ \/ __/   | | /| / / __ `/ __/ _ \/ ___/  / /_/ / / / / __ `__ \/ __ \   / /_/ / ___/ _ \/ ___/ ___/ / / / ___/ _ \
        ' / __  / /_/ / /_     | |/ |/ / /_/ / /_/  __/ /     / ____/ /_/ / / / / / / /_/ /  / ____/ /  /  __(__  |__  ) /_/ / /  /  __/
        '/_/ /_/\____/\__/     |__/|__/\__,_/\__/\___/_/     /_/    \__,_/_/ /_/ /_/ .___/  /_/   /_/   \___/____/____/\__,_/_/   \___/ 
        '                                                                         /_/                                                   
        '-Real Values
        'Water_Pr_HotPre_G1.DataBindings.Add("value", HWPRP_Actual, "value") 'Dashboard Page
        'Water_Pr_HotPre_V1.DataBindings.Add("value", HWPRP_Actual, "value") 'Dashboard Page
        Water_Pr_HotPre_G2.DataBindings.Add("value", HWPRP_Actual, "value") 'Water Page
        Water_Pr_HotPre_V2.DataBindings.Add("value", HWPRP_Actual, "value") 'Water Page
        '-Max Actual Values
        'Water_Pr_HotPre_MAX1.DataBindings.Add("value", HWPRP_High, "value") 'Dashboard Page
        Water_Pr_HotPre_MAX2.DataBindings.Add("value", HWPRP_High, "value") 'Water Page
        '-Min Actual Values
        'Water_Pr_HotPre_MIN1.DataBindings.Add("value", HWPRP_Low, "value") 'Dashboard Page
        Water_Pr_HotPre_MIN2.DataBindings.Add("value", HWPRP_Low, "value") 'Water Page
        '-Visual Min Values
        'Water_Pr_HotPre_G1.DataBindings.Add("Band2StartValue", HWPRP_Min, "value") 'Dashboard Page
        Water_Pr_HotPre_G2.DataBindings.Add("Band2StartValue", HWPRP_Min, "value") 'Water Page
        'Water_Pr_HotPre_V1.DataBindings.Add("ValueLimitLower", HWPRP_Min, "value") 'Dashboard Page
        Water_Pr_HotPre_V2.DataBindings.Add("ValueLimitLower", HWPRP_Min, "value") 'Water Page
        '-Visual Max Values
        'Water_Pr_HotPre_G1.DataBindings.Add("Band2EndValue", HWPRP_Max, "value") 'Dashboard Page
        Water_Pr_HotPre_G2.DataBindings.Add("Band2EndValue", HWPRP_Max, "value") 'Water Page
        'Water_Pr_HotPre_V1.DataBindings.Add("ValueLimitUpper", HWPRP_Max, "value") 'Dashboard Page
        Water_Pr_HotPre_V2.DataBindings.Add("ValueLimitUpper", HWPRP_Max, "value") 'Water Page
        '-Unit Display
        'Water_Pr_HotPre_UN1.DataBindings.Add("text", HWPRP_Unit, "text") 'Dashboard Page
        Water_Pr_HotPre_UN2.DataBindings.Add("text", HWPRP_Unit, "text") 'Water Page
        '    __  __      __     _       __      __               ____                                    
        '   / / / /___  / /_   | |     / /___ _/ /____  _____   / __ \________  ____________  __________ 
        '  / /_/ / __ \/ __/   | | /| / / __ `/ __/ _ \/ ___/  / /_/ / ___/ _ \/ ___/ ___/ / / / ___/ _ \
        ' / __  / /_/ / /_     | |/ |/ / /_/ / /_/  __/ /     / ____/ /  /  __(__  |__  ) /_/ / /  /  __/
        '/_/ /_/\____/\__/     |__/|__/\__,_/\__/\___/_/     /_/   /_/   \___/____/____/\__,_/_/   \___/ 
        '-Real Values
        Water_Pr_Hot_G1.DataBindings.Add("value", HWPOP_Actual, "value") 'Dashboard Page
        Water_Pr_Hot_V1.DataBindings.Add("value", HWPOP_Actual, "value") 'Dashboard Page
        Water_Pr_Hot_G2.DataBindings.Add("value", HWPOP_Actual, "value") 'Water Page
        Water_Pr_Hot_V2.DataBindings.Add("value", HWPOP_Actual, "value") 'Water Page
        '-Max Actual Values
        Water_Pr_Hot_MAX1.DataBindings.Add("value", HWPOP_High, "value") 'Dashboard Page
        Water_Pr_Hot_MAX2.DataBindings.Add("value", HWPOP_High, "value") 'Water Page
        '-Min Actual Values
        Water_Pr_Hot_MIN1.DataBindings.Add("value", HWPOP_Low, "value") 'Dashboard Page
        Water_Pr_Hot_MIN2.DataBindings.Add("value", HWPOP_Low, "value") 'Water Page
        '-Visual Min Values
        Water_Pr_Hot_G1.DataBindings.Add("Band2StartValue", HWPOP_Min, "value") 'Dashboard Page
        Water_Pr_Hot_G2.DataBindings.Add("Band2StartValue", HWPOP_Min, "value") 'Water Page
        Water_Pr_Hot_V1.DataBindings.Add("ValueLimitLower", HWPOP_Min, "value") 'Dashboard Page
        Water_Pr_Hot_V2.DataBindings.Add("ValueLimitLower", HWPOP_Min, "value") 'Water Page
        '-Visual Max Values
        Water_Pr_Hot_G1.DataBindings.Add("Band2EndValue", HWPOP_Max, "value") 'Dashboard Page
        Water_Pr_Hot_G2.DataBindings.Add("Band2EndValue", HWPOP_Max, "value") 'Water Page
        Water_Pr_Hot_V1.DataBindings.Add("ValueLimitUpper", HWPOP_Max, "value") 'Dashboard Page
        Water_Pr_Hot_V2.DataBindings.Add("ValueLimitUpper", HWPOP_Max, "value") 'Water Page
        '-Unit Display
        Water_Pr_Hot_UN1.DataBindings.Add("text", HWPOP_Unit, "text") 'Dashboard Page
        Water_Pr_Hot_UN2.DataBindings.Add("text", HWPOP_Unit, "text") 'Water Page
        '    __  __      __     _       __      __               ______                                     __                
        '   / / / /___  / /_   | |     / /___ _/ /____  _____   /_  __/__  ____ ___  ____  ___  _________ _/ /___  __________ 
        '  / /_/ / __ \/ __/   | | /| / / __ `/ __/ _ \/ ___/    / / / _ \/ __ `__ \/ __ \/ _ \/ ___/ __ `/ __/ / / / ___/ _ \
        ' / __  / /_/ / /_     | |/ |/ / /_/ / /_/  __/ /       / / /  __/ / / / / / /_/ /  __/ /  / /_/ / /_/ /_/ / /  /  __/
        '/_/ /_/\____/\__/     |__/|__/\__,_/\__/\___/_/       /_/  \___/_/ /_/ /_/ .___/\___/_/   \__,_/\__/\__,_/_/   \___/ 
        '                                                                        /_/                                          
        '-Real Values
        Water_Tp_Hot_G1.DataBindings.Add("value", HWTP_Actual, "value") 'Dashboard Page
        Water_Tp_Hot_V1.DataBindings.Add("value", HWTP_Actual, "value") 'Dashboard Page
        Water_Tp_Hot_G2.DataBindings.Add("value", HWTP_Actual, "value") 'Water Page
        Water_Tp_Hot_V2.DataBindings.Add("value", HWTP_Actual, "value") 'Water Page
        '-Max Actual Values
        Water_Tp_Hot_MAX1.DataBindings.Add("value", HWTP_High, "value") 'Dashboard Page
        Water_Tp_Hot_MAX2.DataBindings.Add("value", HWTP_High, "value") 'Water Page
        '-Min Actual Values
        Water_Tp_Hot_MIN1.DataBindings.Add("value", HWTP_Low, "value") 'Dashboard Page
        Water_Tp_Hot_MIN2.DataBindings.Add("value", HWTP_Low, "value") 'Water Page
        ''-Indicator Values
        Water_Tp_Hot_G1.DataBindings.Add("Band2StartValue", HWTP_Min, "value") 'Dashboard Page
        Water_Tp_Hot_G2.DataBindings.Add("Band2StartValue", HWTP_Min, "value") 'Water Page
        Water_Tp_Hot_V1.DataBindings.Add("ValueLimitLower", HWTP_Min, "value") 'Dashboard Page
        Water_Tp_Hot_V2.DataBindings.Add("ValueLimitLower", HWTP_Min, "value") 'Water Page
        '-Visual Max Values
        Water_Tp_Hot_G1.DataBindings.Add("Band2EndValue", HWTP_Max, "value") 'Dashboard Page
        Water_Tp_Hot_G2.DataBindings.Add("Band2EndValue", HWTP_Max, "value") 'Water Page
        Water_Tp_Hot_V1.DataBindings.Add("ValueLimitUpper", HWTP_Max, "value") 'Dashboard Page
        Water_Tp_Hot_V2.DataBindings.Add("ValueLimitUpper", HWTP_Max, "value") 'Water Page
        '-Unit Display
        Water_Tp_Hot_UN1.DataBindings.Add("text", HWTP_Unit, "text") 'Dashboard Page
        Water_Tp_Hot_UN2.DataBindings.Add("text", HWTP_Unit, "text") 'Water Page
        '    __  __      __     _       __      __               ________             
        '   / / / /___  / /_   | |     / /___ _/ /____  _____   / ____/ /___ _      __
        '  / /_/ / __ \/ __/   | | /| / / __ `/ __/ _ \/ ___/  / /_  / / __ \ | /| / /
        ' / __  / /_/ / /_     | |/ |/ / /_/ / /_/  __/ /     / __/ / / /_/ / |/ |/ / 
        '/_/ /_/\____/\__/     |__/|__/\__,_/\__/\___/_/     /_/   /_/\____/|__/|__/  

        '-Real Values

        Water_Fl_Hot_G1.DataBindings.Add("value", HWFL_Actual, "value") 'Dashboard Page
        Water_Fl_Hot_V1.DataBindings.Add("value", HWFL_Actual, "value") 'Dashboard Page
        Water_Fl_Hot_G2.DataBindings.Add("value", HWFL_Actual, "value") 'Water Page
        Water_Fl_Hot_V2.DataBindings.Add("value", HWFL_Actual, "value") 'Water Page
        '-Max Actual Values
        Water_Fl_Hot_MAX1.DataBindings.Add("value", HWFL_High, "value") 'Dashboard Page
        Water_Fl_Hot_MAX2.DataBindings.Add("value", HWFL_High, "value") 'Water Page
        '-Min Actual Values
        Water_Fl_Hot_MIN1.DataBindings.Add("value", HWFL_Low, "value") 'Dashboard Page
        Water_Fl_Hot_MIN2.DataBindings.Add("value", HWFL_Low, "value") 'Water Page
        ''-Indicator Values
        Water_Fl_Hot_G1.DataBindings.Add("Band2StartValue", HWFL_Min, "value") 'Dashboard Page
        Water_Fl_Hot_G2.DataBindings.Add("Band2StartValue", HWFL_Min, "value") 'Water Page
        Water_Fl_Hot_V1.DataBindings.Add("ValueLimitLower", HWFL_Min, "value") 'Dashboard Page
        Water_Fl_Hot_V2.DataBindings.Add("ValueLimitLower", HWFL_Min, "value") 'Water Page
        '-Visual Max Values
        Water_Fl_Hot_G1.DataBindings.Add("Band2EndValue", HWFL_Max, "value") 'Dashboard Page
        Water_Fl_Hot_G2.DataBindings.Add("Band2EndValue", HWFL_Max, "value") 'Water Page
        Water_Fl_Hot_V1.DataBindings.Add("ValueLimitUpper", HWFL_Max, "value") 'Dashboard Page
        Water_Fl_Hot_V2.DataBindings.Add("ValueLimitUpper", HWFL_Max, "value") 'Water Page
        '-Unit Display
        Water_Fl_Hot_UN1.DataBindings.Add("text", HWFL_Unit, "text") 'Dashboard Page
        Water_Fl_Hot_UN2.DataBindings.Add("text", HWFL_Unit, "text") 'Water Page

        '    __  __      __     _       __      __               _______ ____           
        '   / / / /___  / /_   | |     / /___ _/ /____  _____   / ____(_) / /____  _____
        '  / /_/ / __ \/ __/   | | /| / / __ `/ __/ _ \/ ___/  / /_  / / / __/ _ \/ ___/
        ' / __  / /_/ / /_     | |/ |/ / /_/ / /_/  __/ /     / __/ / / / /_/  __/ /    
        '/_/ /_/\____/\__/     |__/|__/\__,_/\__/\___/_/     /_/   /_/_/\__/\___/_/     


        '-Real Values
        Water_FH_Hot_G1.DataBindings.Add("value", HWFD_Actual, "value") 'Dashboard Page
        Water_FH_Hot_V1.DataBindings.Add("value", HWFD_Actual, "value") 'Dashboard Page
        Water_FH_Hot_G2.DataBindings.Add("value", HWFD_Actual, "value")
        Water_FH_Hot_V2.DataBindings.Add("text", HWFD_Actual, "value")
        '-Max Actual Values
        Water_FH_Hot_MAX1.DataBindings.Add("value", HWFD_High, "value") 'Dashboard Page
        'Water_FH_Hot_MAX2.DataBindings.Add("value", HWFD_High, "value")'Water Page
        '-Min Actual Values
        Water_FH_Hot_MIN1.DataBindings.Add("value", HWFD_Low, "value") 'Dashboard Page
        'Water_FH_Hot_MIN2.DataBindings.Add("value", HWFD_Low, "value")'Water Page
        ''-Indicator Values
        'Water_FH_Hot_G1.DataBindings.Add("Band2StartValue", HWFD_Min, "value") 'Dashboard Page
        'Water_FH_Hot_G2.DataBindings.Add("Band2StartValue", HWFD_Min, "value") 'Water Page
        Water_FH_Hot_V1.DataBindings.Add("ValueLimitLower", HWFD_Min, "value") 'Dashboard Page
        'Water_FH_Hot_V2.DataBindings.Add("ValueLimitLower", HWFD_Min, "value") 'Water Page
        '-Visual Max Values
        'Water_FH_Hot_G1.DataBindings.Add("Band2EndValue", HWFD_Max, "value") 'Dashboard Page
        ' Water_FH_Hot_G2.DataBindings.Add("Band2EndValue", HWFD_Max, "value") 'Water Page
        Water_FH_Hot_V1.DataBindings.Add("ValueLimitUpper", HWFD_Max, "value") 'Dashboard Page
        'Water_FH_Hot_V2.DataBindings.Add("ValueLimitUpper", HWFD_Max, "value") 'Water Page
        '-Unit Display
        Water_FH_Hot_UN1.DataBindings.Add("text", HWFD_Unit, "text") 'Dashboard Page
        'Water_FH_Hot_UN2.DataBindings.Add("text", HWFD_Unit, "text")'Water Page

        '   ______      __    __   _       __      __               _______ ____           
        '  / ____/___  / /___/ /  | |     / /___ _/ /____  _____   / ____(_) / /____  _____
        ' / /   / __ \/ / __  /   | | /| / / __ `/ __/ _ \/ ___/  / /_  / / / __/ _ \/ ___/
        '/ /___/ /_/ / / /_/ /    | |/ |/ / /_/ / /_/  __/ /     / __/ / / / /_/  __/ /    
        '\____/\____/_/\__,_/     |__/|__/\__,_/\__/\___/_/     /_/   /_/_/\__/\___/_/     


        '-Real Values
        Water_FH_Cold_G1.DataBindings.Add("value", CWFD_Actual, "value") 'Dashboard Page
        Water_FH_Cold_V1.DataBindings.Add("value", CWFD_Actual, "value") 'Dashboard Page
        Water_FH_Cold_G2.DataBindings.Add("value", CWFD_Actual, "value") 'Water Page
        Water_FH_Cold_V2.DataBindings.Add("text", CWFD_Actual, "value") 'Water Page
        '-Max Actual Values
        Water_FH_Cold_MAX1.DataBindings.Add("value", CWFD_High, "value") 'Dashboard Page
        ' Water_Fh_Cold_MAX2.DataBindings.Add("value", CWFD_High, "value") 'Water Page
        '-Min Actual Values
        Water_FH_Cold_MIN1.DataBindings.Add("value", CWFD_Low, "value") 'Dashboard Page
        ' Water_Fh_Cold_MIN2.DataBindings.Add("value", CWFD_Low, "value") 'Water Page
        '-Visual Min Values
        'Water_FH_Cold_G1.DataBindings.Add("Band2StartValue", CWFD_Min, "value") 'Dashboard Page
        'Water_FH_Cold_G2.DataBindings.Add("Band2StartValue", CWFD_Min, "value") 'Water Page
        Water_FH_Cold_V1.DataBindings.Add("ValueLimitLower", CWFD_Min, "value") 'Dashboard Page
        'Water_FH_Cold_V2.DataBindings.Add("ValueLimitLower", CWFD_Min, "value") 'Water Page
        '-Visual Max Values
        'Water_FH_Cold_G1.DataBindings.Add("Band2EndValue", CWFD_Max, "value") 'Dashboard Page
        'Water_FH_Cold_G2.DataBindings.Add("Band2EndValue", CWFD_Max, "value") 'Water Page
        Water_FH_Cold_V1.DataBindings.Add("ValueLimitUpper", CWFD_Max, "value") 'Dashboard Page
        'Water_FH_Cold_V2.DataBindings.Add("ValueLimitUpper", CWFD_Max, "value") 'Water Page
        '-Unit Display
        Water_FH_Cold_UN1.DataBindings.Add("text", CWFD_Unit, "text") 'Dashboard Page
        ' Water_Fh_Cold_UN2.DataBindings.Add("text", CWFD_Unit, "text") 'Water Page




        ' ______________________________________________________________________________________________________
        '/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/


        '    __  ______    _____   __   ____________________    __  ___   ____  ____  ________________ __  ______  ______
        '   /  |/  /   |  /  _/ | / /  / ___/_  __/ ____/   |  /  |/  /  / __ \/ __ \/ ____/ ___/ ___// / / / __ \/ ____/
        '  / /|_/ / /| |  / //  |/ /   \__ \ / / / __/ / /| | / /|_/ /  / /_/ / /_/ / __/  \__ \\__ \/ / / / /_/ / __/   
        ' / /  / / ___ |_/ // /|  /   ___/ // / / /___/ ___ |/ /  / /  / ____/ _, _/ /___ ___/ /__/ / /_/ / _, _/ /___   
        '/_/  /_/_/  |_/___/_/ |_/   /____//_/ /_____/_/  |_/_/  /_/  /_/   /_/ |_/_____//____/____/\____/_/ |_/_____/  


        '-Real Values
        Steam_Pr_Main_G1.DataBindings.Add("value", STHP_Actual, "value") 'Dashboard Page
        Steam_Pr_Main_V1.DataBindings.Add("value", STHP_Actual, "value") 'Dashboard Page
        Steam_Pr_Main_G2.DataBindings.Add("value", STHP_Actual, "value") 'Steam Page
        Steam_Pr_Main_V2.DataBindings.Add("value", STHP_Actual, "value") 'Steam Page
        '-Max Actual Values
        Steam_Pr_Main_MAX1.DataBindings.Add("value", STHP_High, "value") 'Dashboard Page
        Steam_Pr_Main_MAX2.DataBindings.Add("value", STHP_High, "value") 'Steam Page
        '-Min Actual Values
        Steam_Pr_Main_MIN1.DataBindings.Add("value", STHP_Low, "value") 'Dashboard Page
        Steam_Pr_Main_MIN2.DataBindings.Add("value", STHP_Low, "value") 'Steam Page
        '-Visual Min Values
        Steam_Pr_Main_G1.DataBindings.Add("Band2StartValue", STHP_Min, "value") 'Dashboard Page
        Steam_Pr_Main_G2.DataBindings.Add("Band2StartValue", STHP_Min, "value") 'Steam Page
        Steam_Pr_Main_V1.DataBindings.Add("ValueLimitLower", STHP_Min, "value") 'Dashboard Page
        Steam_Pr_Main_V2.DataBindings.Add("ValueLimitLower", STHP_Min, "value") 'Steam Page
        '-Visual Max Values
        Steam_Pr_Main_G1.DataBindings.Add("Band2EndValue", STHP_Max, "value") 'Dashboard Page
        Steam_Pr_Main_G2.DataBindings.Add("Band2EndValue", STHP_Max, "value") 'Steam Page
        Steam_Pr_Main_V1.DataBindings.Add("ValueLimitUpper", STHP_Max, "value") 'Dashboard Page
        Steam_Pr_Main_V2.DataBindings.Add("ValueLimitUpper", STHP_Max, "value") 'Steam Page
        '-Unit Display
        Steam_Pr_Main_UN1.DataBindings.Add("text", STHP_Unit, "text") 'Dashboard Page
        Steam_Pr_Main_UN2.DataBindings.Add("text", STHP_Unit, "text") 'Steam Page

        '[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]
        ' BOILER FEED WATER PRESSURE
        '[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

        '-Real Values
        'Steam_PR_FeedWat_G1.DataBindings.Add("value", STFWP_Actual, "value") 'Dashboard Page
        'Steam_PR_FeedWat_V1.DataBindings.Add("value", STFWP_Actual, "value") 'Dashboard Page
        Steam_Pr_FeedWat_G2.DataBindings.Add("value", STFWP_Actual, "value") 'Steam Page
        Steam_Pr_FeedWat_V2.DataBindings.Add("value", STFWP_Actual, "value") 'Steam Page
        '-Max Actual Values
        'Steam_PR_FeedWat_MAX1.DataBindings.Add("value", STFWP_High, "value") 'Dashboard Page
        Steam_Pr_FeedWat_MAX2.DataBindings.Add("value", STFWP_High, "value") 'Steam Page
        '-Min Actual Values
        'Steam_PR_FeedWat_MIN1.DataBindings.Add("value", STFWP_Low, "value") 'Dashboard Page
        Steam_Pr_FeedWat_MIN2.DataBindings.Add("value", STFWP_Low, "value") 'Steam Page
        '-Visual Min Values
        ' Steam_PR_FeedWat_G1.DataBindings.Add("Band2StartValue", STFWP_Min, "value") 'Dashboard Page
        Steam_Pr_FeedWat_G2.DataBindings.Add("Band2StartValue", STFWP_Min, "value") 'Steam Page
        ' Steam_PR_FeedWat_V1.DataBindings.Add("ValueLimitLower", STFWP_Min, "value") 'Dashboard Page
        Steam_Pr_FeedWat_V2.DataBindings.Add("ValueLimitLower", STFWP_Min, "value") 'Steam Page
        '-Visual Max Values
        ' Steam_PR_FeedWat_G1.DataBindings.Add("Band2EndValue", STFWP_Max, "value") 'Dashboard Page
        Steam_Pr_FeedWat_G2.DataBindings.Add("Band2EndValue", STFWP_Max, "value") 'Steam Page
        'Steam_PR_FeedWat_V1.DataBindings.Add("ValueLimitUpper", STFWP_Max, "value") 'Dashboard Page
        Steam_Pr_FeedWat_V2.DataBindings.Add("ValueLimitUpper", STFWP_Max, "value") 'Steam Page
        '-Unit Display
        'Steam_PR_FeedWat_UN1.DataBindings.Add("text", STFWP_Unit, "text") 'Dashboard Page
        Steam_Pr_FeedWat_UN2.DataBindings.Add("text", STFWP_Unit, "text") 'Steam Page

        '    __  ______    _____   __   ____________________    __  ___   ________    ____ _       __
        '   /  |/  /   |  /  _/ | / /  / ___/_  __/ ____/   |  /  |/  /  / ____/ /   / __ \ |     / /
        '  / /|_/ / /| |  / //  |/ /   \__ \ / / / __/ / /| | / /|_/ /  / /_  / /   / / / / | /| / / 
        ' / /  / / ___ |_/ // /|  /   ___/ // / / /___/ ___ |/ /  / /  / __/ / /___/ /_/ /| |/ |/ /  
        '/_/  /_/_/  |_/___/_/ |_/   /____//_/ /_____/_/  |_/_/  /_/  /_/   /_____/\____/ |__/|__/   


        '-Real Values
        Steam_Flow_G1.DataBindings.Add("value", STFL_Actual, "value") 'Dashboard Page
        Steam_Flow_V1.DataBindings.Add("value", STFL_Actual, "value") 'Dashboard Page
        Steam_Flow_G2.DataBindings.Add("value", STFL_Actual, "value") 'Steam Page
        Steam_Flow_V2.DataBindings.Add("value", STFL_Actual, "value") 'Steam Page
        '-Max Actual Values
        Steam_Flow_MAX1.DataBindings.Add("value", STFL_High, "value") 'Dashboard Page
        Steam_Flow_MAX2.DataBindings.Add("value", STFL_High, "value") 'Steam Page
        '-Min Actual Values
        Steam_Flow_MIN1.DataBindings.Add("value", STFL_Low, "value") 'Dashboard Page
        Steam_Flow_MIN2.DataBindings.Add("value", STFL_Low, "value") 'Steam Page
        '-Visual Min Values
        'Steam_Flow_G1.DataBindings.Add("Band2StartValue", STFL_Min, "value") 'Dashboard Page
        ' Steam_Flow_G2.DataBindings.Add("Band2StartValue", STFL_Min, "value") 'Steam Page
        Steam_Flow_V1.DataBindings.Add("ValueLimitLower", STFL_Min, "value") 'Dashboard Page
        Steam_Flow_V2.DataBindings.Add("ValueLimitLower", STFL_Min, "value") 'Steam Page
        '-Visual Max Values
        ' Steam_Flow_G1.DataBindings.Add("Band2EndValue", STFL_Max, "value") 'Dashboard Page
        ' Steam_Flow_G2.DataBindings.Add("Band2EndValue", STFL_Max, "value") 'Steam Page
        Steam_Flow_V1.DataBindings.Add("ValueLimitUpper", STFL_Max, "value") 'Dashboard Page
        Steam_Flow_V2.DataBindings.Add("ValueLimitUpper", STFL_Max, "value") 'Steam Page
        '-Unit Display
        Steam_Flow_UN1.DataBindings.Add("text", STFL_Unit, "text") 'Dashboard Page
        Steam_Flow_UN2.DataBindings.Add("text", STFL_Unit, "text") 'Steam Page

        '    __    ____ _       __   ____________________    __  ___   ____  ____  ________________ __  ______  ______
        '   / /   / __ \ |     / /  / ___/_  __/ ____/   |  /  |/  /  / __ \/ __ \/ ____/ ___/ ___// / / / __ \/ ____/
        '  / /   / / / / | /| / /   \__ \ / / / __/ / /| | / /|_/ /  / /_/ / /_/ / __/  \__ \\__ \/ / / / /_/ / __/   
        ' / /___/ /_/ /| |/ |/ /   ___/ // / / /___/ ___ |/ /  / /  / ____/ _, _/ /___ ___/ /__/ / /_/ / _, _/ /___   
        '/_____/\____/ |__/|__/   /____//_/ /_____/_/  |_/_/  /_/  /_/   /_/ |_/_____//____/____/\____/_/ |_/_____/   


        '-Real Values
        Steam_Pr_Low_G1.DataBindings.Add("value", STLP_Actual, "value") 'Dashboard Page
        Steam_Pr_Low_V1.DataBindings.Add("value", STLP_Actual, "value") 'Dashboard Page
        Steam_Pr_Low_G2.DataBindings.Add("value", STLP_Actual, "value") 'Steam Page
        Steam_Pr_Low_V2.DataBindings.Add("value", STLP_Actual, "value") 'Steam Page
        '-Max Actual Values
        Steam_Pr_Low_MAX1.DataBindings.Add("value", STLP_High, "value") 'Dashboard Page
        Steam_Pr_Low_MAX2.DataBindings.Add("value", STLP_High, "value") 'Steam Page
        '-Min Actual Values
        Steam_Pr_Low_MIN1.DataBindings.Add("value", STLP_Low, "value") 'Dashboard Page
        Steam_Pr_Low_MIN2.DataBindings.Add("value", STLP_Low, "value") 'Steam Page
        '-Visual Min Values
        Steam_Pr_Low_G1.DataBindings.Add("Band2StartValue", STLP_Min, "value") 'Dashboard Page
        Steam_Pr_Low_G2.DataBindings.Add("Band2StartValue", STLP_Min, "value") 'Steam Page
        Steam_Pr_Low_V1.DataBindings.Add("ValueLimitLower", STLP_Min, "value") 'Dashboard Page
        Steam_Pr_Low_V2.DataBindings.Add("ValueLimitLower", STLP_Min, "value") 'Steam Page
        '-Visual Max Values
        Steam_Pr_Low_G1.DataBindings.Add("Band2EndValue", STLP_Max, "value") 'Dashboard Page
        Steam_Pr_Low_G2.DataBindings.Add("Band2EndValue", STLP_Max, "value") 'Steam Page
        Steam_Pr_Low_V1.DataBindings.Add("ValueLimitUpper", STLP_Max, "value") 'Dashboard Page
        Steam_Pr_Low_V2.DataBindings.Add("ValueLimitUpper", STLP_Max, "value") 'Steam Page
        '-Unit Display
        Steam_Pr_Low_UN1.DataBindings.Add("text", STLP_Unit, "text") 'Dashboard Page
        Steam_Pr_Low_UN2.DataBindings.Add("text", STLP_Unit, "text") 'Steam Page
        '---------------------------------------------

        '    __  _____________     ____________________    __  ___   ____  ____  ________________ __  ______  ______
        '   /  |/  / ____/ __ \   / ___/_  __/ ____/   |  /  |/  /  / __ \/ __ \/ ____/ ___/ ___// / / / __ \/ ____/
        '  / /|_/ / __/ / / / /   \__ \ / / / __/ / /| | / /|_/ /  / /_/ / /_/ / __/  \__ \\__ \/ / / / /_/ / __/   
        ' / /  / / /___/ /_/ /   ___/ // / / /___/ ___ |/ /  / /  / ____/ _, _/ /___ ___/ /__/ / /_/ / _, _/ /___   
        '/_/  /_/_____/_____/   /____//_/ /_____/_/  |_/_/  /_/  /_/   /_/ |_/_____//____/____/\____/_/ |_/_____/

        '-Real Values
        Steam_Pr_Med_G1.DataBindings.Add("value", STMP_Actual, "value") 'Dashboard Page
        Steam_Pr_Med_V1.DataBindings.Add("value", STMP_Actual, "value") 'Dashboard Page
        Steam_Pr_Med_G2.DataBindings.Add("value", STMP_Actual, "value") 'Steam Page
        Steam_Pr_Med_V2.DataBindings.Add("value", STMP_Actual, "value") 'Steam Page
        '-Max Actual Values
        Steam_Pr_Med_MAX1.DataBindings.Add("value", STMP_High, "value") 'Dashboard Page
        Steam_Pr_Med_max2.DataBindings.Add("value", STMP_High, "value") 'Steam Page
        '-Min Actual Values
        Steam_Pr_Med_MIN1.DataBindings.Add("value", STMP_Low, "value") 'Dashboard Page
        Steam_Pr_Med_MIN2.DataBindings.Add("value", STMP_Low, "value") 'Steam Page
        '-Visual Min Values
        Steam_Pr_Med_G1.DataBindings.Add("Band2StartValue", STMP_Min, "value") 'Dashboard Page
        Steam_Pr_Med_G2.DataBindings.Add("Band2StartValue", STMP_Min, "value") 'Steam Page
        Steam_Pr_Med_V1.DataBindings.Add("ValueLimitLower", STMP_Min, "value") 'Dashboard Page
        Steam_Pr_Med_V2.DataBindings.Add("ValueLimitLower", STMP_Min, "value") 'Steam Page
        '-Visual Max Values
        Steam_Pr_Med_G1.DataBindings.Add("Band2EndValue", STMP_Max, "value") 'Dashboard Page
        Steam_Pr_Med_G2.DataBindings.Add("Band2EndValue", STMP_Max, "value") 'Steam Page
        Steam_Pr_Med_V1.DataBindings.Add("ValueLimitUpper", STMP_Max, "value") 'Dashboard Page
        Steam_Pr_Med_V2.DataBindings.Add("ValueLimitUpper", STMP_Max, "value") 'Steam Page
        '-Unit Display
        Steam_Pr_Med_UN1.DataBindings.Add("text", STMP_Unit, "text") 'Dashboard Page
        Steam_Pr_Med_UN2.DataBindings.Add("text", STMP_Unit, "text") 'Steam Page


        '    __    ____ _       __   ____________________    __  ___   ____  ________  ______    _   ______ 
        '   / /   / __ \ |     / /  / ___/_  __/ ____/   |  /  |/  /  / __ \/ ____/  |/  /   |  / | / / __ \
        '  / /   / / / / | /| / /   \__ \ / / / __/ / /| | / /|_/ /  / / / / __/ / /|_/ / /| | /  |/ / / / /
        ' / /___/ /_/ /| |/ |/ /   ___/ // / / /___/ ___ |/ /  / /  / /_/ / /___/ /  / / ___ |/ /|  / /_/ / 
        '/_____/\____/ |__/|__/   /____//_/ /_____/_/  |_/_/  /_/  /_____/_____/_/  /_/_/  |_/_/ |_/_____/  

        '-Real Values
        Steam_Dem_Low_G1.DataBindings.Add("value", STLPDEM_Actual, "value") 'Dashboard Page
        Steam_Dem_Low_V1.DataBindings.Add("value", STLPDEM_Actual, "value") 'Dashboard Page
        'Steam_Dem_Low_G2.DataBindings.Add("value", STLPDEM_Actual, "value") 'Steam Page
        'Steam_Dem_Low_V2.DataBindings.Add("value", STLPDEM_Actual, "value") 'Steam Page
        '-Max Actual Values
        Steam_Dem_Low_MAX1.DataBindings.Add("value", STLPDEM_High, "value") 'Dashboard Page
        'Steam_Dem_Low_MAX2.DataBindings.Add("value", STLPDEM_High, "value") 'Steam Page
        '-Min Actual Values
        Steam_Dem_Low_MIN1.DataBindings.Add("value", STLPDEM_Low, "value") 'Dashboard Page
        'Steam_Dem_Low_MIN2.DataBindings.Add("value", STLPDEM_Low, "value") 'Steam Page
        '-Visual Min Values
        'Steam_Dem_Low_G1.DataBindings.Add("Band2StartValue", STLPDEM_Min, "value") 'Dashboard Page
        'Steam_Dem_Low_G2.DataBindings.Add("Band2StartValue", STLPDEM_Min, "value") 'Steam Page
        Steam_Dem_Low_V1.DataBindings.Add("ValueLimitLower", STLPDEM_Min, "value") 'Dashboard Page
        'Steam_Dem_Low_V2.DataBindings.Add("ValueLimitLower", STLPDEM_Min, "value") 'Steam Page
        '-Visual Max Values
        'Steam_Dem_Low_G1.DataBindings.Add("Band2EndValue", STLPDEM_Max, "value") 'Dashboard Page
        'Steam_Dem_Low_G2.DataBindings.Add("Band2EndValue", STLPDEM_Max, "value") 'Steam Page
        Steam_Dem_Low_V1.DataBindings.Add("ValueLimitUpper", STLPDEM_Max, "value") 'Dashboard Page
        'Steam_Dem_Low_V2.DataBindings.Add("ValueLimitUpper", STLPDEM_Max, "value") 'Steam Page
        '-Unit Display
        Steam_Dem_Low_UN1.DataBindings.Add("text", STLPDEM_Unit, "text") 'Dashboard Page
        'Steam_Dem_Low_UN2.DataBindings.Add("text", STLPDEM_Unit, "text") 'Steam Page



        '    __  _____________     ____________________    __  ___   ____  ________  ______    _   ______ 
        '   /  |/  / ____/ __ \   / ___/_  __/ ____/   |  /  |/  /  / __ \/ ____/  |/  /   |  / | / / __ \
        '  / /|_/ / __/ / / / /   \__ \ / / / __/ / /| | / /|_/ /  / / / / __/ / /|_/ / /| | /  |/ / / / /
        ' / /  / / /___/ /_/ /   ___/ // / / /___/ ___ |/ /  / /  / /_/ / /___/ /  / / ___ |/ /|  / /_/ / 
        '/_/  /_/_____/_____/   /____//_/ /_____/_/  |_/_/  /_/  /_____/_____/_/  /_/_/  |_/_/ |_/_____/  

        '-Real Values
        Steam_Dem_Med_G1.DataBindings.Add("value", STMPDEM_Actual, "value") 'Dashboard Page
        Steam_Dem_Med_V1.DataBindings.Add("value", STMPDEM_Actual, "value") 'Dashboard Page
        ' Steam_DEM_Med_G2.DataBindings.Add("value", STMPDEM_Actual, "value") 'Steam Page
        'Steam_DEM_Med_V2.DataBindings.Add("value", STMPDEM_Actual, "value") 'Steam Page
        '-Max Actual Values
        Steam_Dem_Med_MAX1.DataBindings.Add("value", STMPDEM_High, "value") 'Dashboard Page
        ' Steam_DEM_Med_MAX2.DataBindings.Add("value", STMPDEM_High, "value") 'Steam Page
        '-Min Actual Values
        Steam_Dem_Med_MIN1.DataBindings.Add("value", STMPDEM_Low, "value") 'Dashboard Page
        ' Steam_DEM_Med_MIN2.DataBindings.Add("value", STMPDEM_Low, "value") 'Steam Page
        '-Visual Min Values
        'Steam_Dem_Med_G1.DataBindings.Add("Band2StartValue", STMPDEM_Min, "value") 'Dashboard Page
        ' Steam_DEM_Med_G2.DataBindings.Add("Band2StartValue", STMPDEM_Min, "value") 'Steam Page
        Steam_Dem_Med_V1.DataBindings.Add("ValueLimitLower", STMPDEM_Min, "value") 'Dashboard Page
        ' Steam_DEM_Med_V2.DataBindings.Add("ValueLimitLower", STMPDEM_Min, "value") 'Steam Page
        '-Visual Max Values
        'Steam_Dem_Med_G1.DataBindings.Add("Band2EndValue", STMPDEM_Max, "value") 'Dashboard Page
        ' Steam_DEM_Med_G2.DataBindings.Add("Band2EndValue", STMPDEM_Max, "value") 'Steam Page
        Steam_Dem_Med_V1.DataBindings.Add("ValueLimitUpper", STMPDEM_Max, "value") 'Dashboard Page
        ' Steam_DEM_Med_V2.DataBindings.Add("ValueLimitUpper", STMPDEM_Max, "value") 'Steam Page
        '-Unit Display
        Steam_Dem_Med_UN1.DataBindings.Add("text", STMPDEM_Unit, "text") 'Dashboard Page
        ' Steam_DEM_Med_UN2.DataBindings.Add("text", STMPDEM_Unit, "text") 'Steam Page

        ' ______________________________________________________________________________________________________
        '/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/
        '    ___    ________     ____  ____  ________________ __  ______  ______
        '   /   |  /  _/ __ \   / __ \/ __ \/ ____/ ___/ ___// / / / __ \/ ____/
        '  / /| |  / // /_/ /  / /_/ / /_/ / __/  \__ \\__ \/ / / / /_/ / __/   
        ' / ___ |_/ // _, _/  / ____/ _, _/ /___ ___/ /__/ / /_/ / _, _/ /___   
        '/_/  |_/___/_/ |_|  /_/   /_/ |_/_____//____/____/\____/_/ |_/_____/   


        '-Real Values
        Air_Pr_Main_G1.DataBindings.Add("value", ALP_Actual, "value") 'Dashboard Page
        Air_Pr_Main_V1.DataBindings.Add("value", ALP_Actual, "value") 'Dashboard Page
        Air_Pr_Main_G2.DataBindings.Add("value", ALP_Actual, "value") 'Air Page
        Air_Pr_Main_V2.DataBindings.Add("value", ALP_Actual, "value") 'Air Page
        '-Max Actual Values
        Air_Pr_Main_MAX1.DataBindings.Add("value", ALP_High, "value") 'Dashboard Page
        Air_Pr_Main_MAX2.DataBindings.Add("value", ALP_High, "value") 'Air Page
        '-Min Actual Values
        Air_Pr_Main_MIN1.DataBindings.Add("value", ALP_Low, "value") 'Dashboard Page
        Air_Pr_Main_MIN2.DataBindings.Add("value", ALP_Low, "value") 'Air Page
        '-Visual Min Values
        Air_Pr_Main_G1.DataBindings.Add("Band2StartValue", ALP_Min, "value") 'Dashboard Page
        Air_Pr_Main_G2.DataBindings.Add("Band2StartValue", ALP_Min, "value") 'Air Page
        Air_Pr_Main_V1.DataBindings.Add("ValueLimitLower", ALP_Min, "value") 'Dashboard Page
        Air_Pr_Main_V2.DataBindings.Add("ValueLimitLower", ALP_Min, "value") 'Air Page
        '-Visual Max Values
        Air_Pr_Main_G1.DataBindings.Add("Band2EndValue", ALP_Max, "value") 'Dashboard Page
        Air_Pr_Main_G2.DataBindings.Add("Band2EndValue", ALP_Max, "value") 'Air Page
        Air_Pr_Main_V1.DataBindings.Add("ValueLimitUpper", ALP_Max, "value") 'Dashboard Page
        Air_Pr_Main_V2.DataBindings.Add("ValueLimitUpper", ALP_Max, "value") 'Air Page
        '-Unit Display
        Air_Pr_Main_UN1.DataBindings.Add("text", ALP_Unit, "text") 'Dashboard Page
        Air_Pr_Main_UN2.DataBindings.Add("text", ALP_Unit, "text") 'Air Page



        '                                      ____________
        ' _____________________________________|ELECTRICAL|_____________________________________________________
        '/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/
        '    _   __           __  __       ___ 
        '   / | / /___  _____/ /_/ /_     /   |
        '  /  |/ / __ \/ ___/ __/ __ \   / /| |
        ' / /|  / /_/ / /  / /_/ / / /  / ___ |
        '/_/ |_/\____/_/   \__/_/ /_/  /_/  |_|

        '-Real Values
        Electrical_NA_G1.DataBindings.Add("value", ELNA_Actual, "value") 'Dashboard Page
        Electrical_NA_V1.DataBindings.Add("value", ELNA_Actual, "value") 'Dashboard Page
        Electrical_NA_G2.DataBindings.Add("value", ELNA_Actual, "value") 'Electrical Page
        Electrical_NA_V2.DataBindings.Add("value", ELNA_Actual, "value") 'Electrical Page
        '-Max Actual Values
        'Electrical_NA_MAX1.DataBindings.Add("value", ELNA_High, "value") 'Dashboard Page
        Electrical_NA_MAX2.DataBindings.Add("value", ELNA_High, "value") 'Electrical Page
        '-Min Actual Values
        'Electrical_NA_MIN1.DataBindings.Add("value", ELNA_Low, "value") 'Dashboard Page
        Electrical_NA_MIN2.DataBindings.Add("value", ELNA_Low, "value") 'Electrical Page
        '-Visual Min Values
        Electrical_NA_G1.DataBindings.Add("Band2StartValue", ELNA_Min, "value") 'Dashboard Page
        Electrical_NA_G2.DataBindings.Add("Band2StartValue", ELNA_Min, "value") 'Electrical Page
        Electrical_NA_V1.DataBindings.Add("ValueLimitLower", ELNA_Min, "value") 'Dashboard Page
        Electrical_NA_V2.DataBindings.Add("ValueLimitLower", ELNA_Min, "value") 'Electrical Page
        '-Visual Max Values
        Electrical_NA_G1.DataBindings.Add("Band2EndValue", ELNA_Max, "value") 'Dashboard Page
        Electrical_NA_G2.DataBindings.Add("Band2EndValue", ELNA_Max, "value") 'Electrical Page
        Electrical_NA_V1.DataBindings.Add("ValueLimitUpper", ELNA_Max, "value") 'Dashboard Page
        Electrical_NA_V2.DataBindings.Add("ValueLimitUpper", ELNA_Max, "value") 'Electrical Page
        '-Unit Display
        'Electrical_NA_UN1.DataBindings.Add("text", ELNA_Unit, "text") 'Dashboard Page
        Electrical_NA_UN2.DataBindings.Add("text", ELNA_Unit, "text") 'Electrical Page



        '    _   __           __  __       ____ 
        '   / | / /___  _____/ /_/ /_     / __ )
        '  /  |/ / __ \/ ___/ __/ __ \   / __  |
        ' / /|  / /_/ / /  / /_/ / / /  / /_/ / 
        '/_/ |_/\____/_/   \__/_/ /_/  /_____/  

        '-Real Values
        Electrical_NB_G1.DataBindings.Add("value", ELNB_Actual, "value") 'Dashboard Page
        Electrical_NB_V1.DataBindings.Add("value", ELNB_Actual, "value") 'Dashboard Page
        Electrical_NB_G2.DataBindings.Add("value", ELNB_Actual, "value") 'Electrical Page
        Electrical_NB_V2.DataBindings.Add("value", ELNB_Actual, "value") 'Electrical Page
        '-Max Actual Values
        'Electrical_NB_MAX1.DataBindings.Add("value", ELNB_High, "value") 'Dashboard Page
        Electrical_NB_MAX2.DataBindings.Add("value", ELNB_High, "value") 'Electrical Page
        '-Min Actual Values
        'Electrical_NB_MIN1.DataBindings.Add("value", ELNB_Low, "value") 'Dashboard Page
        Electrical_NB_MIN2.DataBindings.Add("value", ELNB_Low, "value") 'Electrical Page
        '-Visual Min Values
        Electrical_NB_G1.DataBindings.Add("Band2StartValue", ELNB_Min, "value") 'Dashboard Page
        Electrical_NB_G2.DataBindings.Add("Band2StartValue", ELNB_Min, "value") 'Electrical Page
        Electrical_NB_V1.DataBindings.Add("ValueLimitLower", ELNB_Min, "value") 'Dashboard Page
        Electrical_NB_V2.DataBindings.Add("ValueLimitLower", ELNB_Min, "value") 'Electrical Page
        '-Visual Max Values
        Electrical_NB_G1.DataBindings.Add("Band2EndValue", ELNB_Max, "value") 'Dashboard Page
        Electrical_NB_G2.DataBindings.Add("Band2EndValue", ELNB_Max, "value") 'Electrical Page
        Electrical_NB_V1.DataBindings.Add("ValueLimitUpper", ELNB_Max, "value") 'Dashboard Page
        Electrical_NB_V2.DataBindings.Add("ValueLimitUpper", ELNB_Max, "value") 'Electrical Page
        '-Unit Display
        'Electrical_NB_UN1.DataBindings.Add("text", ELNB_Unit, "text") 'Dashboard Page
        Electrical_NB_UN2.DataBindings.Add("text", ELNB_Unit, "text") 'Electrical Page


        '    _   __           __  __       ______
        '   / | / /___  _____/ /_/ /_     / ____/
        '  /  |/ / __ \/ ___/ __/ __ \   / /     
        ' / /|  / /_/ / /  / /_/ / / /  / /___   
        '/_/ |_/\____/_/   \__/_/ /_/   \____/   
        '-Real Values
        Electrical_NC_G1.DataBindings.Add("value", ELNC_Actual, "value") 'Dashboard Page
        Electrical_NC_V1.DataBindings.Add("value", ELNC_Actual, "value") 'Dashboard Page
        Electrical_NC_G2.DataBindings.Add("value", ELNC_Actual, "value") 'Electrical Page
        Electrical_NC_V2.DataBindings.Add("value", ELNC_Actual, "value") 'Electrical Page
        '-Max Actual Values
        'Electrical_NC_MAX1.DataBindings.Add("value", ELNC_High, "value") 'Dashboard Page
        Electrical_NC_MAX2.DataBindings.Add("value", ELNC_High, "value") 'Electrical Page
        '-Min Actual Values
        'Electrical_NC_MIN1.DataBindings.Add("value", ELNC_Low, "value") 'Dashboard Page
        Electrical_NC_MIN2.DataBindings.Add("value", ELNC_Low, "value") 'Electrical Page
        '-Visual Min Values
        Electrical_NC_G1.DataBindings.Add("Band2StartValue", ELNC_Min, "value") 'Dashboard Page
        Electrical_NC_G2.DataBindings.Add("Band2StartValue", ELNC_Min, "value") 'Electrical Page
        Electrical_NC_V1.DataBindings.Add("ValueLimitLower", ELNC_Min, "value") 'Dashboard Page
        Electrical_NC_V2.DataBindings.Add("ValueLimitLower", ELNC_Min, "value") 'Electrical Page
        '-Visual Max Values
        Electrical_NC_G1.DataBindings.Add("Band2EndValue", ELNC_Max, "value") 'Dashboard Page
        Electrical_NC_G2.DataBindings.Add("Band2EndValue", ELNC_Max, "value") 'Electrical Page
        Electrical_NC_V1.DataBindings.Add("ValueLimitUpper", ELNC_Max, "value") 'Dashboard Page
        Electrical_NC_V2.DataBindings.Add("ValueLimitUpper", ELNC_Max, "value") 'Electrical Page
        '-Unit Display
        'Electrical_NC_UN1.DataBindings.Add("text", ELNC_Unit, "text") 'Dashboard Page
        Electrical_NC_UN2.DataBindings.Add("text", ELNC_Unit, "text") 'Electrical Page

        '   _____             __  __       ___ 
        '  / ___/____  __  __/ /_/ /_     /   |
        '  \__ \/ __ \/ / / / __/ __ \   / /| |
        ' ___/ / /_/ / /_/ / /_/ / / /  / ___ |
        '/____/\____/\__,_/\__/_/ /_/  /_/  |_|


        '-Real Values
        Electrical_SA_G1.DataBindings.Add("value", ELSA_Actual, "value") 'Dashboard Page
        Electrical_SA_V1.DataBindings.Add("value", ELSA_Actual, "value") 'Dashboard Page
        Electrical_SA_G2.DataBindings.Add("value", ELSA_Actual, "value") 'Electrical Page
        Electrical_SA_V2.DataBindings.Add("value", ELSA_Actual, "value") 'Electrical Page
        '-Max Actual Values
        'Electrical_SA_MAX1.DataBindings.Add("value", ELSA_High, "value") 'Dashboard Page
        Electrical_SA_MAX2.DataBindings.Add("value", ELSA_High, "value") 'Electrical Page
        '-Min Actual Values
        'Electrical_SA_MIN1.DataBindings.Add("value", ELSA_Low, "value") 'Dashboard Page
        Electrical_SA_MIN2.DataBindings.Add("value", ELSA_Low, "value") 'Electrical Page
        '-Visual Min Values
        Electrical_SA_G1.DataBindings.Add("Band2StartValue", ELSA_Min, "value") 'Dashboard Page
        Electrical_SA_G2.DataBindings.Add("Band2StartValue", ELSA_Min, "value") 'Electrical Page
        Electrical_SA_V1.DataBindings.Add("ValueLimitLower", ELSA_Min, "value") 'Dashboard Page
        Electrical_SA_V2.DataBindings.Add("ValueLimitLower", ELSA_Min, "value") 'Electrical Page
        '-Visual Max Values
        Electrical_SA_G1.DataBindings.Add("Band2EndValue", ELSA_Max, "value") 'Dashboard Page
        Electrical_SA_G2.DataBindings.Add("Band2EndValue", ELSA_Max, "value") 'Electrical Page
        Electrical_SA_V1.DataBindings.Add("ValueLimitUpper", ELSA_Max, "value") 'Dashboard Page
        Electrical_SA_V2.DataBindings.Add("ValueLimitUpper", ELSA_Max, "value") 'Electrical Page
        '-Unit Display
        'Electrical_SA_UN1.DataBindings.Add("text", ELSA_Unit, "text") 'Dashboard Page
        Electrical_SA_UN2.DataBindings.Add("text", ELSA_Unit, "text") 'Electrical Page

        '   _____             __  __       ____ 
        '  / ___/____  __  __/ /_/ /_     / __ )
        '  \__ \/ __ \/ / / / __/ __ \   / __  |
        ' ___/ / /_/ / /_/ / /_/ / / /  / /_/ / 
        '/____/\____/\__,_/\__/_/ /_/  /_____/  


        '-Real Values
        Electrical_SB_G1.DataBindings.Add("value", ELSB_Actual, "value") 'Dashboard Page
        Electrical_SB_V1.DataBindings.Add("value", ELSB_Actual, "value") 'Dashboard Page
        Electrical_SB_G2.DataBindings.Add("value", ELSB_Actual, "value") 'Electrical Page
        Electrical_SB_V2.DataBindings.Add("value", ELSB_Actual, "value") 'Electrical Page
        '-Max Actual Values
        'Electrical_SB_MAX1.DataBindings.Add("value", ELSB_High, "value") 'Dashboard Page
        Electrical_SB_MAX2.DataBindings.Add("value", ELSB_High, "value") 'Electrical Page
        '-Min Actual Values
        'Electrical_SB_MIN1.DataBindings.Add("value", ELSB_Low, "value") 'Dashboard Page
        Electrical_SB_MIN2.DataBindings.Add("value", ELSB_Low, "value") 'Electrical Page
        '-Visual Min Values
        Electrical_SB_G1.DataBindings.Add("Band2StartValue", ELSB_Min, "value") 'Dashboard Page
        Electrical_SB_G2.DataBindings.Add("Band2StartValue", ELSB_Min, "value") 'Electrical Page
        Electrical_SB_V1.DataBindings.Add("ValueLimitLower", ELSB_Min, "value") 'Dashboard Page
        Electrical_SB_V2.DataBindings.Add("ValueLimitLower", ELSB_Min, "value") 'Electrical Page
        '-Visual Max Values
        Electrical_SB_G1.DataBindings.Add("Band2EndValue", ELSB_Max, "value") 'Dashboard Page
        Electrical_SB_G2.DataBindings.Add("Band2EndValue", ELSB_Max, "value") 'Electrical Page
        Electrical_SB_V1.DataBindings.Add("ValueLimitUpper", ELSB_Max, "value") 'Dashboard Page
        Electrical_SB_V2.DataBindings.Add("ValueLimitUpper", ELSB_Max, "value") 'Electrical Page
        '-Unit Display
        'Electrical_SB_UN1.DataBindings.Add("text", ELSB_Unit, "text") 'Dashboard Page
        Electrical_SB_UN2.DataBindings.Add("text", ELSB_Unit, "text") 'Electrical Page



        '   _____             __  __       ______
        '  / ___/____  __  __/ /_/ /_     / ____/
        '  \__ \/ __ \/ / / / __/ __ \   / /     
        ' ___/ / /_/ / /_/ / /_/ / / /  / /___   
        '/____/\____/\__,_/\__/_/ /_/   \____/   


        '-Real Values
        Electrical_SC_G1.DataBindings.Add("value", ELSC_Actual, "value") 'Dashboard Page
        Electrical_SC_V1.DataBindings.Add("value", ELSC_Actual, "value") 'Dashboard Page
        Electrical_SC_G2.DataBindings.Add("value", ELSC_Actual, "value") 'Electrical Page
        Electrical_SC_V2.DataBindings.Add("value", ELSC_Actual, "value") 'Electrical Page
        '-Max Actual Values
        'Electrical_SC_MAX1.DataBindings.Add("value", ELSC_High, "value") 'Dashboard Page
        Electrical_SC_MAX2.DataBindings.Add("value", ELSC_High, "value") 'Electrical Page
        '-Min Actual Values
        'Electrical_SC_MIN1.DataBindings.Add("value", ELSC_Low, "value") 'Dashboard Page
        Electrical_SC_MIN2.DataBindings.Add("value", ELSC_Low, "value") 'Electrical Page
        '-Visual Min Values
        Electrical_SC_G1.DataBindings.Add("Band2StartValue", ELSC_Min, "value") 'Dashboard Page
        Electrical_SC_G2.DataBindings.Add("Band2StartValue", ELSC_Min, "value") 'Electrical Page
        Electrical_SC_V1.DataBindings.Add("ValueLimitLower", ELSC_Min, "value") 'Dashboard Page
        Electrical_SC_V2.DataBindings.Add("ValueLimitLower", ELSC_Min, "value") 'Electrical Page
        '-Visual Max Values
        Electrical_SC_G1.DataBindings.Add("Band2EndValue", ELSC_Max, "value") 'Dashboard Page
        Electrical_SC_G2.DataBindings.Add("Band2EndValue", ELSC_Max, "value") 'Electrical Page
        Electrical_SC_V1.DataBindings.Add("ValueLimitUpper", ELSC_Max, "value") 'Dashboard Page
        Electrical_SC_V2.DataBindings.Add("ValueLimitUpper", ELSC_Max, "value") 'Electrical Page
        '-Unit Display
        'Electrical_SC_UN1.DataBindings.Add("text", ELSC_Unit, "text") 'Dashboard Page
        Electrical_SC_UN2.DataBindings.Add("text", ELSC_Unit, "text") 'Electrical Page

        ' ______________________________________________________________________________________________________
        '/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/
        'ENVIROMENTAL 
        InsideTempDisp1.DataBindings.Add("value", EVITP_Actual, "text")
        InsideHumidityDisp1.DataBindings.Add("value", EVIHM_Actual, "text")
        OutsideTempDisp1.DataBindings.Add("value", EVOTP_Actual, "text")
        OutsideHumidityDisp1.DataBindings.Add("value", EVOHM_Actual, "text")







        ' ______________________________________________________________________________________________________
        '/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/_____/
    End Sub



    Public Sub SaveSensorData(DT As String, CWSUP As Object, CWPRP As Object, CWPOP As Object, CWFD As Object,
                              HWPRP As Object, HWPOP As Object, HWFD As Object, HWTP As Object,
                              HWFL As Object, STFWP As Object, STHP As Object, STLP As Object, STMP As Object,
                              STLD As Object, STMD As Object, STFL As Object, ELNA As Object,
                              ELNB As Object, ELNC As Object, ELSA As Object, ELSB As Object,
                              ELSC As Object, APR As Object)

        Using conn As New MySqlConnection(SQL_ConString)

            Try
                conn.Open()

                Dim sql As String = "INSERT INTO a_vals (Date_Time,CW_Supply, CW_PreFiltPres, CW_PostFiltPres, CW_FiltDif, HW_PreFiltPres, HW_PostFiltPres, HW_FiltDif, HW_Temp, HW_Flow, ST_FeedWaterPres, ST_HeadPres,ST_LowPres, ST_MedPres, ST_LowDem,ST_MedDem, ST_Flow,  EL_NorthA, EL_NorthB, EL_NorthC, EL_SouthA, EL_SouthB,EL_SouthC,AR_LinePres) 
VALUES (@DT,@CWSUP, @CWPRP, @CWPOP, @CWFD, @HWPRP, @HWPOP, @HWFD, @HWTP, @HWFL, @STFWP, @STHP, @STLP, @STMP, @STLD, @STMD, @STFL, @ELNA, @ELNB,@ELNC, @ELSA, @ELSB, @ELSC, @APR)"
                If IsNumeric(CWSUP) AndAlso IsNumeric(CWPRP) AndAlso IsNumeric(CWPOP) AndAlso IsNumeric(CWFD) AndAlso
                    IsNumeric(HWPRP) AndAlso IsNumeric(HWPOP) AndAlso IsNumeric(HWFD) AndAlso IsNumeric(HWFL) AndAlso
                    IsNumeric(HWTP) AndAlso IsNumeric(APR) AndAlso IsNumeric(STLP) AndAlso IsNumeric(STMP) AndAlso
                    IsNumeric(STHP) AndAlso IsNumeric(STFL) AndAlso IsNumeric(STLD) AndAlso IsNumeric(STMD) AndAlso
                    IsNumeric(STFWP) AndAlso IsNumeric(ELNA) AndAlso IsNumeric(ELNB) AndAlso IsNumeric(ELNC) AndAlso
                    IsNumeric(ELSA) AndAlso IsNumeric(ELSB) AndAlso IsNumeric(ELSC) Then

                    Using cmd As New MySqlCommand(sql, conn)
                        cmd.Parameters.AddWithValue("@DT", DT) '-------Dateandalso Time
                        cmd.Parameters.AddWithValue("@CWSUP", CWSUP) '-------Dateandalso Time
                        cmd.Parameters.AddWithValue("@CWPRP", CWPRP) '-Cold Water Pre Filter Pressure
                        cmd.Parameters.AddWithValue("@CWPOP", CWPOP) '-Cold Water Post Filter Pressure
                        cmd.Parameters.AddWithValue("@CWFD", CWFD) '---Cold Water Filter Differential Pressure
                        cmd.Parameters.AddWithValue("@HWPRP", HWPRP) '-Hot Water Pre Filter Pressure
                        cmd.Parameters.AddWithValue("@HWPOP", HWPOP) '-Hot Water Post Filter Pressure
                        cmd.Parameters.AddWithValue("@HWFD", HWFD) '---Hot Water Filter Differential Pressure
                        cmd.Parameters.AddWithValue("@HWTP", HWTP) '---Hot Water Temperature
                        cmd.Parameters.AddWithValue("@HWFL", HWFL) '---Hot Water Flow Rate
                        cmd.Parameters.AddWithValue("@STFWP", STFWP) '--Boiler Feed Water Pressure
                        cmd.Parameters.AddWithValue("@STHP", STHP) '---Steam Header Pressure
                        cmd.Parameters.AddWithValue("@STLP", STLP) '---Low Steam Pressure
                        cmd.Parameters.AddWithValue("@STMP", STMP) '---Med Steam Pressure
                        cmd.Parameters.AddWithValue("@STLD", STLD) '---Low Steam Demand
                        cmd.Parameters.AddWithValue("@STMD", STMD) '---Med Steam Demand
                        cmd.Parameters.AddWithValue("@STFL", STFL) '---Steam Flow Rate
                        cmd.Parameters.AddWithValue("@ELNA", ELNA) '---Electrical North A Phase Voltage
                        cmd.Parameters.AddWithValue("@ELNB", ELNB) '---Electrical North B Phase Voltage
                        cmd.Parameters.AddWithValue("@ELNC", ELNC) '---Electrical North C Phase Voltage
                        cmd.Parameters.AddWithValue("@ELSA", ELSA) '---Electrical South A Phase Voltage
                        cmd.Parameters.AddWithValue("@ELSB", ELSB) '---Electrical South B Phase Voltage
                        cmd.Parameters.AddWithValue("@ELSC", ELSC) '---Electrical South C Phase Voltage
                        cmd.Parameters.AddWithValue("@APR", APR) '-----Air Pressure 


                        cmd.ExecuteNonQuery() '--------------------------SEND DATA TO SERVER------------
                    End Using
                End If
            Catch ex As Exception
                RunSQL = False
                LogAlarmIcon1.Image = My.Resources.LogFileBad
                LogAlarmIcon1.BackgroundImage = My.Resources.BADBG
                Set2_RunLogLabel.Text = "FALSE"
                MsgBox("(Sensor Log) " & ex.Message)
            End Try

        End Using
    End Sub
    Public Sub SaveAlarmData(DT As String, State As String, Message As String, Type As String)

        Using conn As New MySqlConnection(SQL_ConString)

            Try
                conn.Open()
                Dim sql As String = "INSERT INTO alarm_history (Date_Time,State,Message,Alarm_Type) VALUES (@DT,@State, @Message, @Type)"
                Using cmd As New MySqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@DT", DT)
                    cmd.Parameters.AddWithValue("@State", State)
                    cmd.Parameters.AddWithValue("@Message", Message)
                    cmd.Parameters.AddWithValue("@Type", Type)
                    cmd.ExecuteNonQuery() '----SEND DATA TO SERVER------------
                End Using

            Catch ex As Exception
                RunSQL = False
                LogAlarmIcon1.Image = My.Resources.LogFileBad
                LogAlarmIcon1.BackgroundImage = My.Resources.BADBG
                Set2_RunLogLabel.Text = "FALSE"
                MessageBox.Show("(Alarm Log) " & ex.Message)
            End Try
        End Using
    End Sub
    Public Sub SaveSystemData(DT As String, Message As String, User As String)

        Using conn As New MySqlConnection(SQL_ConString)

            Try
                conn.Open()
                Dim sql As String = "INSERT INTO system_log (Date_Time,Message,Active_User) VALUES (@DT, @Message, @Active_User)"
                Using cmd As New MySqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@DT", DT)
                    cmd.Parameters.AddWithValue("@Message", Message)
                    cmd.Parameters.AddWithValue("@Active_User", User)
                    cmd.ExecuteNonQuery() '----SEND DATA TO SERVER------------
                End Using
            Catch ex As Exception
                RunSQL = False
                LogAlarmIcon1.Image = My.Resources.LogFileBad
                LogAlarmIcon1.BackgroundImage = My.Resources.BADBG
                Set2_RunLogLabel.Text = "FALSE"
                MessageBox.Show("(System Log) " & ex.Message)
            End Try
        End Using
    End Sub
    Dim AlarmsInList As Integer = 250
    Private Sub RefreshAlarmTable()
        Dim query As String = "SELECT * FROM alarm_history ORDER BY id DESC LIMIT " & AlarmsInList & ";"
        Using connection As New MySqlConnection(SQL_ConString)
            Dim adapter As New MySqlDataAdapter(query, connection)
            Dim table As New DataTable()
            adapter.Fill(table)
            AlarmGrid.DataSource = table
        End Using
        AlarmGrid.Columns(1).DefaultCellStyle.Format = "MM/dd/yyyy hh:mm:ss tt"
    End Sub


    Public Function GetNow() As String
        Return (Now.Year.ToString("0000") & Now.Month.ToString("00") & Now.Day.ToString("00") &
                Now.Hour.ToString("00") & Now.Minute.ToString("00") & Now.Second.ToString("00"))
    End Function
    Public Sub Update_Elec_Charts(sender As System.Object, e As EventArgs) Handles ELSA_Actual.ValueChanged,
        ELSB_Actual.ValueChanged, ELSC_Actual.ValueChanged, ELNA_Actual.ValueChanged,
        ELNB_Actual.ValueChanged, ELNC_Actual.ValueChanged
        If Firsttick Then
            If IsNumeric(sender.value) Then
                UpdateElectricalCharts()
            End If
        End If
    End Sub

    Private Sub Zoom_Gauge(sender As System.Object, e As MouseEventArgs) Handles Steam_Pr_Low_G1.MouseWheel,
    Steam_Pr_Low_G2.MouseWheel, Steam_Pr_Med_G1.MouseWheel, Steam_Pr_Med_G2.MouseWheel, Steam_Pr_Main_G1.MouseWheel,
    Steam_Pr_Main_G2.MouseWheel, Steam_Pr_FeedWat_G2.MouseWheel, Water_Pr_ColdPre_G2.MouseWheel, Water_Pr_Cold_G1.MouseWheel,
    Water_Pr_Cold_G2.MouseWheel, Water_Pr_HotPre_G2.MouseWheel, Water_Pr_Hot_G1.MouseWheel, Water_Pr_Hot_G2.MouseWheel,
    Water_Tp_Hot_G1.MouseWheel, Water_Tp_Hot_G2.MouseWheel, Water_Fl_Hot_G1.MouseWheel, Water_Fl_Hot_G2.MouseWheel,
    Water_Sup_City_G1.MouseWheel, Water_Sup_City_G2.MouseWheel, Air_Pr_Main_G1.MouseWheel, Air_Pr_Main_G2.MouseWheel,
    Electrical_NA_G1.MouseWheel, Electrical_NA_G2.MouseWheel, Electrical_NB_G1.MouseWheel, Electrical_NB_G2.MouseWheel,
    Electrical_NC_G1.MouseWheel, Electrical_NC_G2.MouseWheel, Electrical_SA_G1.MouseWheel, Electrical_SA_G2.MouseWheel,
    Electrical_SB_G1.MouseWheel, Electrical_SB_G2.MouseWheel, Electrical_SC_G1.MouseWheel, Electrical_SC_G2.MouseWheel
        If CurrentUserLevel > 1 Then
            If e.Delta > 0 Then ' UP
                If sender.Minimum < (sender.Band2StartValue - 5) Then sender.Minimum += 1 Else sender.minimum = sender.Band2StartValue - 5
                If sender.Maximum > (sender.Band2EndValue + 5) Then sender.Maximum -= 1 Else sender.maximum = sender.Band2EndValue + 5
            ElseIf e.Delta < 0 Then ' DOWN
                If sender.Minimum > 0 Then sender.Minimum -= 1 Else sender.Minimum = 0
                If sender.Maximum < (sender.Band2EndValue - sender.Band2StartValue) + (sender.Band2StartValue * 2) Then sender.Maximum += 1 Else sender.Maximum = (sender.Band2EndValue - sender.Band2StartValue) + (sender.Band2StartValue * 2)
            End If
        End If
        'ref_gauges()


    End Sub
    Sub Refresh_Gauges()
        Try '-----------------------------------------------------------------------------------------------------------------------------------------
            ' STEAM Displays  =====================================================================================================================
            '-----------------------------------------------------------------------------------------------------------------------------------------
            With Steam_Flow_G1
                .Minimum = 0
                .Band2StartValue = 0
                .Band2EndValue = STFL_Max.Value / 1000
                .Maximum = (.Band2EndValue * 1.25)
            End With
            With Steam_Flow_G2
                .Minimum = 0
                .Band2StartValue = 0
                .Band2EndValue = STFL_Max.Value / 1000
                .Maximum = 15
            End With
            With Steam_Pr_Med_G1
                .Minimum = 0
                .Band2StartValue = STMP_Min.Value
                .Band2EndValue = STMP_Max.Value
                .Maximum = (.Band2EndValue - .Band2StartValue) + (.Band2StartValue * 2)
            End With
            With Steam_Pr_Med_G2
                .Minimum = 0
                .Band2StartValue = STMP_Min.Value
                .Band2EndValue = STMP_Max.Value
                .Maximum = (.Band2EndValue - .Band2StartValue) + (.Band2StartValue * 2)
            End With
            With Steam_Dem_Med_G1
                .Minimum = 0
                .Band2StartValue = 0
                .Band2EndValue = STMPDEM_Max.Value
                .Maximum = 100
            End With
            With Steam_Pr_Low_G1
                .Minimum = 0
                .Band2StartValue = STLP_Min.Value
                .Band2EndValue = STLP_Max.Value
                .Maximum = (.Band2EndValue - .Band2StartValue) + (.Band2StartValue * 2)
            End With
            With Steam_Pr_Low_G2
                .Minimum = 0
                .Band2StartValue = STLP_Min.Value
                .Band2EndValue = STLP_Max.Value
                .Maximum = (.Band2EndValue - .Band2StartValue) + (.Band2StartValue * 2)
            End With
            With Steam_Dem_Low_G1
                .Minimum = 0
                .Band2StartValue = 0
                .Band2EndValue = STLPDEM_Max.Value
                .Maximum = 100
            End With
            With Steam_Pr_Main_G1
                .Minimum = 0
                .Band2StartValue = STHP_Min.Value
                .Band2EndValue = STHP_Max.Value
                .Maximum = (.Band2EndValue - .Band2StartValue) + (.Band2StartValue * 2)
            End With
            With Steam_Pr_Main_G2
                .Minimum = 0
                .Band2StartValue = STHP_Min.Value
                .Band2EndValue = STHP_Max.Value
                .Maximum = (.Band2EndValue - .Band2StartValue) + (.Band2StartValue * 2)
            End With
            With Steam_Pr_FeedWat_G2
                .Minimum = 0
                .Band2StartValue = STFWP_Min.Value
                .Band2EndValue = STFWP_Max.Value
                .Maximum = (.Band2EndValue - .Band2StartValue) + (.Band2StartValue * 2)
            End With
            '-----------------------------------------------------------------------------------------------------------------------------------------
            'AIR Displays ==============================================================================================================================
            '-----------------------------------------------------------------------------------------------------------------------------------------
            With Air_Pr_Main_G1
                .Minimum = 0
                .Band2StartValue = ALP_Min.Value
                .Band2EndValue = ALP_Max.Value
                .Maximum = (.Band2EndValue - .Band2StartValue) + (.Band2StartValue * 2)
            End With
            With Air_Pr_Main_G2
                .Minimum = 0
                .Band2StartValue = ALP_Min.Value
                .Band2EndValue = ALP_Max.Value
                .Maximum = (.Band2EndValue - .Band2StartValue) + (.Band2StartValue * 2)
            End With
            '-----------------------------------------------------------------------------------------------------------------------------------------
            'WATER Displays  ===========================================================================================================================
            '-----------------------------------------------------------------------------------------------------------------------------------------
            With Water_Sup_City_G1
                .Minimum = 0
                .Band2StartValue = CWSUP_Min.Value
                .Band2EndValue = CWSUP_Max.Value
                .Maximum = (.Band2EndValue - .Band2StartValue) + (.Band2StartValue * 2)
            End With
            With Water_Sup_City_G2
                .Minimum = 0
                .Band2StartValue = CWSUP_Min.Value
                .Band2EndValue = CWSUP_Max.Value
                .Maximum = (.Band2EndValue - .Band2StartValue) + (.Band2StartValue * 2)
            End With
            With Water_Pr_ColdPre_G2
                .Minimum = 0
                .Band2StartValue = CWPRP_Min.Value
                .Band2EndValue = CWPRP_Max.Value
                .Maximum = (.Band2EndValue - .Band2StartValue) + (.Band2StartValue * 2)
            End With
            With Water_Pr_Cold_G1
                .Minimum = 0
                .Band2StartValue = CWPOP_Min.Value
                .Band2EndValue = CWPOP_Max.Value
                .Maximum = (.Band2EndValue - .Band2StartValue) + (.Band2StartValue * 2)
            End With
            With Water_Pr_Cold_G2
                .Minimum = 0
                .Band2StartValue = CWPOP_Min.Value
                .Band2EndValue = CWPOP_Max.Value
                .Maximum = (.Band2EndValue - .Band2StartValue) + (.Band2StartValue * 2)
            End With
            With Water_FH_Cold_G1
                .Minimum = 0
                .Band2StartValue = CWFD_Min.Value
                .Band2EndValue = CWFD_Max.Value
                .Maximum = (.Band2EndValue * 1.25)
            End With
            With Water_FH_Cold_G2
                .Minimum = 0
                .Band2StartValue = CWFD_Min.Value
                .Band2EndValue = CWFD_Max.Value
                .Maximum = (.Band2EndValue * 1.25)
            End With
            With Water_Pr_HotPre_G2
                .Minimum = 0
                .Band2StartValue = HWPRP_Min.Value
                .Band2EndValue = HWPRP_Max.Value
                .Maximum = (.Band2EndValue - .Band2StartValue) + (.Band2StartValue * 2)
            End With
            With Water_Pr_Hot_G1
                .Minimum = 0
                .Band2StartValue = HWPOP_Min.Value
                .Band2EndValue = HWPOP_Max.Value
                .Maximum = (.Band2EndValue - .Band2StartValue) + (.Band2StartValue * 2)
            End With
            With Water_Pr_Hot_G2
                .Minimum = 0
                .Band2StartValue = HWPOP_Min.Value
                .Band2EndValue = HWPOP_Max.Value
                .Maximum = (.Band2EndValue - .Band2StartValue) + (.Band2StartValue * 2)
            End With
            With Water_FH_Hot_G1
                .Minimum = 0
                .Band2StartValue = HWFD_Min.Value
                .Band2EndValue = HWFD_Max.Value
                .Maximum = (.Band2EndValue * 1.25)
            End With
            With Water_FH_Hot_G2
                .Minimum = 0
                .Band2StartValue = HWFD_Min.Value
                .Band2EndValue = HWFD_Max.Value
                .Maximum = (.Band2EndValue * 1.25)
            End With
            With Water_Fl_Hot_G2
                .Minimum = 0
                .Band2StartValue = HWFL_Min.Value
                .Band2EndValue = HWFL_Max.Value
                .Maximum = (.Band2EndValue - .Band2StartValue) + (.Band2StartValue * 2)
            End With
            With Water_Fl_Hot_G1
                .Minimum = 0
                .Band2StartValue = HWFL_Min.Value
                .Band2EndValue = HWFL_Max.Value
                .Maximum = (.Band2EndValue - .Band2StartValue) + (.Band2StartValue * 2)
            End With
            With Water_Tp_Hot_G1
                .Minimum = 0
                .Band2StartValue = HWTP_Min.Value
                .Band2EndValue = HWTP_Max.Value
                .Maximum = (.Band2EndValue - .Band2StartValue) + (.Band2StartValue * 2)
            End With
            With Water_Tp_Hot_G2
                .Minimum = 0
                .Band2StartValue = HWTP_Min.Value
                .Band2EndValue = HWTP_Max.Value
                .Maximum = (.Band2EndValue - .Band2StartValue) + (.Band2StartValue * 2)
            End With
            '-----------------------------------------------------------------------------------------------------------------------------------------
            'ELECTRICAL Displays  ====================================================================================================================
            '-----------------------------------------------------------------------------------------------------------------------------------------
            With Electrical_NA_G1
                .Minimum = 0
                .Band2StartValue = ELNA_Min.Value
                .Band2EndValue = ELNA_Max.Value
                .Maximum = (.Band2EndValue - .Band2StartValue) + (.Band2StartValue * 2)
            End With
            With Electrical_NA_G2
                .Minimum = 0
                .Band2StartValue = ELNA_Min.Value
                .Band2EndValue = ELNA_Max.Value
                .Maximum = (.Band2EndValue - .Band2StartValue) + (.Band2StartValue * 2)
            End With
            With Electrical_NB_G1
                .Minimum = 0
                .Band2StartValue = ELNB_Min.Value
                .Band2EndValue = ELNB_Max.Value
                .Maximum = (.Band2EndValue - .Band2StartValue) + (.Band2StartValue * 2)
            End With
            With Electrical_NB_G2
                .Minimum = 0
                .Band2StartValue = ELNB_Min.Value
                .Band2EndValue = ELNB_Max.Value
                .Maximum = (.Band2EndValue - .Band2StartValue) + (.Band2StartValue * 2)
            End With
            With Electrical_NC_G1
                .Minimum = 0
                .Band2StartValue = ELNC_Min.Value
                .Band2EndValue = ELNC_Max.Value
                .Maximum = (.Band2EndValue - .Band2StartValue) + (.Band2StartValue * 2)
            End With
            With Electrical_NC_G2
                .Minimum = 0
                .Band2StartValue = ELNC_Min.Value
                .Band2EndValue = ELNC_Max.Value
                .Maximum = (.Band2EndValue - .Band2StartValue) + (.Band2StartValue * 2)
            End With
            With Electrical_SA_G1
                .Minimum = 0
                .Band2StartValue = ELSA_Min.Value
                .Band2EndValue = ELSA_Max.Value
                .Maximum = (.Band2EndValue - .Band2StartValue) + (.Band2StartValue * 2)
            End With
            With Electrical_SA_G2
                .Minimum = 0
                .Band2StartValue = ELSA_Min.Value
                .Band2EndValue = ELSA_Max.Value
                .Maximum = (.Band2EndValue - .Band2StartValue) + (.Band2StartValue * 2)
            End With
            With Electrical_SB_G1
                .Minimum = 0
                .Band2StartValue = ELSB_Min.Value
                .Band2EndValue = ELSB_Max.Value
                .Maximum = (.Band2EndValue - .Band2StartValue) + (.Band2StartValue * 2)
            End With
            With Electrical_SB_G2
                .Minimum = 0
                .Band2StartValue = ELSB_Min.Value
                .Band2EndValue = ELSB_Max.Value
                .Maximum = (.Band2EndValue - .Band2StartValue) + (.Band2StartValue * 2)
            End With
            With Electrical_SC_G1
                .Minimum = 0
                .Band2StartValue = ELSC_Min.Value
                .Band2EndValue = ELSC_Max.Value
                .Maximum = (.Band2EndValue - .Band2StartValue) + (.Band2StartValue * 2)
            End With
            With Electrical_SC_G2
                .Minimum = 0
                .Band2StartValue = ELSC_Min.Value
                .Band2EndValue = ELSC_Max.Value
                .Maximum = (.Band2EndValue - .Band2StartValue) + (.Band2StartValue * 2)
            End With


        Catch ex As Exception
        End Try
    End Sub
    Public Sub Refresh_Gauge_SPs(sender As Object, e As EventArgs) Handles ALP_Min.ValueChanged, ALP_Max.ValueChanged,
        ELSC_Min.ValueChanged, ELSC_Max.ValueChanged,
        ELSB_Min.ValueChanged, ELSB_Max.ValueChanged,
        ELSA_Min.ValueChanged, ELSA_Max.ValueChanged,
        ELNC_Min.ValueChanged, ELNC_Max.ValueChanged,
        ELNB_Min.ValueChanged, ELNB_Max.ValueChanged,
        ELNA_Min.ValueChanged, ELNA_Max.ValueChanged,
        STMP_Min.ValueChanged, STMP_Max.ValueChanged,
        STLP_Min.ValueChanged, STLP_Max.ValueChanged,
        STHP_Min.ValueChanged, STHP_Max.ValueChanged,
        STFL_Min.ValueChanged, STFL_Max.ValueChanged,
        STFWP_Min.ValueChanged, STFWP_Max.ValueChanged,
        CWPRP_Min.ValueChanged, CWPRP_Max.ValueChanged,
        CWPOP_Min.ValueChanged, CWPOP_Max.ValueChanged,
        CWFD_Min.ValueChanged, CWFD_Max.ValueChanged,
        CWSUP_Min.ValueChanged, CWSUP_Max.ValueChanged,
        HWPRP_Min.ValueChanged, HWPRP_Max.ValueChanged,
        HWPOP_Min.ValueChanged, HWPOP_Max.ValueChanged,
        HWFD_Min.ValueChanged, HWFD_Max.ValueChanged,
        HWTP_Min.ValueChanged, HWTP_Max.ValueChanged,
        HWFL_Min.ValueChanged, HWFL_Max.ValueChanged,
        STMPDEM_Min.ValueChanged, STMPDEM_Max.ValueChanged,
        STLPDEM_Min.ValueChanged, STLPDEM_Max.ValueChanged
        Refresh_Gauges()

        If Firsttick Then
            If RunSQL Then SaveSystemData(GetNow(), "Value for (" & sender.name & ") changed to: " & sender.text, CurrentUser)
        End If

    End Sub
    Private Sub INI_RemoveSection_USER(ByVal USER)
        Dim file As New IniFile()
        Try
            file.Load(My.Settings.INI_Loc & "\UserConfig.ini")
        Catch ex As Exception
            MsgBox("Fatal Error!", "Ref#000001 (no setting file found)")
        End Try
        Dim section As IniSection = file.Sections(USER)
        If section IsNot Nothing Then
            file.Sections.Remove(section)
        End If
        file.Save(My.Settings.INI_Loc & "\UserConfig.ini")
    End Sub
    Private Sub INI_AddSection_USER(ByVal USER, ByVal PASS, ByVal LEVEL)
        Dim file As New IniFile()
        Try
            file.Load(My.Settings.INI_Loc & "\UserConfig.ini")
        Catch ex As Exception
            MsgBox("Fatal Error!", "Ref#000001 (no setting file found)")
        End Try
        Dim newSection As IniSection = file.Sections.Add(USER)
        newSection.Keys.Add("PASS", Encode(PASS))
        newSection.Keys.Add("LAST", "NEVER")
        newSection.Keys.Add("LEVEL", LEVEL)
        file.Save(My.Settings.INI_Loc & "\UserConfig.ini")
    End Sub
    Private Sub INI_SetKey_USER(ByVal Section, ByVal Key, ByVal Val)
        Dim file As New IniFile()
        Try
            file.Load(My.Settings.INI_Loc & "\UserConfig.ini")
        Catch ex As Exception
            MsgBox("Fatal Error!", "Ref#000001 (no setting file found)")
        End Try
        file.Sections(Section).Keys(Key).Value = Val
        file.Save(My.Settings.INI_Loc & "\UserConfig.ini")
    End Sub


    Private Sub INI_SetKey_PLC(ByVal Section, ByVal Key, ByVal Val)
        Dim file As New IniFile()
        Try
            file.Load(My.Settings.INI_Loc & "\DataConfig.ini")
        Catch ex As Exception
            MsgBox("Fatal Error!", "Ref#000001 (no setting file found)")
        End Try
        file.Sections(Section).Keys(Key).Value = Val
        file.Save(My.Settings.INI_Loc & "\DataConfig.ini")
    End Sub
    Private Sub INI_SetKey_System(ByVal Section, ByVal Key, ByVal Val)
        Dim file As New IniFile()
        Try
            file.Load(My.Settings.INI_Loc & "\SystemConfig.ini")
        Catch ex As Exception
            MsgBox("Fatal Error!", "Ref#000001 (no setting file found)")
        End Try
        file.Sections(Section).Keys(Key).Value = Val
        file.Save(My.Settings.INI_Loc & "\SystemConfig.ini")
    End Sub
    Private Sub INI_SetKey_Alarm(ByVal Section, ByVal Key, ByVal Val)
        Dim file As New IniFile()
        Try
            file.Load(My.Settings.INI_Loc & "\AlarmConfig.ini")
        Catch ex As Exception
            MsgBox("Fatal Error!", "Ref#000001 (no setting file found)")
        End Try
        file.Sections(Section).Keys(Key).Value = Val
        file.Save(My.Settings.INI_Loc & "\AlarmConfig.ini")
    End Sub
    Private Sub INI_SetKey_IO(ByVal Section, ByVal Key, ByVal Val)
        Dim file As New IniFile()
        Try
            file.Load(My.Settings.INI_Loc & "\IOConfig.ini")
        Catch ex As Exception
            MsgBox("Fatal Error!", "Ref#000001 (no setting file found)")
        End Try
        file.Sections(Section).Keys(Key).Value = Val
        file.Save(My.Settings.INI_Loc & "\IOConfig.ini")
    End Sub
    Public Function INI_GetKey_AllUSER() As List(Of String)
        Dim file As New IniFile()
        Try
            file.Load(My.Settings.INI_Loc & "\UserConfig.ini")
        Catch ex As Exception
            MsgBox("Fatal Error! - Ref#000001 (UserConfig.ini) NOT found")
        End Try
        Dim Values As New List(Of String)
        For Each i As IniSection In file.Sections()
            Values.Add(i.Name)
        Next
        If Values IsNot Nothing Then
            Return Values
        Else
            Values.Add("NONE")
            Return Values
            MsgBox("Fatal Error! - Ref#000002 (no data found in key)")
        End If
    End Function
    Public Function INI_GetKey_USER(ByVal Section, ByVal Key) As String
        'Creat new INI Fileandalso populate it with the saved ini file in the resorces folder if it exists
        Dim file As New IniFile()
        Try
            file.Load(My.Settings.INI_Loc & "\UserConfig.ini")
        Catch ex As Exception
            MsgBox("Fatal Error! - Ref#000001 (UserConfig.ini) NOT found")
        End Try
        Dim Values As String = file.Sections(Section)?.Keys(Key)?.Value
        If Values IsNot Nothing Then
            Return Values
        Else
            Return "*\"
            MsgBox("Fatal Error! - Ref#000002 (no data found in key)")
        End If
    End Function
    Public Function INI_GetKey_PLC(ByVal Section, ByVal Key) As String
        'Creat new INI Fileandalso populate it with the saved ini file in the resorces folder if it exists
        Dim file As New IniFile()
        Try
            file.Load(My.Settings.INI_Loc & "\DataConfig.ini")
        Catch ex As Exception
            MsgBox("Fatal Error! - Ref#000001 (no setting file found)")
        End Try
        Dim Values As String = file.Sections(Section)?.Keys(Key)?.Value
        If Values IsNot Nothing Then
            Return Values
        Else
            Return "*\"
            MsgBox("Fatal Error! - Ref#000002 (no data found in key)")
        End If
    End Function
    Public Function INI_GetKey_System(ByVal Section, ByVal Key) As String
        'Creat new INI Fileandalso populate it with the saved ini file in the resorces folder if it exists
        Dim file As New IniFile()
        Try
            file.Load(My.Settings.INI_Loc & "\SystemConfig.ini")
        Catch ex As Exception
            MsgBox("Fatal Error! - Ref#000001 (no setting file found)")
        End Try
        Dim Values As String = file.Sections(Section)?.Keys(Key)?.Value
        If Values IsNot Nothing Then
            Return Values
        Else
            Return "*\"
            MsgBox("Fatal Error! - Ref#000002 (no data found in key)")
        End If
    End Function
    Public Function INI_GetKey_Alarm(ByVal Section, ByVal Key) As String
        'Creat new INI Fileandalso populate it with the saved ini file in the resorces folder if it exists
        Dim file As New IniFile()
        Try
            file.Load(My.Settings.INI_Loc & "\AlarmConfig.ini")
        Catch ex As Exception
            MsgBox("Fatal Error! - Ref#000001 (no setting file found)")
        End Try
        Dim Values As String = file.Sections(Section)?.Keys(Key)?.Value
        If Values IsNot Nothing Then
            Return Values
        Else
            Return "*\"
            MsgBox("Fatal Error! - Ref#000002 (no data found in key)")
        End If
    End Function
    Public Function INI_GetKey_IO(ByVal Section, ByVal Key) As String
        'Creat new INI Fileandalso populate it with the saved ini file in the resorces folder if it exists
        Dim file As New IniFile()
        Try
            file.Load(My.Settings.INI_Loc & "\IOConfig.ini")
        Catch ex As Exception
            MsgBox("Fatal Error! - Ref#000001 (no setting file found)")
        End Try
        Dim Values As String = file.Sections(Section)?.Keys(Key)?.Value
        If Values IsNot Nothing Then
            Return Values
        Else
            Return "*\"
            MsgBox("Fatal Error! - Ref#000002 (no data found in key)")
        End If
    End Function


    Function ChangeIODescription(NewDesc As String, Index As Integer, Card As Integer, IOSlotDescCollection As Collections.Specialized.StringCollection)
        Status_Page.Controls("Slot" & Card.ToString & "Panel").Controls("IO" & Card.ToString & "_" & Index.ToString & "Desc").Text = NewDesc
        IOSlotDescCollection(Index) = NewDesc
        My.Settings("Card" & Card.ToString & "IODesc")(Index) = NewDesc
        Return IOSlotDescCollection

    End Function
    Sub UpdateIODescriptionLabels()
        For Each IODescControl As Control In Slot2Panel.Controls
            If IODescControl.Name.StartsWith("IO") Then
                Dim Point As String = "P" & GetNumeric(IODescControl.Name.Split("_")(1))
                IODescControl.Text = INI_GetKey_IO("CARD2", Point)
            End If
        Next
        For Each IODescControl As Control In Slot3Panel.Controls
            If IODescControl.Name.StartsWith("IO") Then
                Dim Point As String = "P" & GetNumeric(IODescControl.Name.Split("_")(1))
                IODescControl.Text = INI_GetKey_IO("CARD3", Point)
            End If
        Next
        For Each IODescControl As Control In Slot4Panel.Controls
            If IODescControl.Name.StartsWith("IO") Then
                Dim Point As String = "P" & GetNumeric(IODescControl.Name.Split("_")(1))
                IODescControl.Text = INI_GetKey_IO("CARD4", Point)
            End If
        Next
        For Each IODescControl As Control In Slot5Panel.Controls
            If IODescControl.Name.StartsWith("IO") Then
                Dim Point As String = "P" & GetNumeric(IODescControl.Name.Split("_")(1))
                IODescControl.Text = INI_GetKey_IO("CARD5", Point)
            End If
        Next
        For Each IODescControl As Control In Slot6Panel.Controls
            If IODescControl.Name.StartsWith("IO") Then
                Dim Point As String = "P" & GetNumeric(IODescControl.Name.Split("_")(1))
                IODescControl.Text = INI_GetKey_IO("CARD6", Point)
            End If
        Next
        For Each IODescControl As Control In Slot7Panel.Controls
            If IODescControl.Name.StartsWith("IO") Then
                Dim Point As String = "P" & GetNumeric(IODescControl.Name.Split("_")(1))
                IODescControl.Text = INI_GetKey_IO("CARD7", Point)
            End If
        Next
        For Each IODescControl As Control In Slot8Panel.Controls
            If IODescControl.Name.StartsWith("IO") Then
                Dim Point As String = "P" & GetNumeric(IODescControl.Name.Split("_")(1))
                IODescControl.Text = INI_GetKey_IO("CARD8", Point)
            End If
        Next
    End Sub


    Function ReadLineWithNumberFrom(filePath As String, ByVal lineNumber As Integer) As String
        Using file As New System.IO.StreamReader(filePath)
            ' Skip all preceding lines: '
            For i As Integer = 1 To lineNumber - 1
                If file.ReadLine() Is Nothing Then
                    Throw New ArgumentOutOfRangeException("lineNumber")
                End If
            Next
            ' Attempt to read the line you're interested in: '
            Dim line As String = file.ReadLine()
            If line Is Nothing Then
                Throw New ArgumentOutOfRangeException("lineNumber")
            End If
            ' Succeded!
            Return line
        End Using
    End Function
    Public Function GetNumeric(value As String) As String
        Dim output As New StringBuilder
        For i = 0 To value.Length - 1
            If IsNumeric(value(i)) Then
                output.Append(value(i))
            End If
        Next
        Return output.ToString()
    End Function
    Function GetCurrentState(Actual As Integer, ByVal HiLimit As Integer, ByVal LowLimit As Integer) As Boolean()
        Dim InLimit As Boolean = False
        Dim OverLimit As Boolean = False
        Dim UnderLimit As Boolean = False
        If Actual >= HiLimit Then OverLimit = True
        If Actual <= LowLimit Then UnderLimit = True
        If Actual < HiLimit Then
            If Actual > LowLimit Then InLimit = True
        End If
        Dim Result(3) As Boolean
        Result(0) = InLimit
        Result(1) = OverLimit
        Result(2) = UnderLimit
        Return Result
    End Function
    Function ShowHWUsePerHour() As String
        Dim Message As String = "Last 24 Hours Steam Usage" & vbNewLine
        For i = 0 To 23
            Message = Message & (Now.AddHours((i + 1) * (-1))).Hour.ToString("00") & ":00" & " to " & (Now.AddHours(i * (-1))).Hour.ToString("00") & ":00 " & "(" & PLC_Fast.Read("F9:" & i) & " L)" & vbNewLine
        Next
        Return Message
    End Function
    Function ShowSteamUsePerHour() As String
        Dim Message As String = "Last 24 Hours Hot Water Usage" & vbNewLine
        For i = 0 To 23
            Message = Message & (Now.AddHours((i + 1) * (-1))).Hour.ToString("00") & ":00" & " to " & (Now.AddHours(i * (-1))).Hour.ToString("00") & ":00 " & "(" & PLC_Fast.Read("F10:" & i) & " LBS)" & vbNewLine
        Next
        Return Message
    End Function
    Function Calculate24HrSteamUse() As Boolean
        Dim Total24HRSteamUsage As Integer = 0
        Try
            For i = 0 To 23
                Total24HRSteamUsage += PLC_Fast.Read("F10:" & i)
            Next
            Total24HrSteamUse.Value = Total24HRSteamUsage
            SteamUseAVG.Value = Math.Floor(Total24HRSteamUsage / 24)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Sub GetAllAud_Falc_AlarmStatus()

        ProgressBar1.Value = processCount
        For i = 0 To 31
            'get audible mask values
            Dim CurElecAudStatus As Boolean = PLC_Fast.Read("N14:6/" & i)
            Dim CurAirAudStatus As Boolean = PLC_Fast.Read("N14:16/" & i)
            Dim CurSteamAudStatus As Boolean = PLC_Fast.Read("N14:26/" & i)
            Dim CurWaterAudStatus As Boolean = PLC_Fast.Read("N14:36/" & i)
            'Get falcon mask values
            Dim CurElecFalcStatus As Boolean = PLC_Fast.Read("N14:8/" & i)
            Dim CurAirFalcStatus As Boolean = PLC_Fast.Read("N14:18/" & i)
            Dim CurSteamFalcStatus As Boolean = PLC_Fast.Read("N14:28/" & i)
            Dim CurWaterFalcStatus As Boolean = PLC_Fast.Read("N14:38/" & i)



            If CurElecAudStatus Then
                ElectricalAlarmPanel.Controls("ElecAudAlarm" & i).BackgroundImage = My.Resources.AlarmAudibleOn
            Else
                ElectricalAlarmPanel.Controls("ElecAudAlarm" & i).BackgroundImage = My.Resources.AlarmAudibleOff
            End If
            processCount += 1
            StartupStatusLabel.Text = "Getting Alarm Details..." & processCount & "/384"
            ProgressBar1.Value = processCount
            If CurAirAudStatus Then
                AirAlarmPanel.Controls("AirAudAlarm" & i).BackgroundImage = My.Resources.AlarmAudibleOn
            Else
                AirAlarmPanel.Controls("AirAudAlarm" & i).BackgroundImage = My.Resources.AlarmAudibleOff
            End If
            processCount += 1
            StartupStatusLabel.Text = "Getting Alarm Details..." & processCount & "/384"
            ProgressBar1.Value = processCount
            If CurSteamAudStatus Then
                SteamAlarmPanel.Controls("steamAudAlarm" & i).BackgroundImage = My.Resources.AlarmAudibleOn
            Else
                SteamAlarmPanel.Controls("SteamAudAlarm" & i).BackgroundImage = My.Resources.AlarmAudibleOff
            End If
            processCount += 1
            StartupStatusLabel.Text = "Getting Alarm Details..." & processCount & "/384"
            ProgressBar1.Value = processCount
            If CurWaterAudStatus Then
                WaterAlarmPanel.Controls("WaterAudAlarm" & i).BackgroundImage = My.Resources.AlarmAudibleOn
            Else
                WaterAlarmPanel.Controls("WaterAudAlarm" & i).BackgroundImage = My.Resources.AlarmAudibleOff
            End If
            processCount += 1
            StartupStatusLabel.Text = "Getting Alarm Details..." & processCount & "/384"
            ProgressBar1.Value = processCount
            If CurElecFalcStatus Then
                ElectricalAlarmPanel.Controls("ElecFalcAlarm" & i).BackgroundImage = My.Resources.AlarmFalconOn
            Else
                ElectricalAlarmPanel.Controls("ElecFalcAlarm" & i).BackgroundImage = My.Resources.AlarmFalconOff
            End If
            processCount += 1
            StartupStatusLabel.Text = "Getting Alarm Details..." & processCount & "/384"
            ProgressBar1.Value = processCount
            If CurAirFalcStatus Then
                AirAlarmPanel.Controls("AirFalcAlarm" & i).BackgroundImage = My.Resources.AlarmFalconOn
            Else
                AirAlarmPanel.Controls("AirFalcAlarm" & i).BackgroundImage = My.Resources.AlarmFalconOff
            End If
            processCount += 1
            StartupStatusLabel.Text = "Getting Alarm Details..." & processCount & "/384"
            ProgressBar1.Value = processCount
            If CurSteamFalcStatus Then
                SteamAlarmPanel.Controls("steamFalcAlarm" & i).BackgroundImage = My.Resources.AlarmFalconOn
            Else
                SteamAlarmPanel.Controls("SteamFalcAlarm" & i).BackgroundImage = My.Resources.AlarmFalconOff
            End If
            processCount += 1
            StartupStatusLabel.Text = "Getting Alarm Details..." & processCount & "/384"
            ProgressBar1.Value = processCount
            If CurWaterFalcStatus Then
                WaterAlarmPanel.Controls("WaterFalcAlarm" & i).BackgroundImage = My.Resources.AlarmFalconOn
            Else
                WaterAlarmPanel.Controls("WaterFalcAlarm" & i).BackgroundImage = My.Resources.AlarmFalconOff
            End If
            processCount += 1
            StartupStatusLabel.Text = "Getting Alarm Details..." & processCount & "/384"
            ProgressBar1.Value = processCount

        Next
    End Sub
    Private Sub CheckBypass()
        For Each ctrl As System.Object In ElectricalAlarmPanel.Controls
            If ctrl.Name.Contains("Light") Then
                Dim idx As Integer = GetNumeric(ctrl.Name)
                Dim state As Boolean = PLC_Fast.Read("N15:0/" & idx)
                If state Then ctrl.selectcolor3 = False Else ctrl.selectcolor3 = True
                processCount += 1
                StartupStatusLabel.Text = "Getting Alarm Details..." & processCount & "/384"
                ProgressBar1.Value = processCount
            End If
        Next
        For Each ctrl As System.Object In AirAlarmPanel.Controls
            If ctrl.Name.Contains("Light") Then
                Dim idx As Integer = GetNumeric(ctrl.Name)
                Dim state As Boolean = PLC_Fast.Read("N15:2/" & idx)
                If state Then ctrl.selectcolor3 = False Else ctrl.selectcolor3 = True
                processCount += 1
                StartupStatusLabel.Text = "Getting Alarm Details..." & processCount & "/384"
                ProgressBar1.Value = processCount
            End If
        Next
        For Each ctrl As System.Object In SteamAlarmPanel.Controls
            If ctrl.Name.Contains("Light") Then
                Dim idx As Integer = GetNumeric(ctrl.Name)
                Dim state As Boolean = PLC_Fast.Read("N15:4/" & idx)
                If state Then ctrl.selectcolor3 = False Else ctrl.selectcolor3 = True
                processCount += 1
                StartupStatusLabel.Text = "Getting Alarm Details..." & processCount & "/384"
                ProgressBar1.Value = processCount
            End If
        Next
        For Each ctrl As System.Object In WaterAlarmPanel.Controls
            If ctrl.Name.Contains("Light") Then
                Dim idx As Integer = GetNumeric(ctrl.Name)
                Dim state As Boolean = PLC_Fast.Read("N15:6/" & idx)
                If state Then ctrl.selectcolor3 = False Else ctrl.selectcolor3 = True
                processCount += 1
                StartupStatusLabel.Text = "Getting Alarm Details..." & processCount & "/384"
                ProgressBar1.Value = processCount
            End If
        Next
        processCount = 0
        ProgressBar1.Value = processCount
        ProgressBar1.Maximum = 6
    End Sub
    Sub SetPLC_DateTime()
        PLC.Write("N7:50", Now.Hour.ToString)
        PLC.Write("N7:51", Now.Minute.ToString)
        PLC.Write("N7:52", Now.Second.ToString)
        PLC.Write("N7:53", Now.Day.ToString)
        PLC.Write("N7:54", Now.Month.ToString)
        PLC.Write("N7:55", Now.Year.ToString)
        PLC.Write("B16:1/12", "1")
        PLC.Write("B16:1/11", "1")
    End Sub

    Public Sub SetPropertyValue(obj As Object, propName As String, value As Object)
        Dim prop = obj.GetType().GetProperty(propName)
        If prop IsNot Nothing AndAlso prop.CanWrite Then
            prop.SetValue(obj, value, Nothing)
        Else
        End If
    End Sub

    Public Function IntToBin(int As Int16) As Boolean()
        Dim bits(16) As Boolean
        Dim binaryString As String = StrReverse(Convert.ToString(int, 2).PadLeft(16, "0"c))
        Dim idx As Integer = 0
        For Each ch As Char In binaryString
            If ch = "1" Then bits(idx) = True Else bits(idx) = False
            idx += 1
        Next
        Return bits
    End Function

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click
        Dim result As DialogResult = MessageBox.Show("Change SQL Server Settings?", "SQL Server", MessageBoxButtons.YesNo)

        If result = DialogResult.Yes Then

            Dim SER, USR, PAS, DB As String
            Try
                SER = InputBox("Enter New Server Address", "SLQ Settings", INI_GetKey_System("SQL", "SERVER"))
                USR = InputBox("Enter New User ID", "SLQ Settings", INI_GetKey_System("SQL", "USER"))
                PAS = InputBox("Enter New Password", "SLQ Settings", INI_GetKey_System("SQL", "PASS"))
                DB = InputBox("Enter New Server Database", "SLQ Settings", INI_GetKey_System("SQL", "DB"))
                SQLLBL_Server.Text = SER
                SQLLBL_User.Text = USR
                SQLLBL_Pass.Text = PAS
                SQLLBL_Database.Text = DB
                SQL_ConString = "server=" & SER & ";user id=" & USR & ";password=" & PAS & ";database=" & DB
                INI_SetKey_System("SQL", "SERVER", SER)
                INI_SetKey_System("SQL", "USER", USR)
                INI_SetKey_System("SQL", "PASS", PAS)
                INI_SetKey_System("SQL", "DB", DB)
                If RunSQL = False Then
                    Dim SQLres As MsgBoxResult = MsgBox("Do you want to enable SQL Logging?", MsgBoxStyle.YesNo, "Enable SQL")
                    If SQLres = MsgBoxResult.Yes Then
                        RunSQL = True
                        Set2_RunLogLabel.Text = "TRUE"
                    End If
                End If
            Catch ex As Exception
                MsgBox(ex.Message)
            End Try
        End If

    End Sub

    Private Sub Chng2_RunLogLabel_Click(sender As Object, e As EventArgs) Handles Chng2_RunLogLabel.Click
        If RunSQL Then
            Dim result As DialogResult = MessageBox.Show("By disabling the countinous log, no data will be sent to the SQL Server." & vbNewLine & "Would you like to continue disabling the countinuous log?", "SQL Log", MessageBoxButtons.YesNo)
            If result = DialogResult.Yes Then
                RunSQL = False
                INI_SetKey_System("SQL", "SQL_EN", "FALSE")
                Set2_RunLogLabel.Text = "FALSE"
            End If
        Else
            Dim result As DialogResult = MessageBox.Show("By enableing the countinous log, data will be sent to the SQL Server every 1 second." & vbNewLine & "Would you like to continue disabling the countinuous log?", "SQL Log", MessageBoxButtons.YesNo)
            If result = DialogResult.Yes Then
                RunSQL = True
                INI_SetKey_System("SQL", "SQL_EN", "TRUE")
                Set2_RunLogLabel.Text = "TRUE"
            End If
        End If
    End Sub

    Private Sub Chng3_SysTimeoutLabel_Click(sender As Object, e As EventArgs) Handles Chng3_SysTimeoutLabel.Click
        Dim result As String = InputBox("Enter new System Inactivity Timeout:", "System Inactivity", INI_GetKey_System("APP", "SCREEN_TO"))
        If IsNumeric(result) Then
            INI_SetKey_System("APP", "SCREEN_TO", result.ToString)
            Set3_SysTimeoutLabel.Text = result
        End If
    End Sub

    Private Sub Chng4_UserTimeoutLabel_Click(sender As Object, e As EventArgs) Handles Chng4_UserTimeoutLabel.Click
        Dim result As String = InputBox("Enter new User Inactivity Timeout:", "User Inactivity", INI_GetKey_System("APP", "USER_TO"))
        If IsNumeric(result) Then
            INI_SetKey_System("APP", "USER_TO", result)
            Set4_UserTimeoutLabel.Text = result
        End If
    End Sub

    Private Sub Label66_Click(sender As Object, e As EventArgs) Handles Label66.Click
        Dim result As MsgBoxResult = MsgBox("Do you want to edit the file location for the system configuration files?", MsgBoxStyle.YesNoCancel, "System Configuration")
        If result = MsgBoxResult.Yes Then
            Dim NewLoc As String = ""
            Dim NewFBD As New FolderBrowserDialog
            NewFBD.SelectedPath = My.Settings.INI_Loc
            NewFBD.ShowDialog()
            If NewFBD.SelectedPath IsNot My.Settings.INI_Loc Then
                If NewFBD.SelectedPath IsNot "" Then
                    My.Settings.INI_Loc = NewFBD.SelectedPath
                    MsgBox(NewFBD.SelectedPath)
                    Label64.Text = NewFBD.SelectedPath
                End If
            End If
        End If
    End Sub
    '║                                                                                                              ║
    '╠══════════════════════════════════════════════════════════════════════════════════════════════════════════════╣
    '║                                                 [FUNCTIONS]                                                  ║
    '║                                                 Section End                                                  ║
    '╚══════════════════════════════════════════════════════════════════════════════════════════════════════════════╝
#End Region
#Region "Timers"
    '╔══════════════════════════════════════════════════════════════════════════════════════════════════════════════╗
    '║                                                   [TIMERS]                                                   ║
    '║                                                Section Start                                                 ║
    '╠══════════════════════════════════════════════════════════════════════════════════════════════════════════════╣
    '║                                                                                                              ║
    'This timer adds the data points to the continuous log chartandalso saves all of the analog values to the log file location


    Private Sub Log_Chart_Timer(sender As Object, e As EventArgs) Handles LogChartTimer.Tick
        If Firsttick = False Then
            Firsttick = True
        End If

        If RunLog Then

            'SERIES VISIBILITY LOGIC
            For SerVis As Integer = 0 To CheckedListBox1.Items().Count - 1
                If CheckedListBox1.GetItemChecked(SerVis) Then
                    Chart3.Series(CheckedListBox1.Items(SerVis)).BorderWidth = 1
                Else
                    Chart3.Series(CheckedListBox1.Items(SerVis)).BorderWidth = 0
                End If
            Next
            For SerVis As Integer = 0 To CheckedListBox2.Items().Count - 1
                If CheckedListBox2.GetItemChecked(SerVis) Then
                    Chart3.Series(CheckedListBox2.Items(SerVis)).BorderWidth = 1
                Else
                    Chart3.Series(CheckedListBox2.Items(SerVis)).BorderWidth = 0
                End If
            Next
            For SerVis As Integer = 0 To CheckedListBox3.Items().Count - 1
                If CheckedListBox3.GetItemChecked(SerVis) Then
                    Chart3.Series(CheckedListBox3.Items(SerVis)).BorderWidth = 1
                Else
                    Chart3.Series(CheckedListBox3.Items(SerVis)).BorderWidth = 0
                End If
            Next
            For SerVis As Integer = 0 To CheckedListBox4.Items().Count - 1
                If CheckedListBox4.GetItemChecked(SerVis) Then
                    Chart3.Series(CheckedListBox4.Items(SerVis)).BorderWidth = 1
                Else
                    Chart3.Series(CheckedListBox4.Items(SerVis)).BorderWidth = 0
                End If
            Next

            ' ADD NEW DATA POINTS TO THE CHARTS

            Chart3.Series(0).Points.AddXY(Now.ToString("HH:mm:ss"), ELNA_Actual.Value)
            Chart3.Series(1).Points.AddXY(Now.ToString("HH:mm:ss"), ELNB_Actual.Value)
            Chart3.Series(2).Points.AddXY(Now.ToString("HH:mm:ss"), ELNC_Actual.Value)
            Chart3.Series(3).Points.AddXY(Now.ToString("HH:mm:ss"), ELSA_Actual.Value)
            Chart3.Series(4).Points.AddXY(Now.ToString("HH:mm:ss"), ELSB_Actual.Value)
            Chart3.Series(5).Points.AddXY(Now.ToString("HH:mm:ss"), ELSC_Actual.Value)
            Chart3.Series(6).Points.AddXY(Now.ToString("HH:mm:ss"), STMPDEM_Actual.Value)
            Chart3.Series(7).Points.AddXY(Now.ToString("HH:mm:ss"), STLPDEM_Actual.Value)
            Chart3.Series(8).Points.AddXY(Now.ToString("HH:mm:ss"), HWFL_Actual.Value)
            Chart3.Series(9).Points.AddXY(Now.ToString("HH:mm:ss"), ALP_Actual.Value)
            Chart3.Series(10).Points.AddXY(Now.ToString("HH:mm:ss"), CWPOP_Actual.Value)
            Chart3.Series(11).Points.AddXY(Now.ToString("HH:mm:ss"), HWPOP_Actual.Value)
            Chart3.Series(12).Points.AddXY(Now.ToString("HH:mm:ss"), STLP_Actual.Value)
            Chart3.Series(13).Points.AddXY(Now.ToString("HH:mm:ss"), STMP_Actual.Value)
            Chart3.Series(14).Points.AddXY(Now.ToString("HH:mm:ss"), STHP_Actual.Value)
            Chart3.Series(15).Points.AddXY(Now.ToString("HH:mm:ss"), HWTP_Actual.Value)
            Chart3.Series(16).Points.AddXY(Now.ToString("HH:mm:ss"), (STFL_Actual.Value / 1000).ToString("0"))
            Chart3.Series(17).Points.AddXY(Now.ToString("HH:mm:ss"), CWSUP_Actual.Value)

            'DETERMINE IF AUTOSCALLING IS NESSASARYandalso ALL SCALES ACCORDINLY.
            Dim ElecMinVal, ElecMaxVal,
                AirMinVal, AirMaxVal,
                SteamMinVal, SteamMaxVal,
                WaterMinVal, WaterMaxVal As Double

            For Each i As Integer In {0, 1, 2, 3, 4, 5}
                If i = 0 Then
                    ElecMinVal = Chart3.Series(i).Points.FindMinByValue().YValues(0)
                    ElecMaxVal = Chart3.Series(i).Points.FindMaxByValue().YValues(0)
                Else
                    If ElecMinVal > Chart3.Series(i).Points.FindMinByValue().YValues(0) Then
                        ElecMinVal = Chart3.Series(i).Points.FindMinByValue().YValues(0)
                    End If
                    If ElecMaxVal < Chart3.Series(i).Points.FindMaxByValue().YValues(0) Then
                        ElecMaxVal = Chart3.Series(i).Points.FindMaxByValue().YValues(0)
                    End If
                End If
            Next
            If ElecAutoYPosCB.Checked Then
                With Chart3.ChartAreas(0)
                    .AxisY.ScaleView.Position = Math.Floor(ElecMinVal) - 10
                    .AxisY.ScaleView.Size = Math.Ceiling((ElecMinVal - ElecMaxVal) * -1) + 20
                End With
            Else
                Chart3.ChartAreas(0).AxisY.ScaleView.Size = NumericUpDown4.Value
            End If
            Chart3.ChartAreas(0).AxisX.ScaleView.Size = ElecChartXSize.Value
            If ElecAutoXPosCB.Checked Then
                If Chart3.Series(0).Points.Count > Chart3.ChartAreas(0).AxisX.ScaleView.Size Then
                    Chart3.ChartAreas(0).AxisX.ScaleView.Position = Chart3.Series(0).Points.Count - Chart3.ChartAreas(0).AxisX.ScaleView.Size
                End If
            End If



            AirMinVal = Chart3.Series(9).Points.FindMinByValue().YValues(0)
            AirMaxVal = Chart3.Series(9).Points.FindMaxByValue().YValues(0)

            If AirAutoYPosCB.Checked Then
                With Chart3.ChartAreas(1)
                    .AxisY.ScaleView.Position = Math.Floor(AirMinVal) - 10
                    .AxisY.ScaleView.Size = Math.Ceiling((AirMinVal - AirMaxVal) * -1) + 20
                End With
            Else
                Chart3.ChartAreas(1).AxisY.ScaleView.Size = NumericUpDown21.Value
            End If
            Chart3.ChartAreas(1).AxisX.ScaleView.Size = NumericUpDown24.Value
            If AirAutoXPosCB.Checked Then
                If Chart3.Series(9).Points.Count > Chart3.ChartAreas(1).AxisX.ScaleView.Size Then
                    Chart3.ChartAreas(1).AxisX.ScaleView.Position = Chart3.Series(9).Points.Count - Chart3.ChartAreas(1).AxisX.ScaleView.Size
                End If

            End If


            For Each i As Integer In {6, 7, 12, 13, 14, 16}
                If i = 6 Then
                    SteamMinVal = Chart3.Series(i).Points.FindMinByValue().YValues(0)
                    SteamMaxVal = Chart3.Series(i).Points.FindMaxByValue().YValues(0)
                Else
                    If SteamMinVal > Chart3.Series(i).Points.FindMinByValue().YValues(0) Then
                        SteamMinVal = Chart3.Series(i).Points.FindMinByValue().YValues(0)
                    End If
                    If SteamMaxVal < Chart3.Series(i).Points.FindMaxByValue().YValues(0) Then
                        SteamMaxVal = Chart3.Series(i).Points.FindMaxByValue().YValues(0)
                    End If
                End If
            Next
            If SteamAutoYPosCB.Checked Then
                With Chart3.ChartAreas(2)
                    .AxisY.ScaleView.Position = Math.Floor(SteamMinVal) - 10
                    .AxisY.ScaleView.Size = Math.Ceiling((SteamMinVal - SteamMaxVal) * -1) + 20
                End With
            Else
                Chart3.ChartAreas(2).AxisY.ScaleView.Size = NumericUpDown17.Value
            End If
            Chart3.ChartAreas(2).AxisX.ScaleView.Size = NumericUpDown20.Value
            If SteamAutoXPosCB.Checked Then
                If Chart3.Series(6).Points.Count > Chart3.ChartAreas(2).AxisX.ScaleView.Size Then
                    Chart3.ChartAreas(2).AxisX.ScaleView.Position = Chart3.Series(6).Points.Count - Chart3.ChartAreas(2).AxisX.ScaleView.Size
                End If

            End If


            For Each i As Integer In {8, 10, 11, 15, 17}
                If i = 8 Then
                    WaterMinVal = Chart3.Series(i).Points.FindMinByValue().YValues(0)
                    WaterMaxVal = Chart3.Series(i).Points.FindMaxByValue().YValues(0)
                Else
                    If WaterMinVal > Chart3.Series(i).Points.FindMinByValue().YValues(0) Then
                        WaterMinVal = Chart3.Series(i).Points.FindMinByValue().YValues(0)
                    End If
                    If WaterMaxVal < Chart3.Series(i).Points.FindMaxByValue().YValues(0) Then
                        WaterMaxVal = Chart3.Series(i).Points.FindMaxByValue().YValues(0)
                    End If
                End If
            Next

            If WaterAutoYPosCB.Checked Then
                With Chart3.ChartAreas(3)
                    .AxisY.ScaleView.Position = Math.Floor(WaterMinVal) - 10
                    .AxisY.ScaleView.Size = Math.Ceiling((WaterMinVal - WaterMaxVal) * -1) + 20
                End With
            Else
                Chart3.ChartAreas(3).AxisY.ScaleView.Size = NumericUpDown13.Value
            End If
            Chart3.ChartAreas(3).AxisX.ScaleView.Size = NumericUpDown16.Value
            If WaterAutoXPosCB.Checked Then
                If Chart3.Series(8).Points.Count > Chart3.ChartAreas(3).AxisX.ScaleView.Size Then
                    Chart3.ChartAreas(3).AxisX.ScaleView.Position = Chart3.Series(8).Points.Count - Chart3.ChartAreas(3).AxisX.ScaleView.Size
                End If

            End If

            'If any chart has more that 30 minutes of data points delete oldest data points

            For i = 0 To Chart3.Series().Count - 1
                If Chart3.Series(i).Points.Count > 3600 Then
                    Chart3.Series(i).Points.RemoveAt(Chart3.Series(i).Points.Count - 3601)
                End If
            Next

            'Set Chart Label Interval based on scale
            With Chart3
                For Q = 0 To Chart3.ChartAreas().Count - 1
                    .ChartAreas(Q).AxisY.LabelStyle.Interval = Math.Ceiling(.ChartAreas(Q).AxisY.ScaleView.Size / 11)
                    .ChartAreas(Q).AxisX.LabelStyle.Interval = Math.Ceiling(.ChartAreas(Q).AxisX.ScaleView.Size / 20)
                Next
            End With
        End If

        If Firsttick Then

            'SEND SQL DATA TO SERVER IF RunSQL IS TRUE
            If RunSQL Then
                Dim Date_Time As String = Now.Year & Now.Month.ToString("00") & Now.Day.ToString("00") & Now.Hour.ToString("00") & Now.Minute.ToString("00") & Now.Second.ToString("00")
                SaveSensorData(Date_Time, CWSUP_Actual.Value, CWPRP_Actual.Value, CWPOP_Actual.Value,
                               CWFD_Actual.Value, HWPRP_Actual.Value, HWPOP_Actual.Value,
                               HWFD_Actual.Value, HWTP_Actual.Value, HWFL_Actual.Value, STFWP_Actual.Value,
                               STHP_Actual.Value, STLP_Actual.Value,
                               STMP_Actual.Value, STLPDEM_Actual.Value,
                               STMPDEM_Actual.Value, STFL_Actual.Value, ELNA_Actual.Value,
                               ELNB_Actual.Value, ELNC_Actual.Value, ELSA_Actual.Value,
                               ELSB_Actual.Value, ELSC_Actual.Value, ALP_Actual.Value)
            End If
        Else
            For i = 0 To Chart3.ChartAreas().Count
                Chart3.ChartAreas(i).Visible = False
            Next
        End If

        If Firsttick Then
            Enviro_Chart.Series(1).LegendText = "Inside Humidity (" & (EVIHM_Actual.Value * 1).ToString("0.0") & " °" & EVIHM_Unit.Text & ")"
            Enviro_Chart.Series(0).LegendText = "Inside Temp (" & (EVITP_Actual.Value * 1).ToString("0.0") & " °" & EVITP_Unit.Text & ")"
            Enviro_Chart.Series(2).LegendText = "Outside Temp (" & (EVOTP_Actual.Value * 1).ToString("0.0") & " °" & EVOTP_Unit.Text & ")"
            Enviro_Chart.Series(3).LegendText = "Outside Humidity (" & (EVOHM_Actual.Value * 1).ToString("0.0") & " °" & EVOHM_Unit.Text & ")"
        End If



    End Sub
    'This timer becomes active when a user is logged inandalso will automatically log the user out after the set time gets to 0
    Private Sub ActivityTimer_Tick(sender As Object, e As EventArgs) Handles LoginTimer.Tick
        LoginTimeRemaining -= 1
        Label23.Text = "CurrentUser:" & vbNewLine & CurrentUser & " | " & LoginTimeRemaining
        If LoginTimeRemaining = 0 Then
            LoginTimer.Enabled = False
            Label23.Text = "NO USER"

            If RunSQL Then SaveSystemData(GetNow(), "User Automatically Logged Out (" & CurrentUser & ")", "System")
            CurrentUserLevel = 0
            CurrentUser = "System"
            USER_LOGOUT()
        End If
    End Sub
    'This Timer Sets the Clock displayandalsos sets the Global Time for Application Use
    Private Sub ClockTimer_Tick(sender As Object, e As EventArgs) Handles ClockTimer.Tick
        Dim NewTime As String
        If Now.Hour = 0 Then
            NewTime = "12:" & Now.Minute.ToString("00") & ":" & Now.Second.ToString("00") & " AM"
        ElseIf Now.Hour > 12 Then
            NewTime = Now.AddHours(-12).Hour.ToString & ":" & Now.Minute.ToString("00") & ":" & Now.Second.ToString("00") & " PM"
        Else
            NewTime = Now.Hour.ToString & ":" & Now.Minute.ToString("00") & ":" & Now.Second.ToString("00") & " PM"
        End If
        ClockDisplay.Text = NewTime
        CurrentTime = NewTime
        DateDisplay.Text = Now.Date.ToLongDateString
        CurrentDate = Now.Month.ToString("00") & "-" & Now.Day.ToString("00") & "-" & Now.Year

        If Now.Hour = temphour Then Else temphour = Now.Hour

        If Now.Hour = 1 AndAlso Now.Minute = 15 AndAlso Now.Second > 0 AndAlso Now.Second < 5 Then
            If MinMaxLogSaved = False Then
                'SaveMINMAXReport()
                MinMaxLogSaved = True
            End If
        Else
            If MinMaxLogSaved Then MinMaxLogSaved = False
        End If

        If TempMinute = Now.Minute Then
        Else
            If Firsttick Then
                Enviro_Chart.Series(0).Points.AddXY(Now.ToString("HH:mm:ss"), EVITP_Actual.Value)
                Enviro_Chart.Series(1).Points.AddXY(Now.ToString("HH:mm:ss"), EVIHM_Actual.Value)
                Enviro_Chart.Series(2).Points.AddXY(Now.ToString("HH:mm:ss"), EVOTP_Actual.Value)
                Enviro_Chart.Series(3).Points.AddXY(Now.ToString("HH:mm:ss"), EVOHM_Actual.Value)

                For i = 0 To Enviro_Chart.Series().Count - 1
                    If Enviro_Chart.Series(i).Points.Count > 360 Then
                        Enviro_Chart.Series(i).Points.RemoveAt(0)
                    End If
                Next

            End If
            TempMinute = Now.Minute
        End If
        If Firsttick Then AlarmStateCheck()

        SystemDateTimeDisp.Text = Now.Month.ToString("00") &
            " / " & Now.Day.ToString("00") & " / " & Now.Year.ToString("00") &
            "   " & Now.Hour.ToString("00") & " : " & Now.Minute.ToString("00") &
            " : " & Now.Second.ToString("00")
        Dim PLCtime As New DateTime(BasicLabel2.Value, PLCMonth.Value, BasicLabel1.Value, BasicLabel4.Value, BasicLabel7.Value, BasicLabel8.Value)
        Dim TimeDifference As TimeSpan
        TimeDifference = Now.Subtract(PLCtime)
        SystemTimeDifferenceDisp.Text = TimeDifference.ToString("g").Split(".")(0)
        Label337.Text = PLCtime.ToString("HH:mm:ss")
        If TimeDifference.Seconds > 30 Then
            SetPLC_DateTime()
            SystemTimeDifferenceDisp.ForeColor = Color.Red
        ElseIf TimeDifference.Seconds < 5 Then
            PLCTimeSyncError = False
            SystemTimeDifferenceDisp.ForeColor = Color.White
        End If

    End Sub
    ' This timer verifys PLC Connection 
    Private Sub PLC_HeartBeat_Timer(sender As Object, e As EventArgs) Handles HeartBeatTimer.Tick

        If FirstTickDone Then

            If LogChartTimer.Enabled = False Then
                LogChartTimer.Enabled = True
            End If

            Dim NewHBTime As New DateTime
            NewHBTime = Now
            Dim TimeSinceLastHB As New TimeSpan
            TimeSinceLastHB = NewHBTime.Subtract(lastHBTime)
            Label42.Text = TimeSinceLastHB.Seconds & "." & TimeSinceLastHB.Milliseconds & " sec"
            BarLevel2.Value = TimeSinceLastHB.Milliseconds + (TimeSinceLastHB.Seconds * 1000)

            Dim NewHBTime_5s As New DateTime
            NewHBTime_5s = Now
            Dim TimeSinceLastHB_5s As New TimeSpan
            TimeSinceLastHB_5s = NewHBTime_5s.Subtract(lastHBTime_5s)
            Label67.Text = TimeSinceLastHB_5s.Seconds & "." & TimeSinceLastHB_5s.Milliseconds & " sec"
            BarLevel1.Value = TimeSinceLastHB_5s.Milliseconds + (TimeSinceLastHB_5s.Seconds * 1000)

            If BarLevel2.Value > 5000 Then
                If HeartBeatError = False Then HeartBeatError = True
            ElseIf BarLevel2.Value < 5000 AndAlso BarLevel2.Value > 3999 Then
                BarLevel2.BarContentColor = Color.Red
                HeartBeatError = False
            ElseIf BarLevel2.Value > 1999 AndAlso BarLevel2.Value < 4000 Then
                BarLevel2.BarContentColor = Color.Yellow
                HeartBeatError = False
            ElseIf BarLevel2.Value < 2000 Then
                BarLevel2.BarContentColor = Color.DodgerBlue
                HeartBeatError = False
            End If

            If HeartBeatError Then
                Label53.Text = "PLC Connection Timed Out"
                Label53.ForeColor = Color.Red
                PLCConIndicator.Image = My.Resources.StatusPLCBad
                If PLCConInAlarm = False Then
                    Dim CurrentDT As DateTime = Now
                    If RunSQL Then SaveAlarmData(GetNow(), "ACTIVE", "PLC CONNECTION LOST", "System")
                    PLCConInAlarm = True
                End If
            Else
                Label53.Text = "PLC Connected"
                Label53.ForeColor = Color.Green
                PLCConIndicator.Image = My.Resources.StatusPLCGood
                If PLCConInAlarm = True Then
                    Dim CurrentDT As DateTime = Now
                    If RunSQL Then SaveAlarmData(GetNow(), "CLEAR", "PLC CONNECTION RESTORED", "System")
                    PLCConInAlarm = False
                End If
                ' 
            End If
        End If
    End Sub
    Private Sub BoilerPumpRunTimer_Tick(sender As Object, e As EventArgs) Handles BoilerPumpRunTimer.Tick
        Dim RightNow As DateTime = Now
        Dim day, hour, min As String

        If FeedWaterPump1.Value = True Then
            Dim FeedPump1StartTime As New DateTime(BasicLabel62.Value, BasicLabel61.Value, BasicLabel57.Value, BasicLabel58.Value, BasicLabel59.Value, 0)
            Dim TimeSinceF1Started As TimeSpan
            TimeSinceF1Started = RightNow.Subtract(FeedPump1StartTime)
            If TimeSinceF1Started.ToString("g").Split(".")(0).Split(":").Length = 3 Then
                day = 0
                hour = TimeSinceF1Started.ToString("g").Split(".")(0).Split(":")(0)
                min = TimeSinceF1Started.ToString("g").Split(".")(0).Split(":")(1)
            Else
                day = TimeSinceF1Started.ToString("g").Split(".")(0).Split(":")(0)
                hour = TimeSinceF1Started.ToString("g").Split(".")(0).Split(":")(1)
                min = TimeSinceF1Started.ToString("g").Split(".")(0).Split(":")(2)
            End If
            Label52.Text = "Time In Operation" & vbNewLine & " D:" & day & " H:" & hour & " M:" & min
        Else
            Label52.Text = "Time In Operation" & vbNewLine & " D:0 H:0 M:0"
        End If

        If FeedWaterPump2.Value = True Then
            Dim FeedPump2StartTime As New DateTime(BasicLabel63.Value, BasicLabel65.Value, BasicLabel64.Value, BasicLabel66.Value, BasicLabel67.Value, 0)
            Dim TimeSinceF2Started As TimeSpan
            TimeSinceF2Started = RightNow.Subtract(FeedPump2StartTime)
            If TimeSinceF2Started.ToString("g").Split(".")(0).Split(":").Length = 3 Then
                day = 0
                hour = TimeSinceF2Started.ToString("g").Split(".")(0).Split(":")(0)
                min = TimeSinceF2Started.ToString("g").Split(".")(0).Split(":")(1)
            Else
                day = TimeSinceF2Started.ToString("g").Split(".")(0).Split(":")(0)
                hour = TimeSinceF2Started.ToString("g").Split(".")(0).Split(":")(1)
                min = TimeSinceF2Started.ToString("g").Split(".")(0).Split(":")(2)
            End If
            Label298.Text = "Time In Operation" & vbNewLine & " D:" & day & " H:" & hour & " M:" & min
        Else
            Label298.Text = "Time In Operation" & vbNewLine & " D:0 H:0 M:0"
        End If

        If CondensatePump1.Value = True Then
            Dim CondPump1StartTime As New DateTime(BasicLabel75.Value, BasicLabel77.Value, BasicLabel76.Value, BasicLabel78.Value, BasicLabel79.Value, 0)
            Dim TimeSinceC1Started As TimeSpan
            TimeSinceC1Started = RightNow.Subtract(CondPump1StartTime)
            If TimeSinceC1Started.ToString("g").Split(".")(0).Split(":").Length = 3 Then
                day = 0
                hour = TimeSinceC1Started.ToString("g").Split(".")(0).Split(":")(0)
                min = TimeSinceC1Started.ToString("g").Split(".")(0).Split(":")(1)
            Else
                day = TimeSinceC1Started.ToString("g").Split(".")(0).Split(":")(0)
                hour = TimeSinceC1Started.ToString("g").Split(".")(0).Split(":")(1)
                min = TimeSinceC1Started.ToString("g").Split(".")(0).Split(":")(2)
            End If
            Label68.Text = "Time In Operation" & vbNewLine & " D:" & day & " H:" & hour & " M:" & min
        Else
            Label68.Text = "Time In Operation" & vbNewLine & " D:0 H:0 M:0"

        End If

        If CondensatePump2.Value = True Then
            Dim CondPump2StartTime As New DateTime(BasicLabel69.Value, BasicLabel71.Value, BasicLabel70.Value, BasicLabel72.Value, BasicLabel73.Value, 0)
            Dim TimeSinceC2Started As TimeSpan
            TimeSinceC2Started = RightNow.Subtract(CondPump2StartTime)
            If TimeSinceC2Started.ToString("g").Split(".")(0).Split(":").Length = 3 Then
                day = 0
                hour = TimeSinceC2Started.ToString("g").Split(".")(0).Split(":")(0)
                min = TimeSinceC2Started.ToString("g").Split(".")(0).Split(":")(1)
            Else
                day = TimeSinceC2Started.ToString("g").Split(".")(0).Split(":")(0)
                hour = TimeSinceC2Started.ToString("g").Split(".")(0).Split(":")(1)
                min = TimeSinceC2Started.ToString("g").Split(".")(0).Split(":")(2)
            End If
            Label299.Text = "Time In Operation" & vbNewLine & " D:" & day & " H:" & hour & " M:" & min
        Else
            Label299.Text = "Time In Operation" & vbNewLine & " D:0 H:0 M:0"
        End If
    End Sub
    '║                                                                                                              ║
    '╠══════════════════════════════════════════════════════════════════════════════════════════════════════════════╣
    '║                                                   [TIMERS]                                                   ║
    '║                                                 Section End                                                  ║
    '╚══════════════════════════════════════════════════════════════════════════════════════════════════════════════╝
#End Region
#Region "Screen Controls"
    '╔══════════════════════════════════════════════════════════════════════════════════════════════════════════════╗
    '║                                              [SCREEN CONTROL]                                                ║
    '║                                                Section Start                                                 ║
    '╠══════════════════════════════════════════════════════════════════════════════════════════════════════════════╣
    '║                                                                                                              ║
    Private Sub Clear_Screens()
        Boiler_Page.Hide()
        Compressor_Page.Hide()
        Main_Page.Hide()
        Electrical_Page.Hide()
        Setpoints_Page.Hide()
        Realtime_Page.Hide()
        Alarms_Page.Hide()
        Enviro_Page.Hide()
        Water_Page.Hide()
        Status_Page.Hide()
        Login_Screen.Hide()
        Steam_Page.Hide()
        AdminSettings_Page.Hide()
    End Sub
    Private Sub Clear_Nav_Buttons()
        Goto_Dashboard_BTN.BackColor = Color.Black
        Goto_Dashboard_BTN.ForeColor = Color.Green
        Goto_Boilers_BTN.BackColor = Color.Black
        Goto_Boilers_BTN.ForeColor = Color.Green
        Goto_Compressors_BTN.BackColor = Color.Black
        Goto_Compressors_BTN.ForeColor = Color.Green
        Goto_Electrical_BTN.BackColor = Color.Black
        Goto_Electrical_BTN.ForeColor = Color.Green
        Goto_Environment_BTN.BackColor = Color.Black
        Goto_Environment_BTN.ForeColor = Color.Green
        Goto_Logs_BTN.BackColor = Color.Black
        Goto_Logs_BTN.ForeColor = Color.Green
        Goto_Alarms_BTN.BackColor = Color.Black
        Goto_Alarms_BTN.ForeColor = Color.Green
        Goto_Water_BTN.BackColor = Color.Black
        Goto_Water_BTN.ForeColor = Color.Green
        Goto_Settings_BTN.BackColor = Color.Black
        Goto_Settings_BTN.ForeColor = Color.Green
        Goto_Faults_BTN.BackColor = Color.Black
        Goto_Faults_BTN.ForeColor = Color.Green
        Goto_Steam_BTN.BackColor = Color.Black
        Goto_Steam_BTN.ForeColor = Color.Green
    End Sub
    Private Sub Goto_Steam_Page_Click(sender As Object, e As EventArgs) Handles Goto_Steam_BTN.Click, SteamAlarmIcon1.Click
        Clear_Screens()
        Steam_Page.Show()
        Clear_Nav_Buttons()
        Goto_Steam_BTN.BackColor = Color.Green
        Goto_Steam_BTN.ForeColor = Color.Black
    End Sub
    Private Sub Goto_Dashboard(sender As Object, e As EventArgs) Handles Goto_Dashboard_BTN.Click
        Clear_Screens()
        Main_Page.Show()
        Clear_Nav_Buttons()
        Goto_Dashboard_BTN.BackColor = Color.Green
        Goto_Dashboard_BTN.ForeColor = Color.Black
    End Sub
    Private Sub Goto_Boiler_Screen(sender As Object, e As EventArgs) Handles Goto_Boilers_BTN.Click
        Clear_Screens()
        Boiler_Page.Show()
        Clear_Nav_Buttons()
        Goto_Boilers_BTN.BackColor = Color.Green
        Goto_Boilers_BTN.ForeColor = Color.Black
    End Sub
    Private Sub Goto_Settings(sender As Object, e As EventArgs) Handles Goto_Settings_BTN.Click
        Clear_Screens()
        Setpoints_Page.Show()

        Clear_Nav_Buttons()
        Goto_Settings_BTN.BackColor = Color.Green
        Goto_Settings_BTN.ForeColor = Color.Black
    End Sub
    Private Sub Goto_Logs(sender As Object, e As EventArgs) Handles Goto_Logs_BTN.Click
        Clear_Screens()
        Realtime_Page.Show()
        Clear_Nav_Buttons()
        Goto_Logs_BTN.BackColor = Color.Green
        Goto_Logs_BTN.ForeColor = Color.Black
        If Chart3.ChartAreas(0).Visible = True Then Button2.ForeColor = Color.Green Else Button2.ForeColor = Color.Red
        If Chart3.ChartAreas(1).Visible = True Then Button3.ForeColor = Color.Green Else Button3.ForeColor = Color.Red
        If Chart3.ChartAreas(2).Visible = True Then Button4.ForeColor = Color.Green Else Button4.ForeColor = Color.Red
        If Chart3.ChartAreas(3).Visible = True Then Button5.ForeColor = Color.Green Else Button5.ForeColor = Color.Red

    End Sub
    Private Sub Goto_Water(sender As Object, e As EventArgs) Handles Goto_Water_BTN.Click, WaterAlarmIcon1.Click
        Clear_Screens()
        Water_Page.Show()
        Clear_Nav_Buttons()
        Goto_Water_BTN.BackColor = Color.Green
        Goto_Water_BTN.ForeColor = Color.Black
    End Sub
    Private Sub Goto_Alarms(sender As Object, e As EventArgs) Handles Goto_Alarms_BTN.Click
        Clear_Screens()
        Alarms_Page.Show()
        Clear_Nav_Buttons()
        Goto_Alarms_BTN.BackColor = Color.Green
        Goto_Alarms_BTN.ForeColor = Color.Black

    End Sub
    Private Sub Goto_Electrical(sender As Object, e As EventArgs) Handles Goto_Electrical_BTN.Click, ElecAlarmIcon1.Click
        Clear_Screens()
        Electrical_Page.Show()
        Clear_Nav_Buttons()
        Goto_Electrical_BTN.BackColor = Color.Green
        Goto_Electrical_BTN.ForeColor = Color.Black
    End Sub
    Private Sub Goto_Air(sender As Object, e As EventArgs) Handles Goto_Compressors_BTN.Click, AirAlarmIcon1.Click
        Clear_Screens()
        Compressor_Page.Show()
        Clear_Nav_Buttons()
        Goto_Compressors_BTN.BackColor = Color.Green
        Goto_Compressors_BTN.ForeColor = Color.Black
    End Sub
    Private Sub Goto_Enviro(sender As Object, e As EventArgs) Handles Goto_Environment_BTN.Click
        UpdateWeatherData()
        Clear_Screens()
        Enviro_Page.Show()
        Clear_Nav_Buttons()
        Goto_Environment_BTN.BackColor = Color.Green
        Goto_Environment_BTN.ForeColor = Color.Black
    End Sub
    Private Sub Goto_Faults(sender As Object, e As EventArgs) Handles Goto_Faults_BTN.Click
        If Status_Page.Visible = False Then
            Clear_Screens()
            Status_Page.Show()
            Clear_Nav_Buttons()
            Goto_Faults_BTN.BackColor = Color.Green
            Goto_Faults_BTN.ForeColor = Color.Black
        End If

    End Sub
    Sub HideIOPanels()
        Slot0Panel.Hide()
        Slot1Panel.Hide()
        Slot2Panel.Hide()
        Slot3Panel.Hide()
        Slot4Panel.Hide()
        Slot5Panel.Hide()
        Slot6Panel.Hide()
        Slot7Panel.Hide()
        Slot8Panel.Hide()
    End Sub
    Private Sub Slot0_Show(sender As Object, e As EventArgs) Handles PictureBox73.Click
        HideIOPanels()
        Slot0Panel.Show()
    End Sub
    Private Sub Slot1_Show(sender As Object, e As EventArgs) Handles PictureBox72.Click
        HideIOPanels()
        Slot1Panel.Show()
    End Sub
    Private Sub Slot2_Show(sender As Object, e As EventArgs) Handles PictureBox74.Click
        HideIOPanels()
        Slot2Panel.Show()
    End Sub
    Private Sub Slot3_Show(sender As Object, e As EventArgs) Handles PictureBox75.Click
        HideIOPanels()
        Slot3Panel.Show()
    End Sub
    Private Sub Slot4_Show(sender As Object, e As EventArgs) Handles PictureBox76.Click
        HideIOPanels()
        Slot4Panel.Show()
    End Sub
    Private Sub Slot5_Show(sender As Object, e As EventArgs) Handles PictureBox69.Click
        HideIOPanels()
        Slot5Panel.Show()
    End Sub
    Private Sub Slot7_Show(sender As Object, e As EventArgs) Handles PictureBox70.Click
        HideIOPanels()
        Slot6Panel.Show()
    End Sub
    Private Sub Slot8_Show(sender As Object, e As EventArgs) Handles PictureBox71.Click
        HideIOPanels()
        Slot7Panel.Show()
    End Sub
    Private Sub Slot9_Show(sender As Object, e As EventArgs) Handles PictureBox77.Click
        HideIOPanels()
        Slot8Panel.Show()
    End Sub
    '║                                                                                                              ║
    '╠══════════════════════════════════════════════════════════════════════════════════════════════════════════════╣
    '║                                              [SCREEN CONTROL]                                                ║
    '║                                                 Section End                                                  ║
    '╚══════════════════════════════════════════════════════════════════════════════════════════════════════════════╝
#End Region
#Region "User Functions"
    '╔══════════════════════════════════════════════════════════════════════════════════════════════════════════════╗
    '║                                              [USER FUNCTIONS]                                                ║
    '║                                                Section Start                                                 ║
    '╠══════════════════════════════════════════════════════════════════════════════════════════════════════════════╣
    '║                                                                                                              ║
    'login logic
    ' Encode plain text to Base64
    Public Shared Function Encode(password As String) As String
        Dim plainBytes As Byte() = Encoding.UTF8.GetBytes(password)
        Return Convert.ToBase64String(plainBytes)
    End Function

    ' Decode Base64 back to plain text
    Public Shared Function Decode(encoded As String) As String
        Dim decodedBytes As Byte() = Convert.FromBase64String(encoded)
        Return Encoding.UTF8.GetString(decodedBytes)
    End Function
    Public Sub ClickHandler(sender As Object, e As MouseEventArgs) Handles Me.MouseMove,
        Boiler_Page.MouseMove, Alarms_Page.MouseMove, Login_Screen.MouseMove, AlarmHistoryPanel.MouseMove, Steam_Page.MouseMove,
        Main_Page.MouseMove, Compressor_Page.MouseMove, Electrical_Page.MouseMove, Realtime_Page.MouseMove, Status_Page.MouseMove,
        Setpoints_Page.MouseMove, Enviro_Page.MouseMove, Goto_Faults_BTN.Click, Goto_Settings_BTN.Click, Goto_Alarms_BTN.Click,
        Goto_Logs_BTN.Click, Goto_Environment_BTN.Click, Goto_Electrical_BTN.Click, Goto_Water_BTN.Click, Goto_Compressors_BTN.Click,
        Goto_Steam_BTN.Click, Goto_Dashboard_BTN.Click, Goto_Boilers_BTN.Click, Main_Page.Click, Boiler_Page.Click, Compressor_Page.Click,
        Alarms_Page.Click, Login_Screen.Click

        If CurrentUser = "System" Then
            LoginTimeRemaining = GetNumeric(INI_GetKey_System("APP", "SCREEN_TO"))
            If LoginTimer.Enabled = False Then LoginTimer.Start()
        Else
            LoginTimeRemaining = GetNumeric(INI_GetKey_System("APP", "USER_TO"))
        End If
    End Sub

    Public Sub SP_LevelCheck(sender As Object, e As MouseEventArgs) Handles ALP_Min.Click, ALP_Max.Click,
        ELSC_Min.Click, ELSC_Max.Click,
        ELSB_Min.Click, ELSB_Max.Click,
        ELSA_Min.Click, ELSA_Max.Click,
        ELNC_Min.Click, ELNC_Max.Click,
        ELNB_Min.Click, ELNB_Max.Click,
        ELNA_Min.Click, ELNA_Max.Click,
        STMP_Min.Click, STMP_Max.Click,
        STLP_Min.Click, STLP_Max.Click,
        STHP_Min.Click, STHP_Max.Click,
        STFL_Min.Click, STFL_Max.Click,
        STFWP_Min.Click, STFWP_Max.Click,
        CWPRP_Min.Click, CWPRP_Max.Click,
        CWPOP_Min.Click, CWPOP_Max.Click,
        CWFD_Min.Click, CWFD_Max.Click,
        CWSUP_Min.Click, CWSUP_Max.Click,
        HWPRP_Min.Click, HWPRP_Max.Click,
        HWPOP_Min.Click, HWPOP_Max.Click,
        HWFD_Min.Click, HWFD_Max.Click,
        HWTP_Min.Click, HWTP_Max.Click,
        HWFL_Min.Click, HWFL_Max.Click,
        STMPDEM_Min.Click, STMPDEM_Max.Click,
        STLPDEM_Min.Click, STLPDEM_Max.Click

        If CurrentUser = "System" Or CurrentUserLevel < 3 Then
            ShowLoginScreen()
        End If

    End Sub

    Sub ShowLoginScreen()
        USER_Input_CB.Items.Clear()
        Dim USERS As List(Of String) = INI_GetKey_AllUSER()
        For Each i As String In USERS
            USER_Input_CB.Items.Add(i)
        Next
        LastLogin_Label.Text = ""
        Login_Screen.Show()
        Login_Screen.BringToFront()

    End Sub
    Private Sub Goto_Login(sender As Object, e As EventArgs) Handles Goto_Login_Screen.Click
        If LoginTimer.Enabled = True AndAlso CurrentUser IsNot "System" Then
            USER_LOGOUT()
        Else
            ShowLoginScreen()

        End If
    End Sub
    Private Sub UserInputListOpen(sender As Object, e As EventArgs) Handles USER_Input_CB.Click
        USER_Input_CB.DroppedDown = True
    End Sub
    Private Sub EditUserInputListOpen(sender As Object, e As EventArgs) Handles ComboBox9.Click
        ComboBox9.DroppedDown = True
    End Sub
    Private Sub Admin_User_Selection_Changed(sender As Object, e As EventArgs) Handles ComboBox9.SelectedIndexChanged

        Dim User As String = ComboBox9.Text
        Dim UserLevel As String = ""

        UserPassLabel.Text = Decode(INI_GetKey_USER(ComboBox9.Text, "PASS"))
        UserLevel = GetNumeric(INI_GetKey_USER(ComboBox9.Text, "LEVEL"))
        UserLastLoginLabel.Text = INI_GetKey_USER(ComboBox9.Text, "LAST")
        If UserLevel = 1 Then
            UserLevelLabel.Text = "1-Operator"
        ElseIf UserLevel = 2 Then
            UserLevelLabel.Text = "2-Technician"
        ElseIf UserLevel = 3 Then
            UserLevelLabel.Text = "3-Administrator"
        ElseIf UserLevel = 4 Then
            UserLevelLabel.Text = "4-Engineer"
        End If

    End Sub



    Sub TRYLOGIN()
        'declare variables
        Dim User_IN As String = USER_Input_CB.Text
        Dim Pass_IN As String = Pass_Input_TB.Text
        If Decode(INI_GetKey_USER(User_IN, "PASS")) = Pass_IN Then
            CurrentUser = User_IN
            CurrentUserLevel = GetNumeric(INI_GetKey_USER(User_IN, "LEVEL"))
            'Log Event - Successful Login
            If RunSQL Then SaveSystemData(GetNow(), "User Logged In (" & User_IN & ")", "SYSTEM")
            INI_SetKey_USER(User_IN, "LAST", Now.ToLocalTime.ToString)

            If CurrentUserLevel > 1 Then

                For Each Settings_CNTL As System.Object In Setpoints_Page.Controls()
                    If Settings_CNTL.name.endswith("Max") Or Settings_CNTL.name.endswith("Min") Then
                        Settings_CNTL.PLCAddressKeypad = Settings_CNTL.PLCAddressValue
                    End If
                Next
            Else
                For Each Settings_CNTL As System.Object In Setpoints_Page.Controls()
                    If Settings_CNTL.name.endswith("Max") Or Settings_CNTL.name.endswith("Min") Then
                        Settings_CNTL.PLCAddressKeypad = ""
                    End If
                Next

            End If

            Login_Screen.Hide()
            LoginTimeRemaining = INI_GetKey_System("APP", "USER_TO")
            LoginTimer.Enabled = True
            Goto_Login_Screen.Text = "LOGOUT"
            USER_Input_CB.Items.Clear()
            Pass_Input_TB.Text = ""
        Else
            'Log Event - Incorrect Password
            If RunSQL Then SaveSystemData(GetNow(), "Login Attempt Faied (" & User_IN & ")", "SYSTEM")
            MessageBox.Show("Incorrect Password")
            Pass_Input_TB.Text = ""
        End If




    End Sub

    Private Sub LoginButton_Click(sender As Object, e As EventArgs) Handles LoginButton.Click
        TRYLOGIN()
    End Sub
    Private Sub TextBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles Pass_Input_TB.KeyDown
        If e.KeyValue = Keys.Enter Then
            TRYLOGIN()
        End If
    End Sub
    Private Sub ComboBox7_TextChanged(sender As Object, e As EventArgs) Handles USER_Input_CB.TextChanged
        LastLogin_Label.Text = "Last Login: " & INI_GetKey_USER(sender.text, "LAST")
    End Sub
    Public Sub USER_LOGOUT()

        For Each Settings_CNTL As System.Object In Setpoints_Page.Controls()
            If Settings_CNTL.name.endswith("Max") Or Settings_CNTL.name.endswith("Min") Then
                Settings_CNTL.PLCAddressKeypad = ""
            End If
        Next
        'HIDE ADMIN CONTROLS
        OPButton.Hide()
        TECHButton.Hide()
        ADMINButton.Hide()
        NewPassTB.Hide()
        ComboBox9.Enabled = True
        Button25.Show()
        Button26.Show()
        Button27.Show()
        'SET USERLEVEL TO 0-SYSTEM
        CurrentUserLevel = 0
        CurrentUser = "System"
        Goto_Login_Screen.Text = "LOGIN"
        Label23.Text = "Current User:" & vbNewLine & "System"
        LoginTimeRemaining = 0
        LoginTimer.Enabled = False
        'LOG EVENT TO SQL
        If RunSQL Then SaveSystemData(GetNow(), "User Logged Out" & "(" & CurrentUser & ")", "System")





        If Main_Page.Visible = False Then
            Clear_Screens()
            Clear_Nav_Buttons()
            Main_Page.Show()
            Goto_Dashboard_BTN.ForeColor = Color.Black
            Goto_Dashboard_BTN.BackColor = Color.Green
        End If
        USER_Input_CB.Text = ""

        For Each ctrl As Control In Setpoints_Page.Controls
            If TypeOf ctrl Is AdvancedHMIControls.BasicLabel AndAlso ctrl.Name.Contains("MIN") Or ctrl.Name.Contains("MAX") Then
                CType(ctrl, AdvancedHMIControls.BasicLabel).PLCAddressKeypad = ""
            End If
        Next
        For Each ctrl As Control In Setpoints_Page.Controls
            If TypeOf ctrl Is AdvancedHMIControls.BasicLabel AndAlso ctrl.Name.Contains("Feedback") Or ctrl.Name.Contains("Command") Then
                CType(ctrl, AdvancedHMIControls.BasicLabel).PLCAddressKeypad = ""
            End If
        Next

    End Sub

    'Admin Functions
    Public Sub RefreshSettingsScreen()
        Clear_Screens()
        Clear_Nav_Buttons()
        AdminSettings_Page.Show()
        ComboBox9.Items.Clear()
        ComboBox9.Text = ""
        UserLastLoginLabel.Text = ""
        UserLevelLabel.Text = ""
        UserPassLabel.Text = ""
        Dim x As Integer = 0
        For Each i As String In INI_GetKey_AllUSER()
            ComboBox9.Items.Add(i)
        Next

    End Sub
    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles HD_BTN.Click
        Me.WindowState = FormWindowState.Minimized
    End Sub
    Private Sub Button25_Click(sender As Object, e As EventArgs) Handles Button25.Click
        If NewPassTB.Visible = False Then
            NewPassTB.Show()
            Button25.Text = "Confirm Change"
            Button26.Hide()
            Button27.Hide()
            ComboBox9.Enabled = False
        Else
            If NewPassTB.Text = "" Then
                MessageBox.Show("You must enter a New password first")
            Else
                INI_SetKey_USER(ComboBox9.Text, "PASS", Encode(NewPassTB.Text))
                NewPassTB.Hide()
                Button25.Text = "Change User Password"
                Button26.Show()
                Button27.Show()
                ComboBox9.Enabled = True
                RefreshSettingsScreen()
                If RunSQL Then SaveSystemData(GetNow(), "User (" & ComboBox9.Text & ") password has been changed", CurrentUser)
            End If
        End If
        NewPassTB.Show()
    End Sub
    Private Sub Button26_Click(sender As Object, e As EventArgs) Handles Button26.Click
        OPButton.Show()
        TECHButton.Show()
        ADMINButton.Show()
        Button25.Hide()
        Button27.Hide()
        ComboBox9.Enabled = False
    End Sub
    Private Sub OPButton_Click(sender As Object, e As EventArgs) Handles OPButton.Click

        If RunSQL Then SaveSystemData(GetNow(), "(" & ComboBox9.Text & ") Level set to: 1", CurrentUser)
        INI_SetKey_USER(ComboBox9.Text, "LEVEL", "1")
        OPButton.Hide()
        TECHButton.Hide()
        ADMINButton.Hide()
        Button25.Enabled = True
        Button27.Enabled = True
        ComboBox9.Enabled = True
        RefreshSettingsScreen()
    End Sub
    Private Sub TECHButton_Click(sender As Object, e As EventArgs) Handles TECHButton.Click

        If RunSQL Then SaveSystemData(GetNow(), "(" & ComboBox9.Text & ") Level set to: 2", CurrentUser)
        INI_SetKey_USER(ComboBox9.Text, "LEVEL", "2")
        OPButton.Hide()
        TECHButton.Hide()
        ADMINButton.Hide()
        Button25.Enabled = True
        Button27.Enabled = True
        ComboBox9.Enabled = True
        RefreshSettingsScreen()
    End Sub
    Private Sub ADMINButton_Click(sender As Object, e As EventArgs) Handles ADMINButton.Click

        If RunSQL Then SaveSystemData(GetNow(), "(" & ComboBox9.Text & ") Level set to: 3", CurrentUser)
        INI_SetKey_USER(ComboBox9.Text, "LEVEL", "3")
        OPButton.Hide()
        TECHButton.Hide()
        ADMINButton.Hide()
        Button25.Show()
        Button27.Show()
        ComboBox9.Enabled = True
        RefreshSettingsScreen()
    End Sub
    Private Sub Remove_User_Clicked(sender As Object, e As EventArgs) Handles Button27.Click

        Dim answer As Integer
        answer = MsgBox("Are you sure you want to remove user (" & ComboBox9.Text & ")?", vbQuestion + vbYesNo + vbDefaultButton2, "Remove User")
        If answer = vbYes Then
            INI_RemoveSection_USER(ComboBox9.Text)
            If RunSQL Then SaveSystemData(GetNow(), "User (" & ComboBox9.Text & ") Removed", CurrentUser)
            MessageBox.Show("User (" & ComboBox9.Text & ") Successfully Removed", "Remove User")
            RefreshSettingsScreen()
        End If
    End Sub
    Private Sub ADD_User_Clicked(sender As Object, e As EventArgs) Handles Button28.Click

        Dim userrights As Integer = 1
        If RadioButton1.Checked = True Then userrights = 1
        If RadioButton2.Checked = True Then userrights = 2
        If RadioButton3.Checked = True Then userrights = 3

        If NewUsernameTB.Text = "" Or NewPasswordTB.Text = "" Then
            MessageBox.Show("You must enter a usernameandalso password to create a new user")
        Else
            INI_AddSection_USER(NewUsernameTB.Text, NewPasswordTB.Text, userrights.ToString)
            If RunSQL Then SaveSystemData(GetNow(), "User (" & NewUsernameTB.Text & ") added. Level: " & userrights, CurrentUser)
        End If

        NewUsernameTB.Text = ""
        NewPasswordTB.Text = ""
        RefreshSettingsScreen()

    End Sub




    '║                                                                                                              ║
    '╠══════════════════════════════════════════════════════════════════════════════════════════════════════════════╣
    '║                                              [USER FUNCTIONS]                                                ║
    '║                                                 Section End                                                  ║
    '╚══════════════════════════════════════════════════════════════════════════════════════════════════════════════╝
#End Region

#Region "Alarm Logic"

    '╔══════════════════════════════════════════════════════════════════════════════════════════════════════════════╗
    '║                                                [ALARM LOGIC]                                                 ║
    '║                                                Section Start                                                 ║
    '╠══════════════════════════════════════════════════════════════════════════════════════════════════════════════╣
    '║                                                                                                              ║
    Private Sub EA1_ValueChanged(sender As Object, e As EventArgs) Handles EA1.ValueChanged
        Dim AlarmArray() As Boolean = IntToBin(sender.value)
        ElecAlarmLight0.SelectColor2 = AlarmArray(0)
        ElecAlarmLight1.SelectColor2 = AlarmArray(1)
        ElecAlarmLight2.SelectColor2 = AlarmArray(2)
        ElecAlarmLight3.SelectColor2 = AlarmArray(3)
        ElecAlarmLight4.SelectColor2 = AlarmArray(4)
        ElecAlarmLight5.SelectColor2 = AlarmArray(5)
        ElecAlarmLight6.SelectColor2 = AlarmArray(6)
        ElecAlarmLight7.SelectColor2 = AlarmArray(7)
        ElecAlarmLight8.SelectColor2 = AlarmArray(8)
        ElecAlarmLight9.SelectColor2 = AlarmArray(9)
        ElecAlarmLight10.SelectColor2 = AlarmArray(10)
        ElecAlarmLight11.SelectColor2 = AlarmArray(11)
        ElecAlarmLight12.SelectColor2 = AlarmArray(12)
        ElecAlarmLight13.SelectColor2 = AlarmArray(13)
        ElecAlarmLight14.SelectColor2 = AlarmArray(14)
        ElecAlarmLight15.SelectColor2 = AlarmArray(15)
    End Sub
    Private Sub EA2_ValueChanged(sender As Object, e As EventArgs) Handles EA2.ValueChanged
        Dim AlarmArray() As Boolean = IntToBin(sender.value)
        ElecAlarmLight16.SelectColor2 = AlarmArray(0)
        ElecAlarmLight17.SelectColor2 = AlarmArray(1)
        ElecAlarmLight18.SelectColor2 = AlarmArray(2)
        ElecAlarmLight19.SelectColor2 = AlarmArray(3)
        ElecAlarmLight20.SelectColor2 = AlarmArray(4)
        ElecAlarmLight21.SelectColor2 = AlarmArray(5)
        ElecAlarmLight22.SelectColor2 = AlarmArray(6)
        ElecAlarmLight23.SelectColor2 = AlarmArray(7)
        ElecAlarmLight24.SelectColor2 = AlarmArray(8)
        ElecAlarmLight25.SelectColor2 = AlarmArray(9)
        ElecAlarmLight26.SelectColor2 = AlarmArray(10)
        ElecAlarmLight27.SelectColor2 = AlarmArray(11)
        ElecAlarmLight28.SelectColor2 = AlarmArray(12)
        ElecAlarmLight29.SelectColor2 = AlarmArray(13)
        ElecAlarmLight30.SelectColor2 = AlarmArray(14)
        ElecAlarmLight31.SelectColor2 = AlarmArray(15)
    End Sub

    Private Sub WA1_ValueChanged(sender As Object, e As EventArgs) Handles WA1.ValueChanged
        Dim AlarmArray() As Boolean = IntToBin(sender.value)
        WaterAlarmLight0.SelectColor2 = AlarmArray(0)
        WaterAlarmLight1.SelectColor2 = AlarmArray(1)
        WaterAlarmLight2.SelectColor2 = AlarmArray(2)
        WaterAlarmLight3.SelectColor2 = AlarmArray(3)
        WaterAlarmLight4.SelectColor2 = AlarmArray(4)
        WaterAlarmLight5.SelectColor2 = AlarmArray(5)
        WaterAlarmLight6.SelectColor2 = AlarmArray(6)
        WaterAlarmLight7.SelectColor2 = AlarmArray(7)
        WaterAlarmLight8.SelectColor2 = AlarmArray(8)
        WaterAlarmLight9.SelectColor2 = AlarmArray(9)
        WaterAlarmLight10.SelectColor2 = AlarmArray(10)
        WaterAlarmLight11.SelectColor2 = AlarmArray(11)
        WaterAlarmLight12.SelectColor2 = AlarmArray(12)
        WaterAlarmLight13.SelectColor2 = AlarmArray(13)
        WaterAlarmLight14.SelectColor2 = AlarmArray(14)
        WaterAlarmLight15.SelectColor2 = AlarmArray(15)
    End Sub
    Private Sub WA2_ValueChanged(sender As Object, e As EventArgs) Handles WA2.ValueChanged
        Dim AlarmArray() As Boolean = IntToBin(sender.value)
        WaterAlarmLight16.SelectColor2 = AlarmArray(0)
        WaterAlarmLight17.SelectColor2 = AlarmArray(1)
        WaterAlarmLight18.SelectColor2 = AlarmArray(2)
        WaterAlarmLight19.SelectColor2 = AlarmArray(3)
        WaterAlarmLight20.SelectColor2 = AlarmArray(4)
        WaterAlarmLight21.SelectColor2 = AlarmArray(5)
        WaterAlarmLight22.SelectColor2 = AlarmArray(6)
        WaterAlarmLight23.SelectColor2 = AlarmArray(7)
        WaterAlarmLight24.SelectColor2 = AlarmArray(8)
        WaterAlarmLight25.SelectColor2 = AlarmArray(9)
        WaterAlarmLight26.SelectColor2 = AlarmArray(10)
        WaterAlarmLight27.SelectColor2 = AlarmArray(11)
        WaterAlarmLight28.SelectColor2 = AlarmArray(12)
        WaterAlarmLight29.SelectColor2 = AlarmArray(13)
        WaterAlarmLight30.SelectColor2 = AlarmArray(14)
        WaterAlarmLight31.SelectColor2 = AlarmArray(15)
    End Sub
    Private Sub AA1_ValueChanged(sender As Object, e As EventArgs) Handles AA1.ValueChanged
        Dim AlarmArray() As Boolean = IntToBin(sender.value)
        AirAlarmLight0.SelectColor2 = AlarmArray(0)
        AirAlarmLight1.SelectColor2 = AlarmArray(1)
        AirAlarmLight2.SelectColor2 = AlarmArray(2)
        AirAlarmLight3.SelectColor2 = AlarmArray(3)
        AirAlarmLight4.SelectColor2 = AlarmArray(4)
        AirAlarmLight5.SelectColor2 = AlarmArray(5)
        AirAlarmLight6.SelectColor2 = AlarmArray(6)
        AirAlarmLight7.SelectColor2 = AlarmArray(7)
        AirAlarmLight8.SelectColor2 = AlarmArray(8)
        AirAlarmLight9.SelectColor2 = AlarmArray(9)
        AirAlarmLight10.SelectColor2 = AlarmArray(10)
        AirAlarmLight11.SelectColor2 = AlarmArray(11)
        AirAlarmLight12.SelectColor2 = AlarmArray(12)
        AirAlarmLight13.SelectColor2 = AlarmArray(13)
        AirAlarmLight14.SelectColor2 = AlarmArray(14)
        AirAlarmLight15.SelectColor2 = AlarmArray(15)
    End Sub
    Private Sub AA2_ValueChanged(sender As Object, e As EventArgs) Handles AA2.ValueChanged
        Dim AlarmArray() As Boolean = IntToBin(sender.value)
        AirAlarmLight16.SelectColor2 = AlarmArray(0)
        AirAlarmLight17.SelectColor2 = AlarmArray(1)
        AirAlarmLight18.SelectColor2 = AlarmArray(2)
        AirAlarmLight19.SelectColor2 = AlarmArray(3)
        AirAlarmLight20.SelectColor2 = AlarmArray(4)
        AirAlarmLight21.SelectColor2 = AlarmArray(5)
        AirAlarmLight22.SelectColor2 = AlarmArray(6)
        AirAlarmLight23.SelectColor2 = AlarmArray(7)
        AirAlarmLight24.SelectColor2 = AlarmArray(8)
        AirAlarmLight25.SelectColor2 = AlarmArray(9)
        AirAlarmLight26.SelectColor2 = AlarmArray(10)
        AirAlarmLight27.SelectColor2 = AlarmArray(11)
        AirAlarmLight28.SelectColor2 = AlarmArray(12)
        AirAlarmLight29.SelectColor2 = AlarmArray(13)
        AirAlarmLight30.SelectColor2 = AlarmArray(14)
        AirAlarmLight31.SelectColor2 = AlarmArray(15)
    End Sub
    Private Sub SA1_ValueChanged(sender As Object, e As EventArgs) Handles SA1.ValueChanged
        Dim AlarmArray() As Boolean = IntToBin(sender.value)
        SteamAlarmLight0.SelectColor2 = AlarmArray(0)
        SteamAlarmLight1.SelectColor2 = AlarmArray(1)
        SteamAlarmLight2.SelectColor2 = AlarmArray(2)
        SteamAlarmLight3.SelectColor2 = AlarmArray(3)
        SteamAlarmLight4.SelectColor2 = AlarmArray(4)
        SteamAlarmLight5.SelectColor2 = AlarmArray(5)
        SteamAlarmLight6.SelectColor2 = AlarmArray(6)
        SteamAlarmLight7.SelectColor2 = AlarmArray(7)
        SteamAlarmLight8.SelectColor2 = AlarmArray(8)
        SteamAlarmLight9.SelectColor2 = AlarmArray(9)
        SteamAlarmLight10.SelectColor2 = AlarmArray(10)
        SteamAlarmLight11.SelectColor2 = AlarmArray(11)
        SteamAlarmLight12.SelectColor2 = AlarmArray(12)
        SteamAlarmLight13.SelectColor2 = AlarmArray(13)
        SteamAlarmLight14.SelectColor2 = AlarmArray(14)
        SteamAlarmLight15.SelectColor2 = AlarmArray(15)
    End Sub
    Private Sub SA2_ValueChanged(sender As Object, e As EventArgs) Handles SA2.ValueChanged
        Dim AlarmArray() As Boolean = IntToBin(sender.value)
        SteamAlarmLight16.SelectColor2 = AlarmArray(0)
        SteamAlarmLight17.SelectColor2 = AlarmArray(1)
        SteamAlarmLight18.SelectColor2 = AlarmArray(2)
        SteamAlarmLight19.SelectColor2 = AlarmArray(3)
        SteamAlarmLight20.SelectColor2 = AlarmArray(4)
        SteamAlarmLight21.SelectColor2 = AlarmArray(5)
        SteamAlarmLight22.SelectColor2 = AlarmArray(6)
        SteamAlarmLight23.SelectColor2 = AlarmArray(7)
        SteamAlarmLight24.SelectColor2 = AlarmArray(8)
        SteamAlarmLight25.SelectColor2 = AlarmArray(9)
        SteamAlarmLight26.SelectColor2 = AlarmArray(10)
        SteamAlarmLight27.SelectColor2 = AlarmArray(11)
        SteamAlarmLight28.SelectColor2 = AlarmArray(12)
        SteamAlarmLight29.SelectColor2 = AlarmArray(13)
        SteamAlarmLight30.SelectColor2 = AlarmArray(14)
        SteamAlarmLight31.SelectColor2 = AlarmArray(15)
    End Sub

    Dim Elec_CNT, Air_CNT, Water_CNT, Steam_CNT As Integer

    Private Sub Elec_Alarm_Changed(sender As System.Object, e As EventArgs) Handles ElecAlarmLight0.ValueSelectColor2Changed,
            ElecAlarmLight1.ValueSelectColor2Changed, ElecAlarmLight2.ValueSelectColor2Changed, ElecAlarmLight3.ValueSelectColor2Changed, ElecAlarmLight4.ValueSelectColor2Changed,
            ElecAlarmLight5.ValueSelectColor2Changed, ElecAlarmLight6.ValueSelectColor2Changed, ElecAlarmLight7.ValueSelectColor2Changed, ElecAlarmLight8.ValueSelectColor2Changed,
            ElecAlarmLight9.ValueSelectColor2Changed, ElecAlarmLight10.ValueSelectColor2Changed, ElecAlarmLight11.ValueSelectColor2Changed, ElecAlarmLight12.ValueSelectColor2Changed,
            ElecAlarmLight13.ValueSelectColor2Changed, ElecAlarmLight14.ValueSelectColor2Changed, ElecAlarmLight15.ValueSelectColor2Changed, ElecAlarmLight16.ValueSelectColor2Changed,
            ElecAlarmLight17.ValueSelectColor2Changed, ElecAlarmLight18.ValueSelectColor2Changed, ElecAlarmLight19.ValueSelectColor2Changed, ElecAlarmLight20.ValueSelectColor2Changed,
            ElecAlarmLight21.ValueSelectColor2Changed, ElecAlarmLight22.ValueSelectColor2Changed, ElecAlarmLight23.ValueSelectColor2Changed, ElecAlarmLight24.ValueSelectColor2Changed,
            ElecAlarmLight25.ValueSelectColor2Changed, ElecAlarmLight26.ValueSelectColor2Changed, ElecAlarmLight27.ValueSelectColor2Changed, ElecAlarmLight28.ValueSelectColor2Changed,
            ElecAlarmLight29.ValueSelectColor2Changed, ElecAlarmLight30.ValueSelectColor2Changed, ElecAlarmLight31.ValueSelectColor2Changed

        If Firsttick Then
            Dim AlarmDesc As String = ElectricalAlarmPanel.Controls("ElecAlarmDesc" & GetNumeric(sender.name)).Text
            Dim AlarmStatus As String
            If sender.selectcolor2 = True Then AlarmStatus = "ACTIVE" Else AlarmStatus = "CLEAR"

            If RunSQL Then
                Dim CurrentDT As DateTime = Now
                SaveAlarmData(GetNow(), AlarmStatus, AlarmDesc, "Electrical")
            End If
        End If

        Elec_CNT = 0
        For i = 0 To 31
            Dim Eobj As System.Object = ElectricalAlarmPanel.Controls("ElecAlarmLight" & i)
            If Eobj.selectcolor2 = True Then Elec_CNT += 1
        Next
        AlarmDispUpdate()
    End Sub
    Private Sub Air_Alarm_Changed(sender As System.Object, e As EventArgs) Handles AirAlarmLight0.ValueSelectColor2Changed,
            AirAlarmLight1.ValueSelectColor2Changed, AirAlarmLight2.ValueSelectColor2Changed, AirAlarmLight3.ValueSelectColor2Changed, AirAlarmLight4.ValueSelectColor2Changed,
            AirAlarmLight5.ValueSelectColor2Changed, AirAlarmLight6.ValueSelectColor2Changed, AirAlarmLight7.ValueSelectColor2Changed, AirAlarmLight8.ValueSelectColor2Changed,
            AirAlarmLight9.ValueSelectColor2Changed, AirAlarmLight10.ValueSelectColor2Changed, AirAlarmLight11.ValueSelectColor2Changed, AirAlarmLight12.ValueSelectColor2Changed,
            AirAlarmLight13.ValueSelectColor2Changed, AirAlarmLight14.ValueSelectColor2Changed, AirAlarmLight15.ValueSelectColor2Changed, AirAlarmLight16.ValueSelectColor2Changed,
            AirAlarmLight17.ValueSelectColor2Changed, AirAlarmLight18.ValueSelectColor2Changed, AirAlarmLight19.ValueSelectColor2Changed, AirAlarmLight20.ValueSelectColor2Changed,
            AirAlarmLight21.ValueSelectColor2Changed, AirAlarmLight22.ValueSelectColor2Changed, AirAlarmLight23.ValueSelectColor2Changed, AirAlarmLight24.ValueSelectColor2Changed,
            AirAlarmLight25.ValueSelectColor2Changed, AirAlarmLight26.ValueSelectColor2Changed, AirAlarmLight27.ValueSelectColor2Changed, AirAlarmLight28.ValueSelectColor2Changed,
            AirAlarmLight29.ValueSelectColor2Changed, AirAlarmLight30.ValueSelectColor2Changed, AirAlarmLight31.ValueSelectColor2Changed

        If Firsttick Then
            Dim AlarmDesc As String = AirAlarmPanel.Controls("AirAlarmDesc" & GetNumeric(sender.name)).Text
            Dim AlarmStatus As String
            If sender.selectcolor2 = True Then AlarmStatus = "ACTIVE" Else AlarmStatus = "CLEAR"

            If RunSQL Then
                Dim CurrentDT As DateTime = Now
                SaveAlarmData(GetNow(), AlarmStatus, AlarmDesc, "Air")
            End If
        End If

        Air_CNT = 0
        For i = 0 To 31
            Dim Aobj As System.Object = AirAlarmPanel.Controls("AirAlarmLight" & i)
            If Aobj.selectcolor2 = True Then Air_CNT += 1
        Next
        AlarmDispUpdate()
    End Sub
    Private Sub Steam_Alarm_Changed(sender As System.Object, e As EventArgs) Handles SteamAlarmLight0.ValueSelectColor2Changed,
            SteamAlarmLight1.ValueSelectColor2Changed, SteamAlarmLight2.ValueSelectColor2Changed, SteamAlarmLight3.ValueSelectColor2Changed, SteamAlarmLight4.ValueSelectColor2Changed,
            SteamAlarmLight5.ValueSelectColor2Changed, SteamAlarmLight6.ValueSelectColor2Changed, SteamAlarmLight7.ValueSelectColor2Changed, SteamAlarmLight8.ValueSelectColor2Changed,
            SteamAlarmLight9.ValueSelectColor2Changed, SteamAlarmLight10.ValueSelectColor2Changed, SteamAlarmLight11.ValueSelectColor2Changed, SteamAlarmLight12.ValueSelectColor2Changed,
            SteamAlarmLight13.ValueSelectColor2Changed, SteamAlarmLight14.ValueSelectColor2Changed, SteamAlarmLight15.ValueSelectColor2Changed, SteamAlarmLight16.ValueSelectColor2Changed,
            SteamAlarmLight17.ValueSelectColor2Changed, SteamAlarmLight18.ValueSelectColor2Changed, SteamAlarmLight19.ValueSelectColor2Changed, SteamAlarmLight20.ValueSelectColor2Changed,
            SteamAlarmLight21.ValueSelectColor2Changed, SteamAlarmLight22.ValueSelectColor2Changed, SteamAlarmLight23.ValueSelectColor2Changed, SteamAlarmLight24.ValueSelectColor2Changed,
            SteamAlarmLight25.ValueSelectColor2Changed, SteamAlarmLight26.ValueSelectColor2Changed, SteamAlarmLight27.ValueSelectColor2Changed, SteamAlarmLight28.ValueSelectColor2Changed,
            SteamAlarmLight29.ValueSelectColor2Changed, SteamAlarmLight30.ValueSelectColor2Changed, SteamAlarmLight31.ValueSelectColor2Changed

        If Firsttick Then
            Dim AlarmDesc As String = SteamAlarmPanel.Controls("SteamAlarmDesc" & GetNumeric(sender.name)).Text
            Dim AlarmStatus As String
            If sender.selectcolor2 = True Then AlarmStatus = "ACTIVE" Else AlarmStatus = "CLEAR"

            If RunSQL Then
                Dim CurrentDT As DateTime = Now
                SaveAlarmData(GetNow(), AlarmStatus, AlarmDesc, "Steam")
            End If
        End If

        'Re-count Current Steam Alarms
        Steam_CNT = 0
        For i = 0 To 31
            Dim Sobj As System.Object = SteamAlarmPanel.Controls("SteamAlarmLight" & i)
            If Sobj.selectcolor2 = True Then Steam_CNT += 1
        Next
        AlarmDispUpdate()
    End Sub
    Private Sub Water_Alarm_Changed(sender As System.Object, e As EventArgs) Handles WaterAlarmLight0.ValueSelectColor2Changed,
            WaterAlarmLight1.ValueSelectColor2Changed, WaterAlarmLight2.ValueSelectColor2Changed, WaterAlarmLight3.ValueSelectColor2Changed, WaterAlarmLight4.ValueSelectColor2Changed,
            WaterAlarmLight5.ValueSelectColor2Changed, WaterAlarmLight6.ValueSelectColor2Changed, WaterAlarmLight7.ValueSelectColor2Changed, WaterAlarmLight8.ValueSelectColor2Changed,
            WaterAlarmLight9.ValueSelectColor2Changed, WaterAlarmLight10.ValueSelectColor2Changed, WaterAlarmLight11.ValueSelectColor2Changed, WaterAlarmLight12.ValueSelectColor2Changed,
            WaterAlarmLight13.ValueSelectColor2Changed, WaterAlarmLight14.ValueSelectColor2Changed, WaterAlarmLight15.ValueSelectColor2Changed, WaterAlarmLight16.ValueSelectColor2Changed,
            WaterAlarmLight17.ValueSelectColor2Changed, WaterAlarmLight18.ValueSelectColor2Changed, WaterAlarmLight19.ValueSelectColor2Changed, WaterAlarmLight20.ValueSelectColor2Changed,
            WaterAlarmLight21.ValueSelectColor2Changed, WaterAlarmLight22.ValueSelectColor2Changed, WaterAlarmLight23.ValueSelectColor2Changed, WaterAlarmLight24.ValueSelectColor2Changed,
            WaterAlarmLight25.ValueSelectColor2Changed, WaterAlarmLight26.ValueSelectColor2Changed, WaterAlarmLight27.ValueSelectColor2Changed, WaterAlarmLight28.ValueSelectColor2Changed,
            WaterAlarmLight29.ValueSelectColor2Changed, WaterAlarmLight30.ValueSelectColor2Changed, WaterAlarmLight31.ValueSelectColor2Changed

        If Firsttick Then
            Dim AlarmDesc As String = WaterAlarmPanel.Controls("WaterAlarmDesc" & GetNumeric(sender.name)).Text
            Dim AlarmStatus As String
            If sender.selectcolor2 = True Then AlarmStatus = "ACTIVE" Else AlarmStatus = "CLEAR"
            If RunSQL Then
                Dim CurrentDT As DateTime = Now
                SaveAlarmData(GetNow(), AlarmStatus, AlarmDesc, "Water")
            End If
        End If
        Water_CNT = 0
        For i = 0 To 31
            Dim Wobj As System.Object = WaterAlarmPanel.Controls("WaterAlarmLight" & i)
            If Wobj.selectcolor2 = True Then Water_CNT += 1
        Next
        AlarmDispUpdate()

    End Sub

    Private Function AlarmDispUpdate()
        Dim System_CNT As Integer = 0
        If C_Als Then System_CNT += 1
        If L_Als Then System_CNT += 1
        Dim A_CNT As Integer = Water_CNT + Air_CNT + Steam_CNT + Elec_CNT + System_CNT
        If A_CNT > 0 Then
            Dash_ALM_Indicator.Font = New Font(Dash_ALM_Indicator.Font.FontFamily,
                                               (36), Dash_ALM_Indicator.Font.Style)
            Dash_ALM_Indicator.Text = A_CNT & vbNewLine & " ACTIVE-ALARMS"
            Return 1
        Else
            Dash_ALM_Indicator.Font = New Font(Dash_ALM_Indicator.Font.FontFamily,
                                               (52), Dash_ALM_Indicator.Font.Style)
            Dash_ALM_Indicator.Text = "System" & vbNewLine & "Normal"
            Return 0
        End If
    End Function

    Sub UpdateWaterAlarmDesc()
        For i = 1 To 32
            Dim section As String = "WR" & i
            WaterAlarmDisp1.Messages(i - 1).BackColor = Color.FromArgb(INI_GetKey_Alarm(section, "BACK_COL"))
            WaterAlarmDisp1.Messages(i - 1).ForeColor = Color.FromArgb(INI_GetKey_Alarm(section, "FORE_COL"))
            WaterAlarmDisp1.Messages(i - 1).BitNumber = INI_GetKey_Alarm(section, "BIT_NUM")
            WaterAlarmDisp1.Messages(i - 1).Message = INI_GetKey_Alarm(section, "MESSAGE")

            With Me.Controls("Alarms_Page").Controls("WaterAlarmPanel").Controls("WaterAlarmDesc" & i - 1)
                If INI_GetKey_Alarm(section, "MESSAGE") = "Undefined Message" Then
                    .BackColor = Color.Black
                    .ForeColor = Color.DimGray
                    .Text = "-"
                Else
                    .BackColor = Color.FromArgb(INI_GetKey_Alarm(section, "BACK_COL"))
                    .ForeColor = Color.FromArgb(INI_GetKey_Alarm(section, "FORE_COL"))
                    .Text = INI_GetKey_Alarm(section, "MESSAGE")
                End If
            End With
        Next
    End Sub
    Dim processCount As Integer = 0
    Sub UpdateElecAlarmDesc()
        For i = 1 To 32
            processCount += 1
            StartupStatusLabel.Text = "Getting Alarm Details..." & processCount & "/128"
            Dim section As String = "EL" & i
            ElecAlarmDisp1.Messages(i - 1).BackColor = Color.FromArgb(INI_GetKey_Alarm(section, "BACK_COL"))
            ElecAlarmDisp1.Messages(i - 1).ForeColor = Color.FromArgb(INI_GetKey_Alarm(section, "FORE_COL"))
            ElecAlarmDisp1.Messages(i - 1).BitNumber = INI_GetKey_Alarm(section, "BIT_NUM")
            ElecAlarmDisp1.Messages(i - 1).Message = INI_GetKey_Alarm(section, "MESSAGE")

            With Me.Controls("Alarms_Page").Controls("ElectricalAlarmPanel").Controls("ElecAlarmDesc" & i - 1)
                If INI_GetKey_Alarm(section, "MESSAGE") = "Undefined Message" Then
                    .BackColor = Color.Black
                    .ForeColor = Color.DimGray
                    .Text = "-"
                Else
                    .BackColor = Color.FromArgb(INI_GetKey_Alarm(section, "BACK_COL"))
                    .ForeColor = Color.FromArgb(INI_GetKey_Alarm(section, "FORE_COL"))
                    .Text = INI_GetKey_Alarm(section, "MESSAGE")
                End If
            End With
        Next
    End Sub
    Sub UpdateSteamAlarmDesc()
        For i = 1 To 32
            processCount += 1
            StartupStatusLabel.Text = "Getting Alarm Details..." & processCount & "/128"
            Dim section As String = "ST" & i
            SteamAlarmDisp1.Messages(i - 1).BackColor = Color.FromArgb(INI_GetKey_Alarm(section, "BACK_COL"))
            SteamAlarmDisp1.Messages(i - 1).ForeColor = Color.FromArgb(INI_GetKey_Alarm(section, "FORE_COL"))
            SteamAlarmDisp1.Messages(i - 1).BitNumber = INI_GetKey_Alarm(section, "BIT_NUM")
            SteamAlarmDisp1.Messages(i - 1).Message = INI_GetKey_Alarm(section, "MESSAGE")

            With Me.Controls("Alarms_Page").Controls("SteamAlarmPanel").Controls("SteamAlarmDesc" & i - 1)
                If INI_GetKey_Alarm(section, "MESSAGE") = "Undefined Message" Then
                    .BackColor = Color.Black
                    .ForeColor = Color.DimGray
                    .Text = "-"
                Else
                    .BackColor = Color.FromArgb(INI_GetKey_Alarm(section, "BACK_COL"))
                    .ForeColor = Color.FromArgb(INI_GetKey_Alarm(section, "FORE_COL"))
                    .Text = INI_GetKey_Alarm(section, "MESSAGE")
                End If
            End With
        Next
    End Sub
    Sub UpdateAirAlarmDesc()
        For i = 1 To 32
            processCount += 1
            StartupStatusLabel.Text = "Getting Alarm Details..." & processCount & "/128"
            Dim section As String = "AR" & i
            AirAlarmDisp1.Messages(i - 1).BackColor = Color.FromArgb(INI_GetKey_Alarm(section, "BACK_COL"))
            AirAlarmDisp1.Messages(i - 1).ForeColor = Color.FromArgb(INI_GetKey_Alarm(section, "FORE_COL"))
            AirAlarmDisp1.Messages(i - 1).BitNumber = INI_GetKey_Alarm(section, "BIT_NUM")
            AirAlarmDisp1.Messages(i - 1).Message = INI_GetKey_Alarm(section, "MESSAGE")

            With Me.Controls("Alarms_Page").Controls("AirAlarmPanel").Controls("AirAlarmDesc" & i - 1)
                If INI_GetKey_Alarm(section, "MESSAGE") = "Undefined Message" Then
                    .BackColor = Color.Black
                    .ForeColor = Color.DimGray
                    .Text = "-"
                Else
                    .BackColor = Color.FromArgb(INI_GetKey_Alarm(section, "BACK_COL"))
                    .ForeColor = Color.FromArgb(INI_GetKey_Alarm(section, "FORE_COL"))
                    .Text = INI_GetKey_Alarm(section, "MESSAGE")
                End If
            End With
        Next
    End Sub

    Private Sub AlarmStateCheck()

        If HeartBeatError = False Then
            C_Als = False
            PLCAlarmIcon1.Image = My.Resources.StatusPLCGood
            PLCAlarmIcon1.BackgroundImage = My.Resources.GOODBG
        Else
            C_Als = True
            PLCAlarmIcon1.Image = My.Resources.StatusPLCBad
            PLCAlarmIcon1.BackgroundImage = My.Resources.BADBG
        End If

        If RunSQL = True Then
            L_Als = False
            LogAlarmIcon1.Image = My.Resources.LogFileGood
            LogAlarmIcon1.BackgroundImage = My.Resources.GOODBG
        Else
            L_Als = True
            LogAlarmIcon1.Image = My.Resources.LogFileBad
            LogAlarmIcon1.BackgroundImage = My.Resources.BADBG
        End If

        If WaterAlarmDisp1.Value > 0 Then
            W_Als = True
            WaterAlarmIcon1.Image = My.Resources.StatusWaterBad
            WaterAlarmIcon1.BackgroundImage = My.Resources.BADBG
        Else
            W_Als = False
            WaterAlarmIcon1.Image = My.Resources.StatusWaterGood
            WaterAlarmIcon1.BackgroundImage = My.Resources.GOODBG
        End If

        If AirAlarmDisp1.Value > 0 Then
            A_Als = True
            AirAlarmIcon1.Image = My.Resources.StatusAirBad
            AirAlarmIcon1.BackgroundImage = My.Resources.BADBG
        Else
            A_Als = False
            AirAlarmIcon1.Image = My.Resources.StatusAirGood
            AirAlarmIcon1.BackgroundImage = My.Resources.GOODBG
        End If

        If ElecAlarmDisp1.Value > 0 Then
            E_Als = True
            ElecAlarmIcon1.Image = My.Resources.StatusElecBad
            ElecAlarmIcon1.BackgroundImage = My.Resources.BADBG
        Else
            E_Als = False
            ElecAlarmIcon1.Image = My.Resources.StatusElecGood
            ElecAlarmIcon1.BackgroundImage = My.Resources.GOODBG
        End If


        If SteamAlarmDisp1.Value > 0 Then
            S_Als = True
            SteamAlarmIcon1.Image = My.Resources.StatusSteamBad
            SteamAlarmIcon1.BackgroundImage = My.Resources.BADBG
        Else
            S_Als = False
            SteamAlarmIcon1.Image = My.Resources.StatusSteamGood
            SteamAlarmIcon1.BackgroundImage = My.Resources.GOODBG
        End If


        If E_Als Or A_Als Or S_Als Or W_Als Or L_Als Or C_Als Then
            Dash_ALM_Indicator.BackgroundImage = My.Resources.BADBG
            BypassAllButton.BackColor = Color.Red
            BasicIndicator116.BackColor = Color.Yellow
        Else
            Dash_ALM_Indicator.BackgroundImage = My.Resources.GOODBG
            BypassAllButton.BackColor = Color.FromArgb(56, 56, 56)
            BasicIndicator116.BackColor = Color.FromArgb(56, 56, 56)
        End If

        If StatusBanner.Visible = True Then
            If StatusBanner.SelectColor2 = True Then
                StatusBanner.Text = "ALARMS BYPASSED"
                StatusBanner.ForeColor = Color.White
                StatusBanner.OutlineColor = Color.FromArgb(128, 0, 0)
                Dash_ALM_Indicator.OutlineColor = Color.FromArgb(128, 0, 0)
            ElseIf StatusBanner.SelectColor3 = True Then
                StatusBanner.Text = "ALARMS SILENCED"
                StatusBanner.ForeColor = Color.Black
                StatusBanner.OutlineColor = Color.FromArgb(128, 128, 0)
                Dash_ALM_Indicator.OutlineColor = Color.FromArgb(128, 128, 0)
            Else
                StatusBanner.Text = "POINTS BYPASSED"
                StatusBanner.ForeColor = Color.DarkGray
                StatusBanner.OutlineColor = Color.FromArgb(0, 0, 255)
                Dash_ALM_Indicator.OutlineColor = Color.FromArgb(0, 0, 255)
            End If
        Else
            StatusBanner.Text = ""
            StatusBanner.OutlineColor = Color.FromArgb(32, 32, 32)
            Dash_ALM_Indicator.OutlineColor = Color.FromArgb(0, 128, 0)
        End If
    End Sub
    Private Sub DataGridView1_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles AlarmGrid.CellFormatting
        If AlarmGrid.Columns(e.ColumnIndex).Name = "State" AndAlso e.Value IsNot Nothing Then
            Dim row As DataGridViewRow = AlarmGrid.Rows(e.RowIndex)
            Select Case e.Value.ToString()
                Case "ACTIVE"
                    row.DefaultCellStyle.BackColor = Color.DarkRed
                Case "CLEAR"
                    row.DefaultCellStyle.BackColor = Color.DarkGreen
                Case Else
                    row.DefaultCellStyle.BackColor = Color.FromArgb(10, 10, 10)
            End Select
        End If
    End Sub
    Private Sub ShowAlarmHistoryPanel(sender As Object, e As EventArgs) Handles AlarmHistoryButton.Click
        If sender.text = "HISTORY" Then

            'ElecAL_CB.Items.Clear()
            'For Each desc As AdvancedHMI.Controls.MessageByBit In ElecAlarmDisp1.Messages
            '    If desc.Message = "Undefined Message" Then Else Dim i = ElecAL_CB.Items.Add(desc.Message)
            'Next
            'AirAL_CB.Items.Clear()
            'For Each desc As AdvancedHMI.Controls.MessageByBit In AirAlarmDisp1.Messages
            '    If desc.Message = "Undefined Message" Then Else Dim i = AirAL_CB.Items.Add(desc.Message)
            'Next
            'SteamAL_CB.Items.Clear()
            'For Each desc As AdvancedHMI.Controls.MessageByBit In SteamAlarmDisp1.Messages
            '    If desc.Message = "Undefined Message" Then Else Dim i = SteamAL_CB.Items.Add(desc.Message)
            'Next
            'WaterAL_CB.Items.Clear()
            'For Each desc As AdvancedHMI.Controls.MessageByBit In WaterAlarmDisp1.Messages
            '    If desc.Message = "Undefined Message" Then Else Dim i = WaterAL_CB.Items.Add(desc.Message)
            'Next

            EditAlarmDescPanel.Hide()
            AlarmHistoryPanel.Show()
            AirAlarmPanel.Hide()
            WaterAlarmPanel.Hide()
            SteamAlarmPanel.Hide()
            ElectricalAlarmPanel.Hide()
            sender.text = "ALARM VIEW"
        Else
            EditAlarmDescPanel.Hide()
            AlarmHistoryPanel.Hide()
            AirAlarmPanel.Show()
            WaterAlarmPanel.Show()
            SteamAlarmPanel.Show()
            ElectricalAlarmPanel.Show()
            sender.text = "HISTORY"
        End If





    End Sub

    Private Sub ElecFalcAlarmMask_Changed(sender As System.Object, e As EventArgs) Handles ElecFalcAlarm0.Click, ElecFalcAlarm1.Click, ElecFalcAlarm2.Click, ElecFalcAlarm3.Click, ElecFalcAlarm4.Click, ElecFalcAlarm5.Click,
        ElecFalcAlarm6.Click, ElecFalcAlarm7.Click, ElecFalcAlarm8.Click, ElecFalcAlarm9.Click, ElecFalcAlarm10.Click,
        ElecFalcAlarm11.Click, ElecFalcAlarm12.Click, ElecFalcAlarm13.Click, ElecFalcAlarm14.Click, ElecFalcAlarm15.Click,
        ElecFalcAlarm16.Click, ElecFalcAlarm17.Click, ElecFalcAlarm18.Click, ElecFalcAlarm19.Click, ElecFalcAlarm20.Click,
        ElecFalcAlarm21.Click, ElecFalcAlarm22.Click, ElecFalcAlarm23.Click, ElecFalcAlarm24.Click, ElecFalcAlarm25.Click,
        ElecFalcAlarm26.Click, ElecFalcAlarm27.Click, ElecFalcAlarm28.Click, ElecFalcAlarm29.Click, ElecFalcAlarm30.Click,
        ElecFalcAlarm31.Click
        If CurrentUserLevel = 3 Then
            Dim ElecFalcWord As Integer = 8
            Dim ElecFalcIndex As Integer = GetNumeric(sender.name)
            If ElecFalcIndex > 15 Then ElecFalcIndex -= 15 & ElecFalcWord = 9
            Dim PLCAddress As String = "N14:" & ElecFalcWord & "/" & ElecFalcIndex
            If PLC_Fast.Read(PLCAddress) Then
                PLC_Fast.Write(PLCAddress, "0")
                ElectricalAlarmPanel.Controls(sender.name).BackgroundImage = My.Resources.AlarmFalconOff
                If RunSQL Then SaveSystemData(GetNow(), "Electrical Alarm #" & GetNumeric(sender.name) & " Falcon Alarm set to: False", CurrentUser)
            Else
                PLC_Fast.Write(PLCAddress, "1")
                ElectricalAlarmPanel.Controls(sender.name).BackgroundImage = My.Resources.AlarmFalconOn
                If RunSQL Then SaveSystemData(GetNow(), "Electrical Alarm #" & GetNumeric(sender.name) & " Falcon Alarm set to: True", CurrentUser)
            End If
        Else

            ShowLoginScreen()
        End If
    End Sub
    Private Sub ElecAudAlarmMask_Changed(sender As System.Object, e As EventArgs) Handles ElecAudAlarm0.Click, ElecAudAlarm1.Click, ElecAudAlarm2.Click, ElecAudAlarm3.Click, ElecAudAlarm4.Click, ElecAudAlarm5.Click,
        ElecAudAlarm6.Click, ElecAudAlarm7.Click, ElecAudAlarm8.Click, ElecAudAlarm9.Click, ElecAudAlarm10.Click,
        ElecAudAlarm11.Click, ElecAudAlarm12.Click, ElecAudAlarm13.Click, ElecAudAlarm14.Click, ElecAudAlarm15.Click,
        ElecAudAlarm16.Click, ElecAudAlarm17.Click, ElecAudAlarm18.Click, ElecAudAlarm19.Click, ElecAudAlarm20.Click,
        ElecAudAlarm21.Click, ElecAudAlarm22.Click, ElecAudAlarm23.Click, ElecAudAlarm24.Click, ElecAudAlarm25.Click,
        ElecAudAlarm26.Click, ElecAudAlarm27.Click, ElecAudAlarm28.Click, ElecAudAlarm29.Click, ElecAudAlarm30.Click,
        ElecAudAlarm31.Click
        If CurrentUserLevel = 3 Then

            Dim ElecAudWord As Integer = 6
            Dim ElecAudIndex As Integer = GetNumeric(sender.name)
            Dim PLCAddress As String = "N14:" & ElecAudWord & "/" & ElecAudIndex
            If PLC_Fast.Read(PLCAddress) Then
                PLC_Fast.Write(PLCAddress, "0")
                ElectricalAlarmPanel.Controls(sender.name).BackgroundImage = My.Resources.AlarmAudibleOff
                If RunSQL Then SaveSystemData(GetNow(), "Electrical Alarm #" & GetNumeric(sender.name) & " Audible Alarm set to: False", CurrentUser)
            Else
                PLC_Fast.Write(PLCAddress, "1")
                ElectricalAlarmPanel.Controls(sender.name).BackgroundImage = My.Resources.AlarmAudibleOn
                If RunSQL Then SaveSystemData(GetNow(), "Electrical Alarm #" & GetNumeric(sender.name) & " Audible Alarm set to: True", CurrentUser)
            End If
        Else
            ShowLoginScreen()
        End If


    End Sub

    Private Sub AirFalcAlarmMask_Changed(sender As System.Object, e As EventArgs) Handles AirFalcAlarm0.Click, AirFalcAlarm1.Click, AirFalcAlarm2.Click, AirFalcAlarm3.Click, AirFalcAlarm4.Click, AirFalcAlarm5.Click,
        AirFalcAlarm6.Click, AirFalcAlarm7.Click, AirFalcAlarm8.Click, AirFalcAlarm9.Click, AirFalcAlarm10.Click,
        AirFalcAlarm11.Click, AirFalcAlarm12.Click, AirFalcAlarm13.Click, AirFalcAlarm14.Click, AirFalcAlarm15.Click,
        AirFalcAlarm16.Click, AirFalcAlarm17.Click, AirFalcAlarm18.Click, AirFalcAlarm19.Click, AirFalcAlarm20.Click,
        AirFalcAlarm21.Click, AirFalcAlarm22.Click, AirFalcAlarm23.Click, AirFalcAlarm24.Click, AirFalcAlarm25.Click,
        AirFalcAlarm26.Click, AirFalcAlarm27.Click, AirFalcAlarm28.Click, AirFalcAlarm29.Click, AirFalcAlarm30.Click,
        AirFalcAlarm31.Click
        If CurrentUserLevel = 3 Then
            Dim AirFalcWord As Integer = 18
            Dim AirFalcIndex As Integer = GetNumeric(sender.name)

            Dim PLCAddress As String = "N14:" & AirFalcWord & "/" & AirFalcIndex
            If PLC_Fast.Read(PLCAddress) Then
                PLC_Fast.Write(PLCAddress, "0")
                AirAlarmPanel.Controls(sender.name).BackgroundImage = My.Resources.AlarmFalconOff
                If RunSQL Then SaveSystemData(GetNow(), "Air Alarm #" & GetNumeric(sender.name) & " Falcon Alarm set to: False", CurrentUser)
            Else
                PLC_Fast.Write(PLCAddress, "1")
                AirAlarmPanel.Controls(sender.name).BackgroundImage = My.Resources.AlarmFalconOn
                If RunSQL Then SaveSystemData(GetNow(), "Air Alarm #" & GetNumeric(sender.name) & " Falcon Alarm set to: True", CurrentUser)
            End If
        Else
            ShowLoginScreen()
        End If
    End Sub
    Private Sub AirAudAlarmMask_Changed(sender As System.Object, e As EventArgs) Handles AirAudAlarm0.Click, AirAudAlarm1.Click, AirAudAlarm2.Click, AirAudAlarm3.Click, AirAudAlarm4.Click, AirAudAlarm5.Click,
        AirAudAlarm6.Click, AirAudAlarm7.Click, AirAudAlarm8.Click, AirAudAlarm9.Click, AirAudAlarm10.Click,
        AirAudAlarm11.Click, AirAudAlarm12.Click, AirAudAlarm13.Click, AirAudAlarm14.Click, AirAudAlarm15.Click,
        AirAudAlarm16.Click, AirAudAlarm17.Click, AirAudAlarm18.Click, AirAudAlarm19.Click, AirAudAlarm20.Click,
        AirAudAlarm21.Click, AirAudAlarm22.Click, AirAudAlarm23.Click, AirAudAlarm24.Click, AirAudAlarm25.Click,
        AirAudAlarm26.Click, AirAudAlarm27.Click, AirAudAlarm28.Click, AirAudAlarm29.Click, AirAudAlarm30.Click,
        AirAudAlarm31.Click
        If CurrentUserLevel = 3 Then
            Dim AirAudWord As Integer = 16
            Dim AirAudIndex As Integer = GetNumeric(sender.name)

            Dim PLCAddress As String = "N14:" & AirAudWord & "/" & AirAudIndex
            If PLC_Fast.Read(PLCAddress) Then
                PLC_Fast.Write(PLCAddress, "0")
                AirAlarmPanel.Controls(sender.name).BackgroundImage = My.Resources.AlarmAudibleOff
                If RunSQL Then SaveSystemData(GetNow(), "Air Alarm #" & GetNumeric(sender.name) & " Audible Alarm set to: False", CurrentUser)
            Else
                PLC_Fast.Write(PLCAddress, "1")
                AirAlarmPanel.Controls(sender.name).BackgroundImage = My.Resources.AlarmAudibleOn
                If RunSQL Then SaveSystemData(GetNow(), "Air Alarm #" & GetNumeric(sender.name) & " Audible Alarm set to: True", CurrentUser)
            End If
        Else
            ShowLoginScreen()
        End If
    End Sub
    Private Sub WaterFalcAlarmMask_Changed(sender As System.Object, e As EventArgs) Handles WaterFalcAlarm0.Click, WaterFalcAlarm1.Click, WaterFalcAlarm2.Click, WaterFalcAlarm3.Click, WaterFalcAlarm4.Click, WaterFalcAlarm5.Click,
        WaterFalcAlarm6.Click, WaterFalcAlarm7.Click, WaterFalcAlarm8.Click, WaterFalcAlarm9.Click, WaterFalcAlarm10.Click,
        WaterFalcAlarm11.Click, WaterFalcAlarm12.Click, WaterFalcAlarm13.Click, WaterFalcAlarm14.Click, WaterFalcAlarm15.Click,
        WaterFalcAlarm16.Click, WaterFalcAlarm17.Click, WaterFalcAlarm18.Click, WaterFalcAlarm19.Click, WaterFalcAlarm20.Click,
        WaterFalcAlarm21.Click, WaterFalcAlarm22.Click, WaterFalcAlarm23.Click, WaterFalcAlarm24.Click, WaterFalcAlarm25.Click,
        WaterFalcAlarm26.Click, WaterFalcAlarm27.Click, WaterFalcAlarm28.Click, WaterFalcAlarm29.Click, WaterFalcAlarm30.Click,
        WaterFalcAlarm31.Click
        If CurrentUserLevel = 3 Then
            Dim WaterFalcWord As Integer = 38
            Dim WaterFalcIndex As Integer = GetNumeric(sender.name)

            Dim PLCAddress As String = "N14:" & WaterFalcWord & "/" & WaterFalcIndex
            If PLC_Fast.Read(PLCAddress) Then
                PLC_Fast.Write(PLCAddress, "0")
                WaterAlarmPanel.Controls(sender.name).BackgroundImage = My.Resources.AlarmFalconOff
                If RunSQL Then SaveSystemData(GetNow(), "Water Alarm #" & GetNumeric(sender.name) & " Falcon Alarm set to: False", CurrentUser)
            Else
                PLC_Fast.Write(PLCAddress, "1")
                WaterAlarmPanel.Controls(sender.name).BackgroundImage = My.Resources.AlarmFalconOn
                If RunSQL Then SaveSystemData(GetNow(), "Water Alarm #" & GetNumeric(sender.name) & " Falcon Alarm set to: True", CurrentUser)
            End If
        Else
            ShowLoginScreen()
        End If
    End Sub
    Private Sub WaterAudAlarmMask_Changed(sender As System.Object, e As EventArgs) Handles WaterAudAlarm0.Click, WaterAudAlarm1.Click, WaterAudAlarm2.Click, WaterAudAlarm3.Click, WaterAudAlarm4.Click, WaterAudAlarm5.Click,
        WaterAudAlarm6.Click, WaterAudAlarm7.Click, WaterAudAlarm8.Click, WaterAudAlarm9.Click, WaterAudAlarm10.Click,
        WaterAudAlarm11.Click, WaterAudAlarm12.Click, WaterAudAlarm13.Click, WaterAudAlarm14.Click, WaterAudAlarm15.Click,
        WaterAudAlarm16.Click, WaterAudAlarm17.Click, WaterAudAlarm18.Click, WaterAudAlarm19.Click, WaterAudAlarm20.Click,
        WaterAudAlarm21.Click, WaterAudAlarm22.Click, WaterAudAlarm23.Click, WaterAudAlarm24.Click, WaterAudAlarm25.Click,
        WaterAudAlarm26.Click, WaterAudAlarm27.Click, WaterAudAlarm28.Click, WaterAudAlarm29.Click, WaterAudAlarm30.Click,
        WaterAudAlarm31.Click
        If CurrentUserLevel = 3 Then
            Dim WaterAudWord As Integer = 36
            Dim WaterAudIndex As Integer = GetNumeric(sender.name)

            Dim PLCAddress As String = "N14:" & WaterAudWord & "/" & WaterAudIndex
            If PLC_Fast.Read(PLCAddress) Then
                PLC_Fast.Write(PLCAddress, "0")
                WaterAlarmPanel.Controls(sender.name).BackgroundImage = My.Resources.AlarmAudibleOff
                If RunSQL Then SaveSystemData(GetNow(), "Water Alarm #" & GetNumeric(sender.name) & " Audible Alarm set to: False", CurrentUser)
            Else
                PLC_Fast.Write(PLCAddress, "1")
                WaterAlarmPanel.Controls(sender.name).BackgroundImage = My.Resources.AlarmAudibleOn
                If RunSQL Then SaveSystemData(GetNow(), "Water Alarm #" & GetNumeric(sender.name) & " Audible Alarm set to: True", CurrentUser)
            End If
        Else
            ShowLoginScreen()
        End If
    End Sub
    Private Sub SteamFalcAlarmMask_Changed(sender As System.Object, e As EventArgs) Handles SteamFalcAlarm0.Click, SteamFalcAlarm1.Click, SteamFalcAlarm2.Click, SteamFalcAlarm3.Click, SteamFalcAlarm4.Click, SteamFalcAlarm5.Click,
        SteamFalcAlarm6.Click, SteamFalcAlarm7.Click, SteamFalcAlarm8.Click, SteamFalcAlarm9.Click, SteamFalcAlarm10.Click,
        SteamFalcAlarm11.Click, SteamFalcAlarm12.Click, SteamFalcAlarm13.Click, SteamFalcAlarm14.Click, SteamFalcAlarm15.Click,
        SteamFalcAlarm16.Click, SteamFalcAlarm17.Click, SteamFalcAlarm18.Click, SteamFalcAlarm19.Click, SteamFalcAlarm20.Click,
        SteamFalcAlarm21.Click, SteamFalcAlarm22.Click, SteamFalcAlarm23.Click, SteamFalcAlarm24.Click, SteamFalcAlarm25.Click,
        SteamFalcAlarm26.Click, SteamFalcAlarm27.Click, SteamFalcAlarm28.Click, SteamFalcAlarm29.Click, SteamFalcAlarm30.Click,
        SteamFalcAlarm31.Click
        If CurrentUserLevel = 3 Then
            Dim SteamFalcWord As Integer = 28
            Dim SteamFalcIndex As Integer = GetNumeric(sender.name)

            Dim PLCAddress As String = "N14:" & SteamFalcWord & "/" & SteamFalcIndex
            If PLC_Fast.Read(PLCAddress) Then
                PLC_Fast.Write(PLCAddress, "0")
                SteamAlarmPanel.Controls(sender.name).BackgroundImage = My.Resources.AlarmFalconOff
                If RunSQL Then SaveSystemData(GetNow(), "Steam Alarm #" & GetNumeric(sender.name) & " Falcon Alarm set to: False", CurrentUser)
            Else
                PLC_Fast.Write(PLCAddress, "1")
                SteamAlarmPanel.Controls(sender.name).BackgroundImage = My.Resources.AlarmFalconOn
                If RunSQL Then SaveSystemData(GetNow(), "Steam Alarm #" & GetNumeric(sender.name) & " Falcon Alarm set to: True", CurrentUser)
            End If
        Else
            ShowLoginScreen()
        End If
    End Sub
    Private Sub SteamAudAlarmMask_Changed(sender As System.Object, e As EventArgs) Handles SteamAudAlarm0.Click, SteamAudAlarm1.Click, SteamAudAlarm2.Click, SteamAudAlarm3.Click, SteamAudAlarm4.Click, SteamAudAlarm5.Click,
        SteamAudAlarm6.Click, SteamAudAlarm7.Click, SteamAudAlarm8.Click, SteamAudAlarm9.Click, SteamAudAlarm10.Click,
        SteamAudAlarm11.Click, SteamAudAlarm12.Click, SteamAudAlarm13.Click, SteamAudAlarm14.Click, SteamAudAlarm15.Click,
        SteamAudAlarm16.Click, SteamAudAlarm17.Click, SteamAudAlarm18.Click, SteamAudAlarm19.Click, SteamAudAlarm20.Click,
        SteamAudAlarm21.Click, SteamAudAlarm22.Click, SteamAudAlarm23.Click, SteamAudAlarm24.Click, SteamAudAlarm25.Click,
        SteamAudAlarm26.Click, SteamAudAlarm27.Click, SteamAudAlarm28.Click, SteamAudAlarm29.Click, SteamAudAlarm30.Click,
        SteamAudAlarm31.Click
        If CurrentUserLevel = 3 Then
            Dim SteamAudWord As Integer = 26
            Dim SteamAudIndex As Integer = GetNumeric(sender.name)

            Dim PLCAddress As String = "N14:" & SteamAudWord & "/" & SteamAudIndex
            If PLC_Fast.Read(PLCAddress) Then
                PLC_Fast.Write(PLCAddress, "0")
                SteamAlarmPanel.Controls(sender.name).BackgroundImage = My.Resources.AlarmAudibleOff
                If RunSQL Then SaveSystemData(GetNow(), "Steam Alarm #" & GetNumeric(sender.name) & " Audible Alarm set to: False", CurrentUser)
            Else
                PLC_Fast.Write(PLCAddress, "1")
                SteamAlarmPanel.Controls(sender.name).BackgroundImage = My.Resources.AlarmAudibleOn
                If RunSQL Then SaveSystemData(GetNow(), "Steam Alarm #" & GetNumeric(sender.name) & " Audible Alarm set to: True", CurrentUser)
            End If
        Else
            ShowLoginScreen()
        End If
    End Sub
    '║                                                                                                              ║
    '╠══════════════════════════════════════════════════════════════════════════════════════════════════════════════╣
    '║                                                [ALARM LOGIC]                                                 ║
    '║                                                 Section End                                                  ║
    '╚══════════════════════════════════════════════════════════════════════════════════════════════════════════════╝
#End Region
#Region "IO Descriptions"
    '╔══════════════════════════════════════════════════════════════════════════════════════════════════════════════╗
    '║                                              [IO DESCRIPTIONS]                                               ║
    '║                                                Section Start                                                 ║
    '╠══════════════════════════════════════════════════════════════════════════════════════════════════════════════╣
    '║                                                                                                              ║


    ' Logic to Change IO Descriptions shown on Faults Screen
    Private Sub IODescription_Clicked(sender As Object, e As EventArgs) Handles IO2_0Desc.Click, IO2_1Desc.Click, IO2_2Desc.Click, IO2_3Desc.Click, IO2_4Desc.Click, IO2_5Desc.Click, IO2_6Desc.Click, IO2_7Desc.Click,
            IO3_0Desc.Click, IO3_1Desc.Click, IO3_2Desc.Click, IO3_3Desc.Click, IO3_4Desc.Click, IO3_5Desc.Click, IO3_6Desc.Click, IO3_7Desc.Click,
            IO4_0desc.Click, IO4_1Desc.Click, IO4_2Desc.Click, IO4_3Desc.Click, IO4_4Desc.Click, IO4_5Desc.Click, IO4_6Desc.Click, IO4_7Desc.Click,
            IO5_0Desc.Click, IO5_1Desc.Click, IO5_2Desc.Click, IO5_3Desc.Click,
            IO6_0Desc.Click, IO6_1Desc.Click, IO6_2Desc.Click, IO6_3Desc.Click, IO6_4Desc.Click, IO6_5Desc.Click, IO6_6Desc.Click, IO6_7Desc.Click,
            IO7_0Desc.Click, IO7_1Desc.Click, IO7_2Desc.Click, IO7_3Desc.Click, IO7_4Desc.Click, IO7_5Desc.Click, IO7_6Desc.Click, IO7_7Desc.Click,
            IO7_8Desc.Click, IO7_9Desc.Click, IO7_10Desc.Click, IO7_11Desc.Click, IO7_12Desc.Click, IO7_13Desc.Click, IO7_14Desc.Click, IO7_15Desc.Click,
            IO7_16Desc.Click, IO7_17Desc.Click, IO7_18Desc.Click, IO7_19Desc.Click, IO7_20Desc.Click, IO7_21Desc.Click, IO7_22Desc.Click, IO7_23Desc.Click,
            IO7_24Desc.Click, IO7_25Desc.Click, IO7_26Desc.Click, IO7_27Desc.Click, IO7_28Desc.Click, IO7_29Desc.Click, IO7_30Desc.Click, IO7_31Desc.Click,
            IO8_0Desc.Click, IO8_1Desc.Click, IO8_2Desc.Click, IO8_3Desc.Click, IO8_4Desc.Click, IO8_5Desc.Click, IO8_6Desc.Click, IO8_7Desc.Click
        If CurrentUserLevel = 3 Then
            Dim NewDesc As String = InputBox("Please Enter New Description", "IO Description", sender.text)
            If NewDesc IsNot Nothing Then

                Dim Card As String = "CARD" & GetNumeric(sender.name.Split("_")(0))
                Dim Point As String = "P" & GetNumeric(sender.name.Split("_")(1))
                INI_SetKey_IO(Card, Point, NewDesc)
                UpdateIODescriptionLabels()
            End If
        Else
            ShowLoginScreen()
        End If


    End Sub



    '║                                                                                                              ║
    '╠══════════════════════════════════════════════════════════════════════════════════════════════════════════════╣
    '║                                              [IO DESCRIPTIONS]                                               ║
    '║                                                 Section End                                                  ║
    '╚══════════════════════════════════════════════════════════════════════════════════════════════════════════════╝
#End Region
#Region "Log Chart Visibility"
    '╔══════════════════════════════════════════════════════════════════════════════════════════════════════════════╗
    '║                                            [LOG CHART VISIBILITY]                                            ║
    '║                                                Section Start                                                 ║
    '╠══════════════════════════════════════════════════════════════════════════════════════════════════════════════╣
    '║                                                                                                              ║
    Private Sub Button2_Click_1(sender As Object, e As EventArgs) Handles Button2.Click
        With Chart3.ChartAreas(0)
            If .Visible = True Then
                .Visible = False
                Button2.ForeColor = Color.Red
            Else
                .Visible = True
                Button2.ForeColor = Color.Green
            End If
        End With

    End Sub
    Private Sub Button3_Click_1(sender As Object, e As EventArgs) Handles Button3.Click
        With Chart3.ChartAreas(1)
            If .Visible = True Then
                .Visible = False
                Button3.ForeColor = Color.Red
            Else
                .Visible = True
                Button3.ForeColor = Color.Green
            End If
        End With
    End Sub
    Private Sub Button4_Click_1(sender As Object, e As EventArgs) Handles Button4.Click
        With Chart3.ChartAreas(2)
            If .Visible = True Then
                .Visible = False
                Button4.ForeColor = Color.Red
            Else
                .Visible = True
                Button4.ForeColor = Color.Green
            End If
        End With
    End Sub
    Private Sub Button5_Click_1(sender As Object, e As EventArgs) Handles Button5.Click
        With Chart3.ChartAreas(3)
            If .Visible = True Then
                .Visible = False
                Button5.ForeColor = Color.Red
            Else
                .Visible = True
                Button5.ForeColor = Color.Green
            End If
        End With
    End Sub

    '║                                                                                                              ║
    '╠══════════════════════════════════════════════════════════════════════════════════════════════════════════════╣
    '║                                            [LOG CHART VISIBILITY]                                            ║
    '║                                                 Section End                                                  ║
    '╚══════════════════════════════════════════════════════════════════════════════════════════════════════════════╝
#End Region
#Region "Analog Status"
    '╔══════════════════════════════════════════════════════════════════════════════════════════════════════════════╗
    '║                                               [ANALOG STATUS]                                                ║
    '║                                                Section Start                                                 ║
    '╠══════════════════════════════════════════════════════════════════════════════════════════════════════════════╣
    '║                                                                                                              ║
    Private Sub I_2_7_Status_Click(sender As Object, e As EventArgs) Handles I_2_7_Status.Click
        If CurrentUserLevel = "3" Then
            Dim answer As Integer

            If I_2_7_Status.SelectColor3 = True Then
                answer = MsgBox("I:2/7 is currently enabled. Would You like to disable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_2_7_Status.PLCAddressSelectColor3.ToString, "0")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:2/7 Disabled", CurrentUser)
                End If
            Else
                answer = MsgBox("I:2/7 is currently disabled. Would You like to enable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_2_7_Status.PLCAddressSelectColor3.ToString, "1")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:2/7 Enabled", CurrentUser)
                End If
            End If

        End If
    End Sub
    Private Sub I_2_6_Status_Click(sender As Object, e As EventArgs) Handles I_2_6_Status.Click
        If CurrentUserLevel = "3" Then
            Dim answer As Integer
            If I_2_6_Status.SelectColor3 = True Then
                answer = MsgBox("I:2/6 is currently enabled. Would You like to disable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_2_6_Status.PLCAddressSelectColor3.ToString, "0")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:2/6 Disabled", CurrentUser)
                End If
            Else
                answer = MsgBox("I:2/6 is currently disabled. Would You like to enable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_2_6_Status.PLCAddressSelectColor3.ToString, "1")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:2/6 Enabled", CurrentUser)
                End If
            End If

        End If
    End Sub
    Private Sub I_2_5_Status_Click(sender As Object, e As EventArgs) Handles I_2_5_Status.Click
        If CurrentUserLevel = "3" Then
            Dim answer As Integer
            If I_2_5_Status.SelectColor3 = True Then
                answer = MsgBox("I:2/5 is currently enabled. Would You like to disable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_2_5_Status.PLCAddressSelectColor3.ToString, "0")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:2/5 Disabled", CurrentUser)
                End If
            Else
                answer = MsgBox("I:2/5 is currently disabled. Would You like to enable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_2_5_Status.PLCAddressSelectColor3.ToString, "1")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:2/5 Enabled", CurrentUser)
                End If
            End If

        End If
    End Sub
    Private Sub BasicIndicator53_Click(sender As Object, e As EventArgs) Handles I_2_4_Status.Click
        If CurrentUserLevel = "3" Then
            Dim answer As Integer
            If I_2_4_Status.SelectColor3 = True Then
                answer = MsgBox("I:2/4 is currently enabled. Would You like to disable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_2_4_Status.PLCAddressSelectColor3.ToString, "0")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:2/4 Disabled", CurrentUser)
                End If
            Else
                answer = MsgBox("I:2/4 is currently disabled. Would You like to enable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_2_4_Status.PLCAddressSelectColor3.ToString, "1")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:2/4 Enabled", CurrentUser)
                End If
            End If

        End If
    End Sub
    Private Sub BasicIndicator63_Click(sender As Object, e As EventArgs) Handles I_2_3_Status.Click
        If CurrentUserLevel = "3" Then
            Dim answer As Integer
            If I_2_3_Status.SelectColor3 = True Then
                answer = MsgBox("I:2/3 is currently enabled. Would You like to disable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_2_3_Status.PLCAddressSelectColor3.ToString, "0")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:2/3 Disabled", CurrentUser)
                End If
            Else
                answer = MsgBox("I:2/3 is currently disabled. Would You like to enable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_2_3_Status.PLCAddressSelectColor3.ToString, "1")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:2/3 Enabled", CurrentUser)
                End If
            End If
        End If
    End Sub
    Private Sub BasicIndicator59_Click(sender As Object, e As EventArgs) Handles I_2_2_Status.Click
        If CurrentUserLevel = "3" Then
            Dim answer As Integer
            If I_2_2_Status.SelectColor3 = True Then
                answer = MsgBox("I:2/2 is currently enabled. Would You like to disable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_2_2_Status.PLCAddressSelectColor3.ToString, "0")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:2/2 Disabled", CurrentUser)
                End If
            Else
                answer = MsgBox("I:2/2 is currently disabled. Would You like to enable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_2_2_Status.PLCAddressSelectColor3.ToString, "1")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:2/2 Enabled", CurrentUser)
                End If
            End If

        End If
    End Sub
    Private Sub BasicIndicator55_Click(sender As Object, e As EventArgs) Handles I_2_1_Status.Click
        If CurrentUserLevel = "3" Then
            Dim answer As Integer
            If I_2_1_Status.SelectColor3 = True Then
                answer = MsgBox("I:2/1 is currently enabled. Would You like to disable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_2_1_Status.PLCAddressSelectColor3.ToString, "0")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:2/1 Disabled", CurrentUser)
                End If
            Else
                answer = MsgBox("I:2/1 is currently disabled. Would You like to enable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_2_1_Status.PLCAddressSelectColor3.ToString, "1")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:2/1 Enabled", CurrentUser)
                End If
            End If

        End If
    End Sub
    Private Sub BasicIndicator51_Click(sender As Object, e As EventArgs) Handles I_2_0_Status.Click
        If CurrentUserLevel = "3" Then
            Dim answer As Integer
            If I_2_0_Status.SelectColor3 = True Then
                answer = MsgBox("I:2/0 is currently enabled. Would You like to disable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_2_0_Status.PLCAddressSelectColor3.ToString, "0")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:2/0 Disabled", CurrentUser)
                End If
            Else
                answer = MsgBox("I:2/0 is currently disabled. Would You like to enable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_2_0_Status.PLCAddressSelectColor3.ToString, "1")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:2/0 Enabled", CurrentUser)
                End If
            End If

        End If
    End Sub
    Private Sub I_3_0_Status_Click(sender As Object, e As EventArgs) Handles I_3_0_Status.Click
        If CurrentUserLevel = "3" Then
            Dim answer As Integer
            If I_3_0_Status.SelectColor3 = True Then
                answer = MsgBox("I:3/0 is currently enabled. Would You like to disable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_3_0_Status.PLCAddressSelectColor3.ToString, "0")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:3/0 Disabled", CurrentUser)
                End If
            Else
                answer = MsgBox("I:3/0 is currently disabled. Would You like to enable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_3_0_Status.PLCAddressSelectColor3.ToString, "1")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:3/0 Enabled", CurrentUser)
                End If
            End If

        End If
    End Sub
    Private Sub I_3_1_Status_Click(sender As Object, e As EventArgs) Handles I_3_1_Status.Click
        If CurrentUserLevel = "3" Then
            Dim answer As Integer
            If I_3_1_Status.SelectColor3 = True Then
                answer = MsgBox("I:3/1 is currently enabled. Would You like to disable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_3_1_Status.PLCAddressSelectColor3.ToString, "0")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:3/1 Disabled", CurrentUser)
                End If
            Else
                answer = MsgBox("I:3/1 is currently disabled. Would You like to enable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_3_1_Status.PLCAddressSelectColor3.ToString, "1")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:3/1 Enabled", CurrentUser)
                End If
            End If

        End If
    End Sub
    Private Sub I_3_2_Status_Click(sender As Object, e As EventArgs) Handles I_3_2_Status.Click
        If CurrentUserLevel = "3" Then
            Dim answer As Integer
            If I_3_2_Status.SelectColor3 = True Then
                answer = MsgBox("I:3/2 is currently enabled. Would You like to disable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_3_2_Status.PLCAddressSelectColor3.ToString, "0")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:3/2 Disabled", CurrentUser)
                End If
            Else
                answer = MsgBox("I:3/2 is currently disabled. Would You like to enable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_3_2_Status.PLCAddressSelectColor3.ToString, "1")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:3/2 Disabled", CurrentUser)
                End If
            End If

        End If
    End Sub
    Private Sub I_3_3_Status_Click(sender As Object, e As EventArgs) Handles I_3_3_Status.Click
        If CurrentUserLevel = "3" Then
            Dim answer As Integer
            If I_3_3_Status.SelectColor3 = True Then
                answer = MsgBox("I:3/3 is currently enabled. Would You like to disable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_3_3_Status.PLCAddressSelectColor3.ToString, "0")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:3/3 Disabled", CurrentUser)
                End If
            Else
                answer = MsgBox("I:3/3 is currently disabled. Would You like to enable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_3_3_Status.PLCAddressSelectColor3.ToString, "1")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:3/3 Enabled", CurrentUser)
                End If
            End If

        End If
    End Sub
    Private Sub I_3_4_Status_Click(sender As Object, e As EventArgs) Handles I_3_4_Status.Click
        If CurrentUserLevel = "3" Then
            Dim answer As Integer
            If I_3_4_Status.SelectColor3 = True Then
                answer = MsgBox("I:3/4 is currently enabled. Would You like to disable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_3_4_Status.PLCAddressSelectColor3.ToString, "0")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:3/4 Disabled", CurrentUser)
                End If
            Else
                answer = MsgBox("I:3/4 is currently disabled. Would You like to enable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_3_4_Status.PLCAddressSelectColor3.ToString, "1")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:3/4 Enabled", CurrentUser)
                End If
            End If

        End If
    End Sub
    Private Sub I_3_5_Status_Click(sender As Object, e As EventArgs) Handles I_3_5_Status.Click
        If CurrentUserLevel = "3" Then
            Dim answer As Integer
            If I_3_5_Status.SelectColor3 = True Then
                answer = MsgBox("I:3/5 is currently enabled. Would You like to disable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_3_5_Status.PLCAddressSelectColor3.ToString, "0")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:3/5 Disabled", CurrentUser)
                End If
            Else
                answer = MsgBox("I:3/5 is currently disabled. Would You like to enable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_3_5_Status.PLCAddressSelectColor3.ToString, "1")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:3/5 Enabled", CurrentUser)
                End If
            End If

        End If
    End Sub
    Private Sub I_3_6_Status_Click(sender As Object, e As EventArgs) Handles I_3_6_Status.Click
        If CurrentUserLevel = "3" Then
            Dim answer As Integer
            If I_3_6_Status.SelectColor3 = True Then
                answer = MsgBox("I:3/6 is currently enabled. Would You like to disable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_3_6_Status.PLCAddressSelectColor3.ToString, "0")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:3/6 Disabled", CurrentUser)
                End If
            Else
                answer = MsgBox("I:3/6 is currently disabled. Would You like to enable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_3_6_Status.PLCAddressSelectColor3.ToString, "1")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:3/6 Enabled", CurrentUser)
                End If
            End If

        End If
    End Sub
    Private Sub I_3_7_Status_Click(sender As Object, e As EventArgs) Handles I_3_7_Status.Click
        If CurrentUserLevel = "3" Then
            Dim answer As Integer
            If I_3_7_Status.SelectColor3 = True Then
                answer = MsgBox("I:3/7 is currently enabled. Would You like to disable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_3_7_Status.PLCAddressSelectColor3.ToString, "0")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:3/7 Disabled", CurrentUser)
                End If
            Else
                answer = MsgBox("I:3/7 is currently disabled. Would You like to enable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_3_7_Status.PLCAddressSelectColor3.ToString, "1")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:3/7 Enabled", CurrentUser)
                End If
            End If

        End If
    End Sub
    Private Sub I_4_0_Status_Click(sender As Object, e As EventArgs) Handles I_4_0_Status.Click
        If CurrentUserLevel = "3" Then
            Dim answer As Integer
            If I_4_0_Status.SelectColor3 = True Then
                answer = MsgBox("I:4/0 is currently enabled. Would You like to disable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_4_0_Status.PLCAddressSelectColor3.ToString, "0")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:4/0 Disabled", CurrentUser)
                End If
            Else
                answer = MsgBox("I:4/0 is currently disabled. Would You like to enable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_4_0_Status.PLCAddressSelectColor3.ToString, "1")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:4/0 Enabled", CurrentUser)
                End If
            End If

        End If
    End Sub
    Private Sub I_4_1_Status_Click(sender As Object, e As EventArgs) Handles I_4_1_Status.Click
        If CurrentUserLevel = "3" Then
            Dim answer As Integer
            If I_4_1_Status.SelectColor3 = True Then
                answer = MsgBox("I:4/1 is currently enabled. Would You like to disable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_4_1_Status.PLCAddressSelectColor3.ToString, "0")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:4/1 Disabled", CurrentUser)
                End If
            Else
                answer = MsgBox("I:4/1 is currently disabled. Would You like to enable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_4_1_Status.PLCAddressSelectColor3.ToString, "1")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:4/1 Enabled", CurrentUser)
                End If
            End If

        End If
    End Sub
    Private Sub I_4_2_Status_Click(sender As Object, e As EventArgs) Handles I_4_2_Status.Click
        If CurrentUserLevel = "3" Then
            Dim answer As Integer
            If I_4_2_Status.SelectColor3 = True Then
                answer = MsgBox("I:4/2 is currently enabled. Would You like to disable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_4_2_Status.PLCAddressSelectColor3.ToString, "0")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:4/2 Disabled", CurrentUser)
                End If
            Else
                answer = MsgBox("I:4/2 is currently disabled. Would You like to enable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_4_2_Status.PLCAddressSelectColor3.ToString, "1")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:4/2 Enabled", CurrentUser)
                End If
            End If

        End If
    End Sub



    Private Sub I_4_3_Status_Click(sender As Object, e As EventArgs) Handles I_4_3_Status.Click
        If CurrentUserLevel = "3" Then
            Dim answer As Integer
            If I_4_3_Status.SelectColor3 = True Then
                answer = MsgBox("I:4/3 is currently enabled. Would You like to disable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_4_3_Status.PLCAddressSelectColor3.ToString, "0")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:4/3 Disabled", CurrentUser)
                End If
            Else
                answer = MsgBox("I:4/3 is currently disabled. Would You like to enable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_4_3_Status.PLCAddressSelectColor3.ToString, "1")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:4/3 Enabled", CurrentUser)
                End If
            End If

        End If
    End Sub



    Private Sub I_4_4_Status_Click(sender As Object, e As EventArgs) Handles I_4_4_Status.Click
        If CurrentUserLevel = "3" Then
            Dim answer As Integer
            If I_4_4_Status.SelectColor3 = True Then
                answer = MsgBox("I:4/4 is currently enabled. Would You like to disable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_4_4_Status.PLCAddressSelectColor3.ToString, "0")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:4/3 Disabled", CurrentUser)
                End If
            Else
                answer = MsgBox("I:4/4 is currently disabled. Would You like to enable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_4_4_Status.PLCAddressSelectColor3.ToString, "1")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:4/3 Enabled", CurrentUser)
                End If
            End If

        End If
    End Sub
    Private Sub I_4_5_Status_Click(sender As Object, e As EventArgs) Handles I_4_5_Status.Click
        If CurrentUserLevel = "3" Then
            Dim answer As Integer
            If I_4_5_Status.SelectColor3 = True Then
                answer = MsgBox("I:4/5 is currently enabled. Would You like to disable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_4_5_Status.PLCAddressSelectColor3.ToString, "0")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:4/5 Disabled", CurrentUser)
                End If
            Else
                answer = MsgBox("I:4/5 is currently disabled. Would You like to enable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_4_5_Status.PLCAddressSelectColor3.ToString, "1")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:4/5 Enabled", CurrentUser)
                End If
            End If

        End If
    End Sub



    Private Sub I_4_6_Status_Click(sender As Object, e As EventArgs) Handles I_4_6_Status.Click
        If CurrentUserLevel = "3" Then
            Dim answer As Integer
            If I_4_6_Status.SelectColor3 = True Then
                answer = MsgBox("I:4/6 is currently enabled. Would You like to disable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_4_6_Status.PLCAddressSelectColor3.ToString, "0")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:4/6 Disabled", CurrentUser)
                End If
            Else
                answer = MsgBox("I:4/6 is currently disabled. Would You like to enable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_4_6_Status.PLCAddressSelectColor3.ToString, "1")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:4/6 Enabled", CurrentUser)
                End If
            End If

        End If
    End Sub
    Private Sub I_4_7_Status_Click(sender As Object, e As EventArgs) Handles I_4_7_Status.Click
        If CurrentUserLevel = "3" Then
            Dim answer As Integer
            If I_4_7_Status.SelectColor3 = True Then
                answer = MsgBox("I:4/7 is currently enabled. Would You like to disable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_4_7_Status.PLCAddressSelectColor3.ToString, "0")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:4/7 Disabled", CurrentUser)
                End If
            Else
                answer = MsgBox("I:4/7 is currently disabled. Would You like to enable it?", vbQuestion + vbYesNo + vbDefaultButton2, "Input Status")
                If answer = vbYes Then
                    PLC_Fast.Write(I_4_7_Status.PLCAddressSelectColor3.ToString, "1")
                    PLC_Fast.Write("B3:6/0", "1")
                    If RunSQL Then SaveSystemData(GetNow(), "I:4/7 Enabled", CurrentUser)
                End If
            End If

        End If
    End Sub
    '║                                                                                                              ║
    '╠══════════════════════════════════════════════════════════════════════════════════════════════════════════════╣
    '║                                               [ANALOG STATUS]                                                ║
    '║                                                 Section End                                                  ║
    '╚══════════════════════════════════════════════════════════════════════════════════════════════════════════════╝
#End Region
#Region "Analog Displays"
    '╔══════════════════════════════════════════════════════════════════════════════════════════════════════════════╗
    '║                                              [ANALOG DISPLAYS]                                               ║
    '║                                                Section Start                                                 ║
    '╠══════════════════════════════════════════════════════════════════════════════════════════════════════════════╣
    '║                                                                                                              ║
    ' This Code Updates all the values of the Analog Displays On all of the screens Based on the corrisponding values shown on the setting screen
    '            ***** ONLY CONTROLS ON SETTINGS SCREEN SHOULD HAVE A "PLCAddressValue" PROPERTY SET (TO MINIMIZE PLC TRAFFIC) *****


    Private Sub UpdateElectricalCharts()
        Dim PolarDataPoint As DataPoint = NorthSubDeltaChart.Series(0).Points(0)
        With NorthSubDeltaChart.Series("Actual")
            .Points.Clear()
            .Points.AddXY(0, ELNA_Actual.Value)
            .Points.AddXY(120, ELNB_Actual.Value)
            .Points.AddXY(240, ELNC_Actual.Value)
            .Points.AddXY(360, ELNA_Actual.Value)
        End With

        With SouthSubDeltaChart.Series("Actual")
            .Points.Clear()
            .Points.AddXY(0, ELSA_Actual.Value)
            .Points.AddXY(120, ELSB_Actual.Value)
            .Points.AddXY(240, ELSC_Actual.Value)
            .Points.AddXY(360, ELSA_Actual.Value)
        End With

    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        If CurrentUserLevel = 3 Then
            Dim result As DialogResult = MessageBox.Show("Application Settings", "Goto application settings?", MessageBoxButtons.YesNo)
            If result = DialogResult.Yes Then
                RefreshSettingsScreen()
            End If
        Else
            ShowLoginScreen()
        End If
    End Sub

    Private Sub RS_BTN_Click(sender As Object, e As EventArgs) Handles RS_BTN.Click
        Me.Close()
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Login_Screen.Hide()
    End Sub

    Private Sub SD_BTN_Click(sender As Object, e As EventArgs) Handles SD_BTN.Click
        Me.Close()
    End Sub

    Private Sub Chng0_PLCAddressLabel_Click(sender As Object, e As EventArgs) Handles Chng0_PLCAddressLabel.Click
        Dim res As MsgBoxResult = MsgBox("Changing the IP address for the targeted controller could result in loss of communication to the PLCandalso requires the application to be restarted for the changes to take effect" & vbCrLf & vbCrLf & "Are you sure you want to make this change?", MessageBoxButtons.YesNo, "IP ADDRESS CONFIGURATION CHANGE")
        If res = MsgBoxResult.Yes Then
            Dim newip As String = InputBox("Please enter the new IP address to use for the target controller (xxx.xxx.xxx.xxx)", "New IP Address", INI_GetKey_System("APP", "IP"))
            If newip.Split(".").Length = 4 Then
                Dim vals() As Boolean = {False, False, False, False}
                Dim cnt As Integer = 0
                For Each minor As String In newip.Split(".")
                    If IsNumeric(minor) & minor < 255 Then cnt += 1
                Next
                If cnt = 4 Then
                    MessageBox.Show("New target controller IP set to " & newip & vbCrLf & vbCrLf & "application requires restart to finalize settings")
                    INI_SetKey_System("APP", "IP", newip)
                    Set0_PLCAddressLabel.Text = newip
                Else
                    MessageBox.Show("Invalid IP Address")
                End If


            End If
        End If
    End Sub

    Private Sub ChangeValAddressClick(sender As System.Object, e As EventArgs) Handles CWPRP_Actual.Click,
    CWFD_Actual.Click, CWPOP_Actual.Click,
    HWPRP_Actual.Click, HWFD_Actual.Click, HWPOP_Actual.Click,
    HWFL_Actual.Click, HWTP_Actual.Click,
    ELNA_Actual.Click, ELNB_Actual.Click, ELNC_Actual.Click,
    ELSA_Actual.Click, ELSB_Actual.Click, ELSC_Actual.Click,
    ALP_Actual.Click,
    STLP_Actual.Click, STMP_Actual.Click, STFL_Actual.Click,
    STHP_Actual.Click, STFWP_Actual.Click,
    EVITP_Actual.Click, EVIHM_Actual.Click, EVOTP_Actual.Click,
    EVOHM_Actual.Click, STLPDEM_Actual.Click, STMPDEM_Actual.Click

        If CurrentUserLevel = 3 Then
            Dim NewActual As String = InputBox("Enter new PLC Address for the value.", "Change Actual", GetControlPropertyByName(sender.name, "PLCAddressValue"))
            Dim Sect As String = sender.name.split("_")(0)
            If NewActual IsNot "" Then
                INI_SetKey_PLC(Sect, "VALADD", NewActual)
                SetControlPropertyByName(sender.name, "PLCAddressValue", NewActual)
                ToolTip1.SetToolTip(sender, NewActual)
            Else
                INI_SetKey_PLC(Sect, "VALADD", "NONE")
                SetControlPropertyByName(sender.name, "PLCAddressValue", "")
                ToolTip1.SetToolTip(sender, "NONE")
            End If
        Else
            ShowLoginScreen()
        End If
    End Sub
    Private Sub ChangeHighAddressClick(sender As System.Object, e As EventArgs) Handles CWPRP_High.Click,
    CWFD_High.Click, CWPOP_High.Click,
    HWPRP_High.Click, HWFD_High.Click, HWPOP_High.Click,
    HWFL_High.Click, HWTP_High.Click,
    ELNA_High.Click, ELNB_High.Click, ELNC_High.Click,
    ELSA_High.Click, ELSB_High.Click, ELSC_High.Click,
    ALP_High.Click,
    STLP_High.Click, STMP_High.Click, STFL_High.Click,
    STHP_High.Click, STFWP_High.Click,
    EVITP_High.Click, EVIHM_High.Click, EVOTP_High.Click,
    EVOHM_High.Click, STLPDEM_High.Click, STMPDEM_High.Click

        If CurrentUserLevel = 3 Then
            Dim NewHigh As String = InputBox("Enter new PLC Address for the value.", "Change High", GetControlPropertyByName(sender.name, "PLCAddressValue"))
            Dim Sect As String = sender.name.split("_")(0)
            If NewHigh IsNot "" Then
                INI_SetKey_PLC(Sect, "ACMAXADD", NewHigh)
                SetControlPropertyByName(sender.name, "PLCAddressValue", NewHigh)
                ToolTip1.SetToolTip(sender, NewHigh)
            Else
                INI_SetKey_PLC(Sect, "ACMAXADD", "NONE")
                SetControlPropertyByName(sender.name, "PLCAddressValue", "")
                ToolTip1.SetToolTip(sender, "NONE")
            End If
        Else
            ShowLoginScreen()
        End If
    End Sub
    Private Sub ChangeLowAddressClick(sender As System.Object, e As EventArgs) Handles CWPRP_Low.Click,
    CWFD_Low.Click, CWPOP_Low.Click,
    HWPRP_Low.Click, HWFD_Low.Click, HWPOP_Low.Click,
    HWFL_Low.Click, HWTP_Low.Click,
    ELNA_Low.Click, ELNB_Low.Click, ELNC_Low.Click,
    ELSA_Low.Click, ELSB_Low.Click, ELSC_Low.Click,
    ALP_Low.Click,
    STLP_Low.Click, STMP_Low.Click, STFL_Low.Click,
    STHP_Low.Click, STFWP_Low.Click,
    EVITP_Low.Click, EVIHM_Low.Click, EVOTP_Low.Click,
    EVOHM_Low.Click, STLPDEM_Low.Click, STMPDEM_Low.Click

        If CurrentUserLevel = 3 Then
            Dim NewLow As String = InputBox("Enter new PLC Address for the value.", "Change Low", GetControlPropertyByName(sender.name, "PLCAddressValue"))
            Dim Sect As String = sender.name.split("_")(0)
            If NewLow IsNot "" Then
                INI_SetKey_PLC(Sect, "ACMINADD", NewLow)
                SetControlPropertyByName(sender.name, "PLCAddressValue", NewLow)
                ToolTip1.SetToolTip(sender, NewLow)
            Else
                INI_SetKey_PLC(Sect, "ACMINADD", "NONE")
                SetControlPropertyByName(sender.name, "PLCAddressValue", "")
                ToolTip1.SetToolTip(sender, "NONE")
            End If
        Else
            ShowLoginScreen()
        End If
    End Sub
    Private Sub ChangeTitlesClick(sender As System.Object, e As EventArgs) Handles CWPRP_Title.Click,
    CWFD_Title.Click, CWPOP_Title.Click,
    HWPRP_Title.Click, HWFD_Title.Click, HWPOP_Title.Click,
    HWFL_Title.Click, HWTP_Title.Click,
    ELNA_Title.Click, ELNB_Title.Click, ELNC_Title.Click,
    ELSA_Title.Click, ELSB_Title.Click, ELSC_Title.Click,
    ALP_Title.Click,
    STLP_Title.Click, STMP_Title.Click, STFL_Title.Click,
    STHP_Title.Click, STFWP_Title.Click,
    EVITP_Title.Click, EVIHM_Title.Click, EVOTP_Title.Click,
    EVOHM_Title.Click, STLPDEM_Title.Click, STMPDEM_Title.Click

        If CurrentUserLevel = 3 Then
            Dim NewTitle As String = InputBox("Enter new Title to show on the display", "Change Title", sender.text)
            Dim Sect As String = sender.name.split("_")(0)
            If NewTitle IsNot "" Then
                INI_SetKey_PLC(Sect, "VALTitle", NewTitle)
                sender.text = NewTitle
                If RunSQL Then SaveSystemData(GetNow(), "Unit (" & sender.name & ") Changed To:" & sender.text, CurrentUser)
            Else
                MsgBox("Entry must not be empty")
            End If
        Else
            ShowLoginScreen()
        End If

    End Sub
    Private Sub ChangeUnitsClick(sender As System.Object, e As EventArgs) Handles CWPRP_Unit.Click,
    CWFD_Unit.Click, CWPOP_Unit.Click,
    HWPRP_Unit.Click, HWFD_Unit.Click, HWPOP_Unit.Click,
    HWFL_Unit.Click, HWTP_Unit.Click,
    ELNA_Unit.Click, ELNB_Unit.Click, ELNC_Unit.Click,
    ELSA_Unit.Click, ELSB_Unit.Click, ELSC_Unit.Click,
    ALP_Unit.Click,
    STLP_Unit.Click, STMP_Unit.Click, STFL_Unit.Click,
    STHP_Unit.Click, STFWP_Unit.Click,
    EVITP_Unit.Click, EVIHM_Unit.Click, EVOTP_Unit.Click,
    EVOHM_Unit.Click, STLPDEM_Unit.Click, STMPDEM_Unit.Click

        If CurrentUserLevel = 3 Then
            Dim NewUnit As String = InputBox("Enter new units to show on the displays", "Change Units", sender.text)
            Dim Sect As String = sender.name.split("_")(0)
            If NewUnit IsNot "" Then
                INI_SetKey_PLC(Sect, "VALUNIT", NewUnit)
                sender.text = NewUnit
                SaveSystemData(GetNow(), "Unit (" & sender.name & ") Changed To:" & sender.text, CurrentUser)
            Else
                MsgBox("Entry must not be empty")
            End If
        Else
            ShowLoginScreen()
        End If

    End Sub

    '║                                                                                                              ║
    '╠══════════════════════════════════════════════════════════════════════════════════════════════════════════════╣
    '║                                              [ANALOG DISPLAYS]                                               ║
    '║                                                 Section End                                                  ║
    '╚══════════════════════════════════════════════════════════════════════════════════════════════════════════════╝
#End Region
#Region "System Functions"
    '╔══════════════════════════════════════════════════════════════════════════════════════════════════════════════╗
    '║                                                  [SYSTEM]                                                    ║
    '║                                                Section Start                                                 ║
    '╠══════════════════════════════════════════════════════════════════════════════════════════════════════════════╣
    '║                                                                                                              ║
    'Run Code on application startup

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
    Public Function GetControlPropertyByName(controlName As String, propertyName As String) As Object
        ' Find the control by name
        Dim ctrl As Control = FindControlByName(Me, controlName)

        If ctrl IsNot Nothing Then
            ' Use reflection to get the property
            Dim prop = ctrl.GetType().GetProperty(propertyName)

            If prop IsNot Nothing AndAlso prop.CanRead Then
                Return prop.GetValue(ctrl)
            Else
                Throw New MissingMemberException($"Property '{propertyName}' not found or not readable on control '{controlName}'.")
            End If
        Else
            Throw New ArgumentException($"Control '{controlName}' not found.")
        End If
    End Function
    Private Sub Show_Con_String(sender As Object, e As EventArgs) Handles Button1.Click
        Dim SQL As String() =
                {INI_GetKey_System("SQL", "SERVER"),
                INI_GetKey_System("SQL", "USER"),
                INI_GetKey_System("SQL", "PASS"),
                INI_GetKey_System("SQL", "DB")}
        MsgBox("server=" & SQL(0) & ";user id=" & SQL(1) & ";password=" & SQL(2) & ";database=" & SQL(3), MessageBoxButtons.OK, "SQL Connection String")
    End Sub
    Function IsAppInstalled(displayName As String) As Boolean
        ' Keys to check (32andalso 64-bit locations)
        Dim uninstallKeys As String() = {
            "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
            "SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
        }

        For Each keyPath In uninstallKeys
            Using key As RegistryKey = Registry.LocalMachine.OpenSubKey(keyPath)
                If key IsNot Nothing Then
                    For Each subKeyName In key.GetSubKeyNames()
                        Using subKey As RegistryKey = key.OpenSubKey(subKeyName)
                            Dim appName As String = TryCast(subKey.GetValue("DisplayName"), String)

                            If appName IsNot Nothing AndAlso appName.Contains(displayName) Then
                                Return True
                            End If

                        End Using
                    Next
                End If
            End Using
        Next

        Return False
    End Function
    Private Sub Dashboard_Page_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Clear_Screens()
        StartUpScreen.BringToFront()
        If IsAppInstalled("MySQL") Then 'Check to see if MySQL server has been installed
            If IO.Directory.Exists(My.Settings.INI_Loc) Then 'Check to see if the saved configuration directory exists
                If IO.File.Exists(My.Settings.INI_Loc & "\DataConfig.ini") AndAlso ' Check to see if the config files exist inside the configuration directory
                    IO.File.Exists(My.Settings.INI_Loc & "\SystemConfig.ini") AndAlso'*
                    IO.File.Exists(My.Settings.INI_Loc & "\UserConfig.ini") AndAlso'*
                    IO.File.Exists(My.Settings.INI_Loc & "\AlarmConfig.ini") AndAlso'*
                    IO.File.Exists(My.Settings.INI_Loc & "\IOConfig.ini") Then '*

                    StartupStatusLabel.Text = "Checking configuration files..."
                    System_Populate()
                Else
                    Dim result As MsgBoxResult = MsgBox("Configuration files not found. Would you like to select a new folder location?", MsgBoxStyle.YesNo, "Set New Configuration Location")
                    If result = MsgBoxResult.Yes Then
                        Dim NewFBD As New FolderBrowserDialog
                        NewFBD.SelectedPath = My.Settings.INI_Loc
                        NewFBD.ShowDialog()
                        If NewFBD.SelectedPath IsNot "" Then
                            My.Settings.INI_Loc = NewFBD.SelectedPath
                            My.Settings.Save()
                            If IO.File.Exists(My.Settings.INI_Loc & "\DataConfig.ini") AndAlso ' Check to see if the config files exist inside the configuration directory
                                IO.File.Exists(My.Settings.INI_Loc & "\SystemConfig.ini") AndAlso'*
                                IO.File.Exists(My.Settings.INI_Loc & "\UserConfig.ini") AndAlso'*
                                IO.File.Exists(My.Settings.INI_Loc & "\AlarmConfig.ini") AndAlso'*
                                IO.File.Exists(My.Settings.INI_Loc & "\IOConfig.ini") Then '*

                                StartupStatusLabel.Text = "Checking configuration files..."
                                System_Populate()
                            Else
                                MsgBox("Selected Path (" & NewFBD.SelectedPath & ") does contain configuration files!.", MsgBoxStyle.Critical, "Critical Error")
                                Me.Close()
                            End If
                        Else
                            MsgBox("Selected Path (" & NewFBD.SelectedPath & ") does contain configuration files!.", MsgBoxStyle.Critical, "Critical Error")
                            Me.Close()

                        End If
                    Else
                        Me.Close()
                    End If
                End If
            Else
                Dim result As MsgBoxResult = MsgBox("Configuration files not found. Would you like to select a new folder location?", MsgBoxStyle.YesNo, "Set New Configuration Location")
                If result = MsgBoxResult.Yes Then
                    Dim NewFBD As New FolderBrowserDialog
                    NewFBD.SelectedPath = My.Settings.INI_Loc
                    NewFBD.ShowDialog()
                    If NewFBD.SelectedPath IsNot "" Then
                        My.Settings.INI_Loc = NewFBD.SelectedPath
                        My.Settings.Save()
                        If IO.File.Exists(My.Settings.INI_Loc & "\DataConfig.ini") AndAlso ' Check to see if the config files exist inside the configuration directory
                                IO.File.Exists(My.Settings.INI_Loc & "\SystemConfig.ini") AndAlso'*
                                IO.File.Exists(My.Settings.INI_Loc & "\UserConfig.ini") AndAlso'*
                                IO.File.Exists(My.Settings.INI_Loc & "\AlarmConfig.ini") AndAlso'*
                                IO.File.Exists(My.Settings.INI_Loc & "\IOConfig.ini") Then '*

                            StartupStatusLabel.Text = "Checking configuration files..."
                            System_Populate()
                        Else
                            MsgBox("Selected Path (" & NewFBD.SelectedPath & ") does contain configuration files!.", MsgBoxStyle.Critical, "Critical Error")
                            Me.Close()
                        End If
                    Else
                        MsgBox("Selected Path (" & NewFBD.SelectedPath & ") does contain configuration files!.", MsgBoxStyle.Critical, "Critical Error")
                        Me.Close()

                    End If
                Else
                    Me.Close()
                End If
            End If

        Else
            MsgBox("MySQL needs to be installed, check " & Application.StartupPath & "\sql" & " for installation files.")

            Me.Close()
        End If
    End Sub
    Private Sub System_Populate()

        StartupStatusLabel.Text = "Getting system settings..."
        PLC.IPAddress = INI_GetKey_System("APP", "IP")
        PLC_Fast.IPAddress = INI_GetKey_System("APP", "IP")
        If INI_GetKey_System("APP", "TopMost") = "True" Then Me.TopMost = True Else Me.TopMost = False
        If INI_GetKey_System("SQL", "SQL_EN") = "TRUE" Then
            RunSQL = True
            LogAlarmIcon1.Image = My.Resources.LogFileGood
            LogAlarmIcon1.BackgroundImage = My.Resources.GOODBG
            Set2_RunLogLabel.Text = "TRUE"
        Else
            RunSQL = False
            LogAlarmIcon1.Image = My.Resources.LogFileBad
            LogAlarmIcon1.BackgroundImage = My.Resources.BADBG
            Set2_RunLogLabel.Text = "FALSE"
        End If

        Dim SQL As String() =
                {INI_GetKey_System("SQL", "SERVER"),
                INI_GetKey_System("SQL", "USER"),
                INI_GetKey_System("SQL", "PASS"),
                INI_GetKey_System("SQL", "DB")}
        SQL_ConString = "server=" & SQL(0) & ";user id=" & SQL(1) & ";password=" & SQL(2) & ";database=" & SQL(3)
        Settings_Populate()
    End Sub

    Private Sub Settings_Populate()
        StartupStatusLabel.Text = "Getting PLC Tags..."
        Dim SensNames As String() = {"CWSUP", "CWPRP", "CWPOP", "CWFD", "HWPRP", "HWPOP",
               "HWFD", "HWTP", "HWFL", "STFWP", "STHP", "STLP", "STMP", "STLPDEM",
               "STMPDEM", "STFL", "ELNA", "ELNB", "ELNC", "ELSA", "ELSB",
               "ELSC", "ALP", "EVITP", "EVIHM", "EVOTP", "EVOHM"}
        For Each i As String In SensNames
            Try
                Dim ValAdd As String = INI_GetKey_PLC(i, "VALADD")
                If ValAdd IsNot "NONE" Then
                    SetControlPropertyByName(i & "_Actual", "PLCAddressValue", ValAdd)
                    ToolTip1.SetToolTip(Setpoints_Page.Controls(i & "_Actual"), ValAdd)
                End If

                Dim HighAdd As String = INI_GetKey_PLC(i, "ACMAXADD")
                If HighAdd IsNot "NONE" Then
                    SetControlPropertyByName(i & "_High", "PLCAddressValue", HighAdd)
                    ToolTip1.SetToolTip(Setpoints_Page.Controls(i & "_High"), HighAdd)
                End If

                Dim LowAdd As String = INI_GetKey_PLC(i, "ACMINADD")
                If LowAdd IsNot "NONE" Then
                    SetControlPropertyByName(i & "_Low", "PLCAddressValue", LowAdd)
                    ToolTip1.SetToolTip(Setpoints_Page.Controls(i & "_Low"), LowAdd)
                End If

                Dim MinAdd As String = INI_GetKey_PLC(i, "ALMINADD")
                If MinAdd IsNot "NONE" Then
                    SetControlPropertyByName(i & "_Min", "PLCAddressValue", MinAdd)
                    ToolTip1.SetToolTip(Setpoints_Page.Controls(i & "_Min"), MinAdd)
                End If

                Dim MaxAdd As String = INI_GetKey_PLC(i, "ALMAXADD")
                If MaxAdd IsNot "NONE" Then
                    SetControlPropertyByName(i & "_Max", "PLCAddressValue", MaxAdd)
                    ToolTip1.SetToolTip(Setpoints_Page.Controls(i & "_Max"), MaxAdd)
                End If

                Dim Title As String = INI_GetKey_PLC(i, "VALTITLE")
                If Title IsNot "NONE" Then
                    Setpoints_Page.Controls(i & "_Title").Text = Title
                    ToolTip1.SetToolTip(Setpoints_Page.Controls(i & "_Title"), "Click to edit..")
                End If

                Dim Unit As String = INI_GetKey_PLC(i, "VALUNIT")
                If Unit IsNot "NONE" Then
                    Setpoints_Page.Controls(i & "_Unit").Text = Unit
                    ToolTip1.SetToolTip(Setpoints_Page.Controls(i & "_Unit"), "Click to edit..")
                End If

                Dim AlertAdd As String = INI_GetKey_PLC(i, "ALERTADD")
                If AlertAdd IsNot "NONE" Then
                    SetControlPropertyByName(i & "_Actual", "PLCAddressHighlight", AlertAdd)
                End If

            Catch ex As Exception
                MsgBox(ex.Message)
            End Try

        Next
        Description_Populate()
    End Sub

    Private Sub Description_Populate()
        StartupStatusLabel.Text = "Getting Alarm Descriptions..."
        UpdateElecAlarmDesc()
        UpdateAirAlarmDesc()
        UpdateSteamAlarmDesc()
        UpdateWaterAlarmDesc()
        StartupStatusLabel.Text = "Getting all IO descriptions..."
        UpdateIODescriptionLabels()
        StartupStatusLabel.Text = "Setting up realtime chart settings..."
        Chart3.ChartAreas(0).AxisX.LabelStyle.ForeColor = Color.White
        Chart3.ChartAreas(0).AxisY.LabelStyle.ForeColor = Color.White
        For Each Ser As Series In Chart3.Series()
            If Ser.ChartArea.Contains("Elec") Then CheckedListBox1.Items.Add(Ser.Name, True)
            If Ser.ChartArea.Contains("Air") Then CheckedListBox2.Items.Add(Ser.Name, True)
            If Ser.ChartArea.Contains("Steam") Then CheckedListBox3.Items.Add(Ser.Name, True)
            If Ser.ChartArea.Contains("Water") Then CheckedListBox4.Items.Add(Ser.Name, True)
        Next
        Alarm_Populate()
    End Sub
    Private Sub Alarm_Populate()
        StartupStatusLabel.Text = "Getting Alarm Information From PLC..."
        processCount = 0
        GetAllAud_Falc_AlarmStatus()
        CheckBypass()
        Final_Populate()
    End Sub
    Private Sub Final_Populate()
        Set0_PLCAddressLabel.Text = INI_GetKey_System("APP", "IP")
        Set1_TopMostLabel.Text = INI_GetKey_System("APP", "TOPMOST")
        Set3_SysTimeoutLabel.Text = INI_GetKey_System("APP", "SCREEN_TO")
        Set4_UserTimeoutLabel.Text = INI_GetKey_System("APP", "USER_TO")
        Set2_RunLogLabel.Text = INI_GetKey_System("SQL", "SQL_EN")
        SQLLBL_Server.Text = INI_GetKey_System("SQL", "SERVER")
        SQLLBL_User.Text = INI_GetKey_System("SQL", "USER")
        SQLLBL_Pass.Text = INI_GetKey_System("SQL", "PASS")
        SQLLBL_Database.Text = INI_GetKey_System("SQL", "DB")

        StartupStatusLabel.Text = "Binding values..."
        PropertyBind()
        StartupStatusLabel.Text = "Waiting for stable values from PLC..."
        Dash_ALM_Indicator.Text = "System" & vbNewLine & "Normal"
        FirstTick_EN = True
    End Sub
    Private Sub RefreshWeatherLabel_Click(sender As Object, e As EventArgs) Handles RefreshWeatherLabel.Click
        RefreshWeatherLabel.Text = "Last Update: " & Now & " Click To Refresh"
        UpdateWeatherData()
    End Sub
    Public Function BypassAlarmToggle(index As Integer, group As Integer) As Boolean 'N15:g/i
        If CurrentUserLevel = "3" Then
            Dim PanelName As String = ""
            Dim DescCtrl As String = "AlarmDesc" & index
            Dim result As Boolean
            If group = 0 Or group = 1 Then
                DescCtrl = "Elec" & DescCtrl
                PanelName = "ElectricalAlarmPanel"
            ElseIf group = 2 Or group = 3 Then
                DescCtrl = "Air" & DescCtrl
                PanelName = "AirAlarmPanel"
            ElseIf group = 4 Or group = 5 Then
                DescCtrl = "Steam" & DescCtrl
                PanelName = "SteamAlarmPanel"
            ElseIf group = 6 Or group = 7 Then
                DescCtrl = "Water" & DescCtrl
                PanelName = "WaterAlarmPanel"
            End If
            Dim ctrl As System.Object = Me.Alarms_Page.Controls(PanelName).Controls(DescCtrl)


            Dim answer As Integer
            If PLC_Fast.Read("N15:" & group & "/" & index) = False Then
                answer = MsgBox(ctrl.Text & " - bypass is currently active. Clear this bypass?", vbExclamation + vbYesNo + vbDefaultButton2, "Alarm Bypass")
                If answer = vbYes Then
                    result = False
                    PLC_Fast.Write("N15:" & group & "/" & index, "1")
                    If RunSQL Then SaveSystemData(GetNow(), ctrl.text & " - Bypassed Cleared", CurrentUser)
                End If
            Else
                answer = MsgBox("Bypass " & ctrl.Text & " - Alarm?", vbExclamation + vbYesNo + vbDefaultButton2, "Alarm Bypass")
                If answer = vbYes Then
                    result = True
                    PLC_Fast.Write("N15:" & group & "/" & index, "0")
                    If RunSQL Then SaveSystemData(GetNow(), ctrl.text & " Bypassed Active", CurrentUser)
                End If
            End If
            Return result
        End If
    End Function
    Private Sub PointBypass_Clicked(sender As System.Object, e As EventArgs) Handles ElecAlarmLight0.Click, ElecAlarmLight1.Click,
    ElecAlarmLight2.Click, ElecAlarmLight3.Click, ElecAlarmLight4.Click, ElecAlarmLight5.Click, ElecAlarmLight6.Click,
    ElecAlarmLight7.Click, ElecAlarmLight8.Click, ElecAlarmLight9.Click, ElecAlarmLight10.Click, ElecAlarmLight11.Click,
    ElecAlarmLight12.Click, ElecAlarmLight13.Click, ElecAlarmLight14.Click, ElecAlarmLight15.Click, ElecAlarmLight16.Click,
    ElecAlarmLight17.Click, ElecAlarmLight18.Click, ElecAlarmLight19.Click, ElecAlarmLight20.Click, ElecAlarmLight21.Click,
    ElecAlarmLight22.Click, ElecAlarmLight23.Click, ElecAlarmLight24.Click, ElecAlarmLight25.Click, ElecAlarmLight26.Click,
    ElecAlarmLight27.Click, ElecAlarmLight28.Click, ElecAlarmLight29.Click, ElecAlarmLight30.Click, ElecAlarmLight31.Click,
    AirAlarmLight0.Click, AirAlarmLight1.Click,
    AirAlarmLight2.Click, AirAlarmLight3.Click, AirAlarmLight4.Click, AirAlarmLight5.Click, AirAlarmLight6.Click,
    AirAlarmLight7.Click, AirAlarmLight8.Click, AirAlarmLight9.Click, AirAlarmLight10.Click, AirAlarmLight11.Click,
    AirAlarmLight12.Click, AirAlarmLight13.Click, AirAlarmLight14.Click, AirAlarmLight15.Click, AirAlarmLight16.Click,
    AirAlarmLight17.Click, AirAlarmLight18.Click, AirAlarmLight19.Click, AirAlarmLight20.Click, AirAlarmLight21.Click,
    AirAlarmLight22.Click, AirAlarmLight23.Click, AirAlarmLight24.Click, AirAlarmLight25.Click, AirAlarmLight26.Click,
    AirAlarmLight27.Click, AirAlarmLight28.Click, AirAlarmLight29.Click, AirAlarmLight30.Click, AirAlarmLight31.Click,
    SteamAlarmLight0.Click, SteamAlarmLight1.Click,
    SteamAlarmLight2.Click, SteamAlarmLight3.Click, SteamAlarmLight4.Click, SteamAlarmLight5.Click, SteamAlarmLight6.Click,
    SteamAlarmLight7.Click, SteamAlarmLight8.Click, SteamAlarmLight9.Click, SteamAlarmLight10.Click, SteamAlarmLight11.Click,
    SteamAlarmLight12.Click, SteamAlarmLight13.Click, SteamAlarmLight14.Click, SteamAlarmLight15.Click, SteamAlarmLight16.Click,
    SteamAlarmLight17.Click, SteamAlarmLight18.Click, SteamAlarmLight19.Click, SteamAlarmLight20.Click, SteamAlarmLight21.Click,
    SteamAlarmLight22.Click, SteamAlarmLight23.Click, SteamAlarmLight24.Click, SteamAlarmLight25.Click, SteamAlarmLight26.Click,
    SteamAlarmLight27.Click, SteamAlarmLight28.Click, SteamAlarmLight29.Click, SteamAlarmLight30.Click, SteamAlarmLight31.Click,
    WaterAlarmLight0.Click, WaterAlarmLight1.Click,
    WaterAlarmLight2.Click, WaterAlarmLight3.Click, WaterAlarmLight4.Click, WaterAlarmLight5.Click, WaterAlarmLight6.Click,
    WaterAlarmLight7.Click, WaterAlarmLight8.Click, WaterAlarmLight9.Click, WaterAlarmLight10.Click, WaterAlarmLight11.Click,
    WaterAlarmLight12.Click, WaterAlarmLight13.Click, WaterAlarmLight14.Click, WaterAlarmLight15.Click, WaterAlarmLight16.Click,
    WaterAlarmLight17.Click, WaterAlarmLight18.Click, WaterAlarmLight19.Click, WaterAlarmLight20.Click, WaterAlarmLight21.Click,
    WaterAlarmLight22.Click, WaterAlarmLight23.Click, WaterAlarmLight24.Click, WaterAlarmLight25.Click, WaterAlarmLight26.Click,
    WaterAlarmLight27.Click, WaterAlarmLight28.Click, WaterAlarmLight29.Click, WaterAlarmLight30.Click, WaterAlarmLight31.Click
        '-----------------------------------------------------------------------------------------------------------------------------
        Dim Group As Integer = 0
        Dim Index As Integer = GetNumeric(sender.name)

        If sender.name.contains("Elec") Then
            Group = 0
        ElseIf sender.name.contains("Air") Then
            Group = 2
        ElseIf sender.name.contains("Steam") Then
            Group = 4
        ElseIf sender.name.contains("Water") Then
            Group = 6
        Else MsgBox("Invalid data" & vbNewLine & Group & vbNewLine & Index)
        End If
        If Index >= 16 Then
            Index -= 16
            Group += 1
        End If
        If BypassAlarmToggle(Index, Group) Then
            sender.selectcolor3 = True
        Else
            sender.selectcolor3 = False
        End If
    End Sub
    'code to close application 
    Private Sub Close_Application(sender As Object, e As EventArgs)
        If CurrentUserLevel = 3 Then
            Dim answer As Integer
            answer = MsgBox("Do you want to close Facility Monitor?", vbQuestion + vbYesNo + vbDefaultButton2, "Close Application")
            If answer = vbYes Then
                If RunSQL Then SaveSystemData(GetNow(), "SYSTEM SHUTDOWN", CurrentUser)
                Me.Close()
            End If
        End If

    End Sub

    Private Sub Change_Alarm_Description(sender As System.Object, e As EventArgs) Handles ElecAlarmDesc0.Click,
        ElecAlarmDesc1.Click, ElecAlarmDesc2.Click, ElecAlarmDesc3.Click, ElecAlarmDesc4.Click,
        ElecAlarmDesc5.Click, ElecAlarmDesc6.Click, ElecAlarmDesc7.Click, ElecAlarmDesc8.Click,
        ElecAlarmDesc9.Click, ElecAlarmDesc10.Click, ElecAlarmDesc11.Click, ElecAlarmDesc12.Click,
        ElecAlarmDesc13.Click, ElecAlarmDesc14.Click, ElecAlarmDesc15.Click, ElecAlarmDesc16.Click,
        ElecAlarmDesc17.Click, ElecAlarmDesc18.Click, ElecAlarmDesc19.Click, ElecAlarmDesc20.Click,
        ElecAlarmDesc21.Click, ElecAlarmDesc22.Click, ElecAlarmDesc23.Click, ElecAlarmDesc24.Click,
        ElecAlarmDesc25.Click, ElecAlarmDesc26.Click, ElecAlarmDesc27.Click, ElecAlarmDesc28.Click,
        ElecAlarmDesc29.Click, ElecAlarmDesc30.Click, ElecAlarmDesc31.Click, SteamAlarmDesc0.Click,
        SteamAlarmDesc1.Click, SteamAlarmDesc2.Click, SteamAlarmDesc3.Click, SteamAlarmDesc4.Click,
        SteamAlarmDesc5.Click, SteamAlarmDesc6.Click, SteamAlarmDesc7.Click, SteamAlarmDesc8.Click,
        SteamAlarmDesc9.Click, SteamAlarmDesc10.Click, SteamAlarmDesc11.Click, SteamAlarmDesc12.Click,
        SteamAlarmDesc13.Click, SteamAlarmDesc14.Click, SteamAlarmDesc15.Click, SteamAlarmDesc16.Click,
        SteamAlarmDesc17.Click, SteamAlarmDesc18.Click, SteamAlarmDesc19.Click, SteamAlarmDesc20.Click,
        SteamAlarmDesc21.Click, SteamAlarmDesc22.Click, SteamAlarmDesc23.Click, SteamAlarmDesc24.Click,
        SteamAlarmDesc25.Click, SteamAlarmDesc26.Click, SteamAlarmDesc27.Click, SteamAlarmDesc28.Click,
        SteamAlarmDesc29.Click, SteamAlarmDesc30.Click, SteamAlarmDesc31.Click, WaterAlarmDesc0.Click,
        WaterAlarmDesc1.Click, WaterAlarmDesc2.Click, WaterAlarmDesc3.Click, WaterAlarmDesc4.Click,
        WaterAlarmDesc5.Click, WaterAlarmDesc6.Click, WaterAlarmDesc7.Click, WaterAlarmDesc8.Click,
        WaterAlarmDesc9.Click, WaterAlarmDesc10.Click, WaterAlarmDesc11.Click, WaterAlarmDesc12.Click,
        WaterAlarmDesc13.Click, WaterAlarmDesc14.Click, WaterAlarmDesc15.Click, WaterAlarmDesc16.Click,
        WaterAlarmDesc17.Click, WaterAlarmDesc18.Click, WaterAlarmDesc19.Click, WaterAlarmDesc20.Click,
        WaterAlarmDesc21.Click, WaterAlarmDesc22.Click, WaterAlarmDesc23.Click, WaterAlarmDesc24.Click,
        WaterAlarmDesc25.Click, WaterAlarmDesc26.Click, WaterAlarmDesc27.Click, WaterAlarmDesc28.Click,
        WaterAlarmDesc29.Click, WaterAlarmDesc30.Click, WaterAlarmDesc31.Click, AirAlarmDesc0.Click,
        AirAlarmDesc1.Click, AirAlarmDesc2.Click, AirAlarmDesc3.Click, AirAlarmDesc4.Click,
        AirAlarmDesc5.Click, AirAlarmDesc6.Click, AirAlarmDesc7.Click, AirAlarmDesc8.Click,
        AirAlarmDesc9.Click, AirAlarmDesc10.Click, AirAlarmDesc11.Click, AirAlarmDesc12.Click,
        AirAlarmDesc13.Click, AirAlarmDesc14.Click, AirAlarmDesc15.Click, AirAlarmDesc16.Click,
        AirAlarmDesc17.Click, AirAlarmDesc18.Click, AirAlarmDesc19.Click, AirAlarmDesc20.Click,
        AirAlarmDesc21.Click, AirAlarmDesc22.Click, AirAlarmDesc23.Click, AirAlarmDesc24.Click,
        AirAlarmDesc25.Click, AirAlarmDesc26.Click, AirAlarmDesc27.Click, AirAlarmDesc28.Click,
        AirAlarmDesc29.Click, AirAlarmDesc30.Click, AirAlarmDesc31.Click

        If CurrentUserLevel > 2 Then
            'DETERMINE ALARM CATAGORY
            Dim Catagory As String = ""
            If sender.name.ToString.StartsWith("Air") Then Catagory = "Air"
            If sender.name.ToString.StartsWith("Elec") Then Catagory = "Elec"
            If sender.name.ToString.StartsWith("Steam") Then Catagory = "Steam"
            If sender.name.ToString.StartsWith("Water") Then Catagory = "Water"

            Dim AlarmIndex As Integer = GetNumeric(sender.name)
            Dim MessageDisplayName As String = Catagory & "AlarmDisp1"
            Dim FontColor As Color = sender.forecolor
            Dim Backcolor As Color = sender.backcolor

            NewAlarmDesc.Text = sender.text
            NewAlarmDesc.BackColor = Backcolor
            NewAlarmDesc.ForeColor = FontColor
            NewAlarmDescTB.Text = sender.text
            NewAlarmLight.Text = AlarmIndex
            AlarmDescLabel.Text = sender.name
            AlarmLightLabel.Text = Catagory & "AlarmLight" & AlarmIndex
            AlarmPanelLabel.Text = Catagory & "AlarmDisp1"
            NewFontColorPanel.BackColor = FontColor
            NewBackColorPanel.BackColor = Backcolor
            EditAlarmDescPanel.Show()
            EditAlarmDescPanel.BringToFront()
        End If
    End Sub

    Private Sub NewAlarmDescTB_TextChanged(sender As Object, e As EventArgs) Handles NewAlarmDescTB.TextChanged
        NewAlarmDesc.Text = NewAlarmDescTB.Text
    End Sub
    Private Sub NewFontColorPanel_Paint(sender As Object, e As EventArgs) Handles NewFontColorPanel.Click
        ColorDialog1.Color = NewFontColorPanel.BackColor
        ColorDialog1.ShowDialog()
        NewFontColorPanel.BackColor = ColorDialog1.Color
        NewAlarmDesc.ForeColor = ColorDialog1.Color
    End Sub
    Private Sub NewBackColorPanel_Paint(sender As Object, e As EventArgs) Handles NewBackColorPanel.Click
        ColorDialog1.Color = NewBackColorPanel.BackColor
        ColorDialog1.ShowDialog()
        NewBackColorPanel.BackColor = ColorDialog1.Color
        NewAlarmDesc.BackColor = ColorDialog1.Color
    End Sub
    Private Sub Save_Alarm_Values(sender As Object, e As EventArgs) Handles Button10.Click
        Dim alarmindex As Integer = GetNumeric(AlarmDescLabel.Text)

        If AlarmDescLabel.Text.StartsWith("Elec") Then
            Dim section As String = "EL" & alarmindex + 1
            INI_SetKey_Alarm(section, "BACK_COL", NewBackColorPanel.BackColor.ToArgb)
            INI_SetKey_Alarm(section, "FORE_COL", NewFontColorPanel.BackColor.ToArgb)
            INI_SetKey_Alarm(section, "MESSAGE", NewAlarmDescTB.Text)
            UpdateElecAlarmDesc()
        ElseIf AlarmDescLabel.Text.StartsWith("Air") Then
            Dim section As String = "AR" & alarmindex + 1
            INI_SetKey_Alarm(section, "BACK_COL", NewBackColorPanel.BackColor.ToArgb)
            INI_SetKey_Alarm(section, "FORE_COL", NewFontColorPanel.BackColor.ToArgb)
            INI_SetKey_Alarm(section, "MESSAGE", NewAlarmDescTB.Text)
            UpdateAirAlarmDesc()
        ElseIf AlarmDescLabel.Text.StartsWith("Steam") Then
            Dim section As String = "ST" & alarmindex + 1
            INI_SetKey_Alarm(section, "BACK_COL", NewBackColorPanel.BackColor.ToArgb)
            INI_SetKey_Alarm(section, "FORE_COL", NewFontColorPanel.BackColor.ToArgb)
            INI_SetKey_Alarm(section, "MESSAGE", NewAlarmDescTB.Text)
            UpdateSteamAlarmDesc()
        ElseIf AlarmDescLabel.Text.StartsWith("Water") Then
            Dim section As String = "WR" & alarmindex + 1
            INI_SetKey_Alarm(section, "BACK_COL", NewBackColorPanel.BackColor.ToArgb)
            INI_SetKey_Alarm(section, "FORE_COL", NewFontColorPanel.BackColor.ToArgb)
            INI_SetKey_Alarm(section, "MESSAGE", NewAlarmDescTB.Text)
            UpdateWaterAlarmDesc()
        End If
        EditAlarmDescPanel.Hide()

    End Sub
    Private Sub Chng1_TopMostLabel_Click(sender As Object, e As EventArgs) Handles Chng1_TopMostLabel.Click
        If INI_GetKey_System("APP", "TOPMOST") = "TRUE" Then
            Me.TopMost = False
            INI_SetKey_System("APP", "TOPMOST", "FALSE")
        Else
            Me.TopMost = True
            INI_SetKey_System("APP", "TOPMOST", "TRUE")
        End If
    End Sub
    'logic for PLC HeartBeat 

    Private Sub PLC_Response_Time(sender As Object, e As EventArgs) Handles BasicLabel83.ValueChanged
        If FirstTick_EN Then
            If FirstTickDone Then
                Dim NewHBTime As DateTime = Now
                Dim TimeBetweenLastHB As TimeSpan = Now.Subtract(lastHBTime)
                lastHBTime = Now
                Label51.Text = TimeBetweenLastHB.Seconds & "." & TimeBetweenLastHB.Milliseconds.ToString("000") & " sec"
            End If
        End If
    End Sub
    Private Sub First_Tick_Delay(sender As Object, e As EventArgs) Handles BasicLabel6.ValueChanged
        If FirstTick_EN Then
            If FirstTickDone = False Then
                If IsNumeric(sender.text) Then
                    First5Count += 1
                    ProgressBar1.Increment(1)
                    If First5Count >= 5 Then
                        FirstTickDone = True
                        First5Count = 0
                        Clear_Nav_Buttons()
                        Main_Page.Show()
                        Goto_Dashboard_BTN.ForeColor = Color.Black
                        Goto_Dashboard_BTN.BackColor = Color.Green
                        StartUpScreen.Hide()
                        HeartBeatTimer.Start()
                        If RunSQL Then SaveSystemData(GetNow(), "APPLICATION STARTUP", "System")
                    End If
                End If
            Else
                Dim NewHBTime_5s As DateTime = Now
                Dim TimeBetweenLastHB_5s As TimeSpan = Now.Subtract(lastHBTime_5s)
                lastHBTime_5s = Now
                Label74.Text = TimeBetweenLastHB_5s.Seconds & "." & TimeBetweenLastHB_5s.Milliseconds.ToString("000") & " sec"
            End If
        End If
    End Sub

    'Private Sub ElecAL_CB_SelectedIndexChanged(sender As Object, e As EventArgs)
    '    AlarmSearchListBox.Items.Clear()
    '    For Each alarm As String In AlarmList.Items
    '        If alarm.Contains(ElecAL_CB.Text) Then AlarmSearchListBox.Items.Add(alarm)
    '    Next
    'End Sub
    'Private Sub AirAL_CB_SAirtedIndexChanged(sender As Object, e As EventArgs)
    '    AlarmSearchListBox.Items.Clear()
    '    For Each alarm As String In AlarmList.Items
    '        If alarm.Contains(AirAL_CB.Text) Then AlarmSearchListBox.Items.Add(alarm)
    '    Next
    'End Sub
    'Private Sub SteamAL_CB_SSteamtedIndexChanged(sender As Object, e As EventArgs)
    '    AlarmSearchListBox.Items.Clear()
    '    For Each alarm As String In AlarmList.Items
    '        If alarm.Contains(SteamAL_CB.Text) Then AlarmSearchListBox.Items.Add(alarm)
    '    Next
    'End Sub
    'Private Sub WaterAL_CB_SWatertedIndexChanged(sender As Object, e As EventArgs)
    '    AlarmSearchListBox.Items.Clear()
    '    For Each alarm As String In AlarmList.Items
    '        If alarm.Contains(WaterAL_CB.Text) Then AlarmSearchListBox.Items.Add(alarm)
    '    Next
    'End Sub

    Private Sub Input_Update_CARD7_0to15(sender As Object, e As EventArgs) Handles BasicLabel5.ValueChanged
        Dim AlarmArray() As Boolean = IntToBin(sender.value)
        PL_I_7_0.SelectColor2 = AlarmArray(0)
        PL_I_7_1.SelectColor2 = AlarmArray(1)
        PL_I_7_2.SelectColor2 = AlarmArray(2)
        PL_I_7_3.SelectColor2 = AlarmArray(3)
        PL_I_7_4.SelectColor2 = AlarmArray(4)
        PL_I_7_5.SelectColor2 = AlarmArray(5)
        PL_I_7_6.SelectColor2 = AlarmArray(6)
        PL_I_7_7.SelectColor2 = AlarmArray(7)
        PL_I_7_8.SelectColor2 = AlarmArray(8)
        PL_I_7_9.SelectColor2 = AlarmArray(9)
        PL_I_7_10.SelectColor2 = AlarmArray(10)
        PL_I_7_11.SelectColor2 = AlarmArray(11)
        PL_I_7_12.SelectColor2 = AlarmArray(12)
        PL_I_7_13.SelectColor2 = AlarmArray(13)
        PL_I_7_14.SelectColor2 = AlarmArray(14)
        PL_I_7_15.SelectColor2 = AlarmArray(15)
    End Sub
    Private Sub Input_Update_CARD7_16to31(sender As Object, e As EventArgs) Handles BasicLabel9.ValueChanged
        Dim AlarmArray() As Boolean = IntToBin(sender.value)
        PL_I_7_16.SelectColor2 = AlarmArray(0)
        PL_I_7_17.SelectColor2 = AlarmArray(1)
        PL_I_7_18.SelectColor2 = AlarmArray(2)
        PL_I_7_19.SelectColor2 = AlarmArray(3)
        PL_I_7_20.SelectColor2 = AlarmArray(4)
        PL_I_7_21.SelectColor2 = AlarmArray(5)
        PL_I_7_22.SelectColor2 = AlarmArray(6)
        PL_I_7_23.SelectColor2 = AlarmArray(7)
        PL_I_7_24.SelectColor2 = AlarmArray(8)
        PL_I_7_25.SelectColor2 = AlarmArray(9)
        PL_I_7_26.SelectColor2 = AlarmArray(10)
        PL_I_7_27.SelectColor2 = AlarmArray(11)
        PL_I_7_28.SelectColor2 = AlarmArray(12)
        PL_I_7_29.SelectColor2 = AlarmArray(13)
        PL_I_7_30.SelectColor2 = AlarmArray(14)
        PL_I_7_31.SelectColor2 = AlarmArray(15)
    End Sub
    Private Sub Input_Update_CARD8(sender As Object, e As EventArgs) Handles Card8.ValueChanged
        Dim AlarmArray() As Boolean = IntToBin(sender.value)
        PL_O_8_0.SelectColor2 = AlarmArray(0)
        PL_O_8_1.SelectColor2 = AlarmArray(1)
        PL_O_8_2.SelectColor2 = AlarmArray(2)
        PL_O_8_3.SelectColor2 = AlarmArray(3)
        PL_O_8_4.SelectColor2 = AlarmArray(4)
        PL_O_8_5.SelectColor2 = AlarmArray(5)
        PL_O_8_6.SelectColor2 = AlarmArray(6)
        PL_O_8_7.SelectColor2 = AlarmArray(8)

    End Sub
    Private Sub Input_Update_CARD6(sender As Object, e As EventArgs) Handles Card6.ValueChanged
        Dim AlarmArray() As Boolean = IntToBin(sender.value)
        PL_O_6_0.SelectColor2 = AlarmArray(0)
        PL_O_6_1.SelectColor2 = AlarmArray(1)
        PL_O_6_2.SelectColor2 = AlarmArray(2)
        PL_O_6_3.SelectColor2 = AlarmArray(3)
        PL_O_6_4.SelectColor2 = AlarmArray(4)
        PL_O_6_5.SelectColor2 = AlarmArray(5)
        PL_O_6_6.SelectColor2 = AlarmArray(6)
        PL_O_6_7.SelectColor2 = AlarmArray(6)

    End Sub

    '║                                                                                                              ║
    '╠══════════════════════════════════════════════════════════════════════════════════════════════════════════════╣
    '║                                                  [SYSTEM]                                                    ║
    '║                                                 Section End                                                  ║
    '╚══════════════════════════════════════════════════════════════════════════════════════════════════════════════╝
#End Region
#Region "Weather Logic"
    '╔══════════════════════════════════════════════════════════════════════════════════════════════════════════════╗
    '║                                                  [WEATHER]                                                   ║
    '║                                                Section Start                                                 ║
    '╠══════════════════════════════════════════════════════════════════════════════════════════════════════════════╣
    '║                                                                                                              ║
    Dim weatherIconPath As String() = {"https://openweathermap.org/img/wn/", "@2x.png"}
    Public Class Coord
        Public Property lon As Double
        Public Property lat As Double
    End Class
    Public Class Weather
        Public Property id As Integer
        Public Property main As String
        Public Property description As String
        Public Property icon As String
    End Class
    Public Class Main
        Public Property temp As Double
        Public Property feels_like As Double
        Public Property temp_min As Double
        Public Property temp_max As Double
        Public Property pressure As Integer
        Public Property humidity As Integer
        Public Property sea_level As Integer
        Public Property grnd_level As Integer
    End Class
    Public Class Wind
        Public Property speed As Double
        Public Property deg As Integer
        Public Property gust As Double
    End Class
    Public Class Clouds
        Public Property all As Integer
    End Class

    Private Sub SP_LevelCheck(sender As Object, e As EventArgs) Handles STMPDEM_Min.Click, STMPDEM_Max.Click, STMP_Min.Click, STMP_Max.Click, STLPDEM_Min.Click, STLPDEM_Max.Click, STLP_Min.Click, STLP_Max.Click, STHP_Min.Click, STHP_Max.Click, STFWP_Min.Click, STFWP_Max.Click, STFL_Min.Click, STFL_Max.Click, HWTP_Min.Click, HWTP_Max.Click, HWPRP_Min.Click, HWPRP_Max.Click, HWPOP_Min.Click, HWPOP_Max.Click, HWFL_Min.Click, HWFL_Max.Click, HWFD_Min.Click, HWFD_Max.Click, ELSC_Min.Click, ELSC_Max.Click, ELSB_Min.Click, ELSB_Max.Click, ELSA_Min.Click, ELSA_Max.Click, ELNC_Min.Click, ELNC_Max.Click, ELNB_Min.Click, ELNB_Max.Click, ELNA_Min.Click, ELNA_Max.Click, CWSUP_Min.Click, CWSUP_Max.Click, CWPRP_Min.Click, CWPRP_Max.Click, CWPOP_Min.Click, CWPOP_Max.Click, CWFD_Min.Click, CWFD_Max.Click, ALP_Min.Click, ALP_Max.Click

    End Sub

    Public Class Sys
        Public Property type As Integer
        Public Property id As Integer
        Public Property country As String
        Public Property sunrise As Long
        Public Property sunset As Long
    End Class
    Public Class Root_Weather
        Public Property coord As Coord
        Public Property weather As List(Of Weather)
        Public Property [base] As String
        Public Property main As Main
        Public Property visibility As Integer
        Public Property wind As Wind
        Public Property clouds As Clouds
        Public Property dt As Long
        Public Property sys As Sys
        Public Property timezone As Integer
        Public Property id As Integer
        Public Property name As String
        Public Property cod As Integer
    End Class
    Dim weatherData As Root_Weather
    Sub UpdateWeatherData()
        RefreshWeatherLabel.Text = "Last Update: " & Now & " Click To Refresh"
        Dim url As String = "http://api.openweathermap.org/data/2.5/weather?q=Brockville,Ontario&APPID=63c81ee7feb31a28a3f692ef0357c5c5"
        Dim jsonData As String = GetJsonData(url)
        weatherData = JsonConvert.DeserializeObject(Of Root_Weather)(jsonData)
        weatherData.weather.Sort(Function(x, y) x.id.CompareTo(y.id))
        RefreshWeather()
    End Sub

    Sub RefreshWeather()

        TempLBL.Text = Format((KtoC(weatherData.main.temp)), "0.0") & " °C"
        TempFeelsLBL.Text = Format((KtoC(weatherData.main.feels_like)), "0.0") & " °C Feels"
        TempMinLBL.Text = Format((KtoC(weatherData.main.temp_min)), "0") & " °C Min"
        TempMaxLBL.Text = Format((KtoC(weatherData.main.temp_max)), "0") & " °C Max"
        HumLBL.Text = weatherData.main.humidity & " % RH"
        PressLBL.Text = weatherData.main.pressure & " KPa"
        WindSpeedLBL.Text = weatherData.wind.speed & " KM/H"
        WindDegLBL.Text = weatherData.wind.deg & "° dN"
        WindGustLBL.Text = weatherData.wind.gust & " KPH Gusts"
        CloudLBL.Text = weatherData.clouds.all & " % Cover"
        MainLBL.Text = weatherData.weather(0).main
        DescLBL.Text = weatherData.weather(0).description

    End Sub
    Function GetJsonData(ByVal url As String) As String
        Using client As New WebClient()
            Return client.DownloadString(url)
        End Using
    End Function

    Function KtoC(ByVal kel) As Double
        Return (kel - 273.15)
    End Function

    Private Sub Dashboard_Page_DoubleClick(sender As Object, e As EventArgs) Handles Me.DoubleClick
        RefreshAlarmTable()

    End Sub

    '║                                                                                                              ║
    '╠══════════════════════════════════════════════════════════════════════════════════════════════════════════════╣
    '║                                                  [WEATHER]                                                   ║
    '║                                                 Section End                                                  ║
    '╚══════════════════════════════════════════════════════════════════════════════════════════════════════════════╝
#End Region
End Class
#Region "TEMPLATES"
'Program Section TitleBlock Template

'ASCII Banner Link: https://manytools.org/hacker-tools/ascii-banner/

'╔══════════════════════════════════════════════════════════════════════════════════════════════════════════════╗
'║Font: Rowan Cap                                                                                               ║
'║Vert: Normal                                                                                                  ║
'║Horz: Normal                                                                                                  ║                                          ║
'╠══════════════════════════════════════════════════════════════════════════════════════════════════════════════╣
'║                                                                                                              ║


'║                                                                                                              ║
'╠══════════════════════════════════════════════════════════════════════════════════════════════════════════════╣
'║                                                   [TITLE]                                                    ║
'║                                                 Section End                                                  ║
'╚══════════════════════════════════════════════════════════════════════════════════════════════════════════════╝
#End Region
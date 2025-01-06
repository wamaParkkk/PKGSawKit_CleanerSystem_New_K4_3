namespace PKGSawKit_CleanerSystem_New_K4_3
{
    public enum MODULE
    {
        _PM1 = 0,
        _PM2 = 1,
        _MOTOR = 2
    }

    public enum Page
    {
        LogInPage = 0,
        OperationPage = 1,
        MaintnancePage = 2,
        RecipePage = 3,
        ConfigurePage = 4,
        IOPage = 5,
        AlarmPage = 6,
        EventLogPage = 7,
        UserRegist = 8
    }

    public enum DigitalOffOn
    {
        Off = 0,
        On = 1
    }

    public enum DigitalOnOff
    {
        On = 0,
        Off = 1
    }

    public enum RecipeEditMode : byte
    {
        NORMAL_MODE = 0,
        VIEW_MODE = 1,
        EDIT_MODE = 2
    }

    public struct TDigSet
    {
        public string[] curDigSet;
    }

    // 공정 진행 시 화면에 표시해줄 내용
    public struct TPrcsInfo
    {
        public string[] prcsRecipeName;
        public int[] prcsCurrentStep;
        public int[] prcsTotalStep;
        public string[] prcsStepName;
        public double[] prcsStepCurrentTime;
        public double[] prcsStepTotalTime;
        public string[] prcsEndTime;
    }

    // IO LIST /////////////////////////////////////////////
    public enum DigInputList
    {
        Spare00_i = 0,
        Spare01_i = 1,

        CH1_Brush_Fwd_i = 2,
        CH1_Brush_Bwd_i = 3,
        CH1_Brush_Home_i = 4,
        CH1_Nozzle_Fwd_i = 5,
        CH1_Nozzle_Bwd_i = 6,

        Spare07_i = 7,
        Spare08_i = 8,

        Water_Level_High_i = 9,
        Water_Level_Low_i = 10,

        CH1_Door_Sensor_i = 11,
        CH2_Door_Sensor_i = 12,

        Spare13_i = 13,
        Spare14_i = 14,
        Spare15_i = 15,
        Spare16_i = 16,
        Spare17_i = 17,

        CH2_Nozzle_Fwd_i = 18,
        CH2_Nozzle_Bwd_i = 19,
        CH2_Nozzle_Home_i = 20,

        EMO_Front_i = 21,
        EMO_Rear_i = 22,        
        Front_Door_Sensor_i = 23,
        Left_Door_Sensor_i = 24,
        Right_Door_Sensor_i = 25,
        Back_Door_Sensor_i = 26,

        Spare27_i = 27,
        Spare28_i = 28,
        Spare29_i = 29,
        Spare30_i = 30,
        Spare31_i = 31,        
    }

    public enum DigOutputList
    {
        Spare00_o = 0,
        Spare01_o = 1,

        CH1_WaterValve_Top_o = 2,
        CH1_AirValve_Top_o = 3,
        CH1_AirValve_Bot_o = 4,        
        CH1_Curtain_AirValve_o = 5,

        Spare6_o = 6,
        Spare7_o = 7,

        CH1_Nozzle_FwdBwd_o = 8,

        Spare9_o = 9,
        
        CH1_Brush_FwdBwd_o = 10,
        
        Spare11_o = 11,
        
        CH1_Nozzle_Pwr_o = 12,
        CH1_Brush_Pwr_o = 13,

        Spare14_o = 14,

        Hot_Water_Pump_o = 15,
        Hot_WaterHeater_o = 16,
        Main_Water_Supply = 17,

        CH2_WaterValve_Top_o = 18,
        CH2_AirValve_Top_o = 19,
        CH2_AirValve_Bot_o = 20,        
        CH2_Curtain_AirValve_o = 21,

        Spare22_o = 22,              
        Spare23_o = 23,

        CH2_Nozzle_FwdBwd_o = 24,

        Spare25_o = 25,

        CH2_Nozzle_Pwr_o = 26,        

        Tower_Lamp_Red_o = 27,
        Tower_Lamp_Yellow_o = 28,
        Tower_Lamp_Green_o = 29,                
        Buzzer_o = 30,
        FluorescentLamp_o = 31,
    }
    ////////////////////////////////////////////////////////

    // IO (String to int)///////////////////////////////////
    public static class IO_StrToInt
    {
        private static string _io_Name = "";

        public static string io_code
        {
            get
            {
                if (string.IsNullOrEmpty(_io_Name))
                {
                    _io_Name = "IO Name is null";
                }

                return _io_Name;
            }
            set
            {
                if      (value == "0")      _io_Name = "Spare00_o";              
                else if (value == "1")      _io_Name = "Spare01_o";

                else if (value == "2")      _io_Name = "CH1_WaterValve_Top_o";
                else if (value == "3")      _io_Name = "CH1_AirValve_Top_o";
                else if (value == "4")      _io_Name = "CH1_AirValve_Bot_o";
                else if (value == "5")      _io_Name = "CH1_Curtain_AirValve_o";

                else if (value == "6")      _io_Name = "Spare06_o";                
                else if (value == "7")      _io_Name = "Spare07_o";

                else if (value == "8")      _io_Name = "CH1_Nozzle_FwdBwd_o";

                else if (value == "9")      _io_Name = "Spare09_o";

                else if (value == "10")     _io_Name = "CH1_Brush_FwdBwd_o";

                else if (value == "11")     _io_Name = "Spare11_o";

                else if (value == "12")     _io_Name = "CH1_Nozzle_Pwr_o";
                else if (value == "13")     _io_Name = "CH1_Brush_Pwr_o";

                else if (value == "14")     _io_Name = "Spare14_o";

                else if (value == "15")     _io_Name = "Hot_Water_Pump_o";
                else if (value == "16")     _io_Name = "Hot_WaterHeater_o";
                else if (value == "17")     _io_Name = "Main_Water_Supply";

                else if (value == "18")     _io_Name = "CH2_WaterValve_Top_o";
                else if (value == "19")     _io_Name = "CH2_AirValve_Top_o";
                else if (value == "20")     _io_Name = "CH2_AirValve_Bot_o";
                else if (value == "21")     _io_Name = "CH2_Curtain_AirValve_o";

                else if (value == "22")     _io_Name = "Spare22_o";
                else if (value == "23")     _io_Name = "Spare23_o";

                else if (value == "24")     _io_Name = "CH2_Nozzle_FwdBwd_o";

                else if (value == "25")     _io_Name = "Spare25_o";

                else if (value == "26")     _io_Name = "CH2_Nozzle_Pwr_o";

                else if (value == "27")     _io_Name = "Tower_Lamp_Red_o";
                else if (value == "28")     _io_Name = "Tower_Lamp_Yellow_o";
                else if (value == "29")     _io_Name = "Tower_Lamp_Green_o";
                else if (value == "30")     _io_Name = "Buzzer_o";
                else if (value == "31")     _io_Name = "FluorescentLamp_o";
            }
        }
    }
    ////////////////////////////////////////////////////////

    // ALARM LIST //////////////////////////////////////////
    public class Alarm_List
    {
        private string _alarm_Name = "";

        public string alarm_code
        {
            get
            {
                if (string.IsNullOrEmpty(_alarm_Name))
                {
                    _alarm_Name = "Alarm name is missing";
                }

                return _alarm_Name;
            }
            set
            {
                if (value == "900")         _alarm_Name = "Tool does not exist";
                
                else if (value == "1000")   _alarm_Name = "Door open time out";
                else if (value == "1001")   _alarm_Name = "Door close time out";
                
                else if (value == "1010")   _alarm_Name = "Failed to read recipe file";

                else if (value == "1020")   _alarm_Name = "Nozzle cylinder forward time out";
                else if (value == "1021")   _alarm_Name = "Nozzle cylinder backward time out";
                else if (value == "1022")   _alarm_Name = "Nozzle cylinder home time out";

                else if (value == "1030")   _alarm_Name = "Brush cylinder forward time out";
                else if (value == "1031")   _alarm_Name = "Brush cylinder backward time out";
                else if (value == "1032")   _alarm_Name = "Brush cylinder home time out";

                else if (value == "1040")   _alarm_Name = "Brush up time out (Motor)";
                else if (value == "1041")   _alarm_Name = "Brush down time out (Motor)";
                else if (value == "1042")   _alarm_Name = "Brush home time out (Motor)";

                else if (value == "1045")   _alarm_Name = "Brush rotation run time out (Motor)";
                else if (value == "1046")   _alarm_Name = "Brush rotation stop time out (Motor)";
            }
        }
    }
    ////////////////////////////////////////////////////////

    // CONFIGURE LIST //////////////////////////////////////
    public class Configure_List
    {
        // System        
        public static int Brush_Rotation_Timeout = 0;
        public static int Brush_FwdBwd_Timeout = 0;
        public static int Nozzle_FwdBwd_Timeout = 0;
        public static double Heater_TempSet = 0.0;

        // Motion parameter (공정 진행 시)
        public static double Brush_Rotation_Speed = 0;                
    }
    ////////////////////////////////////////////////////////
    
    class Define
    {
        public const int BUFSIZ = 512;
        public const int MODULE_MAX = 2;
        public const int CH_MAX = 32;
        public const int RECIPE_MAX_STEP = 50;

        // Login 여부
        public static bool bLogin = false;

        // User info
        public static string UserId = "";
        public static string UserName = "";
        public static string UserLevel = "";

        // Eventlog 발생 여부
        public static bool bPM1Event;
        public static bool bPM2Event;        

        public static bool bPM1AlmEvent;
        public static bool bPM2AlmEvent;        
        public static bool bPM1OpAlmEvent;
        public static bool bPM2OpAlmEvent;               


        public static bool bOpActivate = false;
        public static byte currentPage = 0;
        public static byte MaintCurrentPage = 0;
        public static byte RecipeCurrentPage = 0;

        public static bool bInterlockRelease = false;
        public static string sInterlockMsg = "";        
        public static string sInterlockChecklist = "";
        public static bool bDoorAutoRelease = false;
        public static bool bSimulation = false;
        public static bool bManualLamp = false;
        

        // Sequence에서 사용 할 변수
        // PM1, PM2 Process seq//////////////////////////
        public static byte[] seqMode = { 0, 0 };
        public static byte[] seqCtrl = { 0, 0 };
        public static byte[] seqSts = { 0, 0 };

        public const byte MODE_IDLE = 0;
        public const byte MODE_PROCESS = 1;
        public const byte MODE_INIT = 2;

        public const byte CTRL_IDLE = 0;
        public const byte CTRL_RUN = 1;
        public const byte CTRL_RUNNING = 2;
        public const byte CTRL_ALARM = 3;
        public const byte CTRL_RETRY = 4;
        public const byte CTRL_HOLD = 5;
        public const byte CTRL_WAIT = 6;
        public const byte CTRL_ABORT = 7;

        public const byte STS_IDLE = 0;
        public const byte STS_PROCESS_ING = 1;
        public const byte STS_PROCESS_END = 2;
        public const byte STS_INIT_ING = 3;
        public const byte STS_INIT_END = 4;
        public const byte STS_ABORTOK = 5;        
        /////////////////////////////////////////////////

        // PM1, PM2 Nozzle fwd/bwd seq //////////////////
        public static byte[] seqCylinderMode = { 0, 0 };
        public static byte[] seqCylinderCtrl = { 0, 0 };
        public static byte[] seqCylinderSts = { 0, 0 };

        public const byte MODE_CYLINDER_IDLE = 0;
        public const byte MODE_CYLINDER_RUN = 1;
        public const byte MODE_CYLINDER_HOME = 2;
        public const byte MODE_CYLINDER_FWD = 3;
        public const byte MODE_CYLINDER_BWD = 4;

        public const byte STS_CYLINDER_IDLE = 0;
        public const byte STS_CYLINDER_RUNING = 1;
        public const byte STS_CYLINDER_RUNEND = 2;
        public const byte STS_CYLINDER_HOMEING = 3;
        public const byte STS_CYLINDER_HOMEEND = 4;
        public const byte STS_CYLINDER_FWDING = 5;
        public const byte STS_CYLINDER_FWDEND = 6;
        public const byte STS_CYLINDER_BWDING = 7;
        public const byte STS_CYLINDER_BWDEND = 8;
        public const byte STS_CYLINDER_ABORTOK = 9;
        /////////////////////////////////////////////////

        // PM1 Brush fwd/bwd seq ////////////////////////
        public static byte seqBrushFwBwMode;
        public static byte seqBrushFwBwCtrl;
        public static byte seqBrushFwBwSts;

        public const byte MODE_BRUSH_FWBW_IDLE = 0;
        public const byte MODE_BRUSH_FWBW_RUN = 1;
        public const byte MODE_BRUSH_FWBW_HOME = 2;
        public const byte MODE_BRUSH_FWBW_FWD = 3;
        public const byte MODE_BRUSH_FWBW_BWD = 4;

        public const byte STS_BRUSH_FWBW_IDLE = 0;
        public const byte STS_BRUSH_FWBW_RUNING = 1;
        public const byte STS_BRUSH_FWBW_RUNEND = 2;
        public const byte STS_BRUSH_FWBW_HOMEING = 3;
        public const byte STS_BRUSH_FWBW_HOMEEND = 4;
        public const byte STS_BRUSH_FWBW_FWDING = 5;
        public const byte STS_BRUSH_FWBW_FWDEND = 6;
        public const byte STS_BRUSH_FWBW_BWDING = 7;
        public const byte STS_BRUSH_FWBW_BWDEND = 8;
        public const byte STS_BRUSH_FWBW_ABORTOK = 9;
        /////////////////////////////////////////////////


        // Recipe 선택 관련 변수
        public static int iSelectRecipeModule;                        // 선택 된 Module
        public static string[] sSelectRecipeName = { null, null };    // 선택 된 Recipe name

        // 알람 name
        public static string sAlarmName;

        // Daily count
        public static int iPM1DailyCnt;
        public static int iPM2DailyCnt;

        // SPH : 2대 * 3(1대 기준 시간 당 최대 공정 tool 갯수)
        public const int iCapa = 108;   // SPH * 24hour * 0.75(가동률 75%)

        // 가동 시간
        public static double dTodayRunTime;
        public const int iSemiAutoTime = 86400; // 24h * 60m * 60s

        // Chamber Enable/Disable
        public static bool[] bChamberDisable = { false, false };

        // Tool check
        public static bool[] bDontCheckTool = { false, false };        

        // Motor axis        
        public const int axis_r = 0;   // Brush rotation axis

        // PKG saw-kit barcode
        public static string[] strToolBarcode = { string.Empty, string.Empty };

        // Heater controller
        public static double temp_PV = 0.0;
        public static double temp_SV = 0.0;
    }    
}

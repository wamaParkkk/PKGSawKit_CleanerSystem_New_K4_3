using Ajin_IO_driver;
using Ajin_motion_driver;
using MsSqlManagerLibrary;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace PKGSawKit_CleanerSystem_New_K4_3
{
    public struct TStep
    {
        public bool Flag;
        public byte Layer;
        public double Times;

        public void INC_TIMES()
        {
            Times++;
            Thread.Sleep(990);
        }

        public void INC_TIMES_10()
        {
            Times += 0.01;
        }

        public void INC_TIMES_100()
        {
            Times += 0.1;
        }
    }

    public class TBaseThread
    {
        public byte module;
        public string ModuleName;

        public TStep step;
    }

    class Global
    {
        public static string userdataPath = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\UserData.accdb"));
        public static string logfilePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\EventLog\"));
        public static string alarmHistoryPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\AlarmHistory\"));
        public static string RecipeFilePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\Recipes\"));
        public static string ConfigurePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\Configure\"));
        public static string serialPortInfoPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\SerialComm\"));
        public static string dailyCntfilePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\DailyCount\"));

        public static string hostEquipmentInfo = "K5EE_PKGsawCleaningSystem";
        public static string hostEquipmentInfo_Log = "K5EE_PKGsawCleaningSystemLog";

        private static Timer timer = new Timer();

        public static TDigSet digSet;
        public static TPrcsInfo prcsInfo;

        private static InterlockDisplayForm interlockDisplayForm;
        private static uint nSeqWaitCnt = 0;

        static string sendMsg_System = "Idle";
        static string sendMsg_Water = "Idle";        

        #region 이벤트로그 파일 폴더 및 파일 생성       
        public static void EventLog(string Msg, string moduleName, string Mode)
        {
            string sYear = string.Format("{0:yyyy}", DateTime.Now).Trim();
            string sMonth = string.Format("{0:MM}", DateTime.Now).Trim();
            string sDay = string.Format("{0:dd}", DateTime.Now).Trim();            
            string sDate = string.Format("{0}-{1}-{2}", sYear, sMonth, sDay);
            string sTime = DateTime.Now.ToString("HH:mm:ss");
            string sDateTime;            
            sDateTime = string.Format("[{0}, {1}] ", sDate, sTime);
            
            WriteFile(string.Format("{0}{1}", sDateTime, Msg), moduleName, Mode);

            if (Mode == "Event")
            {
                if (moduleName == "PM1")
                {
                    Define.bPM1Event = true;
                }

                if (moduleName == "PM2")
                {
                    Define.bPM2Event = true;
                }                
            }
            else if (Mode == "Alarm")
            {
                if (moduleName == "PM1")
                {
                    Define.bPM1OpAlmEvent = true;
                    Define.bPM1AlmEvent = true;
                }

                if (moduleName == "PM2")
                {
                    Define.bPM2OpAlmEvent = true;
                    Define.bPM2AlmEvent = true;
                }                
            }            
        }

        private static void WriteFile(string Msg, string moduleName, string Mode)
        {            
            string sYear = string.Format("{0:yyyy}", DateTime.Now).Trim();
            string sMonth = string.Format("{0:MM}", DateTime.Now).Trim();
            string sDay = string.Format("{0:dd}", DateTime.Now).Trim();            
            string FileName = string.Format("{0}.txt", sDay);
            string sPath = string.Empty;
            if (Mode == "Event")
            {
                sPath = logfilePath;
            }                
            else if (Mode == "Alarm")
            {
                sPath = alarmHistoryPath;
            }

            try
            {                
                if (!Directory.Exists(string.Format("{0}{1}\\{2}", sPath, moduleName, sYear)))
                {
                    CreateYearFolder(string.Format("{0}{1}", sPath, moduleName));
                }
                
                if (!Directory.Exists(string.Format("{0}{1}\\{2}\\{3}", sPath, moduleName, sYear, sMonth)))
                {
                    CreateMonthFolder(string.Format("{0}{1}", sPath, moduleName));
                }
                
                if (File.Exists(string.Format("{0}{1}\\{2}\\{3}\\{4}", sPath, moduleName, sYear, sMonth, FileName)))
                {
                    StreamWriter writer;                    
                    writer = File.AppendText(string.Format("{0}{1}\\{2}\\{3}\\{4}", sPath, moduleName, sYear, sMonth, FileName));
                    writer.WriteLine(Msg);
                    writer.Close();
                }
                else
                {                    
                    CreateFile(string.Format("{0}{1}", sPath, moduleName), Msg);

                    StreamWriter writer;                    
                    writer = File.AppendText(string.Format("{0}{1}\\{2}\\{3}\\{4}", sPath, moduleName, sYear, sMonth, FileName));
                    writer.WriteLine(Msg);
                    writer.Close();
                }
            }
            catch (IOException)
            {
                
            }
        }

        private static void CreateYearFolder(string Path)
        {
            string sYear = string.Format("{0:yyyy}", DateTime.Now).Trim();
            string FolderName = sYear;
            
            Directory.CreateDirectory(string.Format("{0}\\{1}", Path, FolderName));
        }

        private static void CreateMonthFolder(string Path)
        {
            string sYear = string.Format("{0:yyyy}", DateTime.Now).Trim();
            string sMonth = string.Format("{0:MM}", DateTime.Now).Trim();
            string FolderName = sMonth;
            
            Directory.CreateDirectory(string.Format("{0}\\{1}\\{2}", Path, sYear, FolderName));
        }

        private static void CreateFile(string Path, string Msg)
        {
            StreamWriter writer;

            string sYear = string.Format("{0:yyyy}", DateTime.Now).Trim();
            string sMonth = string.Format("{0:MM}", DateTime.Now).Trim();
            string sDay = string.Format("{0:dd}", DateTime.Now).Trim();            
            string FileName = string.Format("{0}.txt", sDay);
            
            if (!File.Exists(string.Format("{0}\\{1}\\{2}\\{3}", Path, sYear, sMonth, FileName)))
            {                
                using (File.Create(string.Format("{0}\\{1}\\{2}\\{3}", Path, sYear, sMonth, FileName))) ;
            }
        }
        #endregion

        #region Daily count 폴더 및 파일 생성
        public static void DailyLog(int iCnt, string moduleName)
        {
            string sYear = string.Format("{0:yyyy}", DateTime.Now).Trim();
            string sMonth = string.Format("{0:MM}", DateTime.Now).Trim();
            string sDay = string.Format("{0:dd}", DateTime.Now).Trim();
            string FileName = string.Format("{0}.txt", sDay);
            string sPath = dailyCntfilePath;

            try
            {
                if (!Directory.Exists(string.Format("{0}{1}\\{2}", sPath, moduleName, sYear)))                
                {
                    CreateYearFolder(string.Format("{0}{1}", sPath, moduleName));                    
                }

                if (!Directory.Exists(string.Format("{0}{1}\\{2}\\{3}", sPath, moduleName, sYear, sMonth)))                
                {
                    CreateMonthFolder(string.Format("{0}{1}", sPath, moduleName));                    
                }

                if (File.Exists(string.Format("{0}{1}\\{2}\\{3}\\{4}", sPath, moduleName, sYear, sMonth, FileName)))                
                {
                    StreamWriter writer;
                    writer = File.CreateText(string.Format("{0}{1}\\{2}\\{3}\\{4}", sPath, moduleName, sYear, sMonth, FileName));
                    writer.Write(iCnt);
                    writer.Close();
                }
                else
                {
                    CreateFile(sPath + moduleName, "");

                    StreamWriter writer;                    
                    writer = File.CreateText(string.Format("{0}{1}\\{2}\\{3}\\{4}", sPath, moduleName, sYear, sMonth, FileName));
                    writer.Write(iCnt);
                    writer.Close();
                }
            }
            catch (IOException)
            {

            }
        }
        #endregion

        public static void Init()
        {
            digSet.curDigSet = new string[64];
            for (int i = 0; i < 64; i++)
            {
                digSet.curDigSet[i] = DIOClass.doVal.readDigOut[i];
            }

            prcsInfo.prcsRecipeName = new string[Define.MODULE_MAX];
            prcsInfo.prcsCurrentStep = new int[Define.MODULE_MAX];
            prcsInfo.prcsTotalStep = new int[Define.MODULE_MAX];
            prcsInfo.prcsStepName = new string[Define.MODULE_MAX];
            prcsInfo.prcsStepCurrentTime = new double[Define.MODULE_MAX];
            prcsInfo.prcsStepTotalTime = new double[Define.MODULE_MAX];
            prcsInfo.prcsEndTime = new string[Define.MODULE_MAX];

            for (int nModuleCnt = 0; nModuleCnt < Define.MODULE_MAX; nModuleCnt++)
            {
                prcsInfo.prcsRecipeName[nModuleCnt] = string.Empty;
                prcsInfo.prcsCurrentStep[nModuleCnt] = 0;
                prcsInfo.prcsTotalStep[nModuleCnt] = 0;
                prcsInfo.prcsStepName[nModuleCnt] = string.Empty;
                prcsInfo.prcsStepCurrentTime[nModuleCnt] = 1;
                prcsInfo.prcsStepTotalTime[nModuleCnt] = 0;
                prcsInfo.prcsEndTime[nModuleCnt] = string.Empty;
            }

            interlockDisplayForm = new InterlockDisplayForm();            

            timer.Interval = 100;
            timer.Elapsed += new ElapsedEventHandler(VALUE_INTERLOCK_CHECK);
            timer.Start();

            /*
            string strRtn = HostConnection.Connect();
            if (strRtn == "OK")
            {
                HostConnection.Host_Set_SystemStatus(hostEquipmentInfo, "System", "Idle");                

                HostConnection.Host_Set_RunStatus(hostEquipmentInfo, "PM1", "Idle");
                HostConnection.Host_Set_RunStatus(hostEquipmentInfo, "PM2", "Idle");                

                HostConnection.Host_Set_RecipeName(hostEquipmentInfo, "PM1", "");
                HostConnection.Host_Set_RecipeName(hostEquipmentInfo, "PM2", "");                

                HostConnection.Host_Set_AlarmName(hostEquipmentInfo, "PM1", "");
                HostConnection.Host_Set_AlarmName(hostEquipmentInfo, "PM2", "");                

                HostConnection.Host_Set_ProgressTime(hostEquipmentInfo, "PM1", "0/0");
                HostConnection.Host_Set_ProgressTime(hostEquipmentInfo, "PM2", "0/0");                

                HostConnection.Host_Set_ProcessEndTime(hostEquipmentInfo, "PM1", "");
                HostConnection.Host_Set_ProcessEndTime(hostEquipmentInfo, "PM2", "");                                                

                GetDailyLogCount("PM1");
                GetDailyLogCount("PM2");
            }
            else
            {
                MessageBox.Show("EE 서버 접속에 실패했습니다", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }     
            */
        }

        public static void GetDailyLogCount(string moduleName)
        {
            string sTmpData;
            string sYear = string.Format("{0:yyyy}", DateTime.Now).Trim();
            string sMonth = string.Format("{0:MM}", DateTime.Now).Trim();
            string sDay = string.Format("{0:dd}", DateTime.Now).Trim();            
            string FileName = string.Format("{0}.txt", sDay);            
            string sPath = string.Format("{0}{1}\\{2}\\{3}\\{4}", dailyCntfilePath, moduleName, sYear, sMonth, FileName);

            if (File.Exists(sPath))
            {
                byte[] bytes;
                using (var fs = File.Open(sPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, (int)fs.Length);
                    sTmpData = Encoding.Default.GetString(bytes);

                    char sp = ',';
                    string[] spString = sTmpData.Split(sp);
                    for (int i = 0; i < spString.Length; i++)
                    {
                        if (moduleName == "PM1")
                        {
                            Define.iPM1DailyCnt = int.Parse(spString[0]);
                        }
                        else if (moduleName == "PM2")
                        {
                            Define.iPM2DailyCnt = int.Parse(spString[0]);                            
                        }                        
                    }
                }
            }
        }

        public static string GetDigValue(int ioName)
        {
            try
            {
                if ((0 <= ioName) && (15 >= ioName))
                {
                    if (DIOClass.diVal.checkHigh[ioName] != null)
                    {
                        return DIOClass.diVal.checkHigh[ioName];
                    }
                    else
                    {
                        return "Off";
                    }
                }
                else if ((16 <= ioName) && (32 >= ioName))
                {
                    if (DIOClass.diVal.checkLow[ioName - 16] != null)
                    {
                        return DIOClass.diVal.checkLow[ioName - 16];
                    }
                    else
                    {
                        return "Off";
                    }
                }
                else
                {
                    return "Off";
                }
            }
            catch (IOException)
            {
                return "Off";
            }
        }        

        public static void SetDigValue(int ioName, uint setValue, string ModuleName)
        {
            try
            {
                string retMsg = string.Empty;

                if (ModuleName == "PM1")
                {
                    if (SETPOINT_INTERLOCK_CHECK1(ioName, setValue, ModuleName, ref retMsg))
                    {
                        if ((0 <= ioName) && (31 >= ioName))
                        {
                            DIOClass.SelectHighIndex(ioName, setValue);
                        }
                        else if ((32 <= ioName) && (63 >= ioName))
                        {
                            DIOClass.SelectHighIndex2(ioName, setValue);
                        }

                        Define.sAlarmName = "";
                        IO_StrToInt.io_code = ioName.ToString();
                        string IO_Name = IO_StrToInt.io_code;
                        if (setValue == 1)
                        {
                            digSet.curDigSet[ioName] = "On";

                            if ((IO_Name == "Tower_Lamp_Red_o") ||
                                (IO_Name == "Tower_Lamp_Yellow_o") ||
                                (IO_Name == "Tower_Lamp_Green_o"))
                            {
                                //
                            }
                            else
                            {
                                EventLog(string.Format("{0} : On", IO_Name), ModuleName, "Event");
                            }
                        }
                        else
                        {
                            digSet.curDigSet[ioName] = "Off";

                            if ((IO_Name == "Tower_Lamp_Red_o") ||
                                (IO_Name == "Tower_Lamp_Yellow_o") ||
                                (IO_Name == "Tower_Lamp_Green_o"))
                            {
                                //
                            }
                            else
                            {
                                EventLog(string.Format("{0} : Off", IO_Name), ModuleName, "Event");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(retMsg, "Interlock");
                    }
                }
                else if (ModuleName == "PM2")
                {
                    if (SETPOINT_INTERLOCK_CHECK2(ioName, setValue, ModuleName, ref retMsg))
                    {
                        if ((0 <= ioName) && (31 >= ioName))
                        {
                            DIOClass.SelectHighIndex(ioName, setValue);
                        }
                        else if ((32 <= ioName) && (63 >= ioName))
                        {
                            DIOClass.SelectHighIndex2(ioName, setValue);
                        }

                        Define.sAlarmName = "";
                        IO_StrToInt.io_code = ioName.ToString();
                        string IO_Name = IO_StrToInt.io_code;
                        if (setValue == 1)
                        {
                            digSet.curDigSet[ioName] = "On";

                            if ((IO_Name == "Tower_Lamp_Red_o") ||
                                (IO_Name == "Tower_Lamp_Yellow_o") ||
                                (IO_Name == "Tower_Lamp_Green_o"))
                            {
                                //
                            }
                            else
                            {
                                EventLog(string.Format("{0} : On", IO_Name), ModuleName, "Event");
                            }
                        }
                        else
                        {
                            digSet.curDigSet[ioName] = "Off";

                            if ((IO_Name == "Tower_Lamp_Red_o") ||
                                (IO_Name == "Tower_Lamp_Yellow_o") ||
                                (IO_Name == "Tower_Lamp_Green_o"))
                            {
                                //
                            }
                            else
                            {
                                EventLog(string.Format("{0} : Off", IO_Name), ModuleName, "Event");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(retMsg, "Interlock");
                    }
                }
                
            }
            catch (IOException)
            {

            }
        }

        #region 항시 체크 인터락
        private static void VALUE_INTERLOCK_CHECK(object sender, ElapsedEventArgs e)
        {
            // Interlock이 해제 상태인지 체크
            if (!Define.bInterlockRelease)
            {
                if ((GetDigValue((int)DigInputList.EMO_Front_i) == "Off") ||
                    (GetDigValue((int)DigInputList.EMO_Rear_i) == "Off"))
                {
                    ALL_VALVE_CLOSE();
                    PROCESS_ABORT();

                    SetDigValue((int)DigOutputList.Buzzer_o, (uint)DigitalOffOn.On, "PM1");

                    Define.sInterlockMsg = "Emergency occurrence!";
                    Define.sInterlockChecklist = "Check the emergency switch";

                    DialogResult result = interlockDisplayForm.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        Define.sInterlockMsg = "";
                        Define.sInterlockChecklist = "";
                    }

                    if (sendMsg_System != "Alarm")
                    {
                        //HostConnection.Host_Set_SystemStatus(hostEquipmentInfo, "System", "Alarm");
                        sendMsg_System = "Alarm";
                    }
                }
                else
                {
                    if (sendMsg_System != "Idle")
                    {
                        //HostConnection.Host_Set_SystemStatus(hostEquipmentInfo, "System", "Idle");
                        sendMsg_System = "Idle";
                    }
                }
                /*
                if (GetDigValue((int)DigInputList.Front_Door_Sensor_i) == "Off")
                {
                    if (Define.sInterlockMsg == string.Empty)
                    {
                        Define.sInterlockMsg = "Front door is open!";
                        Define.sInterlockChecklist = "Check the front door sensor";

                        DialogResult result = interlockDisplayForm.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            Define.sInterlockMsg = "";
                            Define.sInterlockChecklist = "";
                        }
                    }
                }
                */
                if (GetDigValue((int)DigInputList.Left_Door_Sensor_i) == "Off")
                {
                    if (Define.sInterlockMsg == string.Empty)
                    {
                        Define.sInterlockMsg = "Left door is open!";
                        Define.sInterlockChecklist = "Check the left door sensor";

                        DialogResult result = interlockDisplayForm.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            Define.sInterlockMsg = "";
                            Define.sInterlockChecklist = "";
                        }
                    }
                }

                if (GetDigValue((int)DigInputList.Right_Door_Sensor_i) == "Off")
                {
                    if (Define.sInterlockMsg == string.Empty)
                    {
                        Define.sInterlockMsg = "Right door is open!";
                        Define.sInterlockChecklist = "Check the right door sensor";

                        DialogResult result = interlockDisplayForm.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            Define.sInterlockMsg = "";
                            Define.sInterlockChecklist = "";
                        }
                    }
                }

                if (GetDigValue((int)DigInputList.Back_Door_Sensor_i) == "Off")
                {
                    if (Define.sInterlockMsg == string.Empty)
                    {
                        Define.sInterlockMsg = "Back door is open!";
                        Define.sInterlockChecklist = "Check the back door sensor";

                        DialogResult result = interlockDisplayForm.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            Define.sInterlockMsg = "";
                            Define.sInterlockChecklist = "";
                        }
                    }
                }

                // Water tank level 센서 체크 ////////////////////////////////////////////////////////////////                
                if (GetDigValue((int)DigInputList.Water_Level_High_i) == "Off")
                {
                    if (nSeqWaitCnt >= 30)     // 3초 대기
                    {
                        if (digSet.curDigSet[(int)DigOutputList.Main_Water_Supply] != "On")
                        {
                            SetDigValue((int)DigOutputList.Main_Water_Supply, (uint)DigitalOffOn.On, "PM1");
                        }

                        nSeqWaitCnt = 0;
                    }
                    else
                    {
                        nSeqWaitCnt++;
                    }
                }
                else
                {
                    if (digSet.curDigSet[(int)DigOutputList.Main_Water_Supply] != "Off")
                    {
                        SetDigValue((int)DigOutputList.Main_Water_Supply, (uint)DigitalOffOn.Off, "PM1");
                    }

                    if (nSeqWaitCnt != 0)
                        nSeqWaitCnt = 0;
                }

                if (GetDigValue((int)DigInputList.Water_Level_Low_i) == "On")   // B접
                {
                    if (digSet.curDigSet[(int)DigOutputList.Hot_WaterHeater_o] != null)
                    {
                        if (digSet.curDigSet[(int)DigOutputList.Hot_WaterHeater_o] != "Off")
                        {
                            SetDigValue((int)DigOutputList.Hot_WaterHeater_o, (uint)DigitalOffOn.Off, "PM1");
                        }
                    }

                    if (Define.sInterlockMsg == string.Empty)
                    {
                        ALL_VALVE_CLOSE();
                        PROCESS_ABORT();

                        SetDigValue((int)DigOutputList.Buzzer_o, (uint)DigitalOffOn.On, "PM1");

                        Define.sInterlockMsg = "There is no water in the tank!";
                        Define.sInterlockChecklist = "Check the water tank sensor and supply valve";

                        DialogResult result = interlockDisplayForm.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            Define.sInterlockMsg = "";
                            Define.sInterlockChecklist = "";
                        }

                        if (sendMsg_Water != "Alarm")
                        {
                            //HostConnection.Host_Set_SystemStatus(hostEquipmentInfo, "WaterTank", "Alarm");
                            sendMsg_Water = "Alarm";
                        }
                    }
                    else
                    {
                        if (sendMsg_Water != "Alarm")
                        {
                            //HostConnection.Host_Set_SystemStatus(hostEquipmentInfo, "WaterTank", "Alarm");
                            sendMsg_Water = "Alarm";
                        }
                    }
                }
                else
                {
                    if (HanyoungNXClassLibrary.Define.temp_PV <= Configure_List.Heater_OverTempSet)
                    {
                        if ((GetDigValue((int)DigInputList.EMO_Front_i) == "On") &&
                            (GetDigValue((int)DigInputList.EMO_Rear_i) == "On"))
                        {
                            if (digSet.curDigSet[(int)DigOutputList.Hot_WaterHeater_o] != null)
                            {
                                if (digSet.curDigSet[(int)DigOutputList.Hot_WaterHeater_o] != "On")
                                {
                                    SetDigValue((int)DigOutputList.Hot_WaterHeater_o, (uint)DigitalOffOn.On, "PM1");
                                }
                            }

                            if (sendMsg_Water != "Idle")
                            {
                                //HostConnection.Host_Set_SystemStatus(hostEquipmentInfo, "WaterTank", "Idle");
                                sendMsg_Water = "Idle";
                            }
                        }
                    }
                    else
                    {
                        if (digSet.curDigSet[(int)DigOutputList.Hot_WaterHeater_o] != null)
                        {
                            if (digSet.curDigSet[(int)DigOutputList.Hot_WaterHeater_o] != "Off")
                            {
                                SetDigValue((int)DigOutputList.Hot_WaterHeater_o, (uint)DigitalOffOn.Off, "PM1");
                            }
                        }

                        if (Define.sInterlockMsg == string.Empty)
                        {
                            SetDigValue((int)DigOutputList.Buzzer_o, (uint)DigitalOffOn.On, "PM1");

                            Define.sInterlockMsg = "Water temperature is high!";
                            Define.sInterlockChecklist = "Check the water heater";

                            DialogResult result = interlockDisplayForm.ShowDialog();
                            if (result == DialogResult.OK)
                            {
                                Define.sInterlockMsg = "";
                                Define.sInterlockChecklist = "";
                            }

                            if (sendMsg_Water != "Alarm")
                            {
                                //HostConnection.Host_Set_SystemStatus(hostEquipmentInfo, "WaterTank", "Alarm");
                                sendMsg_Water = "Alarm";
                            }
                        }
                        else
                        {
                            if (sendMsg_Water != "Alarm")
                            {
                                //HostConnection.Host_Set_SystemStatus(hostEquipmentInfo, "WaterTank", "Alarm");
                                sendMsg_Water = "Alarm";
                            }
                        }
                    }
                }
                ////////////////////////////////////////////////////////////////////////////////////////////////

                // 공정 중 Door open시, 시퀀스 Wait / 모터 Stop
                //_F_DOOR_OPEN_SEQ();
            }


            // CH1~2 Water sol valve open 체크
            if ((digSet.curDigSet[(int)DigOutputList.CH1_WaterValve_Top_o] == "On") ||                
                (digSet.curDigSet[(int)DigOutputList.CH2_WaterValve_Top_o] == "On"))
            {
                if (digSet.curDigSet[(int)DigOutputList.Hot_Water_Pump_o] != "On")
                {
                    SetDigValue((int)DigOutputList.Hot_Water_Pump_o, (uint)DigitalOffOn.On, "PM1");
                }
            }
            else
            {
                if ((Define.seqCtrl[(byte)MODULE._PM1] != Define.CTRL_RUNNING) &&
                    (Define.seqCtrl[(byte)MODULE._PM2] != Define.CTRL_RUNNING))
                {
                    if (digSet.curDigSet[(int)DigOutputList.Hot_Water_Pump_o] != "Off")
                    {
                        SetDigValue((int)DigOutputList.Hot_Water_Pump_o, (uint)DigitalOffOn.Off, "PM1");
                    }
                }
            }
        }

        private static void _F_DOOR_OPEN_SEQ()
        {
            // CH1
            if (GetDigValue((int)DigInputList.CH1_Door_Sensor_i) == "Off")
            {
                if ((Define.seqMode[(byte)MODULE._PM1] == Define.MODE_PROCESS) && (Define.seqCtrl[(byte)MODULE._PM1] == Define.CTRL_RUNNING))
                {                   
                    if (Define.seqCtrl[(byte)MODULE._PM1] != Define.CTRL_WAIT)
                    {
                        Define.seqCtrl[(byte)MODULE._PM1] = Define.CTRL_WAIT;

                        SetDigValue((int)DigOutputList.CH1_Brush_Pwr_o, (uint)DigitalOffOn.Off, "PM1");
                        SetDigValue((int)DigOutputList.CH1_Brush_FwdBwd_o, (uint)DigitalOffOn.Off, "PM1");
                    }                        
                }                

                // 모터는 매뉴얼 동작이라도 멈추게
                if (Define.seqCylinderCtrl[(byte)MODULE._PM1] != Define.CTRL_WAIT)
                    Define.seqCylinderCtrl[(byte)MODULE._PM1] = Define.CTRL_WAIT;

                if (Define.seqBrushFwBwCtrl != Define.CTRL_WAIT)
                    Define.seqBrushFwBwCtrl = Define.CTRL_WAIT;
            }
            else
            {
                if ((Define.seqMode[(byte)MODULE._PM1] == Define.MODE_PROCESS) && (Define.seqCtrl[(byte)MODULE._PM1] == Define.CTRL_WAIT))
                {
                    if (Define.seqCtrl[(byte)MODULE._PM1] != Define.CTRL_RUNNING)
                        Define.seqCtrl[(byte)MODULE._PM1] = Define.CTRL_RUNNING;
                }

                if (Define.seqCylinderCtrl[(byte)MODULE._PM1] == Define.CTRL_WAIT)
                {
                    // Water, Air cylinder
                    if ((Define.seqCylinderMode[(byte)MODULE._PM1] == Define.MODE_CYLINDER_RUN) &&
                        (Define.seqCylinderSts[(byte)MODULE._PM1] == Define.STS_CYLINDER_RUNING))
                    {
                        Define.seqCylinderMode[(byte)MODULE._PM1] = Define.MODE_CYLINDER_RUN;
                        Define.seqCylinderCtrl[(byte)MODULE._PM1] = Define.CTRL_RUN;
                        Define.seqCylinderSts[(byte)MODULE._PM1] = Define.STS_CYLINDER_IDLE;
                    }
                    else if ((Define.seqCylinderMode[(byte)MODULE._PM1] == Define.MODE_CYLINDER_HOME) &&
                             (Define.seqCylinderSts[(byte)MODULE._PM1] == Define.STS_CYLINDER_HOMEING))
                    {
                        Define.seqCylinderMode[(byte)MODULE._PM1] = Define.MODE_CYLINDER_HOME;
                        Define.seqCylinderCtrl[(byte)MODULE._PM1] = Define.CTRL_RUN;
                        Define.seqCylinderSts[(byte)MODULE._PM1] = Define.STS_CYLINDER_IDLE;
                    }
                    else if ((Define.seqCylinderMode[(byte)MODULE._PM1] == Define.MODE_CYLINDER_FWD) &&
                             (Define.seqCylinderSts[(byte)MODULE._PM1] == Define.STS_CYLINDER_FWDING))
                    {
                        Define.seqCylinderMode[(byte)MODULE._PM1] = Define.MODE_CYLINDER_FWD;
                        Define.seqCylinderCtrl[(byte)MODULE._PM1] = Define.CTRL_RUN;
                        Define.seqCylinderSts[(byte)MODULE._PM1] = Define.STS_CYLINDER_IDLE;
                    }
                    else if ((Define.seqCylinderMode[(byte)MODULE._PM1] == Define.MODE_CYLINDER_BWD) &&
                             (Define.seqCylinderSts[(byte)MODULE._PM1] == Define.STS_CYLINDER_BWDING))
                    {
                        Define.seqCylinderMode[(byte)MODULE._PM1] = Define.MODE_CYLINDER_BWD;
                        Define.seqCylinderCtrl[(byte)MODULE._PM1] = Define.CTRL_RUN;
                        Define.seqCylinderSts[(byte)MODULE._PM1] = Define.STS_CYLINDER_IDLE;
                    }

                    // Brush 모터
                    if ((Define.seqBrushFwBwMode == Define.MODE_BRUSH_FWBW_RUN) &&
                        (Define.seqBrushFwBwSts == Define.STS_BRUSH_FWBW_RUNING))
                    {
                        Define.seqBrushFwBwMode = Define.MODE_BRUSH_FWBW_RUN;
                        Define.seqBrushFwBwCtrl = Define.CTRL_RUN;
                        Define.seqBrushFwBwSts = Define.STS_BRUSH_FWBW_IDLE;

                        double dVel = Configure_List.Brush_Rotation_Speed;
                        double dAcc = dVel * 2;
                        double dDec = dVel * 2;

                        MotionClass.MotorJogP(Define.axis_r, dVel, dAcc, dDec);
                    }
                    else if ((Define.seqBrushFwBwMode == Define.MODE_BRUSH_FWBW_HOME) &&
                             (Define.seqBrushFwBwSts == Define.STS_BRUSH_FWBW_HOMEING))
                    {
                        Define.seqBrushFwBwMode = Define.MODE_BRUSH_FWBW_HOME;
                        Define.seqBrushFwBwCtrl = Define.CTRL_RUN;
                        Define.seqBrushFwBwSts = Define.STS_BRUSH_FWBW_IDLE;
                    }
                    else if ((Define.seqBrushFwBwMode == Define.MODE_BRUSH_FWBW_FWD) &&
                             (Define.seqBrushFwBwSts == Define.STS_BRUSH_FWBW_FWDING))
                    {
                        Define.seqBrushFwBwMode = Define.MODE_BRUSH_FWBW_FWD;
                        Define.seqBrushFwBwCtrl = Define.CTRL_RUN;
                        Define.seqBrushFwBwSts = Define.STS_BRUSH_FWBW_IDLE;
                    }
                    else if ((Define.seqBrushFwBwMode == Define.MODE_BRUSH_FWBW_BWD) &&
                             (Define.seqBrushFwBwSts == Define.STS_BRUSH_FWBW_BWDING))
                    {
                        Define.seqBrushFwBwMode = Define.MODE_BRUSH_FWBW_BWD;
                        Define.seqBrushFwBwCtrl = Define.CTRL_RUN;
                        Define.seqBrushFwBwSts = Define.STS_BRUSH_FWBW_IDLE;
                    }
                }
            }

            // CH2
            if (GetDigValue((int)DigInputList.CH2_Door_Sensor_i) == "Off")
            {
                if ((Define.seqMode[(byte)MODULE._PM2] == Define.MODE_PROCESS) && (Define.seqCtrl[(byte)MODULE._PM2] == Define.CTRL_RUNNING))
                {
                    if (Define.seqCtrl[(byte)MODULE._PM2] != Define.CTRL_WAIT)
                        Define.seqCtrl[(byte)MODULE._PM2] = Define.CTRL_WAIT;
                }

                // 모터는 매뉴얼 동작이라도 멈추게
                if (Define.seqCylinderCtrl[(byte)MODULE._PM2] != Define.CTRL_WAIT)
                    Define.seqCylinderCtrl[(byte)MODULE._PM2] = Define.CTRL_WAIT;
            }
            else
            {
                if ((Define.seqMode[(byte)MODULE._PM2] == Define.MODE_PROCESS) && (Define.seqCtrl[(byte)MODULE._PM2] == Define.CTRL_WAIT))
                {
                    if (Define.seqCtrl[(byte)MODULE._PM2] != Define.CTRL_RUNNING)
                        Define.seqCtrl[(byte)MODULE._PM2] = Define.CTRL_RUNNING;
                }

                if (Define.seqCylinderCtrl[(byte)MODULE._PM2] == Define.CTRL_WAIT)
                {
                    if ((Define.seqCylinderMode[(byte)MODULE._PM2] == Define.MODE_CYLINDER_RUN) &&
                        (Define.seqCylinderSts[(byte)MODULE._PM2] == Define.STS_CYLINDER_RUNING))
                    {
                        Define.seqCylinderMode[(byte)MODULE._PM2] = Define.MODE_CYLINDER_RUN;
                        Define.seqCylinderCtrl[(byte)MODULE._PM2] = Define.CTRL_RUN;
                        Define.seqCylinderSts[(byte)MODULE._PM2] = Define.STS_CYLINDER_IDLE;
                    }
                    else if ((Define.seqCylinderMode[(byte)MODULE._PM2] == Define.MODE_CYLINDER_HOME) &&
                             (Define.seqCylinderSts[(byte)MODULE._PM2] == Define.STS_CYLINDER_HOMEING))
                    {
                        Define.seqCylinderMode[(byte)MODULE._PM2] = Define.MODE_CYLINDER_HOME;
                        Define.seqCylinderCtrl[(byte)MODULE._PM2] = Define.CTRL_RUN;
                        Define.seqCylinderSts[(byte)MODULE._PM2] = Define.STS_CYLINDER_IDLE;
                    }
                    else if ((Define.seqCylinderMode[(byte)MODULE._PM2] == Define.MODE_CYLINDER_FWD) &&
                             (Define.seqCylinderSts[(byte)MODULE._PM2] == Define.STS_CYLINDER_FWDING))
                    {
                        Define.seqCylinderMode[(byte)MODULE._PM2] = Define.MODE_CYLINDER_FWD;
                        Define.seqCylinderCtrl[(byte)MODULE._PM2] = Define.CTRL_RUN;
                        Define.seqCylinderSts[(byte)MODULE._PM2] = Define.STS_CYLINDER_IDLE;
                    }
                    else if ((Define.seqCylinderMode[(byte)MODULE._PM2] == Define.MODE_CYLINDER_BWD) &&
                             (Define.seqCylinderSts[(byte)MODULE._PM2] == Define.STS_CYLINDER_BWDING))
                    {
                        Define.seqCylinderMode[(byte)MODULE._PM2] = Define.MODE_CYLINDER_BWD;
                        Define.seqCylinderCtrl[(byte)MODULE._PM2] = Define.CTRL_RUN;
                        Define.seqCylinderSts[(byte)MODULE._PM2] = Define.STS_CYLINDER_IDLE;
                    }
                }
            }            
        }
        #endregion

        #region 동작(IO) 명령 시 인터락
        private static bool SETPOINT_INTERLOCK_CHECK1(int ioName, uint setValue, string ModuleName, ref string retMsg)
        {
            // Interlock이 해제 상태인지 체크
            if (Define.bInterlockRelease)
            {
                return true;
            }            

            if (ModuleName == "PM1")
            {
                if ((ioName == (int)DigOutputList.CH1_AirValve_Top_o) ||                    
                    (ioName == (int)DigOutputList.CH1_AirValve_Bot_o) ||

                    (ioName == (int)DigOutputList.CH1_WaterValve_Top_o))
                {
                    if (setValue == (uint)DigitalOffOn.On)
                    {
                        if ((GetDigValue((int)DigInputList.EMO_Front_i) == "On") &&
                            (GetDigValue((int)DigInputList.EMO_Rear_i) == "On"))
                        {
                            return true;
                        }                            
                        else
                        {
                            retMsg = "EMO switch is on";
                            EventLog("[INTERLOCK#1] " + "EMO switch is on", ModuleName, "Event");
                            return false;
                        }                            
                    }
                    else
                    {
                        return true;
                    }
                }

                if (ioName == (int)DigOutputList.CH1_Brush_Pwr_o)
                {
                    if (setValue == (uint)DigitalOffOn.On)
                    {
                        if ((GetDigValue((int)DigInputList.EMO_Front_i) == "On") &&
                            (GetDigValue((int)DigInputList.EMO_Rear_i) == "On"))
                        {
                            return true;
                        }                            
                        else
                        {
                            retMsg = "EMO switch is on";
                            EventLog("[INTERLOCK#1] " + "EMO switch is on", ModuleName, "Event");
                            return false;
                        }                            
                    }
                    else
                    {
                        if ((GetDigValue((int)DigInputList.EMO_Front_i) == "On") &&
                            (GetDigValue((int)DigInputList.EMO_Rear_i) == "On"))
                        {
                            return true;
                        }
                        else
                        {
                            retMsg = "EMO switch is on";
                            EventLog("[INTERLOCK#1] " + "EMO switch is on", ModuleName, "Event");
                            return false;
                        }
                    }
                }

                if (ioName == (int)DigOutputList.CH1_Nozzle_Pwr_o)
                {
                    if (setValue == (uint)DigitalOffOn.On)
                    {
                        if ((GetDigValue((int)DigInputList.EMO_Front_i) == "On") &&
                            (GetDigValue((int)DigInputList.EMO_Rear_i) == "On"))
                        {
                            return true;
                        }
                        else
                        {
                            retMsg = "EMO switch is on";
                            EventLog("[INTERLOCK#1] " + "EMO switch is on", ModuleName, "Event");
                            return false;
                        }
                    }
                    else
                    {
                        if ((GetDigValue((int)DigInputList.EMO_Front_i) == "On") &&
                            (GetDigValue((int)DigInputList.EMO_Rear_i) == "On"))
                        {
                            return true;
                        }
                        else
                        {
                            retMsg = "EMO switch is on";
                            EventLog("[INTERLOCK#1] " + "EMO switch is on", ModuleName, "Event");
                            return false;
                        }
                    }
                }                                
            }            

            return true;
        }

        private static bool SETPOINT_INTERLOCK_CHECK2(int ioName, uint setValue, string ModuleName, ref string retMsg)
        {
            // Interlock이 해제 상태인지 체크
            if (Define.bInterlockRelease)
            {
                return true;
            }
            
            if (ModuleName == "PM2")
            {
                if ((ioName == (int)DigOutputList.CH2_AirValve_Top_o) ||
                    (ioName == (int)DigOutputList.CH2_AirValve_Bot_o) ||

                    (ioName == (int)DigOutputList.CH2_WaterValve_Top_o))                    
                {
                    if (setValue == (uint)DigitalOffOn.On)
                    {
                        if ((GetDigValue((int)DigInputList.EMO_Front_i) == "On") &&
                            (GetDigValue((int)DigInputList.EMO_Rear_i) == "On"))
                        {
                            return true;
                        }
                        else
                        {
                            retMsg = "EMO switch is on";
                            EventLog("[INTERLOCK#2] " + "EMO switch is on", ModuleName, "Event");
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }

                if (ioName == (int)DigOutputList.CH2_Nozzle_Pwr_o)
                {
                    if (setValue == (uint)DigitalOffOn.On)
                    {
                        if ((GetDigValue((int)DigInputList.EMO_Front_i) == "On") &&
                            (GetDigValue((int)DigInputList.EMO_Rear_i) == "On"))
                        {
                            return true;
                        }
                        else
                        {
                            retMsg = "EMO switch is on";
                            EventLog("[INTERLOCK#2] " + "EMO switch is on", ModuleName, "Event");
                            return false;
                        }
                    }
                    else
                    {
                        if ((GetDigValue((int)DigInputList.EMO_Front_i) == "On") &&
                            (GetDigValue((int)DigInputList.EMO_Rear_i) == "On"))
                        {
                            return true;
                        }
                        else
                        {
                            retMsg = "EMO switch is on";
                            EventLog("[INTERLOCK#2] " + "EMO switch is on", ModuleName, "Event");
                            return false;
                        }
                    }
                }
            }

            return true;
        }
        #endregion

        public static bool MOTION_INTERLOCK_CHECK()
        {
            // Interlock이 해제 상태인지 체크
            if (Define.bInterlockRelease)
            {
                return true;
            }

            if ((GetDigValue((int)DigInputList.EMO_Front_i) == "On") &&
                (GetDigValue((int)DigInputList.EMO_Rear_i) == "On"))
            {
                return true;
            }
            else
            {
                EventLog("[INTERLOCK#1] " + "EMO switch is on", "PM1", "Event");
                return false;
            }
        }

        private static void ALL_VALVE_CLOSE()
        {            
            SetDigValue((int)DigOutputList.CH1_AirValve_Top_o, (uint)DigitalOffOn.Off, "PM1");
            SetDigValue((int)DigOutputList.CH1_AirValve_Bot_o, (uint)DigitalOffOn.Off, "PM1");
            SetDigValue((int)DigOutputList.CH1_WaterValve_Top_o, (uint)DigitalOffOn.Off, "PM1");                        
            

            SetDigValue((int)DigOutputList.CH2_AirValve_Top_o, (uint)DigitalOffOn.Off, "PM2");
            SetDigValue((int)DigOutputList.CH2_AirValve_Bot_o, (uint)DigitalOffOn.Off, "PM2");
            SetDigValue((int)DigOutputList.CH2_WaterValve_Top_o, (uint)DigitalOffOn.Off, "PM2");            


            SetDigValue((int)DigOutputList.Hot_Water_Pump_o, (uint)DigitalOffOn.Off, "PM1");
            SetDigValue((int)DigOutputList.Hot_WaterHeater_o, (uint)DigitalOffOn.Off, "PM1");
            SetDigValue((int)DigOutputList.Main_Water_Supply, (uint)DigitalOffOn.Off, "PM1");
        }

        private static void PROCESS_ABORT()
        {
            if (Define.seqCtrl[(byte)MODULE._PM1] != Define.CTRL_IDLE)
            {
                Define.seqCtrl[(byte)MODULE._PM1] = Define.CTRL_ABORT;
            }

            if (Define.seqCtrl[(byte)MODULE._PM2] != Define.CTRL_IDLE)
            {
                Define.seqCtrl[(byte)MODULE._PM2] = Define.CTRL_ABORT;
            }            
        }

        public static bool Value_Check(string[] sValue)
        {
            bool bResult;
            int i;
            bool bRtn = true;
            double dVal = 0.0;

            for (i = 0; i < sValue.Length; i++)
            {
                bResult = double.TryParse(sValue[i], out dVal);
                if (!bResult)
                {
                    bRtn = false;
                    break;
                }
            }

            if (bRtn)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

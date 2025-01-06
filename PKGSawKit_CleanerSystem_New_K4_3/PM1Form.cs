using Ajin_motion_driver;
using MsSqlManagerLibrary;
using PKGSawKit_CleanerSystem_New_K4_3.SerialComm;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace PKGSawKit_CleanerSystem_New_K4_3
{
    public partial class PM1Form : UserControl
    {
        private MaintnanceForm m_Parent;
        int module;
        string ModuleName;

        RecipeSelectForm recipeSelectForm;
        DigitalDlg digitalDlg;
        AnalogDlg analogDlg;

        HanyoungNuxClass heater_ctrl;

        private Timer logdisplayTimer = new Timer();        

        public PM1Form(MaintnanceForm parent)
        {
            m_Parent = parent;

            module = (int)MODULE._PM1;
            ModuleName = "PM1";

            InitializeComponent();         
        }

        private void PM1Form_Load(object sender, EventArgs e)
        {
            Width = 1172;
            Height = 824;
            Top = 0;
            Left = 0;

            logdisplayTimer.Interval = 500;
            logdisplayTimer.Elapsed += new ElapsedEventHandler(Eventlog_Display);
            logdisplayTimer.Start();            
        }

        private void SetDoubleBuffered(Control control, bool doubleBuffered = true)
        {
            PropertyInfo propertyInfo = typeof(Control).GetProperty
            (
                "DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic
            );
            propertyInfo.SetValue(control, doubleBuffered, null);
        }

        public void Display()
        {
            SetDoubleBuffered(Door_Close);            

            // Process seq status
            if (Define.seqMode[module] == Define.MODE_PROCESS)
            {
                if (Define.seqCtrl[module] != Define.CTRL_IDLE)
                {
                    if (btnProcess.Enabled != false)
                        btnProcess.Enabled = false;

                    if (Define.seqCtrl[module] == Define.CTRL_ALARM)
                    {
                        if (btnProcess.BackColor != Color.Red)
                            btnProcess.BackColor = Color.Red;
                        else
                            btnProcess.BackColor = Color.Transparent;

                        if (!btnRetry.Enabled)
                            btnRetry.Enabled = true;
                    }
                    else
                    {
                        if (btnProcess.BackColor != Color.YellowGreen)
                            btnProcess.BackColor = Color.YellowGreen;
                        else
                            btnProcess.BackColor = Color.Transparent;

                        if (label_Alarm.Text != "--")
                            label_Alarm.Text = "--";

                        if (btnRetry.Enabled != false)
                            btnRetry.Enabled = false;
                    }

                    if (!btnAbort.Enabled)
                        btnAbort.Enabled = true;


                    if (btnInit.Enabled != false)
                        btnInit.Enabled = false;

                    if (btnInitStop.Enabled != false)
                        btnInitStop.Enabled = false;

                    if (btnInit.BackColor != Color.Transparent)
                        btnInit.BackColor = Color.Transparent;
                }
            }
            else if (Define.seqMode[module] == Define.MODE_INIT)
            {
                if (Define.seqCtrl[module] != Define.CTRL_IDLE)
                {
                    if (btnInit.Enabled != false)
                        btnInit.Enabled = false;

                    if (Define.seqCtrl[(byte)MODULE._PM1] == Define.CTRL_ALARM)
                    {
                        if (btnInit.BackColor != Color.Red)
                            btnInit.BackColor = Color.Red;
                        else
                            btnInit.BackColor = Color.Transparent;
                    }
                    else
                    {
                        if (btnInit.BackColor != Color.YellowGreen)
                            btnInit.BackColor = Color.YellowGreen;
                        else
                            btnInit.BackColor = Color.Transparent;

                        if (label_Alarm.Text != "--")
                            label_Alarm.Text = "--";
                    }

                    if (!btnInitStop.Enabled)
                        btnInitStop.Enabled = true;


                    if (btnProcess.Enabled != false)
                        btnProcess.Enabled = false;

                    if (btnRetry.Enabled != false)
                        btnRetry.Enabled = false;

                    if (btnAbort.Enabled != false)
                        btnAbort.Enabled = false;

                    if (btnProcess.BackColor != Color.Transparent)
                        btnProcess.BackColor = Color.Transparent;
                }
            }
            else if (Define.seqMode[module] == Define.MODE_IDLE)
            {
                if (!btnProcess.Enabled)
                {
                    btnProcess.Enabled = true;

                    //HostConnection.Host_Set_RunStatus(Global.hostEquipmentInfo, ModuleName, "Idle");
                }                    

                if (btnProcess.BackColor != Color.Transparent)
                    btnProcess.BackColor = Color.Transparent;

                if (btnRetry.Enabled != false)
                    btnRetry.Enabled = false;

                if (btnAbort.Enabled != false)
                    btnAbort.Enabled = false;

                if (label_Alarm.Text != "--")
                    label_Alarm.Text = "--";

                if (!btnInit.Enabled)
                    btnInit.Enabled = true;

                if (btnInitStop.Enabled != false)
                    btnInitStop.Enabled = false;

                if (btnInit.BackColor != Color.Transparent)
                    btnInit.BackColor = Color.Transparent;
            }

            if ((Define.seqMode[module] == Define.MODE_PROCESS) && (Define.seqCtrl[module] == Define.CTRL_WAIT))
            {
                if (labelProcessWait.ForeColor == Color.LightGray)
                    labelProcessWait.ForeColor = Color.Red;
                else
                    labelProcessWait.ForeColor = Color.LightGray;
            }
            else
            {
                if (labelProcessWait.ForeColor != Color.LightGray)
                    labelProcessWait.ForeColor = Color.LightGray;
            }

            // Process recipe 정보
            if (Global.prcsInfo.prcsRecipeName[module] != null)
                textBoxRecipeName.Text = Global.prcsInfo.prcsRecipeName[module];

            textBoxStepNum.Text = Global.prcsInfo.prcsCurrentStep[module].ToString() + " / " + Global.prcsInfo.prcsTotalStep[module];

            if (Global.prcsInfo.prcsStepName[module] != null)
                textBoxStepName.Text = Global.prcsInfo.prcsStepName[module];

            textBoxProcessTime.Text = Global.prcsInfo.prcsStepCurrentTime[module].ToString() + " / " + Global.prcsInfo.prcsStepTotalTime[module].ToString();
            textBoxProcessEndTime.Text = Global.prcsInfo.prcsEndTime[module];

            // Input display           
            if ((Global.GetDigValue((int)DigInputList.CH1_Brush_Fwd_i) == "On") &&
                (Global.GetDigValue((int)DigInputList.CH1_Brush_Bwd_i) == "Off") &&
                (Global.GetDigValue((int)DigInputList.CH1_Brush_Home_i) == "Off"))
            {
                if (PM1BrushFwdSns.BackColor != Color.Lime)
                    PM1BrushFwdSns.BackColor = Color.Lime;

                if (PM1BrushBwdSns.BackColor != Color.Silver)
                    PM1BrushBwdSns.BackColor = Color.Silver;

                if (PM1BrushHomeSns.BackColor != Color.Silver)
                    PM1BrushHomeSns.BackColor = Color.Silver;
            }
            else if ((Global.GetDigValue((int)DigInputList.CH1_Brush_Fwd_i) == "Off") &&
                     (Global.GetDigValue((int)DigInputList.CH1_Brush_Bwd_i) == "On") &&
                     (Global.GetDigValue((int)DigInputList.CH1_Brush_Home_i) == "Off"))
            {
                if (PM1BrushFwdSns.BackColor != Color.Silver)
                    PM1BrushFwdSns.BackColor = Color.Silver;

                if (PM1BrushBwdSns.BackColor != Color.Lime)
                    PM1BrushBwdSns.BackColor = Color.Lime;

                if (PM1BrushHomeSns.BackColor != Color.Silver)
                    PM1BrushHomeSns.BackColor = Color.Silver;
            }
            else if ((Global.GetDigValue((int)DigInputList.CH1_Brush_Fwd_i) == "Off") &&
                     (Global.GetDigValue((int)DigInputList.CH1_Brush_Bwd_i) == "Off") &&
                     (Global.GetDigValue((int)DigInputList.CH1_Brush_Home_i) == "On"))
            {
                if (PM1BrushFwdSns.BackColor != Color.Silver)
                    PM1BrushFwdSns.BackColor = Color.Silver;

                if (PM1BrushBwdSns.BackColor != Color.Silver)
                    PM1BrushBwdSns.BackColor = Color.Silver;

                if (PM1BrushHomeSns.BackColor != Color.Lime)
                    PM1BrushHomeSns.BackColor = Color.Lime;
            }
            else
            {
                if (PM1BrushFwdSns.BackColor != Color.Silver)
                    PM1BrushFwdSns.BackColor = Color.Silver;

                if (PM1BrushBwdSns.BackColor != Color.Silver)
                    PM1BrushBwdSns.BackColor = Color.Silver;

                if (PM1BrushHomeSns.BackColor != Color.Silver)
                    PM1BrushHomeSns.BackColor = Color.Silver;
            }

            if ((Global.GetDigValue((int)DigInputList.CH1_Nozzle_Fwd_i) == "On") &&
                (Global.GetDigValue((int)DigInputList.CH1_Nozzle_Bwd_i) == "Off"))                
            {
                if (PM1NozzleFwdSns.BackColor != Color.Lime)
                    PM1NozzleFwdSns.BackColor = Color.Lime;

                if (PM1NozzleBwdSns.BackColor != Color.Silver)
                    PM1NozzleBwdSns.BackColor = Color.Silver;                
            }
            else if ((Global.GetDigValue((int)DigInputList.CH1_Nozzle_Fwd_i) == "Off") &&
                     (Global.GetDigValue((int)DigInputList.CH1_Nozzle_Bwd_i) == "On"))
            {
                if (PM1NozzleFwdSns.BackColor != Color.Silver)
                    PM1NozzleFwdSns.BackColor = Color.Silver;

                if (PM1NozzleBwdSns.BackColor != Color.Lime)
                    PM1NozzleBwdSns.BackColor = Color.Lime;                
            }            
            else
            {
                if (PM1NozzleFwdSns.BackColor != Color.Silver)
                    PM1NozzleFwdSns.BackColor = Color.Silver;

                if (PM1NozzleBwdSns.BackColor != Color.Silver)
                    PM1NozzleBwdSns.BackColor = Color.Silver;                
            }            

            if (MotionClass.motor[Define.axis_r].sR_BusyStatus == "Moving")
            {
                if (!PM1BrushRotate1.Visible)
                    PM1BrushRotate1.Visible = true;
                else
                    PM1BrushRotate1.Visible = false;

                if (!PM1BrushRotate2.Visible)
                    PM1BrushRotate2.Visible = true;
                else
                    PM1BrushRotate2.Visible = false;
            }
            else
            {
                if (PM1BrushRotate1.Visible != false)
                    PM1BrushRotate1.Visible = false;

                if (PM1BrushRotate2.Visible != false)
                    PM1BrushRotate2.Visible = false;
            }

            if (Global.GetDigValue((int)DigInputList.CH1_Door_Sensor_i) == "Off")
            {
                textBoxDoor.Text = "Open";
                textBoxDoor.BackColor = Color.OrangeRed;
            }
            else if (Global.GetDigValue((int)DigInputList.CH1_Door_Sensor_i) == "On")
            {
                textBoxDoor.Text = "Close";
                textBoxDoor.BackColor = Color.LightSkyBlue;
            }

            // Output display            
            if ((Global.digSet.curDigSet[(int)DigOutputList.CH1_Brush_Pwr_o] == "On") && 
                (Global.digSet.curDigSet[(int)DigOutputList.CH1_Brush_FwdBwd_o] == "Off"))
            {
                textBoxBrushFwdBwd.Text = "Forward";
            }                
            else if ((Global.digSet.curDigSet[(int)DigOutputList.CH1_Brush_Pwr_o] == "On") &&
                     (Global.digSet.curDigSet[(int)DigOutputList.CH1_Brush_FwdBwd_o] == "On"))
            {
                textBoxBrushFwdBwd.Text = "Backward";
            }                
            else
            {
                textBoxBrushFwdBwd.Text = "None";
            }
                
            if ((Global.digSet.curDigSet[(int)DigOutputList.CH1_Nozzle_Pwr_o] == "On") && 
                (Global.digSet.curDigSet[(int)DigOutputList.CH1_Nozzle_FwdBwd_o] == "Off"))
            {
                textBoxNozzleFwdBwd.Text = "Forward";
            }                
            else if ((Global.digSet.curDigSet[(int)DigOutputList.CH1_Nozzle_Pwr_o] == "On") &&
                     (Global.digSet.curDigSet[(int)DigOutputList.CH1_Nozzle_FwdBwd_o] == "On"))
            {
                textBoxNozzleFwdBwd.Text = "Backward";
            }                
            else
            {
                textBoxNozzleFwdBwd.Text = "None";
            }                

            if (Global.digSet.curDigSet[(int)DigOutputList.CH1_AirValve_Top_o] != null)
            {
                if (Global.digSet.curDigSet[(int)DigOutputList.CH1_AirValve_Top_o] == "On")
                {
                    textBoxAirTop.Text = "Open";
                    textBoxAirTop.BackColor = Color.LightSkyBlue;
                    if (!PM1Air1.Visible)
                        PM1Air1.Visible = true;
                    else
                        PM1Air1.Visible = false;
                }
                else
                {
                    textBoxAirTop.Text = "Close";
                    textBoxAirTop.BackColor = Color.WhiteSmoke;
                    if (PM1Air1.Visible != false)
                        PM1Air1.Visible = false;
                }                              
            }

            if (Global.digSet.curDigSet[(int)DigOutputList.CH1_AirValve_Bot_o] != null)
            {
                if (Global.digSet.curDigSet[(int)DigOutputList.CH1_AirValve_Bot_o] == "On")
                {
                    textBoxAirBot.Text = "Open";
                    textBoxAirBot.BackColor = Color.LightSkyBlue;
                    if (!PM1Air2.Visible)
                        PM1Air2.Visible = true;
                    else
                        PM1Air2.Visible = false;
                }
                else
                {
                    textBoxAirBot.Text = "Close";
                    textBoxAirBot.BackColor = Color.WhiteSmoke;
                    if (PM1Air2.Visible != false)
                        PM1Air2.Visible = false;
                }
            }

            if (Global.digSet.curDigSet[(int)DigOutputList.CH1_WaterValve_Top_o] != null)
            {
                if (Global.digSet.curDigSet[(int)DigOutputList.CH1_WaterValve_Top_o] == "On")
                {
                    textBoxWaterTop.Text = "Open";
                    textBoxWaterTop.BackColor = Color.LightSkyBlue;
                }
                else
                {
                    textBoxWaterTop.Text = "Close";
                    textBoxWaterTop.BackColor = Color.WhiteSmoke;
                }
                
                if (Global.digSet.curDigSet[(int)DigOutputList.CH1_WaterValve_Top_o] == "On")                    
                {
                    if (!PM1Water1_1.Visible)
                        PM1Water1_1.Visible = true;
                    else
                        PM1Water1_1.Visible = false;

                    if (!PM1Water1_2.Visible)
                        PM1Water1_2.Visible = true;
                    else
                        PM1Water1_2.Visible = false;

                    if (!PM1Water2_1.Visible)
                        PM1Water2_1.Visible = true;
                    else
                        PM1Water2_1.Visible = false;

                    if (!PM1Water2_2.Visible)
                        PM1Water2_2.Visible = true;
                    else
                        PM1Water2_2.Visible = false;
                }
                else
                {
                    if (PM1Water1_1.Visible != false)
                        PM1Water1_1.Visible = false;

                    if (PM1Water1_2.Visible != false)
                        PM1Water1_2.Visible = false;

                    if (PM1Water2_1.Visible != false)
                        PM1Water2_1.Visible = false;

                    if (PM1Water2_2.Visible != false)
                        PM1Water2_2.Visible = false;
                }
            }

            if (Global.digSet.curDigSet[(int)DigOutputList.CH1_Curtain_AirValve_o] != null)
            {
                if (Global.digSet.curDigSet[(int)DigOutputList.CH1_Curtain_AirValve_o] == "On")
                {
                    textBoxCurtainAir.Text = "Open";
                    textBoxCurtainAir.BackColor = Color.LightSkyBlue;                    
                }
                else
                {
                    textBoxCurtainAir.Text = "Close";
                    textBoxCurtainAir.BackColor = Color.WhiteSmoke;                    
                }
            }

            textBoxCurrentWaterTemp.Text = Define.temp_PV.ToString();

            textBoxAxis1Runsts.Text = MotionClass.motor[Define.axis_r].sR_BusyStatus;            
            textBoxAxis1SpeedCur.Text = string.Format("{0:0.0}", MotionClass.motor[Define.axis_r].dR_CmdVelocity);
            

            // Daily count
            textBoxDailyCnt.Text = Define.iPM1DailyCnt.ToString("00");            
        }

        private void Eventlog_Display(object sender, ElapsedEventArgs e)
        {
            if (Define.bPM1Event)
            {                
                Eventlog_File_Read();                
            }

            if (Define.bPM1AlmEvent)
            {
                Alarmlog_File_Read();
            }
        }

        private void Eventlog_File_Read()
        {
            Define.bPM1Event = false;
            
            try
            {
                string sTmpData;

                string sYear = string.Format("{0:yyyy}", DateTime.Now).Trim();
                string sMonth = string.Format("{0:MM}", DateTime.Now).Trim();
                string sDay = string.Format("{0:dd}", DateTime.Now).Trim();
                string FileName = string.Format("{0}.txt", sDay);                

                if (File.Exists(string.Format("{0}{1}\\{2}\\{3}\\{4}", Global.logfilePath, ModuleName, sYear, sMonth, FileName)))
                {
                    byte[] bytes;
                    using (var fs = File.Open(string.Format("{0}{1}\\{2}\\{3}\\{4}", Global.logfilePath, ModuleName, sYear, sMonth, FileName), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        try
                        {
                            bytes = new byte[fs.Length];
                            fs.Read(bytes, 0, (int)fs.Length);
                            sTmpData = Encoding.Default.GetString(bytes);

                            string[] data = sTmpData.Split('\n');
                            int iLength = data.Length;
                            if (iLength >= 2)
                            {
                                string sVal = data[iLength - 2].ToString();

                                Invoke((Action)(() =>
                                {
                                    listBoxEventLog.Update();

                                    if (listBoxEventLog.Items.Count >= 10)
                                        listBoxEventLog.Items.Clear();

                                    listBoxEventLog.Items.Add(sVal);
                                    listBoxEventLog.SelectedIndex = listBoxEventLog.Items.Count - 1;
                                }));
                            }
                        }
                        catch (ArgumentException)
                        {

                        }                        
                    }
                }
            }
            catch (IOException)
            {

            }           
        }

        private void Alarmlog_File_Read()
        {
            Define.bPM1AlmEvent = false;

            try
            {
                string sTmpData;

                string sYear = string.Format("{0:yyyy}", DateTime.Now).Trim();
                string sMonth = string.Format("{0:MM}", DateTime.Now).Trim();
                string sDay = string.Format("{0:dd}", DateTime.Now).Trim();
                string FileName = string.Format("{0}.txt", sDay);                

                if (File.Exists(string.Format("{0}{1}\\{2}\\{3}\\{4}", Global.alarmHistoryPath, ModuleName, sYear, sMonth, FileName)))
                {
                    byte[] bytes;
                    using (var fs = File.Open(string.Format("{0}{1}\\{2}\\{3}\\{4}", Global.alarmHistoryPath, ModuleName, sYear, sMonth, FileName), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        try
                        {
                            bytes = new byte[fs.Length];
                            fs.Read(bytes, 0, (int)fs.Length);
                            sTmpData = Encoding.Default.GetString(bytes);

                            string[] data = sTmpData.Split('\n');
                            int iLength = data.Length;
                            if (iLength >= 2)
                            {
                                string sVal = data[iLength - 2].ToString();

                                Invoke((Action)(() =>
                                {
                                    label_Alarm.Text = sVal;
                                }));
                            }
                        }
                        catch (ArgumentException)
                        {

                        }                        
                    }
                }
            }
            catch (IOException)
            {

            }
        }

        private void Digital_Click(object sender, EventArgs e)
        {                        
            digitalDlg = new DigitalDlg();

            TextBox btn = (TextBox)sender;
            string strTmp = btn.Tag.ToString();
            switch (strTmp)
            {                
                case "2":
                    {
                        digitalDlg.Init("Close", "Open", "CH1 Top Air valve");
                        if (digitalDlg.ShowDialog() == DialogResult.OK)
                        {
                            if (digitalDlg.m_strResult == "Close")
                            {
                                Global.SetDigValue((int)DigOutputList.CH1_AirValve_Top_o, (uint)DigitalOffOn.Off, ModuleName);
                            }
                            else
                            {
                                Global.SetDigValue((int)DigOutputList.CH1_AirValve_Top_o, (uint)DigitalOffOn.On, ModuleName);
                            }
                        }
                    }
                    break;

                case "3":
                    {
                        digitalDlg.Init("Close", "Open", "CH1 Bottom Air valve");
                        if (digitalDlg.ShowDialog() == DialogResult.OK)
                        {
                            if (digitalDlg.m_strResult == "Close")
                            {
                                Global.SetDigValue((int)DigOutputList.CH1_AirValve_Bot_o, (uint)DigitalOffOn.Off, ModuleName);
                            }
                            else
                            {
                                Global.SetDigValue((int)DigOutputList.CH1_AirValve_Bot_o, (uint)DigitalOffOn.On, ModuleName);
                            }
                        }
                    }
                    break;

                case "4":
                    {
                        digitalDlg.Init("Close", "Open", "CH1 Top Water valve");
                        if (digitalDlg.ShowDialog() == DialogResult.OK)
                        {
                            if (digitalDlg.m_strResult == "Close")
                            {
                                Global.SetDigValue((int)DigOutputList.CH1_WaterValve_Top_o, (uint)DigitalOffOn.Off, ModuleName);
                            }
                            else
                            {
                                Global.SetDigValue((int)DigOutputList.CH1_WaterValve_Top_o, (uint)DigitalOffOn.On, ModuleName);
                            }
                        }
                    }
                    break;                

                case "6":
                    {
                        digitalDlg.Init("Close", "Open", "CH1 Curtain Air Valve");
                        if (digitalDlg.ShowDialog() == DialogResult.OK)
                        {
                            if (digitalDlg.m_strResult == "Close")
                            {
                                Global.SetDigValue((int)DigOutputList.CH1_Curtain_AirValve_o, (uint)DigitalOffOn.Off, ModuleName);
                            }
                            else
                            {
                                Global.SetDigValue((int)DigOutputList.CH1_Curtain_AirValve_o, (uint)DigitalOffOn.On, ModuleName);
                            }
                        }
                    }
                    break;

                case "8":
                    {
                        digitalDlg.Init("Backward", "Forward", "CH1 Cylinder Fwd/Bwd");
                        if (digitalDlg.ShowDialog() == DialogResult.OK)
                        {
                            if (!Global.MOTION_INTERLOCK_CHECK())
                            {
                                if (Global.GetDigValue((int)DigInputList.CH1_Door_Sensor_i) == "Off")
                                {
                                    MessageBox.Show("Chamber door is opened", "Notification");
                                    return;
                                }
                            }

                            if (digitalDlg.m_strResult == "Backward")
                            {
                                //Global.SetDigValue((int)DigOutputList.CH1_Nozzle_Pwr_o, (uint)DigitalOffOn.On, ModuleName);
                                //Global.SetDigValue((int)DigOutputList.CH1_Nozzle_FwdBwd_o, (uint)DigitalOffOn.On, ModuleName);                                
                                Define.seqCylinderMode[module] = Define.MODE_CYLINDER_BWD;
                                Define.seqCylinderCtrl[module] = Define.CTRL_RUN;
                                Define.seqCylinderSts[module] = Define.STS_CYLINDER_IDLE;
                            }
                            else if (digitalDlg.m_strResult == "Forward")
                            {
                                //Global.SetDigValue((int)DigOutputList.CH1_Nozzle_Pwr_o, (uint)DigitalOffOn.On, ModuleName);
                                //Global.SetDigValue((int)DigOutputList.CH1_Nozzle_FwdBwd_o, (uint)DigitalOffOn.Off, ModuleName);                                
                                Define.seqCylinderMode[module] = Define.MODE_CYLINDER_FWD;
                                Define.seqCylinderCtrl[module] = Define.CTRL_RUN;
                                Define.seqCylinderSts[module] = Define.STS_CYLINDER_IDLE;
                            }
                        }
                    }
                    break;

                case "10":
                    {
                        digitalDlg.Init2("Home", "Backward", "Forward", "CH1 Brush Fwd/Bwd");
                        if (digitalDlg.ShowDialog() == DialogResult.OK)
                        {
                            if (!Global.MOTION_INTERLOCK_CHECK())
                            {
                                if (Global.GetDigValue((int)DigInputList.CH1_Door_Sensor_i) == "Off")
                                {
                                    MessageBox.Show("Chamber door is opened", "Notification");
                                    return;
                                }
                            }

                            if (digitalDlg.m_strResult == "Home")
                            {
                                //Global.SetDigValue((int)DigOutputList.CH1_Brush_Pwr_o, (uint)DigitalOffOn.Off, ModuleName);
                                //Global.SetDigValue((int)DigOutputList.CH1_Brush_FwdBwd_o, (uint)DigitalOffOn.Off, ModuleName);
                                Define.seqBrushFwBwMode = Define.MODE_BRUSH_FWBW_HOME;
                                Define.seqBrushFwBwCtrl = Define.CTRL_RUN;
                                Define.seqBrushFwBwSts = Define.STS_BRUSH_FWBW_IDLE;
                            }
                            else if (digitalDlg.m_strResult == "Backward")
                            {
                                //Global.SetDigValue((int)DigOutputList.CH1_Brush_Pwr_o, (uint)DigitalOffOn.On, ModuleName);                                
                                //Global.SetDigValue((int)DigOutputList.CH1_Brush_FwdBwd_o, (uint)DigitalOffOn.On, ModuleName);
                                Define.seqBrushFwBwMode = Define.MODE_BRUSH_FWBW_BWD;
                                Define.seqBrushFwBwCtrl = Define.CTRL_RUN;
                                Define.seqBrushFwBwSts = Define.STS_BRUSH_FWBW_IDLE;
                            }
                            else if (digitalDlg.m_strResult == "Forward")
                            {
                                //Global.SetDigValue((int)DigOutputList.CH1_Brush_Pwr_o, (uint)DigitalOffOn.On, ModuleName);                                
                                //Global.SetDigValue((int)DigOutputList.CH1_Brush_FwdBwd_o, (uint)DigitalOffOn.Off, ModuleName);
                                Define.seqBrushFwBwMode = Define.MODE_BRUSH_FWBW_FWD;
                                Define.seqBrushFwBwCtrl = Define.CTRL_RUN;
                                Define.seqBrushFwBwSts = Define.STS_BRUSH_FWBW_IDLE;
                            }
                        }
                    }
                    break;                
            }
        }

        private void Analog_Click(object sender, EventArgs e)
        {
            try
            {
                analogDlg = new AnalogDlg();
                heater_ctrl = new HanyoungNuxClass();

                if (analogDlg.ShowDialog() == DialogResult.OK)
                {
                    string strVal = analogDlg.m_strResult;
                    bool bResult = double.TryParse(strVal, out double dVal);
                    if (bResult)
                    {
                        heater_ctrl.set_Temp(dVal);
                        textBoxSettingWaterTemp.Text = dVal.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCylinderStop_Click(object sender, EventArgs e)
        {
            Define.seqCylinderCtrl[module] = Define.CTRL_ABORT;
        }

        private void btnBrushStop_Click(object sender, EventArgs e)
        {
            Define.seqBrushFwBwCtrl = Define.CTRL_ABORT;
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            string strTmp = btn.Text.ToString();
            switch (strTmp)
            {
                case "Start":
                    {                        
                        if (MessageBox.Show("공정을 진행 하겠습니까?", "알림", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            Define.iSelectRecipeModule = module;

                            recipeSelectForm = new RecipeSelectForm();

                            if (recipeSelectForm.ShowDialog() == DialogResult.OK)
                            {
                                if (!Define.bInterlockRelease)
                                {
                                    /*
                                    if (Global.GetDigValue((int)DigInputList.Front_Door_Sensor_i) == "Off")
                                    {
                                        MessageBox.Show("Front door가 열려 있습니다", "알림");
                                        return;
                                    }
                                    */
                                    if (Global.GetDigValue((int)DigInputList.Left_Door_Sensor_i) == "Off")
                                    {
                                        MessageBox.Show("Left door가 열려 있습니다", "알림");
                                        return;
                                    }

                                    if (Global.GetDigValue((int)DigInputList.Right_Door_Sensor_i) == "Off")
                                    {
                                        MessageBox.Show("Right door가 열려 있습니다", "알림");
                                        return;
                                    }

                                    if (Global.GetDigValue((int)DigInputList.Back_Door_Sensor_i) == "Off")
                                    {
                                        MessageBox.Show("Back door가 열려 있습니다", "알림");
                                        return;
                                    }

                                    if (Global.GetDigValue((int)DigInputList.CH1_Door_Sensor_i) == "Off")
                                    {
                                        MessageBox.Show("Chamber door is opened", "Notification");
                                        return;
                                    }
                                }

                                Define.seqMode[module] = Define.MODE_PROCESS;
                                Define.seqCtrl[module] = Define.CTRL_RUN;
                                Define.seqSts[module] = Define.STS_IDLE;
                            }
                        }
                    }
                    break;

                case "Retry":
                    {
                        if (!Define.bInterlockRelease)
                        {
                            /*
                            if (Global.GetDigValue((int)DigInputList.Front_Door_Sensor_i) == "Off")
                            {
                                MessageBox.Show("Front door가 열려 있습니다", "알림");
                                return;
                            }
                            */
                            if (Global.GetDigValue((int)DigInputList.Left_Door_Sensor_i) == "Off")
                            {
                                MessageBox.Show("Left door가 열려 있습니다", "알림");
                                return;
                            }

                            if (Global.GetDigValue((int)DigInputList.Right_Door_Sensor_i) == "Off")
                            {
                                MessageBox.Show("Right door가 열려 있습니다", "알림");
                                return;
                            }

                            if (Global.GetDigValue((int)DigInputList.Back_Door_Sensor_i) == "Off")
                            {
                                MessageBox.Show("Back door가 열려 있습니다", "알림");
                                return;
                            }

                            if (Global.GetDigValue((int)DigInputList.CH1_Door_Sensor_i) == "Off")
                            {
                                MessageBox.Show("Chamber door is opened", "Notification");
                                return;
                            }
                        }

                        Define.seqCtrl[module] = Define.CTRL_RETRY;
                    }
                    break;

                case "Stop":
                    {
                        if (MessageBox.Show("공정을 중지하겠습니까?", "알림", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            Define.seqCtrl[module] = Define.CTRL_ABORT;
                        }
                    }
                    break;
            }
        }

        

        private void btnInit_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            string strTmp = btn.Text.ToString();
            switch (strTmp)
            {
                case "Init":
                    {
                        if (!Define.bInterlockRelease)
                        {
                            /*
                            if (Global.GetDigValue((int)DigInputList.Front_Door_Sensor_i) == "Off")
                            {
                                MessageBox.Show("Front door가 열려 있습니다", "알림");
                                return;
                            }
                            */
                            if (Global.GetDigValue((int)DigInputList.Left_Door_Sensor_i) == "Off")
                            {
                                MessageBox.Show("Left door가 열려 있습니다", "알림");
                                return;
                            }

                            if (Global.GetDigValue((int)DigInputList.Right_Door_Sensor_i) == "Off")
                            {
                                MessageBox.Show("Right door가 열려 있습니다", "알림");
                                return;
                            }

                            if (Global.GetDigValue((int)DigInputList.Back_Door_Sensor_i) == "Off")
                            {
                                MessageBox.Show("Back door가 열려 있습니다", "알림");
                                return;
                            }

                            if (Global.GetDigValue((int)DigInputList.CH1_Door_Sensor_i) == "Off")
                            {
                                MessageBox.Show("Chamber door is opened", "Notification");
                                return;
                            }
                        }

                        Define.seqMode[module] = Define.MODE_INIT;
                        Define.seqCtrl[module] = Define.CTRL_RUN;
                        Define.seqSts[module] = Define.STS_IDLE;
                    }
                    break;

                case "Stop":
                    {
                        Define.seqCtrl[module] = Define.CTRL_ABORT;
                    }
                    break;
            }
        }        
    }
}

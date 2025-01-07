using Ajin_IO_driver;
using Ajin_motion_driver;
using HanyoungNXClassLibrary;
using MsSqlManagerLibrary;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace PKGSawKit_CleanerSystem_New_K4_3
{
    public partial class MainForm : Form
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        LoginForm m_loginForm;
        OperationForm m_operationForm;
        MaintnanceForm m_maintnanceForm;
        RecipeForm m_recipeForm;
        ConfigureForm m_configureForm;
        IOForm m_ioForm;
        AlarmForm m_alarmForm;
        EventLogForm m_eventLogForm;
        UserRegistForm m_userRegistForm;

        Squence.PM1Process pM1Process;
        Squence.PM1Cylinder pM1Cylinder;
        Squence.PM1BrushMoving pM1BrushMoving;
        
        Squence.PM2Process pM2Process;
        Squence.PM2Cylinder pM2Cylinder;        

        bool bLogCnt;

        public MainForm()
        {
            InitializeComponent();

            SubFormCreate();

            CreateThread();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Width = 1280;
            Height = 1024;
            Top = 0;
            Left = 0;

            Define.bLogin = false;
            Define.bOpActivate = false;

            bLogCnt = false;

            MyNativeWindows myNativeWindows = new MyNativeWindows();

            for (int i = 0; i < this.Controls.Count; i++)
            {
                MdiClient mdiClient = this.Controls[i] as MdiClient;
                if (mdiClient != null)
                {
                    myNativeWindows.ReleaseHandle();
                    myNativeWindows.AssignHandle(mdiClient.Handle);
                }
            }

            // IO보드 초기화
            if (DIOClass.OpenDevice())
            {
                m_ioForm.DI_Parsing_timer.Start();
            }

            // Motion driver init
            MotionClass.Ajin_Motion_Init();

            // Heater controller
            HanyoungNXClass.HanyoungNX_Init();

            Global.Init();

            // 가동 시간 불러오기
            RUNTIME_LOAD();

            timerDisplay.Enabled = true;            

            SubFormShow((byte)Page.LogInPage);

            F_ButtonVisible(false, false, false, false, false, false, false, false);
        }

        public class MyNativeWindows : NativeWindow
        {
            public MyNativeWindows()
            {
            }

            private const int WM_NCCALCSIZE = 0x0083;
            private const int SB_BOTH = 3;

            [DllImport("user32.dll")]
            private static extern int ShowScrollBar(IntPtr hWnd, int wBar, int bShow);

            protected override void WndProc(ref Message m)
            {
                switch (m.Msg)
                {
                    case WM_NCCALCSIZE:
                        ShowScrollBar(m.HWnd, SB_BOTH, 0);
                        break;
                }
                base.WndProc(ref m);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            timerDisplay.Enabled = false;            

            FreeThread();

            Dispose();
        }

        private void SubFormCreate()
        {
            m_operationForm = new OperationForm();
            m_operationForm.MdiParent = this;
            m_operationForm.Show();

            m_loginForm = new LoginForm();
            m_loginForm.MdiParent = this;
            m_loginForm.Show();

            m_userRegistForm = new UserRegistForm();
            m_userRegistForm.MdiParent = this;
            m_userRegistForm.Show();

            m_maintnanceForm = new MaintnanceForm();
            m_maintnanceForm.MdiParent = this;
            m_maintnanceForm.Show();

            m_recipeForm = new RecipeForm();
            m_recipeForm.MdiParent = this;
            m_recipeForm.Show();

            m_configureForm = new ConfigureForm();
            m_configureForm.MdiParent = this;
            m_configureForm.Show();

            m_ioForm = new IOForm();
            m_ioForm.MdiParent = this;
            m_ioForm.Show();

            m_alarmForm = new AlarmForm();
            m_alarmForm.MdiParent = this;
            m_alarmForm.Show();

            m_eventLogForm = new EventLogForm();
            m_eventLogForm.MdiParent = this;
            m_eventLogForm.Show();
        }

        private void CreateThread()
        {
            pM1Process = new Squence.PM1Process();
            pM1Cylinder = new Squence.PM1Cylinder();
            pM1BrushMoving = new Squence.PM1BrushMoving();            

            pM2Process = new Squence.PM2Process();
            pM2Cylinder = new Squence.PM2Cylinder();            
        }

        private void FreeThread()
        {
            pM1Process.Dispose();
            pM1Cylinder.Dispose();
            pM1BrushMoving.Dispose();            

            pM2Process.Dispose();
            pM2Cylinder.Dispose();            

            DIOClass.CloseDevice();
            MotionClass.DRV_CLOSE();

            HanyoungNXClass.DRV_CLOSE();
        }

        public void SubFormShow(byte PageNum)
        {
            try
            {
                Define.currentPage = PageNum;
                byte iPage = PageNum;

                switch (iPage)
                {
                    case (byte)Page.LogInPage:
                        {
                            m_loginForm.Activate();
                            m_loginForm.BringToFront();

                            F_ModuleButtonVisible(false, false, false);
                        }
                        break;

                    case (byte)Page.OperationPage:
                        {
                            m_operationForm.Activate();
                            m_operationForm.BringToFront();

                            F_ModuleButtonVisible(false, false, false);
                        }
                        break;

                    case (byte)Page.MaintnancePage:
                        {
                            m_maintnanceForm.Activate();
                            m_maintnanceForm.BringToFront();

                            F_ModuleButtonVisible(true, true, true);                            
                        }
                        break;

                    case (byte)Page.RecipePage:
                        {
                            m_recipeForm.Activate();
                            m_recipeForm.BringToFront();

                            F_ModuleButtonVisible(true, true, false);
                        }
                        break;

                    case (byte)Page.ConfigurePage:
                        {
                            m_configureForm.Activate();
                            m_configureForm.BringToFront();

                            F_ModuleButtonVisible(false, false, false);
                        }
                        break;

                    case (byte)Page.IOPage:
                        {
                            m_ioForm.Activate();
                            m_ioForm.BringToFront();

                            F_ModuleButtonVisible(false, false, false);
                        }
                        break;

                    case (byte)Page.AlarmPage:
                        {
                            m_alarmForm.Activate();
                            m_alarmForm.BringToFront();

                            F_ModuleButtonVisible(false, false, false);
                        }
                        break;

                    case (byte)Page.EventLogPage:
                        {
                            m_eventLogForm.Activate();
                            m_eventLogForm.BringToFront();

                            F_ModuleButtonVisible(false, false, false);
                        }
                        break;

                    case (byte)Page.UserRegist:
                        {
                            m_userRegistForm.Activate();
                            m_userRegistForm.BringToFront();

                            F_ModuleButtonVisible(false, false, false);
                        }
                        break;
                }
            }
            catch
            {
                MessageBox.Show("폼 양식을 가져오는 도중 오류가 발생했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void F_ButtonVisible(bool bOpBtn, bool bMaintBtn, bool bRecipeBtn, bool bConfigureBtn, bool bIOBtn, bool bAlarmBtn, bool bEventLogBtn, bool bUserRegistBtn)
        {
            pictureBoxOperation.Enabled = bOpBtn;
            btnOperation.Enabled = bOpBtn;

            pictureBoxMain.Enabled = bMaintBtn;
            btnMaintnance.Enabled = bMaintBtn;

            pictureBoxRecipe.Enabled = bRecipeBtn;
            btnRecipe.Enabled = bRecipeBtn;

            pictureBoxConfigure.Enabled = bConfigureBtn;
            btnConfigure.Enabled = bConfigureBtn;

            pictureBoxIO.Enabled = bIOBtn;
            btnIO.Enabled = bIOBtn;

            pictureBoxAlarm.Enabled = bAlarmBtn;
            pictureBoxAlarm2.Enabled = bAlarmBtn;
            btnAlarm.Enabled = bAlarmBtn;

            pictureBoxEventLog.Enabled = bEventLogBtn;
            btnEventLog.Enabled = bEventLogBtn;

            pictureBoxUserRegist.Enabled = bUserRegistBtn;
            btnUserRegist.Enabled = bUserRegistBtn;
        }

        private void F_ModuleButtonVisible(bool bCH1Btn, bool bCH2Btn, bool bMotorBtn)
        {
            btnCH1Module.Visible = bCH1Btn;
            btnCH2Module.Visible = bCH2Btn;
            btnMotorModule.Visible = bMotorBtn;
        }        

        private void btnOperation_Click(object sender, EventArgs e)
        {
            SubFormShow((byte)Page.OperationPage);
        }

        private void btnMain_Click(object sender, EventArgs e)
        {
            SubFormShow((byte)Page.MaintnancePage);
        }

        private void btnRecipe_Click(object sender, EventArgs e)
        {
            SubFormShow((byte)Page.RecipePage);
        }

        private void btnConfigure_Click(object sender, EventArgs e)
        {
            SubFormShow((byte)Page.ConfigurePage);
        }

        private void btnIO_Click(object sender, EventArgs e)
        {
            SubFormShow((byte)Page.IOPage);
        }

        private void btnAlarm_Click(object sender, EventArgs e)
        {
            SubFormShow((byte)Page.AlarmPage);
        }

        private void pictureBoxAlarm_Click(object sender, EventArgs e)
        {
            Global.SetDigValue((int)DigOutputList.Buzzer_o, (uint)DigitalOffOn.Off, "PM1");
        }

        private void btnEventLog_Click(object sender, EventArgs e)
        {
            SubFormShow((byte)Page.EventLogPage);
        }

        private void btnUserRegist_Click(object sender, EventArgs e)
        {
            SubFormShow((byte)Page.UserRegist);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("프로그램을 종료 하겠습니까?", "알림", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                Dispose();
                //Application.Exit();
                Application.ExitThread();
                Environment.Exit(0);
            }
        }

        private void btnModule_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            string strTmp = btn.Text.ToString();
            switch (strTmp)
            {
                case "CH1":
                    {
                        if (Define.currentPage == (byte)Page.MaintnancePage)
                        {
                            if (!m_maintnanceForm.m_PM1Form.Visible)
                                m_maintnanceForm.m_PM1Form.Visible = true;

                            if (m_maintnanceForm.m_PM2Form.Visible != false)
                                m_maintnanceForm.m_PM2Form.Visible = false;

                            if (m_maintnanceForm.m_motorForm.Visible != false)
                                m_maintnanceForm.m_motorForm.Visible = false;                            

                            Define.MaintCurrentPage = (byte)MODULE._PM1;
                        }
                        else if (Define.currentPage == (byte)Page.RecipePage)
                        {
                            if (!m_recipeForm.m_PM1RecipeForm.Visible)
                                m_recipeForm.m_PM1RecipeForm.Visible = true;

                            if (m_recipeForm.m_PM2RecipeForm.Visible != false)
                                m_recipeForm.m_PM2RecipeForm.Visible = false;

                            Define.RecipeCurrentPage = (byte)MODULE._PM1;
                        }                        
                    }
                    break;

                case "CH2":
                    {
                        if (Define.currentPage == (byte)Page.MaintnancePage)
                        {
                            if (!m_maintnanceForm.m_PM2Form.Visible)
                                m_maintnanceForm.m_PM2Form.Visible = true;

                            if (m_maintnanceForm.m_PM1Form.Visible != false)
                                m_maintnanceForm.m_PM1Form.Visible = false;

                            if (m_maintnanceForm.m_motorForm.Visible != false)
                                m_maintnanceForm.m_motorForm.Visible = false;                            

                            Define.MaintCurrentPage = (byte)MODULE._PM2;
                        }
                        else if (Define.currentPage == (byte)Page.RecipePage)
                        {
                            if (!m_recipeForm.m_PM2RecipeForm.Visible)
                                m_recipeForm.m_PM2RecipeForm.Visible = true;

                            if (m_recipeForm.m_PM1RecipeForm.Visible != false)
                                m_recipeForm.m_PM1RecipeForm.Visible = false;

                            Define.RecipeCurrentPage = (byte)MODULE._PM2;
                        }                        
                    }
                    break;

                case "Motor":
                    {
                        if (!m_maintnanceForm.m_motorForm.Visible)
                            m_maintnanceForm.m_motorForm.Visible = true;

                        if (m_maintnanceForm.m_PM1Form.Visible != false)
                            m_maintnanceForm.m_PM1Form.Visible = false;

                        if (m_maintnanceForm.m_PM2Form.Visible != false)
                            m_maintnanceForm.m_PM2Form.Visible = false;                        

                        Define.MaintCurrentPage = (byte)MODULE._MOTOR;
                    }
                    break;
            }
        }

        private void panelLogo_Click(object sender, EventArgs e)
        {
            if (panelOption.Visible == false)
                panelOption.Visible = true;
            else
                panelOption.Visible = false;
        }

        private void checkBoxInterlockRelease_Click(object sender, EventArgs e)
        {
            if (checkBoxInterlockRelease.Checked)
            {
                checkBoxInterlockRelease.Checked = true;
                Define.bInterlockRelease = true;
            }
            else
            {
                checkBoxInterlockRelease.Checked = false;
                Define.bInterlockRelease = false;
            }
        }

        private void timerDisplay_Tick(object sender, EventArgs e)
        {
            Display();
        }

        private void Display()
        {
            laDate.Text = System.DateTime.Today.ToShortDateString();
            laTime.Text = System.DateTime.Now.ToLongTimeString();
            laUserLevel.Text = "Level : " + Define.UserLevel;

            if (Define.currentPage == (byte)Page.OperationPage)
            {
                labelPageName.Text = "Operation";
                if (Define.bOpActivate)
                {
                    m_operationForm.Activate();
                    m_operationForm.BringToFront();

                    Define.bOpActivate = false;
                }
            }
            else if (Define.currentPage == (byte)Page.MaintnancePage)
            {
                labelPageName.Text = "Maintnance";

                if (Define.MaintCurrentPage == (byte)MODULE._PM1)
                {
                    btnCH1Module.BackColor = Color.Lime;
                    btnCH2Module.BackColor = Color.Transparent;
                    btnMotorModule.BackColor = Color.Transparent;
                }
                else if (Define.MaintCurrentPage == (byte)MODULE._PM2)
                {
                    btnCH1Module.BackColor = Color.Transparent;
                    btnCH2Module.BackColor = Color.Lime;
                    btnMotorModule.BackColor = Color.Transparent;
                }
                else if (Define.MaintCurrentPage == (byte)MODULE._MOTOR)
                {
                    btnCH1Module.BackColor = Color.Transparent;
                    btnCH2Module.BackColor = Color.Transparent;
                    btnMotorModule.BackColor = Color.Lime;
                }
            }
            else if (Define.currentPage == (byte)Page.RecipePage)
            {
                labelPageName.Text = "Recipe";

                if (Define.RecipeCurrentPage == (byte)MODULE._PM1)
                {
                    btnCH1Module.BackColor = Color.Lime;
                    btnCH2Module.BackColor = Color.Transparent;
                    btnMotorModule.BackColor = Color.Transparent;
                }
                else if (Define.RecipeCurrentPage == (byte)MODULE._PM2)
                {
                    btnCH1Module.BackColor = Color.Transparent;
                    btnCH2Module.BackColor = Color.Lime;
                    btnMotorModule.BackColor = Color.Transparent;
                }                
            }
            else if (Define.currentPage == (byte)Page.ConfigurePage)
            {
                labelPageName.Text = "Configure";
            }
            else if (Define.currentPage == (byte)Page.IOPage)
            {
                labelPageName.Text = "Input/Output";
            }
            else if (Define.currentPage == (byte)Page.AlarmPage)
            {
                labelPageName.Text = "Alarm";
            }
            else if (Define.currentPage == (byte)Page.EventLogPage)
            {
                labelPageName.Text = "Event Log";
            }
            else if (Define.currentPage == (byte)Page.UserRegist)
            {
                labelPageName.Text = "User regist";
                m_userRegistForm.BringToFront();
            }
            else if (Define.currentPage == (byte)Page.LogInPage)
            {
                labelPageName.Text = "Log-In";
            }
            

            // User level에 따른 버튼 활성화
            if (Define.UserLevel == "Master")
            {
                // op, maint, recipe, configure, io, alarm, userRegist
                F_ButtonVisible(true, true, true, true, true, true, true, true);
            }
            else if (Define.UserLevel == "Maintnance")
            {
                F_ButtonVisible(true, true, true, true, true, true, true, false);
            }
            else if (Define.UserLevel == "User")
            {
                F_ButtonVisible(true, false, true, true, false, true, true, false);
            }


            // Tower lamp
            if ((Define.seqCtrl[(byte)MODULE._PM1] == Define.CTRL_ALARM) ||
                (Define.seqCtrl[(byte)MODULE._PM2] == Define.CTRL_ALARM) ||

                (Global.GetDigValue((int)DigInputList.EMO_Front_i) == "Off") ||
                (Global.GetDigValue((int)DigInputList.EMO_Rear_i) == "Off"))               
            {
                if (Global.digSet.curDigSet[(int)DigOutputList.Tower_Lamp_Red_o] != null)
                {
                    if (Global.digSet.curDigSet[(int)DigOutputList.Tower_Lamp_Red_o] != "On")
                        Global.SetDigValue((int)DigOutputList.Tower_Lamp_Red_o, (uint)DigitalOffOn.On, "PM1");
                    else
                        Global.SetDigValue((int)DigOutputList.Tower_Lamp_Red_o, (uint)DigitalOffOn.Off, "PM1");
                }

                if (Global.digSet.curDigSet[(int)DigOutputList.Tower_Lamp_Yellow_o] != null)
                {
                    if (Global.digSet.curDigSet[(int)DigOutputList.Tower_Lamp_Yellow_o] != "Off")                        
                        Global.SetDigValue((int)DigOutputList.Tower_Lamp_Yellow_o, (uint)DigitalOffOn.Off, "PM1");
                }

                if (Global.digSet.curDigSet[(int)DigOutputList.Tower_Lamp_Green_o] != null)
                {
                    if (Global.digSet.curDigSet[(int)DigOutputList.Tower_Lamp_Green_o] != "Off")
                        Global.SetDigValue((int)DigOutputList.Tower_Lamp_Green_o, (uint)DigitalOffOn.Off, "PM1");
                }

                if (pictureBoxAlarm.Visible)
                    pictureBoxAlarm.Visible = false;
                else
                    pictureBoxAlarm.Visible = true;
            }
            else
            {
                if ((Define.seqCtrl[(byte)MODULE._PM1] == Define.CTRL_IDLE) &&
                    (Define.seqCtrl[(byte)MODULE._PM2] == Define.CTRL_IDLE))                    
                {
                    if (Global.digSet.curDigSet[(int)DigOutputList.Tower_Lamp_Red_o] != null)
                    {
                        if (Global.digSet.curDigSet[(int)DigOutputList.Tower_Lamp_Red_o] != "Off")
                            Global.SetDigValue((int)DigOutputList.Tower_Lamp_Red_o, (uint)DigitalOffOn.Off, "PM1");
                    }

                    if (Global.digSet.curDigSet[(int)DigOutputList.Tower_Lamp_Yellow_o] != null)
                    {
                        if (Global.digSet.curDigSet[(int)DigOutputList.Tower_Lamp_Yellow_o] != "On")
                            Global.SetDigValue((int)DigOutputList.Tower_Lamp_Yellow_o, (uint)DigitalOffOn.On, "PM1");
                    }

                    if (Global.digSet.curDigSet[(int)DigOutputList.Tower_Lamp_Green_o] != null)
                    {
                        if (Global.digSet.curDigSet[(int)DigOutputList.Tower_Lamp_Green_o] != "Off")
                            Global.SetDigValue((int)DigOutputList.Tower_Lamp_Green_o, (uint)DigitalOffOn.Off, "PM1");
                    }

                    if (pictureBoxAlarm.Visible != false)
                        pictureBoxAlarm.Visible = false;
                }
                else
                {
                    if (Global.digSet.curDigSet[(int)DigOutputList.Tower_Lamp_Red_o] != null)
                    {
                        if (Global.digSet.curDigSet[(int)DigOutputList.Tower_Lamp_Red_o] != "Off")
                            Global.SetDigValue((int)DigOutputList.Tower_Lamp_Red_o, (uint)DigitalOffOn.Off, "PM1");
                    }

                    if (Global.digSet.curDigSet[(int)DigOutputList.Tower_Lamp_Yellow_o] != null)
                    {
                        if (Global.digSet.curDigSet[(int)DigOutputList.Tower_Lamp_Yellow_o] != "Off")
                            Global.SetDigValue((int)DigOutputList.Tower_Lamp_Yellow_o, (uint)DigitalOffOn.Off, "PM1");
                    }

                    if (Global.digSet.curDigSet[(int)DigOutputList.Tower_Lamp_Green_o] != null)
                    {
                        if (Global.digSet.curDigSet[(int)DigOutputList.Tower_Lamp_Green_o] != "On")
                            Global.SetDigValue((int)DigOutputList.Tower_Lamp_Green_o, (uint)DigitalOffOn.On, "PM1");                        
                    }

                    // 가동 시간
                    RUNTIME_CALC();
                }

                if (pictureBoxAlarm.Visible != false)
                    pictureBoxAlarm.Visible = false;
            }


            if (Define.bInterlockRelease)
            {
                if (labelInterlockEnaDis.Visible)
                    labelInterlockEnaDis.Visible = false;
                else
                    labelInterlockEnaDis.Visible = true;
            }
            else
            {
                if (labelInterlockEnaDis.Visible != false)
                    labelInterlockEnaDis.Visible = false;
            }

            // Fluorescent Lamp set
            if (!Define.bManualLamp)
            {
                if (Global.GetDigValue((int)DigInputList.Front_Door_Sensor_i) == "Off")
                {
                    if (Global.digSet.curDigSet[(int)DigOutputList.FluorescentLamp_o] != null)
                    {
                        if (Global.digSet.curDigSet[(int)DigOutputList.FluorescentLamp_o] != "On")
                        {
                            Global.SetDigValue((int)DigOutputList.FluorescentLamp_o, (uint)DigitalOffOn.On, "PM2");
                        }
                    }
                }
                else
                {
                    if (Global.digSet.curDigSet[(int)DigOutputList.FluorescentLamp_o] != null)
                    {
                        if (Global.digSet.curDigSet[(int)DigOutputList.FluorescentLamp_o] != "Off")
                        {
                            Global.SetDigValue((int)DigOutputList.FluorescentLamp_o, (uint)DigitalOffOn.Off, "PM2");
                        }
                    }
                }
            }
            

            // Process end - buzzer auto off
            /*
            if (Global.GetDigValue((int)DigInputList.Front_Door_Sensor_i) == "Off")
            {
                if (Global.digSet.curDigSet[(int)DigOutputList.Buzzer_o] != null)
                {
                    if (Global.digSet.curDigSet[(int)DigOutputList.Buzzer_o] != "Off")
                        Global.SetDigValue((int)DigOutputList.Buzzer_o, (uint)DigitalOffOn.Off, "PM1");
                }
            }
            */
            // Daily count init
            string sTime = DateTime.Now.ToString("HH:mm:ss");
            if (sTime == "00:00:00")
            {
                if (!bLogCnt)
                {
                    // 가동 시간 및 가동률 서버 업데이트
                    //RUNTIME_UPDATE();

                    // 가동 시간 초기화
                    RUNTIME_INIT();

                    if (Define.iPM1DailyCnt != 0)
                        Define.iPM1DailyCnt = 0;

                    if (Define.iPM2DailyCnt != 0)
                        Define.iPM2DailyCnt = 0;

                    bLogCnt = true;
                }                
            }
            else
            {
                if (bLogCnt != false)
                {
                    bLogCnt = false;
                }
            }                       
        }

        private void RUNTIME_LOAD()
        {
            StringBuilder sbTodayRunTime = new StringBuilder();
            GetPrivateProfileString("TodayRuntime", "Time", "", sbTodayRunTime, sbTodayRunTime.Capacity, string.Format("{0}{1}", Global.dailyCntfilePath, "TodayRuntime.ini"));
            Define.dTodayRunTime = Convert.ToDouble(sbTodayRunTime.ToString());
        }

        private void RUNTIME_CALC()
        {
            Define.dTodayRunTime += 0.5;
            WritePrivateProfileString("TodayRuntime", "Time", Define.dTodayRunTime.ToString(), string.Format("{0}{1}", Global.dailyCntfilePath, "TodayRuntime.ini"));
        }

        private void RUNTIME_UPDATE()
        {
            // Only daily count performance
            int iDailyAllCnt = Define.iPM1DailyCnt + Define.iPM2DailyCnt;
            double dPerformance = ((double)(iDailyAllCnt) / Define.iCapa) * 100;
            string strPerformance = dPerformance.ToString("0.000");

            // 실제 가동 시간 기준 performance
            double dTimePerformance = (Define.dTodayRunTime / (double)Define.iSemiAutoTime) * 100;
            string strTimePerformance = dTimePerformance.ToString("0.000");

            HostConnection.Host_Set_Log(Global.hostEquipmentInfo_Log,
                DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"),
                Define.iPM1DailyCnt.ToString("00"),
                Define.iPM2DailyCnt.ToString("00"),
                "00",
                strPerformance,
                Define.dTodayRunTime.ToString(),
                strTimePerformance);
        }

        private void RUNTIME_INIT()
        {
            WritePrivateProfileString("TodayRuntime", "Time", "0", string.Format("{0}{1}", Global.dailyCntfilePath, "TodayRuntime.ini"));
            Define.dTodayRunTime = 0;
        }
    }
}

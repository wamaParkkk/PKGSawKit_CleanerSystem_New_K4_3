using Ajin_motion_driver;
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
    public partial class OperationForm : Form
    {
        RecipeSelectForm recipeSelectForm;
        ToolCheckInfoForm toolCheckInfoForm;
        ToolInfoRegistForm toolInfoRegistForm;

        private Timer logdisplayTimer = new Timer();

        public OperationForm()
        {
            InitializeComponent();
        }

        private void OperationForm_Load(object sender, EventArgs e)
        {
            Width = 1172;
            Height = 824;
            Top = 0;
            Left = 0;

            displayTimer.Enabled = true;

            logdisplayTimer.Interval = 100;
            logdisplayTimer.Elapsed += new ElapsedEventHandler(Eventlog_Display);
            logdisplayTimer.Start();
        }

        private void OperationForm_Activated(object sender, EventArgs e)
        {
            Top = 0;
            Left = 0;

            SetDoubleBuffered(PM1_Door_Close);            
            SetDoubleBuffered(PM2_Door_Close);            
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

        private void OperationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            displayTimer.Enabled = false;
            logdisplayTimer.Stop();

            Dispose();
        }

        private void displayTimer_Tick(object sender, EventArgs e)
        {
            // CH1 display
            if (Define.seqMode[(byte)MODULE._PM1] == Define.MODE_PROCESS)
            {
                if (Define.seqCtrl[(byte)MODULE._PM1] != Define.CTRL_IDLE)
                {
                    if (btnPM1Process.Enabled != false)
                        btnPM1Process.Enabled = false;

                    if (Define.seqCtrl[(byte)MODULE._PM1] == Define.CTRL_ALARM)
                    {
                        if (btnPM1Process.BackColor != Color.Red)
                            btnPM1Process.BackColor = Color.Red;
                        else
                            btnPM1Process.BackColor = Color.Transparent;

                        if (!btnPM1Retry.Enabled)
                            btnPM1Retry.Enabled = true;
                    }
                    else
                    {
                        if (btnPM1Process.BackColor != Color.YellowGreen)
                            btnPM1Process.BackColor = Color.YellowGreen;
                        else
                            btnPM1Process.BackColor = Color.Transparent;

                        if (label_PM1Alarm.Text != "--")
                            label_PM1Alarm.Text = "--";

                        if (btnPM1Retry.Enabled != false)
                            btnPM1Retry.Enabled = false;
                    }

                    if (!btnPM1Abort.Enabled)
                        btnPM1Abort.Enabled = true;


                    if (btnPM1Init.Enabled != false)
                        btnPM1Init.Enabled = false;

                    if (btnPM1InitStop.Enabled != false)
                        btnPM1InitStop.Enabled = false;

                    if (btnPM1Init.BackColor != Color.Transparent)
                        btnPM1Init.BackColor = Color.Transparent;
                }
            }
            else if (Define.seqMode[(byte)MODULE._PM1] == Define.MODE_INIT)
            {
                if (Define.seqCtrl[(byte)MODULE._PM1] != Define.CTRL_IDLE)
                {
                    if (btnPM1Init.Enabled != false)
                        btnPM1Init.Enabled = false;

                    if (Define.seqCtrl[(byte)MODULE._PM1] == Define.CTRL_ALARM)
                    {
                        if (btnPM1Init.BackColor != Color.Red)
                            btnPM1Init.BackColor = Color.Red;
                        else
                            btnPM1Init.BackColor = Color.Transparent;
                    }
                    else
                    {
                        if (btnPM1Init.BackColor != Color.YellowGreen)
                            btnPM1Init.BackColor = Color.YellowGreen;
                        else
                            btnPM1Init.BackColor = Color.Transparent;

                        if (label_PM1Alarm.Text != "--")
                            label_PM1Alarm.Text = "--";
                    }

                    if (!btnPM1InitStop.Enabled)
                        btnPM1InitStop.Enabled = true;


                    if (btnPM1Process.Enabled != false)
                        btnPM1Process.Enabled = false;

                    if (btnPM1Retry.Enabled != false)
                        btnPM1Retry.Enabled = false;

                    if (btnPM1Abort.Enabled != false)
                        btnPM1Abort.Enabled = false;

                    if (btnPM1Process.BackColor != Color.Transparent)
                        btnPM1Process.BackColor = Color.Transparent;
                }
            }
            else if (Define.seqMode[(byte)MODULE._PM1] == Define.MODE_IDLE)
            {
                if (!btnPM1Process.Enabled)
                    btnPM1Process.Enabled = true;

                if (btnPM1Process.BackColor != Color.Transparent)
                    btnPM1Process.BackColor = Color.Transparent;

                if (btnPM1Retry.Enabled != false)
                    btnPM1Retry.Enabled = false;

                if (btnPM1Abort.Enabled != false)
                    btnPM1Abort.Enabled = false;

                if (label_PM1Alarm.Text != "--")
                    label_PM1Alarm.Text = "--";

                if (!btnPM1Init.Enabled)
                    btnPM1Init.Enabled = true;

                if (btnPM1InitStop.Enabled != false)
                    btnPM1InitStop.Enabled = false;

                if (btnPM1Init.BackColor != Color.Transparent)
                    btnPM1Init.BackColor = Color.Transparent;
            }

            if ((Define.seqMode[(byte)MODULE._PM1] == Define.MODE_PROCESS) && (Define.seqCtrl[(byte)MODULE._PM1] == Define.CTRL_WAIT))
            {
                if (labelCH1ProcessWait.ForeColor == Color.LightGray)
                    labelCH1ProcessWait.ForeColor = Color.Red;
                else
                    labelCH1ProcessWait.ForeColor = Color.LightGray;
            }
            else
            {
                if (labelCH1ProcessWait.ForeColor != Color.LightGray)
                    labelCH1ProcessWait.ForeColor = Color.LightGray;
            }

            if (Global.prcsInfo.prcsRecipeName[(byte)MODULE._PM1] != null)
                textBoxPM1RecipeName.Text = Global.prcsInfo.prcsRecipeName[(byte)MODULE._PM1];

            textBoxPM1StepNum.Text = Global.prcsInfo.prcsCurrentStep[(byte)MODULE._PM1].ToString() + " / " + Global.prcsInfo.prcsTotalStep[(byte)MODULE._PM1];

            if (Global.prcsInfo.prcsStepName[(byte)MODULE._PM1] != null)
                textBoxPM1StepName.Text = Global.prcsInfo.prcsStepName[(byte)MODULE._PM1];

            textBoxPM1ProcessTime.Text = Global.prcsInfo.prcsStepCurrentTime[(byte)MODULE._PM1].ToString() + " / " + Global.prcsInfo.prcsStepTotalTime[(byte)MODULE._PM1].ToString();
            textBoxPM1ProcessEndTime.Text = Global.prcsInfo.prcsEndTime[(byte)MODULE._PM1];            


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
                textBoxCH1Door.Text = "Open";
                textBoxCH1Door.BackColor = Color.OrangeRed;
            }
            else if (Global.GetDigValue((int)DigInputList.CH1_Door_Sensor_i) == "On")
            {
                textBoxCH1Door.Text = "Close";
                textBoxCH1Door.BackColor = Color.LightSkyBlue;
            }

            if (Global.digSet.curDigSet[(int)DigOutputList.CH1_AirValve_Top_o] != null)
            {
                if (Global.digSet.curDigSet[(int)DigOutputList.CH1_AirValve_Top_o] == "On")
                {                    
                    if (!PM1Air1.Visible)
                        PM1Air1.Visible = true;
                    else
                        PM1Air1.Visible = false;
                }
                else
                {                    
                    if (PM1Air1.Visible != false)
                        PM1Air1.Visible = false;
                }
            }

            if (Global.digSet.curDigSet[(int)DigOutputList.CH1_AirValve_Bot_o] != null)
            {
                if (Global.digSet.curDigSet[(int)DigOutputList.CH1_AirValve_Bot_o] == "On")
                {                    
                    if (!PM1Air2.Visible)
                        PM1Air2.Visible = true;
                    else
                        PM1Air2.Visible = false;
                }
                else
                {                    
                    if (PM1Air2.Visible != false)
                        PM1Air2.Visible = false;
                }
            }

            if (Global.digSet.curDigSet[(int)DigOutputList.CH1_WaterValve_Top_o] != null)
            {
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


            // CH2 display
            if (Define.seqMode[(byte)MODULE._PM2] == Define.MODE_PROCESS)
            {
                if (Define.seqCtrl[(byte)MODULE._PM2] != Define.CTRL_IDLE)
                {
                    if (btnPM2Process.Enabled != false)
                        btnPM2Process.Enabled = false;

                    if (Define.seqCtrl[(byte)MODULE._PM2] == Define.CTRL_ALARM)
                    {
                        if (btnPM2Process.BackColor != Color.Red)
                            btnPM2Process.BackColor = Color.Red;
                        else
                            btnPM2Process.BackColor = Color.Transparent;

                        if (!btnPM2Retry.Enabled)
                            btnPM2Retry.Enabled = true;
                    }
                    else
                    {
                        if (btnPM2Process.BackColor != Color.YellowGreen)
                            btnPM2Process.BackColor = Color.YellowGreen;
                        else
                            btnPM2Process.BackColor = Color.Transparent;

                        if (label_PM2Alarm.Text != "--")
                            label_PM2Alarm.Text = "--";

                        if (btnPM2Retry.Enabled != false)
                            btnPM2Retry.Enabled = false;
                    }

                    if (!btnPM2Abort.Enabled)
                        btnPM2Abort.Enabled = true;


                    if (btnPM2Init.Enabled != false)
                        btnPM2Init.Enabled = false;

                    if (btnPM2InitStop.Enabled != false)
                        btnPM2InitStop.Enabled = false;

                    if (btnPM2Init.BackColor != Color.Transparent)
                        btnPM2Init.BackColor = Color.Transparent;
                }
            }
            else if (Define.seqMode[(byte)MODULE._PM2] == Define.MODE_INIT)
            {
                if (Define.seqCtrl[(byte)MODULE._PM2] != Define.CTRL_IDLE)
                {
                    if (btnPM2Init.Enabled != false)
                        btnPM2Init.Enabled = false;

                    if (Define.seqCtrl[(byte)MODULE._PM2] == Define.CTRL_ALARM)
                    {
                        if (btnPM2Init.BackColor != Color.Red)
                            btnPM2Init.BackColor = Color.Red;
                        else
                            btnPM2Init.BackColor = Color.Transparent;
                    }
                    else
                    {
                        if (btnPM2Init.BackColor != Color.YellowGreen)
                            btnPM2Init.BackColor = Color.YellowGreen;
                        else
                            btnPM2Init.BackColor = Color.Transparent;

                        if (label_PM2Alarm.Text != "--")
                            label_PM2Alarm.Text = "--";
                    }

                    if (!btnPM2InitStop.Enabled)
                        btnPM2InitStop.Enabled = true;


                    if (btnPM2Process.Enabled != false)
                        btnPM2Process.Enabled = false;

                    if (btnPM2Retry.Enabled != false)
                        btnPM2Retry.Enabled = false;

                    if (btnPM2Abort.Enabled != false)
                        btnPM2Abort.Enabled = false;

                    if (btnPM2Process.BackColor != Color.Transparent)
                        btnPM2Process.BackColor = Color.Transparent;
                }
            }
            else if (Define.seqMode[(byte)MODULE._PM2] == Define.MODE_IDLE)
            {
                if (!btnPM2Process.Enabled)
                    btnPM2Process.Enabled = true;

                if (btnPM2Process.BackColor != Color.Transparent)
                    btnPM2Process.BackColor = Color.Transparent;

                if (btnPM2Retry.Enabled != false)
                    btnPM2Retry.Enabled = false;

                if (btnPM2Abort.Enabled != false)
                    btnPM2Abort.Enabled = false;

                if (label_PM2Alarm.Text != "--")
                    label_PM2Alarm.Text = "--";

                if (!btnPM2Init.Enabled)
                    btnPM2Init.Enabled = true;

                if (btnPM2InitStop.Enabled != false)
                    btnPM2InitStop.Enabled = false;

                if (btnPM2Init.BackColor != Color.Transparent)
                    btnPM2Init.BackColor = Color.Transparent;
            }

            if ((Define.seqMode[(byte)MODULE._PM2] == Define.MODE_PROCESS) && (Define.seqCtrl[(byte)MODULE._PM2] == Define.CTRL_WAIT))
            {
                if (labelCH2ProcessWait.ForeColor == Color.LightGray)
                    labelCH2ProcessWait.ForeColor = Color.Red;
                else
                    labelCH2ProcessWait.ForeColor = Color.LightGray;
            }
            else
            {
                if (labelCH2ProcessWait.ForeColor != Color.LightGray)
                    labelCH2ProcessWait.ForeColor = Color.LightGray;
            }

            if (Global.prcsInfo.prcsRecipeName[(byte)MODULE._PM2] != null)
                textBoxPM2RecipeName.Text = Global.prcsInfo.prcsRecipeName[(byte)MODULE._PM2];

            textBoxPM2StepNum.Text = Global.prcsInfo.prcsCurrentStep[(byte)MODULE._PM2].ToString() + " / " + Global.prcsInfo.prcsTotalStep[(byte)MODULE._PM2];

            if (Global.prcsInfo.prcsStepName[(byte)MODULE._PM2] != null)
                textBoxPM2StepName.Text = Global.prcsInfo.prcsStepName[(byte)MODULE._PM2];

            textBoxPM2ProcessTime.Text = Global.prcsInfo.prcsStepCurrentTime[(byte)MODULE._PM2].ToString() + " / " + Global.prcsInfo.prcsStepTotalTime[(byte)MODULE._PM2].ToString();
            textBoxPM2ProcessEndTime.Text = Global.prcsInfo.prcsEndTime[(byte)MODULE._PM2];            


            if ((Global.GetDigValue((int)DigInputList.CH2_Nozzle_Fwd_i) == "On") &&
                (Global.GetDigValue((int)DigInputList.CH2_Nozzle_Bwd_i) == "Off") &&
                (Global.GetDigValue((int)DigInputList.CH2_Nozzle_Home_i) == "Off"))
            {
                if (PM2NozzleFwdSns.BackColor != Color.Lime)
                    PM2NozzleFwdSns.BackColor = Color.Lime;

                if (PM2NozzleBwdSns.BackColor != Color.Silver)
                    PM2NozzleBwdSns.BackColor = Color.Silver;

                if (PM2NozzleHomeSns.BackColor != Color.Silver)
                    PM2NozzleHomeSns.BackColor = Color.Silver;
            }
            else if ((Global.GetDigValue((int)DigInputList.CH2_Nozzle_Fwd_i) == "Off") &&
                     (Global.GetDigValue((int)DigInputList.CH2_Nozzle_Bwd_i) == "On") &&
                     (Global.GetDigValue((int)DigInputList.CH2_Nozzle_Home_i) == "Off"))
            {
                if (PM2NozzleFwdSns.BackColor != Color.Silver)
                    PM2NozzleFwdSns.BackColor = Color.Silver;

                if (PM2NozzleBwdSns.BackColor != Color.Lime)
                    PM2NozzleBwdSns.BackColor = Color.Lime;

                if (PM2NozzleHomeSns.BackColor != Color.Silver)
                    PM2NozzleHomeSns.BackColor = Color.Silver;
            }
            else if ((Global.GetDigValue((int)DigInputList.CH2_Nozzle_Fwd_i) == "Off") &&
                     (Global.GetDigValue((int)DigInputList.CH2_Nozzle_Bwd_i) == "Off") &&
                     (Global.GetDigValue((int)DigInputList.CH2_Nozzle_Home_i) == "On"))
            {
                if (PM2NozzleFwdSns.BackColor != Color.Silver)
                    PM2NozzleFwdSns.BackColor = Color.Silver;

                if (PM2NozzleBwdSns.BackColor != Color.Silver)
                    PM2NozzleBwdSns.BackColor = Color.Silver;

                if (PM2NozzleHomeSns.BackColor != Color.Lime)
                    PM2NozzleHomeSns.BackColor = Color.Lime;
            }
            else
            {
                if (PM2NozzleFwdSns.BackColor != Color.Silver)
                    PM2NozzleFwdSns.BackColor = Color.Silver;

                if (PM2NozzleBwdSns.BackColor != Color.Silver)
                    PM2NozzleBwdSns.BackColor = Color.Silver;

                if (PM2NozzleHomeSns.BackColor != Color.Silver)
                    PM2NozzleHomeSns.BackColor = Color.Silver;
            }

            if (Global.GetDigValue((int)DigInputList.CH2_Door_Sensor_i) == "Off")
            {
                textBoxCH2Door.Text = "Open";
                textBoxCH2Door.BackColor = Color.OrangeRed;
            }
            else if (Global.GetDigValue((int)DigInputList.CH2_Door_Sensor_i) == "On")
            {
                textBoxCH2Door.Text = "Close";
                textBoxCH2Door.BackColor = Color.LightSkyBlue;
            }

            if (Global.digSet.curDigSet[(int)DigOutputList.CH2_AirValve_Top_o] != null)
            {
                if ((Global.digSet.curDigSet[(int)DigOutputList.CH2_AirValve_Top_o] == "On") ||
                    (Global.digSet.curDigSet[(int)DigOutputList.CH2_AirValve_Bot_o] == "On"))
                {
                    if (!PM2Air1.Visible)
                        PM2Air1.Visible = true;
                    else
                        PM2Air1.Visible = false;

                    if (!PM2Air2.Visible)
                        PM2Air2.Visible = true;
                    else
                        PM2Air2.Visible = false;

                    if (!PM2Air3.Visible)
                        PM2Air3.Visible = true;
                    else
                        PM2Air3.Visible = false;

                    if (!PM2Air4.Visible)
                        PM2Air4.Visible = true;
                    else
                        PM2Air4.Visible = false;

                    if (!PM2Air5.Visible)
                        PM2Air5.Visible = true;
                    else
                        PM2Air5.Visible = false;
                }
                else
                {
                    if (PM2Air1.Visible != false)
                        PM2Air1.Visible = false;

                    if (PM2Air2.Visible != false)
                        PM2Air2.Visible = false;

                    if (PM2Air3.Visible != false)
                        PM2Air3.Visible = false;

                    if (PM2Air4.Visible != false)
                        PM2Air4.Visible = false;

                    if (PM2Air5.Visible != false)
                        PM2Air5.Visible = false;
                }
            }

            if (Global.digSet.curDigSet[(int)DigOutputList.CH2_WaterValve_Top_o] != null)
            {
                if (Global.digSet.curDigSet[(int)DigOutputList.CH2_WaterValve_Top_o] == "On")                    
                {
                    if (!PM2Water1.Visible)
                        PM2Water1.Visible = true;
                    else
                        PM2Water1.Visible = false;

                    if (!PM2Water2.Visible)
                        PM2Water2.Visible = true;
                    else
                        PM2Water2.Visible = false;

                    if (!PM2Water3.Visible)
                        PM2Water3.Visible = true;
                    else
                        PM2Water3.Visible = false;

                    if (!PM2Water4.Visible)
                        PM2Water4.Visible = true;
                    else
                        PM2Water4.Visible = false;

                    if (!PM2Water5.Visible)
                        PM2Water5.Visible = true;
                    else
                        PM2Water5.Visible = false;
                }
                else
                {
                    if (PM2Water1.Visible != false)
                        PM2Water1.Visible = false;

                    if (PM2Water2.Visible != false)
                        PM2Water2.Visible = false;

                    if (PM2Water3.Visible != false)
                        PM2Water3.Visible = false;

                    if (PM2Water4.Visible != false)
                        PM2Water4.Visible = false;

                    if (PM2Water5.Visible != false)
                        PM2Water5.Visible = false;
                }
            }

            // Hot water           
            if (Global.digSet.curDigSet[(int)DigOutputList.Hot_Water_Pump_o] != null)
            {
                if (Global.digSet.curDigSet[(int)DigOutputList.Hot_Water_Pump_o] == "On")
                {
                    textBoxWaterPump.Text = "On";
                }
                else
                {
                    textBoxWaterPump.Text = "Off";
                }
            }

            if (Global.digSet.curDigSet[(int)DigOutputList.Hot_WaterHeater_o] != null)
            {
                if (Global.digSet.curDigSet[(int)DigOutputList.Hot_WaterHeater_o] == "On")
                {
                    textBoxWaterHeater.Text = "On";
                }
                else
                {
                    textBoxWaterHeater.Text = "Off";
                }
            }

            if (Global.digSet.curDigSet[(int)DigOutputList.Main_Water_Supply] != null)
            {
                if (Global.digSet.curDigSet[(int)DigOutputList.Main_Water_Supply] == "On")
                {
                    textBoxWaterSupply.Text = "Open";
                }
                else
                {
                    textBoxWaterSupply.Text = "Close";
                }
            }

            textBoxWaterTemp.Text = HanyoungNXClassLibrary.Define.temp_PV.ToString("0.0");


            if (Define.bManualLamp)
                checkBox_ManualLamp.Checked = true;
            else
                checkBox_ManualLamp.Checked = false;

            // Daily count
            textBoxPM1DailyCnt.Text = Define.iPM1DailyCnt.ToString("00");
            textBoxPM2DailyCnt.Text = Define.iPM2DailyCnt.ToString("00");            
        }

        private void Eventlog_Display(object sender, ElapsedEventArgs e)
        {
            if (Define.bPM1OpAlmEvent)
            {
                Alarmlog_File_Read("PM1");
            }

            if (Define.bPM2OpAlmEvent)
            {
                Alarmlog_File_Read("PM2");
            }            
        }

        private void Alarmlog_File_Read(string ModuleName)
        {
            if (Define.bPM1OpAlmEvent)
            {
                Define.bPM1OpAlmEvent = false;
            }

            if (Define.bPM2OpAlmEvent)
            {
                Define.bPM2OpAlmEvent = false;
            }            

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
                                    if (ModuleName == "PM1")
                                    {
                                        label_PM1Alarm.Text = sVal;
                                    }
                                    else if (ModuleName == "PM2")
                                    {
                                        label_PM2Alarm.Text = sVal;
                                    }
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
        
        private void btnPM1Process_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            string strTmp = btn.Text.ToString();
            switch (strTmp)
            {
                case "Start":
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

                        if (MessageBox.Show("공정을 진행 하겠습니까?", "알림", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            toolInfoRegistForm = new ToolInfoRegistForm();
                            toolInfoRegistForm.Init((int)MODULE._PM1);
                            if (toolInfoRegistForm.ShowDialog() == DialogResult.OK)
                            {
                                Define.iSelectRecipeModule = (int)MODULE._PM1;

                                recipeSelectForm = new RecipeSelectForm();

                                if (recipeSelectForm.ShowDialog() == DialogResult.OK)
                                {
                                    Define.seqMode[(byte)MODULE._PM1] = Define.MODE_PROCESS;
                                    Define.seqCtrl[(byte)MODULE._PM1] = Define.CTRL_RUN;
                                    Define.seqSts[(byte)MODULE._PM1] = Define.STS_IDLE;
                                }
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

                        Define.seqCtrl[(byte)MODULE._PM1] = Define.CTRL_RETRY;
                    }
                    break;

                case "Stop":
                    {
                        if (MessageBox.Show("공정을 중지하겠습니까?", "알림", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            Define.seqCtrl[(byte)MODULE._PM1] = Define.CTRL_ABORT;
                        }
                    }
                    break;
            }
        }

        private void btnPM2Process_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            string strTmp = btn.Text.ToString();
            switch (strTmp)
            {
                case "Start":
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

                            if (Global.GetDigValue((int)DigInputList.CH2_Door_Sensor_i) == "Off")
                            {
                                MessageBox.Show("Chamber door is opened", "Notification");
                                return;
                            }
                        }

                        if (MessageBox.Show("공정을 진행 하겠습니까?", "알림", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            toolInfoRegistForm = new ToolInfoRegistForm();
                            toolInfoRegistForm.Init((int)MODULE._PM2);
                            if (toolInfoRegistForm.ShowDialog() == DialogResult.OK)
                            {
                                Define.iSelectRecipeModule = (int)MODULE._PM2;

                                recipeSelectForm = new RecipeSelectForm();

                                if (recipeSelectForm.ShowDialog() == DialogResult.OK)
                                {
                                    Define.seqMode[(byte)MODULE._PM2] = Define.MODE_PROCESS;
                                    Define.seqCtrl[(byte)MODULE._PM2] = Define.CTRL_RUN;
                                    Define.seqSts[(byte)MODULE._PM2] = Define.STS_IDLE;
                                }
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

                            if (Global.GetDigValue((int)DigInputList.CH2_Door_Sensor_i) == "Off")
                            {
                                MessageBox.Show("Chamber door is opened", "Notification");
                                return;
                            }
                        }

                        Define.seqCtrl[(byte)MODULE._PM2] = Define.CTRL_RETRY;
                    }
                    break;

                case "Stop":
                    {
                        if (MessageBox.Show("공정을 중지하겠습니까?", "알림", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            Define.seqCtrl[(byte)MODULE._PM2] = Define.CTRL_ABORT;
                        }
                    }
                    break;
            }
        }

        private void btnPM1Init_Click(object sender, EventArgs e)
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

                            if (btn.Tag.ToString() == "0")
                            {
                                if (Global.GetDigValue((int)DigInputList.CH1_Door_Sensor_i) == "Off")
                                {
                                    MessageBox.Show("Chamber door is opened", "Notification");
                                    return;
                                }
                            }
                            else if (btn.Tag.ToString() == "1")
                            {
                                if (Global.GetDigValue((int)DigInputList.CH2_Door_Sensor_i) == "Off")
                                {
                                    MessageBox.Show("Chamber door is opened", "Notification");
                                    return;
                                }
                            }
                        }

                        Define.seqMode[Convert.ToByte(btn.Tag)] = Define.MODE_INIT;
                        Define.seqCtrl[Convert.ToByte(btn.Tag)] = Define.CTRL_RUN;
                        Define.seqSts[Convert.ToByte(btn.Tag)] = Define.STS_IDLE;
                    }
                    break;

                case "Stop":
                    {
                        Define.seqCtrl[Convert.ToByte(btn.Tag)] = Define.CTRL_ABORT;
                    }
                    break;
            }
        }

        private void checkBox_ManualLamp_Click(object sender, EventArgs e)
        {
            if (checkBox_ManualLamp.Checked)
                Define.bManualLamp = true;
            else
                Define.bManualLamp = false; 
        }

        private void Btn_Lamp_Click(object sender, EventArgs e)
        {
            if (Btn_Lamp.Tag.ToString() == "0")
            {
                Btn_Lamp.Tag = "1";
                Btn_Lamp.Text = "ON";
                Btn_Lamp.ForeColor = Color.Lime;

                Global.SetDigValue((int)DigOutputList.FluorescentLamp_o, (uint)DigitalOffOn.On, "PM1");
            }
            else
            {
                Btn_Lamp.Tag = "0";
                Btn_Lamp.Text = "OFF";
                Btn_Lamp.ForeColor = Color.Red;

                Global.SetDigValue((int)DigOutputList.FluorescentLamp_o, (uint)DigitalOffOn.Off, "PM1");
            }
        }
    }
}

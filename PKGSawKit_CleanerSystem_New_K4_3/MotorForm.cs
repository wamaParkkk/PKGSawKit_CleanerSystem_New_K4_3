using Ajin_motion_driver;
using System;
using System.Reflection;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace PKGSawKit_CleanerSystem_New_K4_3
{
    public partial class MotorForm : UserControl
    {
        private MaintnanceForm m_Parent;

        AnalogDlg analogDlg;
        DigitalDlg digitalDlg;

        int module;
        string ModuleName;
        bool bDisplay;

        private Timer logdisplayTimer = new Timer();

        private TextBox[] m_servoBox;
        private TextBox[] m_runStsBox;
        private TextBox[] m_almStsBox;
        private TextBox[] m_limitStsBox;
        private TextBox[] m_actSpdBox;
        private TextBox[] m_setSpdBox;
        private TextBox[] m_actAccelBox;
        private TextBox[] m_setAccelBox;
        private TextBox[] m_actDecelBox;
        private TextBox[] m_setDecelBox;
        private TextBox[] m_actGearBox;
        private TextBox[] m_setGearBox;
        private TextBox[] m_actPositionBox;               
        private TextBox[] m_setPositionBox;

        public MotorForm(MaintnanceForm parent)
        {
            m_Parent = parent;
            
            module = (int)MODULE._MOTOR;
            ModuleName = "MOTOR";

            InitializeComponent();               
        }

        private void MotorForm_Load(object sender, EventArgs e)
        {
            Width = 1172;
            Height = 824;
            Top = 0;
            Left = 0;

            m_servoBox = new TextBox[1] { textBoxAxis0Servo };
            m_runStsBox = new TextBox[1] { textBoxAxis0Runsts };            
            m_limitStsBox = new TextBox[1] { textBoxAxis0Limit };
            m_actSpdBox = new TextBox[1] { textBoxAxis0SpeedCur };
            m_setSpdBox = new TextBox[1] { textBoxAxis0SpeedSet };
            m_actAccelBox = new TextBox[1] { textBoxAxis0AccelCur };
            m_setAccelBox = new TextBox[1] { textBoxAxis0AccelSet };
            m_actDecelBox = new TextBox[1] { textBoxAxis0DecelCur };
            m_setDecelBox = new TextBox[1] { textBoxAxis0DecelSet };
            m_actGearBox = new TextBox[1] { textBoxAxis0GearCur };
            m_setGearBox = new TextBox[1] { textBoxAxis0GearSet };
            m_actPositionBox = new TextBox[1] { textBoxAxis0PositionCur };
            m_setPositionBox = new TextBox[1] { textBoxAxis0PositionSet };

            bDisplay = true;

            Value_Init();            
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

        private void Value_Init()
        {            
            double dBrushRotationAcelDecl;                                    
            dBrushRotationAcelDecl = Configure_List.Brush_Rotation_Speed * 2;   
            
            MotionClass.SetMotorVelocity(Define.axis_r, Configure_List.Brush_Rotation_Speed);            
            MotionClass.SetMotorAccel(Define.axis_r, dBrushRotationAcelDecl);
            MotionClass.SetMotorDecel(Define.axis_r, dBrushRotationAcelDecl);                        
            MotionClass.SetMotorGearing(Define.axis_r, 1);            
        }

        public void Display()
        {
            SetDoubleBuffered(groupBoxAxis0);            
            
            if (bDisplay)
            {
                m_servoBox[0].Text = MotionClass.motor[0].sR_ServoStatus;
                m_runStsBox[0].Text = MotionClass.motor[0].sR_BusyStatus;
                m_limitStsBox[0].Text = MotionClass.motor[0].sR_HomeStatus;
                m_actSpdBox[0].Text = string.Format("{0:0.0}", MotionClass.motor[0].dR_CmdVelocity);
                m_actAccelBox[0].Text = string.Format("{0:0.0}", MotionClass.motor[0].dW_Accel);
                m_actDecelBox[0].Text = string.Format("{0:0.0}", MotionClass.motor[0].dW_Decel);
                m_actGearBox[0].Text = string.Format("{0:0.0}", MotionClass.motor[0].dW_Gearing);
                m_actPositionBox[0].Text = string.Format("{0:0.000}", MotionClass.motor[0].dR_ActPosition_step);

                m_setSpdBox[0].Text = string.Format("{0:0.0}", MotionClass.motor[0].dW_Velocity);
                m_setAccelBox[0].Text = string.Format("{0:0.0}", MotionClass.motor[0].dW_Accel);
                m_setDecelBox[0].Text = string.Format("{0:0.0}", MotionClass.motor[0].dW_Decel);
                m_setGearBox[0].Text = string.Format("{0:0.0}", MotionClass.motor[0].dW_Gearing);
                m_setPositionBox[0].Text = string.Format("{0:0.000}", MotionClass.motor[0].dW_Position_mm);
                

                if (MotionClass.motor[Define.axis_r].sR_ServoStatus == "SVOFF")
                {
                    if (btnAxis0SVOFF.Enabled != false)
                        btnAxis0SVOFF.Enabled = false;

                    if (!btnAxis0SVON.Enabled)
                        btnAxis0SVON.Enabled = true;
                }
                else
                {
                    if (!btnAxis0SVOFF.Enabled)
                        btnAxis0SVOFF.Enabled = true;

                    if (btnAxis0SVON.Enabled != false)
                        btnAxis0SVON.Enabled = false;
                }                                

                if (MotionClass.motor[Define.axis_r].sR_BusyStatus == "Moving")
                {
                    if (btnAxis0AlarmReset.Enabled != false)
                        btnAxis0AlarmReset.Enabled = false;

                    if (btnAxis0Home.Enabled != false)
                        btnAxis0Home.Enabled = false;

                    if (btnAxis0ZeroSet.Enabled != false)
                        btnAxis0ZeroSet.Enabled = false;
                }
                else
                {
                    if (!btnAxis0AlarmReset.Enabled)
                        btnAxis0AlarmReset.Enabled = true;

                    if (!btnAxis0Home.Enabled)
                        btnAxis0Home.Enabled = true;

                    if (!btnAxis0ZeroSet.Enabled)
                        btnAxis0ZeroSet.Enabled = true;
                }                               
            }                                                           
        }

        private void Velocity_Click(object sender, EventArgs e)
        {
            TextBox txtBox = (TextBox)sender;

            analogDlg = new AnalogDlg();
            analogDlg.Init(0);
            if (analogDlg.ShowDialog() == DialogResult.OK)
            {
                string sVal = analogDlg.m_strResult;
                bool bResult = double.TryParse(sVal, out double dVal);
                if ((bResult) && (dVal <= 3000))
                {
                    double dVelocity = dVal;
                    txtBox.Text = dVelocity.ToString();

                    MotionClass.SetMotorVelocity(Convert.ToInt32(txtBox.Tag), dVelocity);
                }
                else
                {
                    MessageBox.Show("속도 값을 확인해 주세요 (MAX:3000)", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
        }

        private void Accel_Click(object sender, EventArgs e)
        {
            TextBox txtBox = (TextBox)sender;

            analogDlg = new AnalogDlg();
            analogDlg.Init(0);
            if (analogDlg.ShowDialog() == DialogResult.OK)
            {
                txtBox.Text = analogDlg.m_strResult.ToString();
                double dVal = Convert.ToDouble(txtBox.Text);

                MotionClass.SetMotorAccel(Convert.ToInt32(txtBox.Tag), dVal);
            }
        }

        private void Decel_Click(object sender, EventArgs e)
        {
            TextBox txtBox = (TextBox)sender;

            analogDlg = new AnalogDlg();
            analogDlg.Init(0);
            if (analogDlg.ShowDialog() == DialogResult.OK)
            {
                txtBox.Text = analogDlg.m_strResult.ToString();
                double dVal = Convert.ToDouble(txtBox.Text);

                MotionClass.SetMotorDecel(Convert.ToInt32(txtBox.Tag), dVal);
            }
        }

        private void Gearing_Click(object sender, EventArgs e)
        {
            TextBox txtBox = (TextBox)sender;

            analogDlg = new AnalogDlg();
            analogDlg.Init(0);
            if (analogDlg.ShowDialog() == DialogResult.OK)
            {
                txtBox.Text = analogDlg.m_strResult.ToString();
                double dVal = Convert.ToDouble(txtBox.Text);

                MotionClass.SetMotorGearing(Convert.ToInt32(txtBox.Tag), dVal);
            }
        }

        private void Move_Click(object sender, EventArgs e)
        {
            TextBox txtBox = (TextBox)sender;

            analogDlg = new AnalogDlg();
            analogDlg.Init(0);
            if (analogDlg.ShowDialog() == DialogResult.OK)
            {
                txtBox.Text = analogDlg.m_strResult.ToString();
                double dVal = Convert.ToDouble(txtBox.Text);

                if (txtBox.Tag.ToString() == "2")
                {
                    if (Global.MOTION_INTERLOCK_CHECK())
                    {
                        MotionClass.MotorMove(Convert.ToInt32(txtBox.Tag), dVal);
                    }
                    else
                    {
                        MessageBox.Show("EMO switch is on / Door is open", "Interlock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MotionClass.MotorMove(Convert.ToInt32(txtBox.Tag), dVal);
                }
            }
        }

        private void Digital_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;            
            string strTmp = btn.Text.ToString();

            switch (strTmp)
            {
                case "SVOFF":
                    {
                        MotionClass.SetMotorServo(Convert.ToInt32(btn.Tag), (uint)DigitalOffOn.Off);
                    }
                    break;

                case "SVON":
                    {
                        MotionClass.SetMotorServo(Convert.ToInt32(btn.Tag), (uint)DigitalOffOn.On);
                    }
                    break;

                case "Stop":
                    {
                        MotionClass.SetMotorSStop(Convert.ToInt32(btn.Tag));
                    }
                    break;

                case "Alarm reset":
                    {
                        MotionClass.SetAlarmReset(Convert.ToInt32(btn.Tag));
                    }
                    break;

                case "Home":
                    {
                        if (btn.Tag.ToString() == "2")
                        {
                            if (Global.MOTION_INTERLOCK_CHECK())
                            {
                                MotionClass.SetMotorHome(Convert.ToInt32(btn.Tag));
                            }
                            else
                            {
                                MessageBox.Show("EMO switch is on / Door is open", "Interlock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        else
                        {
                            MotionClass.SetMotorHome(Convert.ToInt32(btn.Tag));
                        }                    
                    }
                    break;

                case "Zeroset":
                    {
                        MotionClass.SetZeroset(Convert.ToInt32(btn.Tag));
                    }
                    break;
            }
        }

        private void btnAxis0JogN_MouseDown(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;

            double dVelocity = MotionClass.motor[Convert.ToInt32(btn.Tag)].dW_Velocity;
            double dAccel = MotionClass.motor[Convert.ToInt32(btn.Tag)].dW_Accel;
            double dDecel = MotionClass.motor[Convert.ToInt32(btn.Tag)].dW_Decel;

            if (btn.Tag.ToString() == "2")
            {
                if (Global.MOTION_INTERLOCK_CHECK())
                {
                    MotionClass.MotorJogN(Convert.ToInt32(btn.Tag), dVelocity, dAccel, dDecel);
                }
                else
                {
                    MessageBox.Show("EMO switch is on / Door is open", "Interlock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MotionClass.MotorJogN(Convert.ToInt32(btn.Tag), dVelocity, dAccel, dDecel);
            }                     
        }

        private void btnAxis0JogN_MouseUp(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;

            MotionClass.SetMotorSStop(Convert.ToInt32(btn.Tag));
        }

        private void btnAxis0JogP_MouseDown(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;

            double dVelocity = MotionClass.motor[Convert.ToInt32(btn.Tag)].dW_Velocity;
            double dAccel = MotionClass.motor[Convert.ToInt32(btn.Tag)].dW_Accel;
            double dDecel = MotionClass.motor[Convert.ToInt32(btn.Tag)].dW_Decel;

            if (btn.Tag.ToString() == "2")
            {
                if (Global.MOTION_INTERLOCK_CHECK())
                {
                    MotionClass.MotorJogP(Convert.ToInt32(btn.Tag), dVelocity, dAccel, dDecel);
                }
                else
                {
                    MessageBox.Show("EMO switch is on / Door is open", "Interlock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MotionClass.MotorJogP(Convert.ToInt32(btn.Tag), dVelocity, dAccel, dDecel);
            }            
        }
    }
}

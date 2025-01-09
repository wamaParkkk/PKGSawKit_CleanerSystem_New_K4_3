using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace PKGSawKit_CleanerSystem_New_K4_3
{
    public partial class ConfigureForm : Form
    {
        AnalogDlg AnaDlg;

        public ConfigureForm()
        {            
            InitializeComponent();
        }

        private void ConfigureForm_Load(object sender, EventArgs e)
        {
            Width = 1172;
            Height = 824;
            Top = 0;
            Left = 0;

            PARAMETER_LOAD();
            MOTION_PARAMETER_LOAD();
        }

        private void ConfigureForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Dispose();
        }

        private void PARAMETER_LOAD()
        {
            string sTmpData;
            string FileName = "Configure.txt";

            if (File.Exists(Global.ConfigurePath + FileName))
            {
                byte[] bytes;
                using (var fs = File.Open(Global.ConfigurePath + FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, (int)fs.Length);
                    sTmpData = Encoding.Default.GetString(bytes);

                    char sp = ',';
                    string[] spString = sTmpData.Split(sp);
                    for (int i = 0; i < spString.Length; i++)
                    {
                        Configure_List.Brush_Rotation_Timeout = int.Parse(spString[0]);
                        Configure_List.Brush_FwdBwd_Timeout = int.Parse(spString[1]);
                        Configure_List.Nozzle_FwdBwd_Timeout = int.Parse(spString[2]);
                        Configure_List.Heater_TempSet = double.Parse(spString[3]);

                        txtBoxBrushRotateTimeout.Text = (Configure_List.Brush_Rotation_Timeout).ToString();
                        txtBoxBrushFwdBwdTimeout.Text = (Configure_List.Brush_FwdBwd_Timeout).ToString();
                        txtBoxNozzleFwdBwdTimeout.Text = (Configure_List.Nozzle_FwdBwd_Timeout).ToString();
                        txtBoxWaterTempSet.Text = (Configure_List.Heater_TempSet).ToString();
                    }
                }
            }
        }

        private void MOTION_PARAMETER_LOAD()
        {
            string sTmpData;
            string FileName = "MotionConfigure.txt";

            if (File.Exists(Global.ConfigurePath + FileName))
            {
                byte[] bytes;
                using (var fs = File.Open(Global.ConfigurePath + FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, (int)fs.Length);
                    sTmpData = Encoding.Default.GetString(bytes);

                    char sp = ',';
                    string[] spString = sTmpData.Split(sp);
                    for (int i = 0; i < spString.Length; i++)
                    {                        
                        Configure_List.Brush_Rotation_Speed = int.Parse(spString[0]);                                                
                        
                        txtBoxBrushRotationSpeed.Text = (Configure_List.Brush_Rotation_Speed).ToString();                                                
                    }
                }
            }
        }

        private void txtBoxDoorOpenCloseTimeout_Click(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            int iTag = int.Parse(textBox.Tag.ToString());

            AnaDlg = new AnalogDlg();
            AnaDlg.Init(iTag);
            if (AnaDlg.ShowDialog() == DialogResult.OK)
            {
                textBox.Text = AnaDlg.m_strResult;

                string[] sVal = new string[1];
                string sTemp = textBox.Text.ToString().Trim();
                sVal[0] = sTemp;
                if (!Global.Value_Check(sVal))
                {
                    MessageBox.Show("잘못 된 값이 입력되었습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox.Text = "0";
                }
            }
        }

        private void btnParameterSave_Click(object sender, EventArgs e)
        {
            string sBrushRotateTimeout = txtBoxBrushRotateTimeout.Text.ToString().Trim();
            string sBrushFwdBwdTimeout = txtBoxBrushFwdBwdTimeout.Text.ToString().Trim();
            string sNozzleFwdBwdTimeout = txtBoxNozzleFwdBwdTimeout.Text.ToString().Trim();
            string sWaterTempSet = txtBoxWaterTempSet.Text.ToString().Trim();

            if (Parameter_WriteFile(sBrushRotateTimeout, sBrushFwdBwdTimeout, sNozzleFwdBwdTimeout, sWaterTempSet))
            {
                Configure_List.Brush_Rotation_Timeout = int.Parse(sBrushRotateTimeout);
                Configure_List.Brush_FwdBwd_Timeout = int.Parse(sBrushFwdBwdTimeout);
                Configure_List.Nozzle_FwdBwd_Timeout = int.Parse(sNozzleFwdBwdTimeout);
                Configure_List.Heater_TempSet = double.Parse(sWaterTempSet);

                MessageBox.Show("Configure 값이 저장 되었습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Configure 값이 저장 되지 않았습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private bool Parameter_WriteFile(string param1, string param2, string param3, string param4)
        {
            string FileName = "Configure.txt";

            try
            {
                File.WriteAllText(Global.ConfigurePath + FileName, param1 + "," + param2 + "," + param3 + "," + param4, Encoding.Default);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "알림");
                return false;
            }
        }

        private void btnMotionParameterSave_Click(object sender, EventArgs e)
        {            
            string sBrushRotationSpeed = txtBoxBrushRotationSpeed.Text.ToString().Trim();                        

            if (Motion_Parameter_WriteFile(sBrushRotationSpeed))
            {                
                Configure_List.Brush_Rotation_Speed = int.Parse(sBrushRotationSpeed);                                

                MessageBox.Show("Motion parameter 값이 저장 되었습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Motion parameter 값이 저장 되지 않았습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private bool Motion_Parameter_WriteFile(string param1)
        {
            string FileName = "MotionConfigure.txt";

            try
            {
                File.WriteAllText(Global.ConfigurePath + FileName, param1, Encoding.Default);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "알림");
                return false;
            }
        }
    }
}

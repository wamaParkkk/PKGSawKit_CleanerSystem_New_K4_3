using System;
using System.Windows.Forms;

namespace PKGSawKit_CleanerSystem_New_K4_3
{
    public partial class ToolInfoRegistForm : Form
    {
        private int iCH;

        public ToolInfoRegistForm()
        {
            InitializeComponent();
        }

        private void ToolInfoRegistForm_Load(object sender, EventArgs e)
        {
            Top = 350;
            Left = 350;            
        }

        private void ToolInfoRegistForm_Activated(object sender, EventArgs e)
        {
            textBox_User.Focus();
        }

        public void Init(int iModule)
        {
            iCH = iModule;
            
            Define.ToolInfoRegist_User[iModule] = string.Empty;
            Define.ToolInfoRegist_ToolBox[iModule] = string.Empty;
            Define.ToolInfoRegist_MC[iModule] = string.Empty;
            Define.ToolInfoRegist_ToolID[iModule] = string.Empty;
            Define.ToolInfoRegist_Tool_CT[iModule] = string.Empty;
            Define.ToolInfoRegist_Tool_UP[iModule] = string.Empty;
            Define.ToolInfoRegist_Tool_DB[iModule] = string.Empty;
            Define.ToolInfoRegist_Tool_TP[iModule] = string.Empty;
            Define.ToolInfoRegist_Tool_TT[iModule] = string.Empty;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox_User.Text) ||
                string.IsNullOrWhiteSpace(textBox_ToolBox.Text) ||
                string.IsNullOrWhiteSpace(textBox_MC.Text) ||
                string.IsNullOrWhiteSpace(textBox_ToolID.Text))
            {
                MessageBox.Show($"Tool 정보를 입력해 주세요", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                string sInputText = textBox_User.Text.ToString();
                if (sInputText.Length >= 5 && sInputText.Length <= 6)
                {
                    Define.ToolInfoRegist_User[iCH] = textBox_User.Text.ToString();
                }
                else
                {
                    MessageBox.Show($"User 정보가 잘못 입력되었습니다", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                sInputText = textBox_ToolBox.Text.ToString();
                if (sInputText.Length >= 1)
                {
                    Define.ToolInfoRegist_ToolBox[iCH] = textBox_ToolBox.Text.ToString();
                }
                else
                {
                    MessageBox.Show($"Tool 보관함# 정보가 잘못 입력되었습니다", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                sInputText = textBox_MC.Text.ToString();
                if (sInputText.Length >= 1)
                {
                    Define.ToolInfoRegist_MC[iCH] = textBox_MC.Text.ToString();
                }
                else
                {
                    MessageBox.Show($"탈착한 장비# 정보가 잘못 입력되었습니다", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                sInputText = textBox_ToolID.Text.ToString();
                if (sInputText.Length >= 1)
                {
                    Define.ToolInfoRegist_ToolID[iCH] = textBox_ToolID.Text.ToString();
                }
                else
                {
                    MessageBox.Show($"Tool ID 정보가 잘못 입력되었습니다", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (checkBoxToolCT.Checked)
                    Define.ToolInfoRegist_Tool_CT[iCH] = "O";

                if (checkBoxToolUP.Checked)
                    Define.ToolInfoRegist_Tool_UP[iCH] = "O";

                if (checkBoxToolDB.Checked)
                    Define.ToolInfoRegist_Tool_DB[iCH] = "O";

                if (checkBoxToolTP.Checked)
                    Define.ToolInfoRegist_Tool_TP[iCH] = "O";

                if (checkBoxToolTT.Checked)
                    Define.ToolInfoRegist_Tool_TT[iCH] = "O";

                DialogResult = DialogResult.OK;

                Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBox_User_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                textBox_ToolBox.Focus();
            }
        }

        private void textBox_ToolBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                textBox_MC.Focus();
            }
        }       

        private void textBox_MC_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                textBox_ToolID.Focus();
            }
        }        
    }
}

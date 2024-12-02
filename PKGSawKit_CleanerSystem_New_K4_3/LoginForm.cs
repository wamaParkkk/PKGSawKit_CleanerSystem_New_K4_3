using System;
using System.Data.OleDb;
using System.Windows.Forms;

namespace PKGSawKit_CleanerSystem_New_K4_3
{
    public partial class LoginForm : Form
    {
        string sUserId;
        string sUserName;
        string sUserLevel;

        MainForm mainForm;
        KeyboardDlg keyboardDlg;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            Width = 1172;
            Height = 824;
            Top = 0;
            Left = 0;

            textBoxName.Text = "이름";
            textBoxLevel.Text = "권한";
            textBoxPassword.Text = "비밀번호";
            textBoxPassword.PasswordChar = '\0';

            UserDataUpdate();
        }

        private void LoginForm_Activated(object sender, EventArgs e)
        {            
            comboBoxId.Items.Clear();
            UserDataUpdate();
        }

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Dispose();
        }

        private void UserDataUpdate()
        {            
            string connStr = Global.userdataPath;
            using (OleDbConnection conn = new OleDbConnection(connStr))
            {
                conn.Open();

                try
                {
                    string sql = "SELECT * FROM UserTable";
                    OleDbCommand cmd = new OleDbCommand(sql, conn);

                    OleDbDataReader reader;
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        comboBoxId.Items.Add(reader.GetString(0).ToString());
                    }

                    reader.Close();
                }
                catch (OleDbException ex)
                {
                    MessageBox.Show(ex.Message, "알림");
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        private void comboBoxId_SelectedIndexChanged(object sender, EventArgs e)
        {            
            string connStr = Global.userdataPath;
            using (OleDbConnection conn = new OleDbConnection(connStr))
            {
                conn.Open();

                try
                {
                    string sql = "SELECT * FROM UserTable WHERE ID = '" + comboBoxId.Text.ToString() + "' ";
                    OleDbCommand cmd = new OleDbCommand(sql, conn);

                    OleDbDataReader reader;
                    reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        textBoxName.Text = reader.GetString(1).ToString();
                        textBoxLevel.Text = reader.GetString(2).ToString();

                        textBoxPassword.Text = "";
                        textBoxPassword.PasswordChar = '●';
                    }

                    reader.Close();
                }
                catch (OleDbException ex)
                {
                    MessageBox.Show(ex.Message, "알림");
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            F_LOGIN();
        }

        private void textBoxPassword_Click(object sender, EventArgs e)
        {
            keyboardDlg = new KeyboardDlg();
            keyboardDlg.Set_Password(true);
            if (keyboardDlg.ShowDialog() == DialogResult.OK)
            {
                textBoxPassword.Text = keyboardDlg.m_strResult;
            }
        }

        private void textBoxPassword_KeyDown(object sender, KeyEventArgs e)
        {            
            if (e.KeyCode == Keys.Enter)
            {
                F_LOGIN();
            }           
        }

        private void F_LOGIN()
        {            
            string connStr = Global.userdataPath;
            using (OleDbConnection conn = new OleDbConnection(connStr))
            {
                conn.Open();

                try
                {
                    string sql = "SELECT * FROM UserTable WHERE ID = '" + comboBoxId.Text.ToString() + "' and Password = '" + textBoxPassword.Text + "' ";
                    OleDbCommand cmd = new OleDbCommand(sql, conn);

                    OleDbDataReader reader;
                    reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Login 성공
                        Define.bLogin = true;

                        Define.bOpActivate = true;

                        sUserId = comboBoxId.Text.ToString();
                        sUserName = textBoxName.Text.ToString();
                        sUserLevel = textBoxLevel.Text.ToString();

                        Define.UserId = sUserId;
                        Define.UserName = sUserName;
                        Define.UserLevel = sUserLevel;

                        Close();

                        mainForm = new MainForm();                        
                        mainForm.SubFormShow((byte)Page.OperationPage);                        
                        
                        Global.EventLog("[LOG-IN] " + "[" + Define.UserId + "_" + Define.UserName + "_" + Define.UserLevel + "]", "PM1", "Event");
                    }
                    else
                    {
                        MessageBox.Show("사원번호 또는 비밀번호를 확인해주세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    reader.Close();
                }
                catch (OleDbException ex)
                {
                    MessageBox.Show(ex.Message, "알림");
                }
                finally
                {
                    conn.Close();
                }
            }
        }
    }
}
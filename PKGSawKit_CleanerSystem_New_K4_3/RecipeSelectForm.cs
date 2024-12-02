using System;
using System.IO;
using System.Windows.Forms;

namespace PKGSawKit_CleanerSystem_New_K4_3
{
    public partial class RecipeSelectForm : Form
    {
        string ModuleName;

        public RecipeSelectForm()
        {
            InitializeComponent();
        }

        private void RecipeSelectForm_Load(object sender, EventArgs e)
        {
            Top = 250;
            Left = 500;            

            Get_lstRecipeFile();
        }

        private void RecipeSelectForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Dispose();
        }

        private void Get_lstRecipeFile()
        {
            try
            {
                if (Define.iSelectRecipeModule == (byte)MODULE._PM1)
                {
                    ModuleName = "PM1";
                }
                else if (Define.iSelectRecipeModule == (byte)MODULE._PM2)
                {
                    ModuleName = "PM2";
                }
                else
                {
                    MessageBox.Show("선택한 챔버 정보가 없습니다. 프로세스를 재 진행해 주세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                listBox_RecipeName.Items.Clear();

                if (Directory.Exists(string.Format("{0}{1}", Global.RecipeFilePath, ModuleName)))
                {
                    string[] FileList = Directory.GetFiles(string.Format("{0}{1}", Global.RecipeFilePath, ModuleName), "*.csv");
                    string[] strSplit = new string[1];
                    strSplit[0] = "\\";

                    for (int i = 0; i < FileList.Length; i++)
                    {
                        string[] FileSplit = FileList[i].Split(strSplit, StringSplitOptions.RemoveEmptyEntries);
                        listBox_RecipeName.Items.Add(FileSplit[FileSplit.Length - 1]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "알림");
            }
        }

        private void btn_RecipeFile_Select_Click(object sender, EventArgs e)
        {
            if (listBox_RecipeName.SelectedItem != null)
            {
                if (Define.iSelectRecipeModule == (int)MODULE._PM1)
                {
                    Define.sSelectRecipeName[(int)MODULE._PM1] = string.Empty;
                    Define.sSelectRecipeName[(int)MODULE._PM1] = listBox_RecipeName.SelectedItem.ToString();
                }
                else if (Define.iSelectRecipeModule == (int)MODULE._PM2)
                {
                    Define.sSelectRecipeName[(int)MODULE._PM2] = string.Empty;
                    Define.sSelectRecipeName[(int)MODULE._PM2] = listBox_RecipeName.SelectedItem.ToString();
                }                

                this.DialogResult = DialogResult.OK;
            }
        }

        private void btn_RecipeFile_SelectCancel_Click(object sender, EventArgs e)
        {
            Close();
        }        
    }
}

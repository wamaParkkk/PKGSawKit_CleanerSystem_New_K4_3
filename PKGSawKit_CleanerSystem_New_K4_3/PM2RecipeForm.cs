using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PKGSawKit_CleanerSystem_New_K4_3
{
    public partial class PM2RecipeForm : UserControl
    {
        private RecipeForm m_Parent;

        AnalogDlg AnaDlg;
        DigitalDlg DigDlg;
        KeyboardDlg KeyDlg;

        private byte m_nEditMode;

        public PM2RecipeForm(RecipeForm parent)
        {
            m_Parent = parent;

            InitializeComponent();
        }

        private void PM2RecipeForm_Load(object sender, EventArgs e)
        {
            Width = 1172;
            Height = 824;
            Top = 0;
            Left = 0;

            RECIPE_GRID_INIT();
        }

        private void RECIPE_GRID_INIT()
        {
            recipeGrid.AllowUserToAddRows = false;
            recipeGrid.AllowUserToDeleteRows = false;

            recipeGrid.AutoSize = false;
            recipeGrid.RowHeadersVisible = false;
            recipeGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            recipeGrid.ReadOnly = true;
            recipeGrid.AutoGenerateColumns = false;
            recipeGrid.MultiSelect = false;

            recipeGrid.ColumnCount = 2;
            recipeGrid.RowCount = 0;

            recipeGrid.Columns[0].Name = "Item/Step";
            recipeGrid.Columns[1].Name = "1";

            recipeGrid.Rows.Add("Step Name");
            recipeGrid.Rows.Add("Air");            
            recipeGrid.Rows.Add("Water");            
            recipeGrid.Rows.Add("공정 시간");

            Get_lstRecipeFile();

            NEditMode = (byte)RecipeEditMode.NORMAL_MODE;
        }

        private void Get_lstRecipeFile()
        {
            try
            {
                listBox_RecipeName.Items.Clear();

                if (Directory.Exists(string.Format("{0}PM2", Global.RecipeFilePath)))
                {
                    string[] FileList = Directory.GetFiles(string.Format("{0}PM2", Global.RecipeFilePath), "*.csv");
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

        private void listBox_RecipeName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox_RecipeName.SelectedItem != null)
            {
                string strFileName = listBox_RecipeName.SelectedItem.ToString();
                txt_RecipeFileName.Text = strFileName;

                Recipe_Open(strFileName);
            }
        }

        private void Recipe_Open(string strFileName)
        {            
            strFileName = string.Format("{0}PM2\\{1}", Global.RecipeFilePath, strFileName);
            Process_Recipe_Load(strFileName);

            NEditMode = (byte)RecipeEditMode.VIEW_MODE;
        }

        private void Process_Recipe_Load(string FileName)
        {
            try
            {
                string rowValue;
                string[] cellValue;
                recipeGrid.Rows.Clear();
                recipeGrid.Columns.Clear();

                if (File.Exists(FileName))
                {
                    StreamReader streamReader = new StreamReader(FileName);
                    rowValue = streamReader.ReadLine();
                    cellValue = rowValue.Split(',');

                    for (int i = 0; i <= cellValue.Count() - 1; i++)
                    {
                        DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                        column.Name = cellValue[i];
                        column.HeaderText = cellValue[i];
                        recipeGrid.Columns.Add(column);
                    }

                    while (streamReader.Peek() != -1)
                    {
                        rowValue = streamReader.ReadLine();
                        cellValue = rowValue.Split(',');
                        recipeGrid.Rows.Add(cellValue);
                    }
                    streamReader.Close();

                    Grid_NotSortable();
                }
                else
                {
                    MessageBox.Show("Recipe 파일이 선택 되지 않았습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "알림");
            }
        }

        private void recipeGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridView view = sender as DataGridView;

                if (e.RowIndex >= 0 && recipeGrid.Rows.Count - 1 >= e.RowIndex && e.ColumnIndex > 0)
                {
                    DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell)recipeGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    txt_CurStep.Text = e.ColumnIndex.ToString();
                    txt_Step.Text = e.ColumnIndex.ToString() + " / " + (recipeGrid.Columns.Count - 1);

                    if (m_nEditMode == (byte)RecipeEditMode.EDIT_MODE)
                    {
                        if (e.RowIndex == 0)
                        {
                            KeyDlg = new KeyboardDlg();
                            KeyDlg.Set_Password(false);
                            if (KeyDlg.ShowDialog() == DialogResult.OK)
                            {
                                cell.Value = KeyDlg.m_strResult;
                            }
                        }
                        else if ((e.RowIndex >= 1) && (e.RowIndex <= 2))
                        {
                            DigDlg = new DigitalDlg();
                            DigDlg.Init("Off", "On", "--");
                            if (DigDlg.ShowDialog() == DialogResult.OK)
                            {
                                cell.Value = DigDlg.m_strResult;
                                if (cell.Value == "On")
                                {
                                    view.CurrentCell.Style.BackColor = Color.SkyBlue;
                                }
                                else if (cell.Value == "Off")
                                {
                                    view.CurrentCell.Style.BackColor = Color.White;
                                }
                                else
                                {
                                    cell.Value = "Off";
                                    view.CurrentCell.Style.BackColor = Color.White;
                                }
                            }
                        }
                        else if (e.RowIndex >= 3)
                        {
                            AnaDlg = new AnalogDlg();
                            AnaDlg.Init();
                            if (AnaDlg.ShowDialog() == DialogResult.OK)
                            {
                                cell.Value = AnaDlg.m_strResult;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "알림");
            }
        }

        private void btn_RecipeFile_New_Click(object sender, EventArgs e)
        {
            try
            {
                KeyDlg = new KeyboardDlg();
                KeyDlg.Set_Password(false);
                if (KeyDlg.ShowDialog() == DialogResult.OK)
                {
                    txt_RecipeFileName.Text = KeyDlg.m_strResult;
                    if (txt_RecipeFileName.Text.IndexOf(".csv") < 0)
                    {
                        txt_RecipeFileName.Text += ".csv";
                    }
                    txt_Step.Text = "0 / 0";
                    txt_CurStep.Text = "0";
                    
                    string RecipePath = string.Format("{0}PM2\\", Global.RecipeFilePath);                    
                    if (File.Exists(string.Format("{0}{1}", RecipePath, txt_RecipeFileName.Text)))
                    {
                        MessageBox.Show("같은 이름의 파일이 이미 존재 합니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else
                    {
                        RECIPE_GRID_INIT();

                        NEditMode = (byte)RecipeEditMode.EDIT_MODE;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "알림");
            }
        }

        private void btn_RecipeFile_Edit_Click(object sender, EventArgs e)
        {
            NEditMode = (byte)RecipeEditMode.EDIT_MODE;
        }

        private void btn_RecipeFile_Copy_Click(object sender, EventArgs e)
        {
            try
            {
                if (listBox_RecipeName.Items.Count > 0)
                {
                    if (0 <= listBox_RecipeName.SelectedIndex && listBox_RecipeName.SelectedIndex < listBox_RecipeName.Items.Count)
                    {
                        string strFileName = listBox_RecipeName.SelectedItem.ToString();
                        string strCopyName = "";
                        string RecipePath = "";

                        KeyDlg = new KeyboardDlg();
                        KeyDlg.Set_Password(false);

                        if (KeyDlg.ShowDialog() == DialogResult.OK)
                        {
                            strCopyName = KeyDlg.m_strResult;
                            if (strCopyName.IndexOf(".csv") < 0)
                            {
                                strCopyName += ".csv";
                            }
                            
                            RecipePath = string.Format("{0}PM2\\", Global.RecipeFilePath);

                            strFileName = RecipePath + strFileName;
                            strCopyName = RecipePath + strCopyName;

                            if (File.Exists(strFileName))
                            {
                                if (File.Exists(strCopyName))
                                {
                                    MessageBox.Show("같은 이름의 파일이 이미 존재 합니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                                else
                                {
                                    File.Copy(strFileName, strCopyName, true);
                                }
                            }
                        }
                    }
                }

                Get_lstRecipeFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "알림");
            }
        }

        private void btn_RecipeFile_Save_Click(object sender, EventArgs e)
        {
            ExportToCSV();
        }

        private void ExportToCSV()
        {
            try
            {
                int columnCount = recipeGrid.Columns.Count - 1;

                for (int i = 1; i <= 2; i++)
                {
                    for (int j = 1; j <= columnCount; j++)
                    {
                        if ((recipeGrid.Rows[i].Cells[j].Value == null) || (recipeGrid.Rows[i].Cells[j].Value.ToString() == ""))
                        {
                            recipeGrid.Rows[i].Cells[j].Value = "Off";
                        }
                    }
                }
                
                for (int j = 1; j <= columnCount; j++)
                {
                    if ((recipeGrid.Rows[3].Cells[j].Value == null) || (recipeGrid.Rows[3].Cells[j].Value.ToString() == ""))
                    {
                        recipeGrid.Rows[3].Cells[j].Value = "0";
                    }
                }

                string RecipePath = "";
                RecipePath = string.Format("{0}PM2\\", Global.RecipeFilePath);
                RecipePath += txt_RecipeFileName.Text;

                Save_csv(RecipePath, recipeGrid, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "알림");
            }
        }

        private void Save_csv(string fileName, DataGridView dgView, bool header)
        {
            try
            {
                string delimiter = ",";
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                StreamWriter csvExport = new StreamWriter(fs, Encoding.UTF8);

                if (dgView.Rows.Count == 0)
                {
                    return;
                }

                if (header)
                {
                    for (int i = 0; i < dgView.Columns.Count; i++)
                    {
                        csvExport.Write(dgView.Columns[i].HeaderText);
                        if (i != dgView.Columns.Count - 1)
                        {
                            csvExport.Write(delimiter);
                        }
                    }
                }

                csvExport.Write(csvExport.NewLine);

                foreach (DataGridViewRow row in dgView.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        for (int i = 0; i < dgView.Columns.Count; i++)
                        {
                            csvExport.Write(row.Cells[i].Value);
                            if (i != dgView.Columns.Count - 1)
                            {
                                csvExport.Write(delimiter);
                            }
                        }
                        csvExport.Write(csvExport.NewLine);
                    }
                }

                csvExport.Flush();
                csvExport.Close();
                fs.Close();

                MessageBox.Show("Recipe 파일을 저장 했습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);

                NEditMode = (int)RecipeEditMode.NORMAL_MODE;
                Get_lstRecipeFile();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "알림");
            }
        }

        private void btn_RecipeFile_Del_Click(object sender, EventArgs e)
        {
            try
            {
                if (listBox_RecipeName.Items.Count > 0)
                {
                    if (0 <= listBox_RecipeName.SelectedIndex && listBox_RecipeName.SelectedIndex < listBox_RecipeName.Items.Count)
                    {
                        string RecipePath = "";
                        string strFileName = listBox_RecipeName.SelectedItem.ToString();
                        RecipePath = string.Format("{0}PM2\\", Global.RecipeFilePath);
                        strFileName = RecipePath + strFileName;

                        if (File.Exists(strFileName))
                        {
                            if (MessageBox.Show("Recipe 파일을 삭제 하겠습니까?", "알림", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                            {
                                File.Delete(strFileName);
                            }
                        }
                    }
                }

                txt_RecipeFileName.Text = "";
                NEditMode = (byte)RecipeEditMode.NORMAL_MODE;
                Get_lstRecipeFile();
                RECIPE_GRID_INIT();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "알림");
            }
        }

        private void btn_RecipeFile_Cancel_Click(object sender, EventArgs e)
        {
            if (listBox_RecipeName.Items.Count != 0)
                listBox_RecipeName.SelectedIndex = 0;

            Get_lstRecipeFile();
            Recipe_Open(txt_RecipeFileName.Text.ToString());
        }

        private void btn_RecipeStep_Add_Click(object sender, EventArgs e)
        {
            Recipe_Step_Grid_Add();
        }

        private void Recipe_Step_Grid_Add()
        {
            try
            {
                recipeGrid.Columns.Add(recipeGrid.Columns.Count.ToString(), recipeGrid.Columns.Count.ToString());

                //Grid_NotSortable();
                CurrentCol_Update();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "알림");
            }
        }

        private void btn_RecipeStep_Insert_Click(object sender, EventArgs e)
        {
            Recipe_Step_Grid_Insert();
        }

        private void Recipe_Step_Grid_Insert()
        {
            try
            {
                DataGridViewTextBoxColumn InsertCol = new DataGridViewTextBoxColumn();

                int nInsertCol = Convert.ToInt16(txt_CurStep.Text);
                if (nInsertCol <= 0)
                {
                    nInsertCol++;
                }

                recipeGrid.Columns.Insert(nInsertCol, InsertCol);

                for (int i = 1; i < recipeGrid.Columns.Count; i++)
                {
                    recipeGrid.Columns[i].Name = i.ToString();
                    recipeGrid.Columns[i].HeaderText = i.ToString();
                }

                CurrentCol_Update();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "알림");
            }
        }

        private void btn_RecipeStep_Del_Click(object sender, EventArgs e)
        {
            Recipe_Step_Grid_Delete();
        }

        private void Recipe_Step_Grid_Delete()
        {
            try
            {
                if (recipeGrid.Columns.Count > 2)
                {
                    int nDelCol = Convert.ToInt16(txt_CurStep.Text);
                    if (nDelCol > 0)
                    {
                        recipeGrid.Columns.RemoveAt(nDelCol);
                    }

                    for (int i = 1; i < recipeGrid.Columns.Count; i++)
                    {
                        recipeGrid.Columns[i].Name = i.ToString();
                        recipeGrid.Columns[i].HeaderText = i.ToString();
                    }
                }
                else
                {
                    MessageBox.Show("최소 한 개의 공정 스탭은 필요 합니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                CurrentCol_Update();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "알림");
            }
        }

        private void recipeGrid_SelectionChanged(object sender, EventArgs e)
        {
            CurrentCol_Update();
        }

        private void CurrentCol_Update()
        {
            try
            {
                Point CurrentCell = recipeGrid.CurrentCellAddress;
                txt_CurStep.Text = CurrentCell.X.ToString();
                txt_Step.Text = CurrentCell.X.ToString() + " / " + (recipeGrid.Columns.Count - 1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "알림");
            }
        }

        private void Grid_NotSortable()
        {
            foreach (DataGridViewColumn item in recipeGrid.Columns)
            {
                item.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private byte NEditMode
        {
            get
            {
                return m_nEditMode;
            }
            set
            {
                m_nEditMode = value;
                switch (value)
                {
                    case (byte)RecipeEditMode.NORMAL_MODE:
                        {
                            txt_Mode.Text = "VIEW";
                            btn_RecipeFile_New.Enabled = true;
                            btn_RecipeFile_Edit.Enabled = false;
                            btn_RecipeFile_Copy.Enabled = false;
                            btn_RecipeFile_Save.Enabled = false;
                            btn_RecipeFile_Del.Enabled = false;
                            btn_RecipeFile_Cancel.Enabled = false;

                            btn_RecipeStep_Add.Enabled = false;
                            btn_RecipeStep_Insert.Enabled = false;
                            btn_RecipeStep_Del.Enabled = false;
                        }
                        break;

                    case (byte)RecipeEditMode.VIEW_MODE:
                        {
                            txt_Mode.Text = "VIEW";
                            btn_RecipeFile_New.Enabled = true;
                            btn_RecipeFile_Edit.Enabled = true;
                            btn_RecipeFile_Copy.Enabled = true;
                            btn_RecipeFile_Save.Enabled = false;
                            btn_RecipeFile_Del.Enabled = true;
                            btn_RecipeFile_Cancel.Enabled = false;

                            btn_RecipeStep_Add.Enabled = false;
                            btn_RecipeStep_Insert.Enabled = false;
                            btn_RecipeStep_Del.Enabled = false;
                        }
                        break;

                    case (byte)RecipeEditMode.EDIT_MODE:
                        {
                            txt_Mode.Text = "EDIT";
                            btn_RecipeFile_New.Enabled = false;
                            btn_RecipeFile_Edit.Enabled = false;
                            btn_RecipeFile_Copy.Enabled = false;
                            btn_RecipeFile_Save.Enabled = true;
                            btn_RecipeFile_Del.Enabled = false;
                            btn_RecipeFile_Cancel.Enabled = true;

                            btn_RecipeStep_Add.Enabled = true;
                            btn_RecipeStep_Insert.Enabled = true;
                            btn_RecipeStep_Del.Enabled = true;
                        }
                        break;
                }
            }
        }
    }
}

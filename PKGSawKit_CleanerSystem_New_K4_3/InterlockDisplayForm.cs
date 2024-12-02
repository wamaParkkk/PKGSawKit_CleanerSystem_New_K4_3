using System;
using System.Windows.Forms;

namespace PKGSawKit_CleanerSystem_New_K4_3
{
    public partial class InterlockDisplayForm : Form
    {
        public InterlockDisplayForm()
        {
            InitializeComponent();
        }

        private void InterlockDisplayForm_Load(object sender, EventArgs e)
        {
            Top = 350;
            Left = 350;

            labelMessage.Text = Define.sInterlockMsg;
            labelChecklist.Text = Define.sInterlockChecklist;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {            
            DialogResult = DialogResult.OK;            
            
            Close();
        }
    }
}

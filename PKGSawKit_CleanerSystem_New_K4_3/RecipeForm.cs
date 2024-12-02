using System;
using System.Reflection;
using System.Windows.Forms;

namespace PKGSawKit_CleanerSystem_New_K4_3
{
    public partial class RecipeForm : Form
    {
        public PM1RecipeForm m_PM1RecipeForm;
        public PM2RecipeForm m_PM2RecipeForm;

        public RecipeForm()
        {
            InitializeComponent();

            m_PM1RecipeForm = new PM1RecipeForm(this);
            m_PM1RecipeForm.Visible = false;
            Controls.Add(m_PM1RecipeForm);

            m_PM2RecipeForm = new PM2RecipeForm(this);
            m_PM2RecipeForm.Visible = false;
            Controls.Add(m_PM2RecipeForm);
        }

        private void RecipeForm_Load(object sender, EventArgs e)
        {
            if (!m_PM1RecipeForm.Visible)
                m_PM1RecipeForm.Visible = true;

            if (m_PM2RecipeForm.Visible != false)
                m_PM2RecipeForm.Visible = false;
        }

        private void RecipeForm_Activated(object sender, EventArgs e)
        {
            Top = 0;
            Left = 0;

            SetDoubleBuffered(m_PM1RecipeForm);
            SetDoubleBuffered(m_PM2RecipeForm);
        }

        private void RecipeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_PM1RecipeForm.Dispose();
            m_PM2RecipeForm.Dispose();            

            Dispose();
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
    }
}

using System;
using System.Reflection;
using System.Windows.Forms;

namespace PKGSawKit_CleanerSystem_New_K4_3
{
    public partial class MaintnanceForm : Form
    {        
        public PM1Form m_PM1Form;
        public PM2Form m_PM2Form;
        public MotorForm m_motorForm;
        
        public MaintnanceForm()
        {
            InitializeComponent();
            
            m_PM1Form = new PM1Form(this);
            m_PM1Form.Visible = false;
            Controls.Add(m_PM1Form);

            m_PM2Form = new PM2Form(this);
            m_PM2Form.Visible = false;
            Controls.Add(m_PM2Form);

            m_motorForm = new MotorForm(this);
            m_motorForm.Visible = false;
            Controls.Add(m_motorForm);
        }

        private void MaintnanceForm_Load(object sender, EventArgs e)
        {            
            if (!m_PM1Form.Visible)
                m_PM1Form.Visible = true;

            if (m_PM2Form.Visible != false)
                m_PM2Form.Visible = false;

            if (m_motorForm.Visible != false)
                m_motorForm.Visible = false;
        }

        private void MaintnanceForm_Activated(object sender, EventArgs e)
        {
            Top = 0;
            Left = 0;

            SetDoubleBuffered(m_PM1Form);
            SetDoubleBuffered(m_PM2Form);
            SetDoubleBuffered(m_motorForm);
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

        private void MaintnanceForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_PM1Form.Dispose();
            m_PM2Form.Dispose();
            m_motorForm.Dispose();

            Dispose();            
        }

        private void displayTimer_Tick(object sender, EventArgs e)
        {            
            m_PM1Form.Display();            
            m_PM2Form.Display();
            m_motorForm.Display();
        }        
    }
}

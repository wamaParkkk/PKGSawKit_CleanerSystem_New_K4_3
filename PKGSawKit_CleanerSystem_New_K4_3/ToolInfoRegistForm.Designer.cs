
namespace PKGSawKit_CleanerSystem_New_K4_3
{
    partial class ToolInfoRegistForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_User = new System.Windows.Forms.TextBox();
            this.textBox_ToolBox = new System.Windows.Forms.TextBox();
            this.textBox_MC = new System.Windows.Forms.TextBox();
            this.textBox_ToolID = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkBoxToolCT = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.checkBoxToolUP = new System.Windows.Forms.CheckBox();
            this.checkBoxToolDB = new System.Windows.Forms.CheckBox();
            this.checkBoxToolTP = new System.Windows.Forms.CheckBox();
            this.checkBoxToolTT = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.SystemColors.Control;
            this.btnOK.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOK.Font = new System.Drawing.Font("Nirmala UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.ForeColor = System.Drawing.Color.Navy;
            this.btnOK.Location = new System.Drawing.Point(454, 324);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(121, 55);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.Font = new System.Drawing.Font("Nirmala UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.ForeColor = System.Drawing.Color.Navy;
            this.btnCancel.Location = new System.Drawing.Point(601, 324);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(121, 55);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Nirmala UI", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 47);
            this.label1.TabIndex = 53;
            this.label1.Text = "User";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Nirmala UI", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(12, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(225, 47);
            this.label2.TabIndex = 54;
            this.label2.Text = "Tool 보관함#";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Nirmala UI", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(12, 145);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(226, 47);
            this.label3.TabIndex = 55;
            this.label3.Text = "탈착한 장비#";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Nirmala UI", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(12, 210);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(136, 47);
            this.label4.TabIndex = 56;
            this.label4.Text = "Tool ID";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBox_User
            // 
            this.textBox_User.Font = new System.Drawing.Font("Nirmala UI", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_User.Location = new System.Drawing.Point(243, 12);
            this.textBox_User.Name = "textBox_User";
            this.textBox_User.Size = new System.Drawing.Size(480, 54);
            this.textBox_User.TabIndex = 57;
            this.textBox_User.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_User_KeyDown);
            // 
            // textBox_ToolBox
            // 
            this.textBox_ToolBox.Font = new System.Drawing.Font("Nirmala UI", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_ToolBox.Location = new System.Drawing.Point(243, 77);
            this.textBox_ToolBox.Name = "textBox_ToolBox";
            this.textBox_ToolBox.Size = new System.Drawing.Size(480, 54);
            this.textBox_ToolBox.TabIndex = 58;
            this.textBox_ToolBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_ToolBox_KeyDown);
            // 
            // textBox_MC
            // 
            this.textBox_MC.Font = new System.Drawing.Font("Nirmala UI", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_MC.Location = new System.Drawing.Point(243, 142);
            this.textBox_MC.Name = "textBox_MC";
            this.textBox_MC.Size = new System.Drawing.Size(480, 54);
            this.textBox_MC.TabIndex = 59;
            this.textBox_MC.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_MC_KeyDown);
            // 
            // textBox_ToolID
            // 
            this.textBox_ToolID.Font = new System.Drawing.Font("Nirmala UI", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_ToolID.Location = new System.Drawing.Point(243, 207);
            this.textBox_ToolID.Name = "textBox_ToolID";
            this.textBox_ToolID.Size = new System.Drawing.Size(480, 54);
            this.textBox_ToolID.TabIndex = 60;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Azure;
            this.panel1.Controls.Add(this.checkBoxToolTT);
            this.panel1.Controls.Add(this.checkBoxToolTP);
            this.panel1.Controls.Add(this.checkBoxToolDB);
            this.panel1.Controls.Add(this.checkBoxToolUP);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.checkBoxToolCT);
            this.panel1.Location = new System.Drawing.Point(12, 288);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(414, 91);
            this.panel1.TabIndex = 61;
            // 
            // checkBoxToolCT
            // 
            this.checkBoxToolCT.AutoSize = true;
            this.checkBoxToolCT.Font = new System.Drawing.Font("Nirmala UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxToolCT.Location = new System.Drawing.Point(20, 46);
            this.checkBoxToolCT.Name = "checkBoxToolCT";
            this.checkBoxToolCT.Size = new System.Drawing.Size(55, 25);
            this.checkBoxToolCT.TabIndex = 0;
            this.checkBoxToolCT.Text = "C/T";
            this.checkBoxToolCT.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Nirmala UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(102, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(202, 21);
            this.label5.TabIndex = 1;
            this.label5.Text = "Cleaning 진행한 Tool 선택";
            // 
            // checkBoxToolUP
            // 
            this.checkBoxToolUP.AutoSize = true;
            this.checkBoxToolUP.Font = new System.Drawing.Font("Nirmala UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxToolUP.Location = new System.Drawing.Point(100, 46);
            this.checkBoxToolUP.Name = "checkBoxToolUP";
            this.checkBoxToolUP.Size = new System.Drawing.Size(58, 25);
            this.checkBoxToolUP.TabIndex = 2;
            this.checkBoxToolUP.Text = "U/P";
            this.checkBoxToolUP.UseVisualStyleBackColor = true;
            // 
            // checkBoxToolDB
            // 
            this.checkBoxToolDB.AutoSize = true;
            this.checkBoxToolDB.Font = new System.Drawing.Font("Nirmala UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxToolDB.Location = new System.Drawing.Point(180, 46);
            this.checkBoxToolDB.Name = "checkBoxToolDB";
            this.checkBoxToolDB.Size = new System.Drawing.Size(58, 25);
            this.checkBoxToolDB.TabIndex = 3;
            this.checkBoxToolDB.Text = "D/B";
            this.checkBoxToolDB.UseVisualStyleBackColor = true;
            // 
            // checkBoxToolTP
            // 
            this.checkBoxToolTP.AutoSize = true;
            this.checkBoxToolTP.Font = new System.Drawing.Font("Nirmala UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxToolTP.Location = new System.Drawing.Point(260, 46);
            this.checkBoxToolTP.Name = "checkBoxToolTP";
            this.checkBoxToolTP.Size = new System.Drawing.Size(55, 25);
            this.checkBoxToolTP.TabIndex = 4;
            this.checkBoxToolTP.Text = "T/P";
            this.checkBoxToolTP.UseVisualStyleBackColor = true;
            // 
            // checkBoxToolTT
            // 
            this.checkBoxToolTT.AutoSize = true;
            this.checkBoxToolTT.Font = new System.Drawing.Font("Nirmala UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxToolTT.Location = new System.Drawing.Point(340, 46);
            this.checkBoxToolTT.Name = "checkBoxToolTT";
            this.checkBoxToolTT.Size = new System.Drawing.Size(54, 25);
            this.checkBoxToolTT.TabIndex = 5;
            this.checkBoxToolTT.Text = "T/T";
            this.checkBoxToolTT.UseVisualStyleBackColor = true;
            // 
            // ToolInfoRegistForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 391);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.textBox_ToolID);
            this.Controls.Add(this.textBox_MC);
            this.Controls.Add(this.textBox_ToolBox);
            this.Controls.Add(this.textBox_User);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Name = "ToolInfoRegistForm";
            this.Text = "Tool info regist";
            this.Activated += new System.EventHandler(this.ToolInfoRegistForm_Activated);
            this.Load += new System.EventHandler(this.ToolInfoRegistForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_User;
        private System.Windows.Forms.TextBox textBox_ToolBox;
        private System.Windows.Forms.TextBox textBox_MC;
        private System.Windows.Forms.TextBox textBox_ToolID;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox checkBoxToolTP;
        private System.Windows.Forms.CheckBox checkBoxToolDB;
        private System.Windows.Forms.CheckBox checkBoxToolUP;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox checkBoxToolCT;
        private System.Windows.Forms.CheckBox checkBoxToolTT;
    }
}
namespace InvConfig.Views
{
    partial class AboutForm
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
            this.txtChangeLog = new System.Windows.Forms.TextBox();
            this.btnAboutClose = new System.Windows.Forms.Button();
            this.lblDev = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtChangeLog
            // 
            this.txtChangeLog.BackColor = System.Drawing.SystemColors.Info;
            this.txtChangeLog.CausesValidation = false;
            this.txtChangeLog.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtChangeLog.Location = new System.Drawing.Point(12, 36);
            this.txtChangeLog.Multiline = true;
            this.txtChangeLog.Name = "txtChangeLog";
            this.txtChangeLog.ReadOnly = true;
            this.txtChangeLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtChangeLog.Size = new System.Drawing.Size(555, 213);
            this.txtChangeLog.TabIndex = 0;
            this.txtChangeLog.WordWrap = false;
            // 
            // btnAboutClose
            // 
            this.btnAboutClose.Location = new System.Drawing.Point(492, 7);
            this.btnAboutClose.Name = "btnAboutClose";
            this.btnAboutClose.Size = new System.Drawing.Size(75, 23);
            this.btnAboutClose.TabIndex = 1;
            this.btnAboutClose.Text = "CLOSE";
            this.btnAboutClose.UseVisualStyleBackColor = true;
            // 
            // lblDev
            // 
            this.lblDev.AutoSize = true;
            this.lblDev.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDev.Location = new System.Drawing.Point(12, 12);
            this.lblDev.Name = "lblDev";
            this.lblDev.Size = new System.Drawing.Size(271, 13);
            this.lblDev.TabIndex = 2;
            this.lblDev.Text = "DEVELOP BY : CHATRI NGAMBENCHAWONG";
            // 
            // frmAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(579, 261);
            this.Controls.Add(this.lblDev);
            this.Controls.Add(this.btnAboutClose);
            this.Controls.Add(this.txtChangeLog);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAbout";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About & Change Log";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtChangeLog;
        private System.Windows.Forms.Button btnAboutClose;
        private System.Windows.Forms.Label lblDev;
    }
}
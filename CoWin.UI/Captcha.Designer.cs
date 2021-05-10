namespace CoWin.UI
{
    partial class Captcha
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
            this.captchaDisplayer = new System.Windows.Forms.PictureBox();
            this.captchaInputFromUser = new System.Windows.Forms.TextBox();
            this.captchaSubmit = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.captchaDisplayer)).BeginInit();
            this.SuspendLayout();
            // 
            // captchaDisplayer
            // 
            this.captchaDisplayer.BackColor = System.Drawing.SystemColors.Window;
            this.captchaDisplayer.Location = new System.Drawing.Point(30, 17);
            this.captchaDisplayer.Margin = new System.Windows.Forms.Padding(4);
            this.captchaDisplayer.Name = "captchaDisplayer";
            this.captchaDisplayer.Size = new System.Drawing.Size(208, 68);
            this.captchaDisplayer.TabIndex = 0;
            this.captchaDisplayer.TabStop = false;
            // 
            // captchaInputFromUser
            // 
            this.captchaInputFromUser.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.captchaInputFromUser.Location = new System.Drawing.Point(30, 116);
            this.captchaInputFromUser.Margin = new System.Windows.Forms.Padding(4);
            this.captchaInputFromUser.Name = "captchaInputFromUser";
            this.captchaInputFromUser.PlaceholderText = "Enter Captcha Here";
            this.captchaInputFromUser.Size = new System.Drawing.Size(208, 29);
            this.captchaInputFromUser.TabIndex = 1;
            // 
            // captchaSubmit
            // 
            this.captchaSubmit.Location = new System.Drawing.Point(77, 169);
            this.captchaSubmit.Margin = new System.Windows.Forms.Padding(4);
            this.captchaSubmit.Name = "captchaSubmit";
            this.captchaSubmit.Size = new System.Drawing.Size(96, 32);
            this.captchaSubmit.TabIndex = 2;
            this.captchaSubmit.Text = "Submit";
            this.captchaSubmit.UseVisualStyleBackColor = true;
            this.captchaSubmit.Click += new System.EventHandler(this.captchaSubmit_Click);
            // 
            // Captcha
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(260, 234);
            this.Controls.Add(this.captchaSubmit);
            this.Controls.Add(this.captchaInputFromUser);
            this.Controls.Add(this.captchaDisplayer);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Captcha";
            this.Text = "Captcha";
            ((System.ComponentModel.ISupportInitialize)(this.captchaDisplayer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox captchaDisplayer;
        private System.Windows.Forms.TextBox captchaInputFromUser;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button captchaSubmit;
    }
}
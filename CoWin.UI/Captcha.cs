using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CoWin.UI
{
    public partial class Captcha : Form
    {
        private string captchaValue = "";
        public Captcha()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            captchaValue = captchaInputFromUser.Text;
            if (!string.IsNullOrEmpty(captchaValue))
            {
                DialogResult = DialogResult.OK;
                Close();
            }
                
        }

        public string GetCaptchaValue(Image image)
        {
            pictureBox1.Image = image;
            ShowDialog();
            return captchaValue;
        }
    }
}

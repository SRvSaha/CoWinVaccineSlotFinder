using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CoWin.UI
{
    public partial class Captcha : Form
    {
        private string captchaValue = "";
        private bool isCaptchaEntered = false;
        public Captcha()
        {
            InitializeComponent();
        }

        private void captchaSubmit_Click(object sender, EventArgs e)
        {
            captchaValue = captchaInputFromUser.Text;
            if (!string.IsNullOrEmpty(captchaValue))
            {
                isCaptchaEntered = true;
                DialogResult = DialogResult.OK;
                Close();
            }
                
        }

        public string GetCaptchaValue(Image image)
        {
            // TO RUN THE NOTIFIER IN A DIFFERENT THREAD SO THAT IT CAN KEEP NOTIFYING TILL CAPTCHA IS NOT ENTERED
            new Thread(new ThreadStart(NotifyUser)).Start();
            captchaDisplayer.Image = image;
            ShowDialog();
            
            return captchaValue;
        }

        private void NotifyUser()
        {
            while (!isCaptchaEntered)
            {
                Thread.Sleep(500);
                Console.Beep();
            }
        }
    }
}

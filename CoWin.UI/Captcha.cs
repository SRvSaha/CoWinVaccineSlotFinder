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
        private bool isNotifierToBeStopped = false;
        public Captcha()
        {
            InitializeComponent();
        }

        private void captchaSubmit_Click(object sender, EventArgs e)
        {
            captchaValue = captchaInputFromUser.Text;
            if (!string.IsNullOrEmpty(captchaValue))
            {
                DialogResult = DialogResult.OK;
                Close();
                Dispose();
            }   
        }
        
        public string GetCaptchaValue(Image image)
        {
            // TO RUN THE NOTIFIER IN A DIFFERENT THREAD SO THAT IT CAN KEEP NOTIFYING TILL CAPTCHA IS NOT ENTERED
            new Thread(new ThreadStart(NotifyUser)).Start();
            
            captchaDisplayer.Image = image;
            ShowDialog();

            isNotifierToBeStopped = true; // Always Close the Notified BEEP BEEP when going out, so it doesn't create issue
            return captchaValue;
        }

        private void NotifyUser()
        {
            while (!isNotifierToBeStopped)
            {
                Thread.Sleep(500);
                Console.Beep();
            }
        }
    }
}

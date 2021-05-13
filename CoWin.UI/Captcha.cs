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
            this.Shown += Captcha_Shown;
        }

        private void Captcha_Shown(object sender, EventArgs e)
        {
            this.Activate(); // To Resolve the Issue of Diaglog not being Activated whenever there is a Console.ReadLine before it
        }

        private void CaptchaSubmit_Click(object sender, EventArgs e)
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
            isNotifierToBeStopped = true; // Always Close the Notified BEEP BEEP when going out, so that Notifies doesn't keep on beeping

            return captchaValue;
        }

        private void NotifyUser()
        {
            while (!isNotifierToBeStopped)
            {
                Thread.Sleep(300);
                Console.Beep(); // Default Frequency: 800 Hz, Default Duration of Beep: 200 ms
            }
        }
    }
}

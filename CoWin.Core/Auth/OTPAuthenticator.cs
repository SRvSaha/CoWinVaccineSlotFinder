using System;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Threading;
using Newtonsoft.Json;
using CoWin.Models;
using RestSharp;
using CoWin.Providers;
using System.Net;
using System.Security.Cryptography;

namespace CoWin.Auth
{
    class OTPAuthenticator
    {
        private IConfiguration _configuration;
        public static string BEARER_TOKEN;
        private bool isOTPEntered = false;
        public OTPAuthenticator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ValidateUser()
        {
            string endpoint = "";

            if (Convert.ToBoolean(_configuration["CoWinAPI:Auth:IsToBeUsed"]))
            {
                endpoint = _configuration["CoWinAPI:Auth:OTPGeneratorUrl"];
            }
            string requestBody = JsonConvert.SerializeObject(new OtpModel
            {
                Mobile = _configuration["CoWinAPI:Auth:Mobile"],
                Secret = _configuration["CoWinAPI:Auth:Secret"]
            });
            var response = GenerateOTP(endpoint, requestBody);
            string otp = "";
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine($"[INFO] OTP Generated for Mobile No: {_configuration["CoWinAPI:Auth:Mobile"]} at {DateTime.Now}");
                
                // TO Start Beeping OTP to User so that it can be entered.
                new Thread(new ThreadStart(OTPNotifier)).Start();

                var txnID = JsonConvert.DeserializeObject<OtpModel>(response.Content);
                endpoint = _configuration["CoWinAPI:Auth:OTPValidatorUrl"];
                var enteredOtp = ReadUserInput("Please Enter OTP:  [In Case you haven't received OTP, press Enter to Resend OTP]");

                isOTPEntered = true; // Always Close the Notified BEEP BEEP when going out, so that Notifier doesn't keep on beeping

                otp = ComputeSha256Hash(enteredOtp);
                requestBody = JsonConvert.SerializeObject(new OtpModel
                {
                    TransactionId = txnID.TransactionId,
                    Otp = otp
                });
                response = ValidateOTP(endpoint, requestBody);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine($"[INFO] User Validated with Mobile No {_configuration["CoWinAPI:Auth:Mobile"]} at {DateTime.Now}");
                    BEARER_TOKEN = JsonConvert.DeserializeObject<OtpModel>(response.Content).BearerToken;
                }
                else
                {
                    DisplayErrorMessage(response);
                }
            }
            else
            {
                DisplayErrorMessage(response);
            }

        }

        private IRestResponse GenerateOTP(string endpoint, string requestBody)
        {
            IRestResponse response = new APIFacade(_configuration).Post(endpoint, requestBody);
            return response;
        }

        private IRestResponse ValidateOTP(string endpoint, string requestBody)
        {
            IRestResponse response = new APIFacade(_configuration).Post(endpoint, requestBody);
            return response;
        }
        void DisplayErrorMessage(IRestResponse response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] OTP Error - ResponseCode: {response.StatusDescription} ResponseData: {response.Content}");
            }
        }
        private string ReadUserInput(string message)
        {
            Console.WriteLine(message);
            string userInput = Console.ReadLine();
            return userInput;
        }
        static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        private void OTPNotifier()
        {
            while(!isOTPEntered)
            {
                Thread.Sleep(900);
                Console.Beep(500, 200);
                Console.Beep(1000, 300);
                Console.Beep(2000, 400);
            }
        }
    }
}

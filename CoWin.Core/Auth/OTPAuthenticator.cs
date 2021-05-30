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
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.IO;

namespace CoWin.Auth
{
    class OTPAuthenticator
    {
        private readonly IConfiguration _configuration;
        public static string BEARER_TOKEN;
        private bool isOTPEntered = false;
        private readonly string authTokenFilename = "authToken.json";
        private bool isIPThrottled = false;
        public OTPAuthenticator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // TODO: Needs Refactoring based on SRP
        public void ValidateUser()
        {
            isOTPEntered = false;
            string endpoint = "";

            if (Convert.ToBoolean(_configuration["CoWinAPI:Auth:IsToBeUsed"]))
            {
                endpoint = _configuration["CoWinAPI:Auth:OTPGeneratorUrl"];
            }

            var token = GetBearerToken();
            if (!string.IsNullOrEmpty(token))
            {
                // Check if user already has a valid bearer token, if yes use it.
                if (IsValidBearerToken(token))
                {
                    BEARER_TOKEN = token;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[INFO] Resuming Session for Mobile No: {_configuration["CoWinAPI:Auth:Mobile"]} at {DateTime.Now} using the provided Bearer Token in {authTokenFilename} file");
                    Console.ResetColor();
                    return;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[WARNING] Invalid/Expired Bearer Token provided in config file. Re-generating OTP to establish session!");
                    Console.ResetColor();
                }
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

                    var otpModel = JsonConvert.DeserializeObject<OtpModel>(response.Content);
                    BEARER_TOKEN = otpModel.BearerToken;
                    SetBearerToken(otpModel);
                    var authTokenPath = Path.Combine(Directory.GetCurrentDirectory(), authTokenFilename);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[INFO] Your Bearer Token is stored here: {authTokenPath} valid for only 15 minutes and will expire on {GetTokenExpiryDate(BEARER_TOKEN)} [Your BearerToken will be re-used till it's not expired, so you have seamless UX.]\n");
                    Console.ResetColor();
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

        private string GetBearerToken()
        {
            string fileName = authTokenFilename;
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            try
            {
                var data = File.ReadAllText(filepath);
                var token = JsonConvert.DeserializeObject<OtpModel>(data);
                return token.BearerToken;
            }
            catch(FileNotFoundException e)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[WARNING] AuthToken file is missing in your machine, so unable to resume your existing session: {e.Message}");
                Console.ResetColor();
                return null;
            }
            catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] GET BEARER TOKEN ERROR: {e.Message} {e.StackTrace}");
                Console.ResetColor();
                return null;
            }
        }

        private void SetBearerToken(OtpModel model)
        {
            string fileName = authTokenFilename;
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            var token = JsonConvert.SerializeObject(model);
            File.WriteAllText(filepath, token);
        }


        private bool IsValidBearerToken(string bearerToken)
        {
            DateTime expiryDateTime = GetTokenExpiryDate(bearerToken);
            DateTime currentDateTime = DateTime.Now;
            if (currentDateTime.CompareTo(expiryDateTime) < 0)
                return true;
            return false;
        }

        private static DateTime GetTokenExpiryDate(string bearerToken)
        {
            var handler = new JwtSecurityTokenHandler();

            var jsonToken = handler.ReadJwtToken(bearerToken);

            var expiryTime = Convert.ToInt64(jsonToken.Claims.First(claim => claim.Type == "exp").Value);

            DateTime expiryDateTime = DateTimeOffset.FromUnixTimeSeconds(expiryTime).LocalDateTime;
            return expiryDateTime;
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
            if (response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                isIPThrottled = false;
                new Thread(new ThreadStart(IPThrolledNotifier)).Start();
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"[FATAL] Response From Server: Too many hits from your IP address, hence request has been blocked. You can try following options :\n1.(By Default) Wait for {_configuration["CoWinAPI:ThrottlingRefreshTimeInSeconds"]} seconds, the Application will Automatically resume working.\n2.Switch to a different network which will change your current IP address.\n3.Close the application and try again after sometime");
                Console.ResetColor();
                Console.WriteLine($"[INFO] If you want to change the duration of refresh time, you can increase/decrease the value of ThrottlingRefreshTimeInSeconds in Config file and restart the Application");
                Thread.Sleep(Convert.ToInt32(_configuration["CoWinAPI:ThrottlingRefreshTimeInSeconds"]) * 1000);
                isIPThrottled = true;
            }
            else 
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] OTP Error - ResponseCode: {response.StatusDescription} ResponseData: {response.Content}");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[WARNING] Could not establish Session : Regenerating Auth Token");
                Console.ResetColor();
                ValidateUser();
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
                Console.Beep(500, 200);
                Console.Beep(1000, 300);
                Console.Beep(2000, 400);
                Thread.Sleep(900);
            }
        }
        private void IPThrolledNotifier()
        {
            while (!isIPThrottled)
            {
                Console.Beep(); // Default Frequency: 800 Hz, Default Duration of Beep: 200 ms
                Thread.Sleep(300);
            }
        }
    }
}

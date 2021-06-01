using CoWin.Models;
using CoWin.Providers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Net;
using Svg;
using System.Text.RegularExpressions;
using System.Xml;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;
using CoWin.Core.Models;

namespace CoWin.Auth
{
    public class Captcha
    {
        private readonly IConfiguration _configuration;
        private readonly Dictionary<string, string> _mapping;
        private bool[] _lookup;
        private bool isIPThrottled = false;
        public Captcha(IConfiguration configuration)
        {
            _configuration = configuration;
            _mapping = JsonConvert.DeserializeObject<Dictionary<string, string>>(Crypto.Decrypt(CaptchaModel.SECRET_KEY, CaptchaModel.ENCRYPTED_TRAINED_MODEL));
            _lookup = BuildLookupForOnlyAlphabets();
        }

        public string GetCurrentCaptchaDetails()
        {
            var generatedCaptcha = GenerateCaptcha();
            var captchaAfterProcessing = ProcessCaptcha(generatedCaptcha);
            return captchaAfterProcessing;
        }

        private string GenerateCaptcha()
        {
            string endpoint = "";
            if (Convert.ToBoolean(_configuration["CoWinAPI:ProtectedAPI:IsToBeUsed"]))
            {
                endpoint = _configuration["CoWinAPI:ProtectedAPI:CaptchaGenerationUrl"];
            }

            IRestResponse response = new APIFacade(_configuration).Post(endpoint);

            if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(response.Content))
            {
                var captchaSvg = JsonConvert.DeserializeObject<CaptchaModel>(response.Content);
                return captchaSvg.Captcha;
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[WARNING] Session Expired : Regenerating Auth Token");
                Console.ResetColor();
                new OTPAuthenticator(_configuration).ValidateUser();
            }
            else if (response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.TooManyRequests)
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
                Console.WriteLine($"\n[ERROR] CAPTCHA GENERATION ERROR ResponseStatus: {response.StatusDescription}, ResponseContent: {response.Content}\n");
                Console.ResetColor();
            }

            return "";
        }
        private string ProcessCaptcha(string captchaSvg)
        {
            string captchadata = "";
            string captchaSource;
            string captchaWithoutNoise = RemoveNoiseFromCaptcha(captchaSvg);
            if (string.IsNullOrEmpty(captchaWithoutNoise))
            {
                return captchadata;
            }
            Bitmap bitmapImage = ConvertCaptchSvgToImage(captchaWithoutNoise);
            if (Convert.ToBoolean(_configuration["CoWinAPI:Auth:AutoReadCaptcha"]))
            {
                captchadata = GetAutoCaptcha(captchaWithoutNoise);
                captchaSource = "AI-AutoCaptcha";
            }
            else
            {
                captchadata = new UI.Captcha().GetCaptchaValue(bitmapImage);
                captchaSource = "Manual-Captcha";
            }
            Console.WriteLine($"[INFO] Captcha entered is from {captchaSource} and value is {captchadata}");
            return captchadata;
        }

        public string GetAutoCaptcha(string captchaWithoutNoise)
        {
            Dictionary<decimal, string> indexInSvg = new Dictionary<decimal, string>();
            XmlDocument svgXml = new XmlDocument();
            svgXml.LoadXml(captchaWithoutNoise);
            foreach (XmlNode item in svgXml.DocumentElement.ChildNodes)
            {
                ComputeCaptcha(indexInSvg, item);
            }
            var sortedDictionary = indexInSvg.OrderBy(x => x.Key); // To Obtain the Characters of Captcha is correct order as shown
            var captcha = string.Join("", sortedDictionary.Select(x => x.Value));
            return captcha;
        }

        private void ComputeCaptcha(Dictionary<decimal, string> indexInSvg, XmlNode item)
        {
            // Get Content within the SVG's path
            var pathRawData = item.Attributes["d"].Value.ToUpper();
            
            // Get the Actual position of the Character in SVG
            var position = Convert.ToDecimal(pathRawData.Substring(1, pathRawData.IndexOf(' ')));
            
            // Remove useless data from path
            var encodedData = KeepOnlyAlphabets(pathRawData);

            // Get character from the encoded data
            var mappedCharacter = _mapping[encodedData];

            // Store the mapping in index for re-creating the order of captcha
            indexInSvg[position] = mappedCharacter; 
        }

        private static Bitmap ConvertCaptchSvgToImage(string captchaWithoutNoise)
        {
            XmlDocument svgXml = new XmlDocument();
            svgXml.LoadXml(captchaWithoutNoise);

            var svgDocument = SvgDocument.Open(svgXml);
            var bitmapImage = svgDocument.Draw();
            return bitmapImage;
        }

        private static string RemoveNoiseFromCaptcha(string captchaSvg)
        {
            var patternForNoise = "<path d=.*?fill=\"none\"/>";
            var captchaWithoutNoise = Regex.Replace(captchaSvg, patternForNoise, string.Empty);
            return captchaWithoutNoise;
        }
         
        private bool[] BuildLookupForOnlyAlphabets(){
            _lookup = new bool[4096];
            for (char c = 'A'; c <= 'Z'; c++)
                _lookup[c] = true;
            return _lookup;
        }

        public string KeepOnlyAlphabets(string input)
        {
            char[] buffer = new char[input.Length];
            int index = 0;
            foreach (char c in input)
            {
                if (_lookup[c])
                {
                    buffer[index] = c;
                    index++;
                }
            }
            return new string(buffer, 0, index);
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

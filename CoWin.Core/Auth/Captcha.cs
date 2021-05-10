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

namespace CoWin.Auth
{
    public class Captcha
    {
        private readonly IConfiguration _configuration;
        private string _bearerToken;
        public Captcha(IConfiguration configuration, string bearerToken)
        {
            _configuration = configuration;
            _bearerToken = bearerToken;
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

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var captchaSvg = JsonConvert.DeserializeObject<CaptchaModel>(response.Content);
                return captchaSvg.Captcha;
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
            string captchaWithoutNoise = RemoveNoiseFromCaptcha(captchaSvg);
            Bitmap bitmapImage = ConvertCaptchSvgToImage(captchaWithoutNoise);
            var captchadata = new UI.Captcha().GetCaptchaValue(bitmapImage);
            return captchadata;
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
    }
}

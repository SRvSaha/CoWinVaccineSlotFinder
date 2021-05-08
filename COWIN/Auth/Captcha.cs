using CoWin.Models;
using CoWin.Providers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace CoWin.Auth
{
    public class Captcha
    {
        private readonly IConfiguration _configuration;
        public Captcha(IConfiguration configuration)
        {
            _configuration = configuration;
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
            // TODO Incorporate the Logic to Display and Enter the Captcha to User or to Auto-Read Captcha details from SVG Captcha
            return "";
        }
    }
}

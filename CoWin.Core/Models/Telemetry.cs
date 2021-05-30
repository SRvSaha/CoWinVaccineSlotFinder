using CoWin.Providers;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Web;

namespace CoWin.Core.Models
{
    public class Telemetry
    {
        private const string ENCRYPTED_ENDPOINT_URL = "YOUR_TELEMETRY_WEBHOOK_ENDPOINT_HERE";
        private const string ENCRYPTED_API_KEY = "YOUR_WEBHOOK_ACCESS_API_KEY";
        private const string SECRET_KEY = "YOUR_SECRET_KEY_FOR_ENCRYPTED_DATA";
        private readonly IConfiguration _configuration;

        public Telemetry(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void SendStatistics(string metadata)
        {
            try
            {
                var builder = new UriBuilder(Crypto.Decrypt(SECRET_KEY, ENCRYPTED_ENDPOINT_URL));
                var queryString = HttpUtility.ParseQueryString(builder.Query);
                queryString["code"] = Crypto.Decrypt(SECRET_KEY, ENCRYPTED_API_KEY);
                builder.Query = queryString.ToString();
                string endpoint = builder.ToString();

                IRestResponse response = new APIFacade(_configuration).Post(endpoint, metadata, false);
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[WARNING] Could to send Telemetry data.");
                    Console.ResetColor();
                }
            }
            catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] Issue faced in Sending Telemetry {e}");
                Console.ResetColor();
            }
        }
    }
}

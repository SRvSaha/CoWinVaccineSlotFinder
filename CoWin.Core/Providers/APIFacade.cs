using RestSharp;
using System;
using Microsoft.Extensions.Configuration;
using System.Net;
using CoWin.Auth;
using CoWin.Core.Utilities;

namespace CoWin.Providers
{
    public class APIFacade
    {
        private readonly IConfiguration _configuration;
        public APIFacade(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IRestResponse Get(string endpoint, bool isCowinRelatedHeadersToBeUsed = true)
        {
            RestClient client = InitHttpClient(endpoint, isCowinRelatedHeadersToBeUsed);

            IRestRequest request = new RestRequest(Method.GET);

            AddGenericHeaders(request, isCowinRelatedHeadersToBeUsed);

            IRestResponse response = client.Execute(request);

            return response;
        }

        private void AddGenericHeaders(IRestRequest request, bool isCowinRelatedHeadersToBeUsed = true)
        {
            request.AddHeader("accept", "application/json");
            request.AddHeader("Accept-Language", "en_US");
            request.AddHeader("Cache-Control", "no-cache, no-store, max-age=0, must-revalidate");

            if (Convert.ToBoolean(_configuration["CoWinAPI:ProtectedAPI:IsToBeUsed"]) && isCowinRelatedHeadersToBeUsed)
            {
                request.AddHeader("Origin", _configuration["CoWinAPI:SelfRegistrationPortal"]);
                request.AddHeader("Referer", _configuration["CoWinAPI:SelfRegistrationPortal"]);
                request.AddHeader("Authorization", $"Bearer {OTPAuthenticator.BEARER_TOKEN}");
            }
        }

        public IRestResponse Post(string endpoint, string body = null, bool isCowinRelatedHeadersToBeUsed = true)
        {
            RestClient client = InitHttpClient(endpoint, isCowinRelatedHeadersToBeUsed);

            IRestRequest request = new RestRequest(Method.POST);

            AddGenericHeaders(request, isCowinRelatedHeadersToBeUsed);

            AddPostSpecificParameters(body, request);

            IRestResponse response = client.Execute(request);

            return response;
        }

        private static void AddPostSpecificParameters(string body, IRestRequest request)
        {
            request.AddHeader("Content-Type", "application/json");

            if(body != null)
            {
                request.AddParameter("application/json", body, ParameterType.RequestBody);
            }
            
        }

        private RestClient InitHttpClient(string endpoint, bool isCowinRelatedHeadersToBeUsed = true)
        {
            var client = new RestClient(endpoint);
            client.Timeout = -1;
            if (Convert.ToBoolean(_configuration["CoWinAPI:ProtectedAPI:IsToBeUsed"]) && isCowinRelatedHeadersToBeUsed)
            {
                client.UserAgent = RandomUserAgent.UserAgent;
            }

            if (Convert.ToBoolean(_configuration["Proxy:IsToBeUsed"]))
            {
                client.Proxy = new WebProxy
                {
                    Address = new Uri(_configuration["Proxy:Address"]),
                    UseDefaultCredentials = true
                };
            }

            return client;
        }
    }
}

using CoWin.Models;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Net;

namespace CoWiN.Models
{
    public class CovidVaccinationCenter
    {
        private readonly IConfiguration _configuration;

        public CovidVaccinationCenter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void GetSlotsByDistrictId(string districtId, string currentDate, string vaccineType)
        {
            IRestResponse response = FetchAllSlotsForDistict(districtId, currentDate, vaccineType);
            if(response.StatusCode == HttpStatusCode.OK)
            {
                var covidVaccinationCenters = CovidVaccinationCenters.FromJson(response.Content);
                GetAvailableSlots(covidVaccinationCenters);
            }
            else
            {
                Console.WriteLine($"\n[ERROR] ResponseStatus: {response.StatusDescription}, ResponseContent: {response.Content}\n");
            }
        }

        private IRestResponse FetchAllSlotsForDistict(string districtId, string currentDate, string vaccineType)
        {
            string endpoint = "";
            if (Convert.ToBoolean(_configuration["CoWinAPI:ProtectedAPI:IsToBeUsed"]))
            {
                endpoint = _configuration["CoWinAPI:ProtectedAPI:Url"] + $"?district_id={districtId}&date={currentDate}&vaccine={vaccineType}";
            }
            else
            {
                endpoint = _configuration["CoWinAPI:BaseUrl"] + $"?district_id={districtId}&date={currentDate}";
            }
            
            var client = new RestClient(endpoint);
            client.Timeout = -1;
            client.UserAgent = _configuration["CoWinAPI:SpoofedUserAgentToBypassWAF"];
            
            if (Convert.ToBoolean(_configuration["Proxy:IsToBeUsed"]))
            {
                client.Proxy = new WebProxy { 
                                                Address = new Uri(_configuration["Proxy:Address"]),
                                                UseDefaultCredentials = true 
                                            };
            }
            
            IRestRequest request = new RestRequest(Method.GET);
            request.AddHeader("accept", "application/json");
            request.AddHeader("Accept-Language", "en_US");
            request.AddHeader("Origin", _configuration["CoWinAPI:SelfRegistrationPortal"]);
            request.AddHeader("Referer", _configuration["CoWinAPI:SelfRegistrationPortal"]);
            request.AddHeader("Authorization", $"Bearer {_configuration["CoWinAPI:ProtectedAPI:BearerToken"]}");

            IRestResponse response = client.Execute(request);
            return response;
        }

        private void GetAvailableSlots(CovidVaccinationCenters covidVaccinationCenters)
        {
            foreach (var cvc in covidVaccinationCenters.Centers)
            {
                foreach (var session in cvc?.Sessions)
                {
                    if (session.MinAgeLimit >= Convert.ToInt16(_configuration["CoWinAPI:MinAgeLimit"]) && 
                        session.MinAgeLimit < Convert.ToInt16(_configuration["CoWinAPI:MaxAgeLimit"]) && 
                        session.AvailableCapacity >= Convert.ToInt16(_configuration["CoWinAPI:MinimumVaccineAvailability"]) &&
                        session.Vaccine == _configuration["CoWinAPI:VaccineType"])
                    {
                        DisplaySlotInfo(cvc, session);
                    }
                }
            }
        }

        private static void DisplaySlotInfo(Center cvc, Session session)
        {
            Console.WriteLine("***************************************************************************************************************");
            Console.WriteLine("Name: " + cvc.Name);
            Console.WriteLine("Address: " + cvc.Address);
            Console.WriteLine("District: " + cvc.DistrictName);
            Console.WriteLine("PIN: " + cvc.Pincode);
            Console.WriteLine("FeeType: " + cvc.FeeType);
            Console.WriteLine("VaccineType: " + session.Vaccine);
            Console.WriteLine("AvailableCapacity: " + session.AvailableCapacity);
            Console.WriteLine("DateOfAvailability: " + session.Date);
            Console.WriteLine("Slots Available: " + string.Join(", ", session.Slots));
            Console.WriteLine("***************************************************************************************************************\n");
        }
    }
}

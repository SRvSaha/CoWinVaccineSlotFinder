using CoWin.Models;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using CoWin.Providers;
using CoWin.Auth;
using Newtonsoft.Json;
using System.Web;
using System.Collections.Specialized;

namespace CoWiN.Models
{
    public class CovidVaccinationCenter
    {
        private readonly IConfiguration _configuration;
        public static bool IS_BOOKING_SUCCESSFUL = false;        

        public CovidVaccinationCenter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void GetSlotsByDistrictId(string districtId, string searchDate, string vaccineType)
        {
            IRestResponse response = FetchAllSlotsForDistict(districtId, searchDate, vaccineType);
            if(response.StatusCode == HttpStatusCode.OK)
            {
                var covidVaccinationCenters = JsonConvert.DeserializeObject<CovidVaccinationCenters>(response.Content);
                GetAvailableSlots(covidVaccinationCenters);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[WARNING] Session Expired : Regenerating Auth Token");
                new OTPAuthenticator(_configuration).ValidateUser();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[ERROR] FETCH SLOTS ERROR ResponseStatus: {response.StatusDescription}, ResponseContent: {response.Content}\n");
                Console.ResetColor();
            }
        }

        public void GetSlotsByPINCode(string pinCode, string searchDate, string vaccineType)
        {
            IRestResponse response = FetchAllSlotsForPINCode(pinCode, searchDate, vaccineType);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var covidVaccinationCenters = JsonConvert.DeserializeObject<CovidVaccinationCenters>(response.Content);
                GetAvailableSlots(covidVaccinationCenters);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[WARNING] Session Expired : Regenerating Auth Token");
                new OTPAuthenticator(_configuration).ValidateUser();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[ERROR] FETCH SLOTS ERROR ResponseStatus: {response.StatusDescription}, ResponseContent: {response.Content}\n");
                Console.ResetColor();
            }
        }

        private IRestResponse FetchAllSlotsForDistict(string districtId, string searchDate, string vaccineType)
        {
            UriBuilder builder;
            NameValueCollection queryString;
            if (Convert.ToBoolean(_configuration["CoWinAPI:ProtectedAPI:IsToBeUsed"]))
            {
                builder = new UriBuilder(_configuration["CoWinAPI:ProtectedAPI:FetchCalenderByDistrictUrl"]);
                queryString = HttpUtility.ParseQueryString(builder.Query);

                if (!string.IsNullOrEmpty(vaccineType)){
                    queryString["vaccine"] = vaccineType;
                }
            }
            else
            {
                builder = new UriBuilder(_configuration["CoWinAPI:PublicAPI:FetchCalenderByDistrictUrl"]);
                queryString = HttpUtility.ParseQueryString(builder.Query);
            }

            queryString["district_id"] = districtId;
            queryString["date"] = searchDate;
            builder.Query = queryString.ToString();

            string endpoint = builder.ToString();

            IRestResponse response = new APIFacade(_configuration).Get(endpoint);

            return response;
        }

        private IRestResponse FetchAllSlotsForPINCode(string pinCode, string searchDate, string vaccineType)
        {
            UriBuilder builder;
            NameValueCollection queryString;
            if (Convert.ToBoolean(_configuration["CoWinAPI:ProtectedAPI:IsToBeUsed"]))
            {
                builder = new UriBuilder(_configuration["CoWinAPI:ProtectedAPI:FetchCalenderByPINUrl"]);
                queryString = HttpUtility.ParseQueryString(builder.Query);

                if (!string.IsNullOrEmpty(vaccineType))
                {
                    queryString["vaccine"] = vaccineType;
                }
            }
            else
            {
                builder = new UriBuilder(_configuration["CoWinAPI:PublicAPI:FetchCalenderByPINUrl"]);
                queryString = HttpUtility.ParseQueryString(builder.Query);
            }

            queryString["pincode"] = pinCode;
            queryString["date"] = searchDate;
            builder.Query = queryString.ToString();

            string endpoint = builder.ToString();

            IRestResponse response = new APIFacade(_configuration).Get(endpoint);

            return response;
        }

        private void GetAvailableSlots(CovidVaccinationCenters covidVaccinationCenters)
        {
            string captcha = ""; 
            foreach (var cvc in covidVaccinationCenters.Centers)
            {
                foreach (var session in cvc?.Sessions)
                {
                    if (IsFiltrationCriteraSatisfied(cvc, session))
                    {
                        DisplaySlotInfo(cvc, session);

                        foreach (var slot in session.Slots)
                        {
                            Console.ResetColor();
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"Trying to Book Appointment for CVC: {cvc.Name} - PIN: {cvc.Pincode} - District: {cvc.DistrictName} - Date: {session.Date} - Slot: {slot}");
                            Console.ResetColor();

                            captcha = new Captcha(_configuration).GetCurrentCaptchaDetails();

                            IS_BOOKING_SUCCESSFUL = BookAvailableSlot(session.SessionId, slot, captcha);

                            if (IS_BOOKING_SUCCESSFUL == true)
                            {
                                return;
                            }

                        }

                    }
                }
            }
        }

        private bool IsFiltrationCriteraSatisfied(Center cvc, Session session)
        {
            var minimumAgeLimitFilter = (session.MinAgeLimit >= Convert.ToInt16(_configuration["CoWinAPI:MinAgeLimit"]));
            var maximumAgeLimitFilter = (session.MinAgeLimit < Convert.ToInt16(_configuration["CoWinAPI:MaxAgeLimit"]));
            var minimumVaccineAvailabiltyFilter = session.AvailableCapacity >= Convert.ToInt16(_configuration["CoWinAPI:MinimumVaccineAvailability"]);
            
            var mandatoryFilters = minimumAgeLimitFilter && maximumAgeLimitFilter && minimumVaccineAvailabiltyFilter;

            // Filter Based on VaccineFeeType only when fee type is provided; otherwise don't filter. Keep both Paid and Free Slots
            var vaccineFeeTypeFilter = string.IsNullOrEmpty(_configuration["CoWinAPI:VaccineFeeType"]) || (cvc.FeeType == _configuration["CoWinAPI:VaccineFeeType"]);

            // Filter Based on VaccinationCentreName when there are multiple CVCs in one PIN/District but user wants slot in a specific CVC
            var vaccinationCentreNameWiseFilter = string.IsNullOrEmpty(_configuration["CoWinAPI:VaccinationCentreName"]) || (cvc.Name == _configuration["CoWinAPI:VaccinationCentreName"]);          
            
            var optionalFilters = vaccineFeeTypeFilter && vaccinationCentreNameWiseFilter;
        
            return mandatoryFilters && optionalFilters;
        }

        private static void DisplaySlotInfo(Center cvc, Session session)
        {
            Console.ResetColor();
            Console.WriteLine("\n***************************************************************************************************************");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Name: " + cvc.Name);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Address: " + cvc.Address);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("PIN: " + cvc.Pincode);
            Console.ResetColor();
            Console.WriteLine("District: " + cvc.DistrictName);
            Console.WriteLine("FeeType: " + cvc.FeeType);
            Console.WriteLine("VaccineType: " + session.Vaccine);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("AvailableCapacity: " + session.AvailableCapacity);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("DateOfAvailability: " + session.Date);
            Console.ResetColor();
            Console.WriteLine("Slots Available: " + string.Join(", ", session.Slots));
            Console.WriteLine("***************************************************************************************************************\n");
        }

        private bool BookAvailableSlot(string sessionId, string slot, string captcha)
        {
            string endpoint = "";
            bool isBookingSuccessful = false;
            List<string> beneficiaries = new List<string>();
            
            if (Convert.ToBoolean(_configuration["CoWinAPI:ProtectedAPI:IsToBeUsed"]))
            {
                endpoint = _configuration["CoWinAPI:ProtectedAPI:ScheduleAppointmentUrl"];
            }

            beneficiaries.AddRange(_configuration.GetSection("CoWinAPI:ProtectedAPI:BeneficiaryIds").Get<List<string>>());

            string requestBody = JsonConvert.SerializeObject(new BookingModel
            {
                Dose = Convert.ToInt32(_configuration["CoWinAPI:DoseType"]),
                SessionId = sessionId,
                Slot = slot,
                Beneficiaries = beneficiaries,
                Captcha = captcha
            });

            IRestResponse response = new APIFacade(_configuration).Post(endpoint, requestBody);

            if(response.StatusCode == HttpStatusCode.OK)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[INFO] CONGRATULATIONS! Booking Success - ResponseCode: {response.StatusDescription} ResponseData: {response.Content}");
                Console.ResetColor();
                isBookingSuccessful = true;
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[WARNING] Session Expired : Regenerating Auth Token");
                new OTPAuthenticator(_configuration).ValidateUser();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] BOOKING ERROR Sorry, Booking Failed - ResponseCode: {response.StatusDescription} ResponseData: {response.Content}");
                isBookingSuccessful = false;
            }
            return isBookingSuccessful;
        }

    }
}

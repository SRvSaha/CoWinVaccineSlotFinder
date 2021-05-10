using CoWin.Models;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using CoWin.Providers;
using CoWin.Auth;
using Newtonsoft.Json;

namespace CoWiN.Models
{
    public class CovidVaccinationCenter
    {
        private readonly IConfiguration _configuration;

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
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[ERROR] FETCH SLOTS ERROR ResponseStatus: {response.StatusDescription}, ResponseContent: {response.Content}\n");
                Console.ResetColor();
            }
        }

        private IRestResponse FetchAllSlotsForDistict(string districtId, string searchDate, string vaccineType)
        {
            string endpoint;
            if (Convert.ToBoolean(_configuration["CoWinAPI:ProtectedAPI:IsToBeUsed"]))
            {
                endpoint = _configuration["CoWinAPI:ProtectedAPI:FetchCalenderByDistrictUrl"] + $"?district_id={districtId}&date={searchDate}&vaccine={vaccineType}";
            }
            else
            {
                endpoint = _configuration["CoWinAPI:PublicAPI:FetchCalenderByDistrictUrl"] + $"?district_id={districtId}&date={searchDate}";
            }

            IRestResponse response = new APIFacade(_configuration).Get(endpoint);

            return response;
        }

        private IRestResponse FetchAllSlotsForPINCode(string pinCode, string searchDate, string vaccineType)
        {
            string endpoint;
            if (Convert.ToBoolean(_configuration["CoWinAPI:ProtectedAPI:IsToBeUsed"]))
            {
                endpoint = _configuration["CoWinAPI:ProtectedAPI:FetchCalenderByPINUrl"] + $"?pincode={pinCode}&date={searchDate}&vaccine={vaccineType}";
            }
            else
            {
                endpoint = _configuration["CoWinAPI:PublicAPI:FetchCalenderByPINUrl"] + $"?pincode={pinCode}&date={searchDate}";
            }

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
                    if (session.MinAgeLimit >= Convert.ToInt16(_configuration["CoWinAPI:MinAgeLimit"]) && 
                        session.MinAgeLimit < Convert.ToInt16(_configuration["CoWinAPI:MaxAgeLimit"]) && 
                        session.AvailableCapacity >= Convert.ToInt16(_configuration["CoWinAPI:MinimumVaccineAvailability"]) &&
                        session.Vaccine == _configuration["CoWinAPI:VaccineType"] &&
                        cvc.FeeType == _configuration["CoWinAPI:VaccineFeeType"] )
                    {
                        DisplaySlotInfo(cvc, session);

                        foreach (var slot in session.Slots)
                        {
                            Console.ResetColor();
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"Trying to Book Appointment for CVC: {cvc.Name} - PIN: {cvc.Pincode} - District: {cvc.DistrictName} - Date: {session.Date} - Slot: {slot}");
                            Console.ResetColor();

                            captcha = new Captcha(_configuration).GetCurrentCaptchaDetails();

                            var isBookingSuccessful = BookAvailableSlot(session.SessionId, slot, captcha);

                            if (isBookingSuccessful == true)
                            {
                                break;
                            }

                        }
                        
                    }
                }
            }
        }

        private static void DisplaySlotInfo(Center cvc, Session session)
        {
            Console.ResetColor();
            Console.WriteLine("\n***************************************************************************************************************");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Name: " + cvc.Name);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Address: " + cvc.Address);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("PIN: " + cvc.Pincode);
            Console.ResetColor();
            Console.WriteLine("District: " + cvc.DistrictName);
            Console.WriteLine("FeeType: " + cvc.FeeType);
            Console.WriteLine("VaccineType: " + session.Vaccine);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("AvailableCapacity: " + session.AvailableCapacity);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("DateOfAvailability: " + session.Date);
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("SessionId: " + session.SessionId);
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

            beneficiaries.Add(_configuration["CoWinAPI:ProtectedAPI:BeneficiaryId"]);

            

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

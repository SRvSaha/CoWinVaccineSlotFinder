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
using CoWin.Core.Models;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Globalization;

namespace CoWiN.Models
{
    public class CovidVaccinationCenter
    {
        private readonly IConfiguration _configuration;
        private readonly List<string> beneficiaries = new List<string>();
        private readonly List<string> _vaccinationCentresToSearch;
        public static bool IS_BOOKING_SUCCESSFUL = false;
        private bool isIPThrottled = false;

        public CovidVaccinationCenter(IConfiguration configuration, List<string> vaccinationCentresToSearch)
        {
            _configuration = configuration;
            _vaccinationCentresToSearch = vaccinationCentresToSearch;
        }

        public void GetSlotsByDistrictId(string districtId, string searchDate, string vaccineType)
        {
            IRestResponse response = FetchAllSlotsForDistict(districtId, searchDate, vaccineType);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var covidVaccinationCenters = JsonConvert.DeserializeObject<CovidVaccinationCenters>(response.Content);
                GetAvailableSlots(covidVaccinationCenters);
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
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[WARNING] Session Expired : Regenerating Auth Token");
                Console.ResetColor();
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
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[WARNING] Session Expired : Regenerating Auth Token");
                Console.ResetColor();
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

                if (!string.IsNullOrEmpty(vaccineType))
                {
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
            List<Center> vaccinationCentres = covidVaccinationCenters.Centers;
            if (_vaccinationCentresToSearch.Count != 0 && Convert.ToBoolean(_configuration["CoWinAPI:IsSearchToBeDoneForVaccinationCentreName"]) == true)
            {
                vaccinationCentres = covidVaccinationCenters.Centers.Where(x => _vaccinationCentresToSearch.Any(centrename => centrename == x.Name.ToUpper().Trim())).ToList();
            }

            if (vaccinationCentres.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[WARNING] Sorry! No Vaccination Centers available for your search criteria");
                Console.ResetColor();
                return;
            }

            foreach (var cvc in vaccinationCentres)
            {
                foreach (var session in cvc.Sessions)
                {
                    if (IsFiltrationCriteriaSatisfied(cvc, session))
                    {
                        if (session.Slots.Count > 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"[INFO] HURRAY! Slots Available for search criteria: Age {_configuration["CoWinAPI:MinAgeLimit"]}-{_configuration["CoWinAPI:MaxAgeLimit"]} - PIN: {cvc.Pincode} - District: {cvc.DistrictName} - Date: {session.Date} - Center : {cvc.Name}");
                            Console.ResetColor();
                            DisplaySlotInfo(cvc, session);
                        }

                        // Processing of Slot Booking in Reverse Order so that chances are higher to get the slot
                        for (int i = session.Slots.Count - 1; i >= 0; i--)
                        {
                            var stopwatch = new Stopwatch();
                            stopwatch.Start();

                            Console.ResetColor();
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"[INFO] Trying to Book Appointment for CVC: {cvc.Name} - PIN: {cvc.Pincode} - District: {cvc.DistrictName} - Date: {session.Date} - Slot: {session.Slots[i]}");
                            Console.ResetColor();

                            string captcha = new Captcha(_configuration).GetCurrentCaptchaDetails();
                            IS_BOOKING_SUCCESSFUL = BookAvailableSlot(session.SessionId, session.Slots[i], captcha);

                            if (IS_BOOKING_SUCCESSFUL == true)
                            {
                                stopwatch.Stop();
                                TimeSpan ts = stopwatch.Elapsed;
                                var captchaMode = Convert.ToBoolean(_configuration["CoWinAPI:Auth:AutoReadCaptcha"]) == true ? "AI AutoCaptcha" : "Manual Captcha";
                                var bookDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                                var appVersion = new VersionChecker(_configuration).GetCurrentVersionFromSystem();
                                var uniqueId = Guid.NewGuid();
                                var timeTakenToBook = ts.TotalSeconds;
                                var source = System.Runtime.InteropServices.OSPlatform.Windows.ToString();

                                var telemetryModel = new TelemetryModel
                                {
                                    UniqueId = uniqueId,
                                    AppVersion = appVersion.ToString().Trim(),
                                    Source = source.Trim(),
                                    BookedOn = DateTime.ParseExact(bookDate, "dd-MM-yyyy HH:mm:ss", new CultureInfo("en-US")),
                                    TimeTakenToBookInSeconds = timeTakenToBook,
                                    CaptchaMode = captchaMode.Trim(),
                                    Latitude = Convert.ToInt32(cvc.Lat),
                                    Longitude = Convert.ToInt32(cvc.Long),
                                    PINCode = Convert.ToInt32(cvc.Pincode),
                                    District = cvc.DistrictName.Trim(),
                                    State = cvc.StateName.Trim(),
                                    BeneficiaryCount = beneficiaries.Count,
                                    MinimumAge = Convert.ToInt32(_configuration["CoWinAPI:MinAgeLimit"]),
                                    MaximumAge = Convert.ToInt32(_configuration["CoWinAPI:MaxAgeLimit"]),
                                };

                                var telemetryMetadata = JsonConvert.SerializeObject(telemetryModel);

                                new Telemetry(_configuration).SendStatistics(telemetryMetadata);

                                new Notifier().Notify($"*SLOT BOOKED SUCCESSFULLY +1* \n\n" +
                                                      $"*LocalAppVersion* : `{ appVersion }`\n" +
                                                      $"*BookedOn* : `{ bookDate }`\n" +
                                                      $"*TimeTakenToBook* : `{ timeTakenToBook } seconds`\n" +
                                                      $"*CaptchaMode* : `{captchaMode}`\n" +
                                                      $"*Latitude* : `{ cvc.Lat}`\n" +
                                                      $"*Longitude* : `{ cvc.Long}`\n" +
                                                      $"*PINCode* : `{cvc.Pincode}`\n" +
                                                      $"*District* : `{ cvc.DistrictName}`\n" +
                                                      $"*State* : `{ cvc.StateName}`\n" +
                                                      $"*BeneficiaryCount* : `{ beneficiaries.Count}`\n" +
                                                      $"*AgeGroup* : `{_configuration["CoWinAPI:MinAgeLimit"]} - {_configuration["CoWinAPI:MaxAgeLimit"]}`\n" +
                                                      $"*Source* : `{ source }`\n" +
                                                      $"*UniqueId* : `{ uniqueId }`\n");
                                return;
                            }
                            stopwatch.Stop();
                        }

                    }
                    else
                    {
                        Console.WriteLine($"[INFO] Sorry! No Slots Available for search criteria: Age {_configuration["CoWinAPI:MinAgeLimit"]}-{_configuration["CoWinAPI:MaxAgeLimit"]} - PIN: {cvc.Pincode} - District: {cvc.DistrictName} - Date: {session.Date} - Center : {cvc.Name}");
                        Console.ResetColor();
                    }
                }                
            }
        }

        private bool IsFiltrationCriteriaSatisfied(Center cvc, Session session)
        {
            bool isAgeCriteriaMet = IsAgeFilterSatisfied(session);
            if (!isAgeCriteriaMet)
                return false;

            bool isVaccineAvailable = IsMinimumVaccineAvailabilityFilterSatisfied(session);
            if (!isVaccineAvailable)
                return false;

            bool vaccineFeeTypeFilter = IsVacineFeeTypeFilterSatisfied(cvc);
            if (!vaccineFeeTypeFilter)
                return false;

            return FilteredResult(isAgeCriteriaMet, isVaccineAvailable, vaccineFeeTypeFilter);
        }

        private static bool FilteredResult(bool isAgeCriteriaMet, bool isVaccineAvailable, bool vaccineFeeTypeFilter)
        {
            var mandatoryFilters = isAgeCriteriaMet && isVaccineAvailable;
            var optionalFilters = vaccineFeeTypeFilter;

            return mandatoryFilters && optionalFilters;
        }

        private bool IsVacineFeeTypeFilterSatisfied(Center cvc)
        {
            // Filter Based on VaccineFeeType only when fee type is provided; otherwise don't filter. Keep both Paid and Free Slots
            return string.IsNullOrEmpty(_configuration["CoWinAPI:VaccineFeeType"]) || (cvc.FeeType.ToUpper() == _configuration["CoWinAPI:VaccineFeeType"].ToUpper().Trim());
        }

        private bool IsAgeFilterSatisfied(Session session)
        {
            bool minimumAgeLimitFilter = (session.MinAgeLimit >= Convert.ToInt16(_configuration["CoWinAPI:MinAgeLimit"]));
            bool maximumAgeLimitFilter = (session.MinAgeLimit < Convert.ToInt16(_configuration["CoWinAPI:MaxAgeLimit"]));
            return minimumAgeLimitFilter && maximumAgeLimitFilter;
        }

        private bool IsMinimumVaccineAvailabilityFilterSatisfied(Session session)
        {
            bool minimumVaccineAvailabiltyFilter;
            var availableVaccines = GetVaccineAvailabiltyForDoseType(session);
            minimumVaccineAvailabiltyFilter = availableVaccines >= Convert.ToInt16(_configuration["CoWinAPI:MinimumVaccineAvailability"]);
            return minimumVaccineAvailabiltyFilter;
        }

        private long GetVaccineAvailabiltyForDoseType(Session session)
        {
            if (Convert.ToInt16(_configuration["CoWinAPI:DoseType"]) == (int)DoseTypeModel.FIRSTDOSE)
            {
                return session.AvailableCapacityFirstDose;
            }
            else
            {
                return session.AvailableCapacitySecondDose;
            }
        }

        private void DisplaySlotInfo(Center cvc, Session session)
        {
            Console.ResetColor();
            Console.WriteLine("\n***************************************************************************************************************");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("Name: " + cvc.Name);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Address: " + cvc.Address);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("PIN: " + cvc.Pincode);
            Console.ResetColor();
            Console.WriteLine("District: " + cvc.DistrictName);
            Console.WriteLine("FeeType: " + cvc.FeeType);
            Console.WriteLine("DoseType: " + Enum.GetName(typeof(DoseTypeModel), Convert.ToInt16(_configuration["CoWinAPI:MinimumVaccineAvailability"])));
            Console.WriteLine("VaccineType: " + session.Vaccine);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("AvailableCapacity: " + GetVaccineAvailabiltyForDoseType(session).ToString());
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
            beneficiaries.Clear(); 

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

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[INFO] CONGRATULATIONS! Booking Success - ResponseCode: {response.StatusDescription} ResponseData: {response.Content}");
                Console.ResetColor();
                isBookingSuccessful = true;
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
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[WARNING] Session Expired : Regenerating Auth Token");
                Console.ResetColor();
                new OTPAuthenticator(_configuration).ValidateUser();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] BOOKING ERROR Sorry, Booking Failed - ResponseCode: {response.StatusDescription} ResponseData: {response.Content}");
                Console.ResetColor();
                isBookingSuccessful = false;
            }
            return isBookingSuccessful;
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

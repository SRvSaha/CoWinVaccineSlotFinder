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
using System.Runtime.InteropServices;
using System.IO;

namespace CoWiN.Models
{
    public class CovidVaccinationCenter
    {
        private readonly IConfiguration _configuration;
        private readonly List<string> beneficiaries = new List<string>();
        private readonly List<string> _vaccinationCentresToSearch;
        public static bool IS_BOOKING_SUCCESSFUL = false;
        private bool isIPThrottled = false;
        private readonly string hardRateLimitHeader = "x-amzn-ErrorType";
        private readonly string hardRateLimitHeaderValue = "AccessDeniedException";
        private readonly string osxBeepPlayer = "say";
        private readonly string osxBeepIPThrottledCommand = "Too Many Requests from Your IP Address. Please wait or try after sometime.";
        private readonly string linuxBeepPlayer = "paplay";
        private readonly string linuxBeepIPThrottledCommand = "linux_notifier.ogg  --volume 65536";
        private string appointmentConfirmationNumber;

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
                if (response.Headers.FirstOrDefault(x => x.Name == hardRateLimitHeader)?.Value.ToString() == hardRateLimitHeaderValue)
                {
                    CovidVaccinationCenterFinder.IS_SEARCH_TO_BE_DONE_IN_REALTIME = false;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[WARNING] Too Many Requests for your current Session; Forcefully Expired Session : Regenerating Auth Token");
                    Console.ResetColor();
                    new OTPAuthenticator(_configuration).ValidateUser(forcefullyLogout: true);
                }
                else
                {
                    HandleRateLimiting();
                }
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
                if (response.Headers.FirstOrDefault(x => x.Name == hardRateLimitHeader)?.Value.ToString() == hardRateLimitHeaderValue)
                {
                    CovidVaccinationCenterFinder.IS_SEARCH_TO_BE_DONE_IN_REALTIME = false;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[WARNING] Too Many Requests for your current Session; Forcefully Expired Session : Regenerating Auth Token");
                    Console.ResetColor();
                    new OTPAuthenticator(_configuration).ValidateUser(forcefullyLogout: true);
                }
                else
                {
                    HandleRateLimiting();
                }
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
            if (CovidVaccinationCenterFinder.IS_SEARCH_TO_BE_DONE_IN_REALTIME)
            {
                builder = new UriBuilder(_configuration["CoWinAPI:ProtectedAPI:FetchByDistrictUrl"]);
            }
            else
            {
                builder = new UriBuilder(_configuration["CoWinAPI:PublicAPI:FetchByDistrictUrl"]);
            }

            queryString = HttpUtility.ParseQueryString(builder.Query);
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
            if (CovidVaccinationCenterFinder.IS_SEARCH_TO_BE_DONE_IN_REALTIME)
            {
                builder = new UriBuilder(_configuration["CoWinAPI:ProtectedAPI:FetchByPINUrl"]);
            }
            else
            {
                builder = new UriBuilder(_configuration["CoWinAPI:PublicAPI:FetchByPINUrl"]);
            }

            queryString = HttpUtility.ParseQueryString(builder.Query);
            queryString["pincode"] = pinCode;
            queryString["date"] = searchDate;
            builder.Query = queryString.ToString();

            string endpoint = builder.ToString();

            IRestResponse response = new APIFacade(_configuration).Get(endpoint);

            return response;
        }

        private void GetAvailableSlots(CovidVaccinationCenters covidVaccinationCenters)
        {
            List<Session> vaccinationCentres = covidVaccinationCenters.Sessions;
            if (_vaccinationCentresToSearch.Count != 0 && Convert.ToBoolean(_configuration["CoWinAPI:IsSearchToBeDoneForVaccinationCentreName"]) == true)
            {
                vaccinationCentres = covidVaccinationCenters.Sessions.Where(x => _vaccinationCentresToSearch.Any(centrename => centrename == x.Name.ToUpper().Trim())).ToList();
            }

            if (vaccinationCentres?.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[WARNING] Sorry! No Vaccination Centers available for your search criteria");
                Console.ResetColor();
                return;
            }

            var filteredVaccinationCentres = FilterVaccinationCentres(vaccinationCentres);

            var sortedVaccinationCentres = SortVaccinationCentres(filteredVaccinationCentres);

            foreach (var session in sortedVaccinationCentres)
            {
                if (session.Slots.Count > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[INFO] HURRAY! Slots Available for search criteria: Age {_configuration["CoWinAPI:MinAgeLimit"]}-{_configuration["CoWinAPI:MaxAgeLimit"]} - PIN: {session.Pincode} - District: {session.DistrictName} - Date: {session.Date} - Center : {session.Name}");
                    Console.ResetColor();
                    DisplaySlotInfo(session);

                    // Processing of Slot Booking in Reverse Order so that chances are higher to get the slot
                    if(_configuration["CoWinAPI:SlotPreference"].ToUpper().Trim() == PreferredSlot.LastSlot)
                    {
                        session.Slots.Reverse();
                    }
                    else if(_configuration["CoWinAPI:SlotPreference"].ToUpper().Trim() == PreferredSlot.RandomSlot)
                    {
                        session.Slots = session.Slots.OrderBy(x => Guid.NewGuid()).ToList();
                    }

                    foreach(var slot in session.Slots)
                    {
                        var stopwatch = new Stopwatch();
                        stopwatch.Start();

                        Console.ResetColor();
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"[INFO] Trying to Book Appointment for session: {session.Name} - PIN: {session.Pincode} - District: {session.DistrictName} - Date: {session.Date} - Slot: {slot}");
                        Console.ResetColor();

                        IS_BOOKING_SUCCESSFUL = BookAvailableSlot(session.SessionId, slot);
                        if (IS_BOOKING_SUCCESSFUL == true)
                        {
                            stopwatch.Stop();
                            SendNotification(session, stopwatch);
                            DownloadAppointmentSlip();
                            return;
                        }
                        stopwatch.Stop();
                    }
                }
                else
                {
                    Console.WriteLine($"[INFO] Sorry! No Slots Available for search criteria: Age {_configuration["CoWinAPI:MinAgeLimit"]}-{_configuration["CoWinAPI:MaxAgeLimit"]} - PIN: {session.Pincode} - District: {session.DistrictName} - Date: {session.Date} - Center : {session.Name}");
                    Console.ResetColor();
                }
            }
        }

        private void DownloadAppointmentSlip()
        {
            try
            {
                UriBuilder builder;
                NameValueCollection queryString;
                var fileName = "appointment_confirmation_no_" + appointmentConfirmationNumber + ".pdf";
                builder = new UriBuilder(_configuration["CoWinAPI:ProtectedAPI:AppointmentSlipUrl"]);
                queryString = HttpUtility.ParseQueryString(builder.Query);
                queryString["appointment_id"] = appointmentConfirmationNumber;
                builder.Query = queryString.ToString();

                string endpoint = builder.ToString();

                IRestResponse response = new APIFacade(_configuration).Get(endpoint);

                if(response.StatusCode == HttpStatusCode.OK)
                {
                    File.WriteAllBytes(Path.Combine(Directory.GetCurrentDirectory(), fileName), response.RawBytes);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\n[INFO] Appointment Slip Successfully Generated and Saved here: {Path.Combine(Directory.GetCurrentDirectory(), fileName)}");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[WARNING] ERROR WHILE DOWNLOADING APPOINTMENT SLIP FROM SERVER! Details: ResponseCode: {response.StatusDescription} ResponseContent: {response.Content}");
                    Console.ResetColor();
                }
                
            }
            catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[WARNING] ERROR WHILE DOWNLOADING APPOINTMENT SLIP, PLEASE DOWNLOAD FROM PORTAL! Details: {e}");
                Console.ResetColor();
            }
            
        }

        private List<Session> FilterVaccinationCentres(List<Session> vaccinationCentres)
        {
            List<Session> filteredVaccinationCentres = new List<Session>();
            foreach (var vaccinationCentre in vaccinationCentres)
            {
                if (IsFiltrationCriteriaSatisfied(vaccinationCentre))
                {
                    filteredVaccinationCentres.Add(vaccinationCentre);
                }
                else
                {
                    Console.WriteLine($"[INFO] Sorry! No Slots Available for search criteria: Age {_configuration["CoWinAPI:MinAgeLimit"]}-{_configuration["CoWinAPI:MaxAgeLimit"]} - PIN: {vaccinationCentre.Pincode} - District: {vaccinationCentre.DistrictName} - Date: {vaccinationCentre.Date} - Center : {vaccinationCentre.Name}");
                }
            }
            return filteredVaccinationCentres;
        }

        private List<Session> SortVaccinationCentres(List<Session> filteredVaccinationCentres)
        {
            List<Session> sortedVaccinationCentres = new List<Session>();
            if (filteredVaccinationCentres.Count == 1)
            {
                return filteredVaccinationCentres;
            }
            else
            {
                if (Convert.ToInt16(_configuration["CoWinAPI:DoseType"]) == (int)DoseTypeModel.FIRSTDOSE)
                {
                    sortedVaccinationCentres = filteredVaccinationCentres.OrderByDescending(x => x.AvailableCapacityFirstDose).ToList();
                }
                else
                {
                    sortedVaccinationCentres = filteredVaccinationCentres.OrderByDescending(x => x.AvailableCapacitySecondDose).ToList();
                }
            }

            return sortedVaccinationCentres;
        }

        private void SendNotification(Session session, Stopwatch stopwatch)
        {
            TimeSpan ts = stopwatch.Elapsed;
            var captchaMode = "Without Captcha";
            var bookDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            var appVersion = new VersionChecker(_configuration).GetCurrentVersionFromSystem();
            var uniqueId = Guid.NewGuid();
            var timeTakenToBook = ts.TotalSeconds;
            var source = OSPlatform.Windows.ToString();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                source = OSPlatform.Linux.ToString();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                source = OSPlatform.OSX.ToString();
            }

            var telemetryMetadata = InitTelemetryModel(session, captchaMode, bookDate, appVersion, uniqueId, timeTakenToBook, source);
            var notificationMetadata = InitNoticationModel(session, captchaMode, bookDate, appVersion, uniqueId, timeTakenToBook, source);

            new Telemetry(_configuration).SendStatistics(telemetryMetadata);

            new Notifier().Notify(notificationMetadata);
        }

        private string InitNoticationModel(Session session, string captchaMode, string bookDate, Version appVersion, Guid uniqueId, double timeTakenToBook, string source)
        {
            string notificationMetadata =   $"*SLOT BOOKED SUCCESSFULLY +1* \n\n" +
                                            $"*LocalAppVersion* : `{ appVersion }`\n" +
                                            $"*BookedOn* : `{ bookDate }`\n" +
                                            $"*TimeTakenToBook* : `{ timeTakenToBook } seconds`\n" +
                                            $"*CaptchaMode* : `{captchaMode}`\n" +
                                            $"*Latitude* : `{ session.Lat}`\n" +
                                            $"*Longitude* : `{ session.Long}`\n" +
                                            $"*PINCode* : `{session.Pincode}`\n" +
                                            $"*District* : `{ session.DistrictName}`\n" +
                                            $"*State* : `{ session.StateName}`\n" +
                                            $"*BeneficiaryCount* : `{ beneficiaries.Count}`\n" +
                                            $"*AgeGroup* : `{_configuration["CoWinAPI:MinAgeLimit"]} - {_configuration["CoWinAPI:MaxAgeLimit"]}`\n" +
                                            $"*Source* : `{ source }`\n" +
                                            $"*UniqueId* : `{ uniqueId }`\n";
            return notificationMetadata;
        }

        private string InitTelemetryModel(Session session, string captchaMode, string bookDate, Version appVersion, Guid uniqueId, double timeTakenToBook, string source)
        {
            var telemetryModel = new TelemetryModel
            {
                UniqueId = uniqueId,
                AppVersion = appVersion.ToString().Trim(),
                Source = source.Trim(),
                BookedOn = DateTime.ParseExact(bookDate, "dd-MM-yyyy HH:mm:ss", new CultureInfo("en-US")),
                TimeTakenToBookInSeconds = timeTakenToBook,
                CaptchaMode = captchaMode.Trim(),
                Latitude = Convert.ToInt32(session.Lat),
                Longitude = Convert.ToInt32(session.Long),
                PINCode = Convert.ToInt32(session.Pincode),
                District = session.DistrictName.Trim(),
                State = session.StateName.Trim(),
                BeneficiaryCount = beneficiaries.Count,
                MinimumAge = Convert.ToInt32(_configuration["CoWinAPI:MinAgeLimit"]),
                MaximumAge = Convert.ToInt32(_configuration["CoWinAPI:MaxAgeLimit"]),
            };

            var telemetryMetadata = JsonConvert.SerializeObject(telemetryModel);
            return telemetryMetadata;
        }

        private bool IsFiltrationCriteriaSatisfied(Session session)
        {
            bool isAgeCriteriaMet = IsAgeFilterSatisfied(session);
            if (!isAgeCriteriaMet)
                return false;

            bool isVaccineAvailable = IsMinimumVaccineAvailabilityFilterSatisfied(session);
            if (!isVaccineAvailable)
                return false;

            bool isVaccineTypeCriteriaMet = IsVacineTypeFilterSatisfied(session);
            if (!isVaccineTypeCriteriaMet)
                return false;

            bool isVaccineFeeTypeCriteriaMet = IsVacineFeeTypeFilterSatisfied(session);
            if (!isVaccineFeeTypeCriteriaMet)
                return false;

            return FilteredResult(isAgeCriteriaMet, isVaccineAvailable, isVaccineTypeCriteriaMet, isVaccineFeeTypeCriteriaMet);
        }

        private static bool FilteredResult(bool isAgeCriteriaMet, bool isVaccineAvailable, bool isVaccineTypeCriteriaMet, bool isVaccineFeeTypeCriteriaMet)
        {
            var mandatoryFilters = isAgeCriteriaMet && isVaccineAvailable;
            var optionalFilters = isVaccineTypeCriteriaMet && isVaccineFeeTypeCriteriaMet;

            return mandatoryFilters && optionalFilters;
        }

        private bool IsVacineTypeFilterSatisfied(Session session)
        {
            return string.IsNullOrEmpty(_configuration["CoWinAPI:VaccineType"]) || (session.Vaccine.ToUpper() == _configuration["CoWinAPI:VaccineType"].ToUpper().Trim());
        }

        private bool IsVacineFeeTypeFilterSatisfied(Session session)
        {
            // Filter Based on VaccineFeeType only when fee type is provided; otherwise don't filter. Keep both Paid and Free Slots
            return string.IsNullOrEmpty(_configuration["CoWinAPI:VaccineFeeType"]) || (session.FeeType.ToUpper() == _configuration["CoWinAPI:VaccineFeeType"].ToUpper().Trim());
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

        private void DisplaySlotInfo(Session session)
        {
            Console.ResetColor();
            Console.WriteLine("\n***************************************************************************************************************");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("Name: " + session.Name);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Address: " + session.Address);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("PIN: " + session.Pincode);
            Console.ResetColor();
            Console.WriteLine("District: " + session.DistrictName);
            Console.WriteLine("FeeType: " + session.FeeType);
            Console.WriteLine("DoseType: " + Enum.GetName(typeof(DoseTypeModel), Convert.ToInt16(_configuration["CoWinAPI:DoseType"])));
            Console.WriteLine("VaccineType: " + session.Vaccine);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("AvailableCapacity: " + GetVaccineAvailabiltyForDoseType(session).ToString());
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("DateOfAvailability: " + session.Date);
            Console.ResetColor();
            Console.WriteLine("Slots Available: " + string.Join(", ", session.Slots));
            Console.WriteLine("***************************************************************************************************************\n");
        }

        private bool BookAvailableSlot(string sessionId, string slot)
        {
            string endpoint = "";
            bool isBookingSuccessful = false;
            beneficiaries.Clear(); 

            endpoint = _configuration["CoWinAPI:ProtectedAPI:ScheduleAppointmentUrl"];
            
            beneficiaries.AddRange(_configuration.GetSection("CoWinAPI:ProtectedAPI:BeneficiaryIds").Get<List<string>>());

            string requestBody = JsonConvert.SerializeObject(new BookingModel
            {
                Dose = Convert.ToInt32(_configuration["CoWinAPI:DoseType"]),
                SessionId = sessionId,
                Slot = slot,
                Beneficiaries = beneficiaries
            });

            IRestResponse response = new APIFacade(_configuration).Post(endpoint, requestBody);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[INFO] CONGRATULATIONS! Booking Success - ResponseCode: {response.StatusDescription} ResponseData: {response.Content}");
                Console.ResetColor();
                appointmentConfirmationNumber = JsonConvert.DeserializeObject<BookingModel>(response.Content)?.AppointmentConfirmationNumber;
                isBookingSuccessful = true;
            }
            else if (response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                if (response.Headers.FirstOrDefault(x => x.Name == hardRateLimitHeader)?.Value.ToString() == hardRateLimitHeaderValue)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[WARNING] Too Many Requests for your current Session; Forcefully Expired Session : Regenerating Auth Token");
                    Console.ResetColor();
                    new OTPAuthenticator(_configuration).ValidateUser(forcefullyLogout: true);
                }
                else
                {
                    HandleRateLimiting();
                }
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

        private void HandleRateLimiting()
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

        private void IPThrolledNotifier()
        {
            while (!isIPThrottled)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start(new ProcessStartInfo(linuxBeepPlayer, Path.Combine(Directory.GetCurrentDirectory(), linuxBeepIPThrottledCommand)) { UseShellExecute = true });
                    Thread.Sleep(300);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start(new ProcessStartInfo(osxBeepPlayer, osxBeepIPThrottledCommand) { UseShellExecute = true });
                    Thread.Sleep(5000);
                }
                else
                {
                    Console.Beep(); // Default Frequency: 800 Hz, Default Duration of Beep: 200 ms
                    Thread.Sleep(300);
                }
            }
        }
    }
}

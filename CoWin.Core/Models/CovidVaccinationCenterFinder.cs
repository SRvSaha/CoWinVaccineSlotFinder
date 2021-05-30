using CoWiN.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using CoWin.Auth;
using System.Globalization;
using CoWin.Core.Exceptions;
using CoWin.Core.Validators;
using CoWin.Core.Models;
using System.Diagnostics;

namespace CoWin.Models
{
    public class CovidVaccinationCenterFinder
    {
        private readonly IConfiguration _configuration;
        private readonly List<string> districtsToSearch = new List<string>();
        private readonly List<string> pinCodesToSearch = new List<string>();
        private readonly List<string> vaccinationCentresToSearch = new List<string>();
        private readonly IValidator<string> _pinCodeValidator, _districtValidator, _mobileNumberValidator, _beneficiaryValidator, _vaccinationCentreNameValidator;
        private readonly IValidator<SearchByDistrictModel> _searchByDistrictValidator;
        private readonly IValidator<SearchByPINCodeModel> _searchByPINCodeValidator;
        private readonly IValidator<SearchByVaccinationCentreNameModel> _searchByVaccinationCentreNameValidator;
        private string searchDate;
        private string vaccineType;
        private int totalSearchObjects = 0;
        private double sleepInterval;

        public CovidVaccinationCenterFinder()
        {
            try
            {
                _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", false, true)
                        .Build();
                _pinCodeValidator = new PINCodeValidator();
                _districtValidator = new DistrictValidator();
                _mobileNumberValidator = new MobileNumberValidator();
                _beneficiaryValidator = new BeneficiaryValidator();
                _searchByDistrictValidator = new SearchByDistrictValidator(_districtValidator);
                _searchByPINCodeValidator = new SearchByPINCodeValidator(_pinCodeValidator);
                _vaccinationCentreNameValidator = new VaccinationCentreNameValidator();
                _searchByVaccinationCentreNameValidator = new SearchByVaccinationCentreNameValidator(_vaccinationCentreNameValidator);
            }
            catch (FormatException e)
            {
                throw new ConfigurationNotInitializedException("Oops! appsettings.json file is not in proper JSON format.", e);
            }
        }
        public void FindSlot()
        {
            CheckSoftwareVersion();
            ConfigureSearchCriteria();
            ValidateSearchCriteria();
            ValidateAuthCriteria();
            ValidateFilterCriteria();
            SetSleepInterval();
            AuthenticateUser();
            SearchForAvailableSlots();
        }

        private void CheckSoftwareVersion()
        {
            var shouldAppBeAllowedToRun = new VersionChecker(_configuration).EvaluateCurrentSoftwareVersion();
            if (shouldAppBeAllowedToRun == false)
                Environment.Exit(0);
        }

        private void ValidateFilterCriteria()
        {

            VaccinationCentreNameValidation();

            SearchByVaccinationCentreNameValidation();

        }

        private void SearchByVaccinationCentreNameValidation()
        {
            var userEnteredSearchByVaccinationCentreNameDto = new SearchByVaccinationCentreNameModel
            {
                IsSearchToBeDoneByVaccinationCentreName = Convert.ToBoolean(_configuration["CoWinAPI:IsSearchToBeDoneForVaccinationCentreName"]),
                VaccinationCentreNames = vaccinationCentresToSearch

            };

            if (!_searchByVaccinationCentreNameValidator.IsValid(userEnteredSearchByVaccinationCentreNameDto))
            {
                throw new InvalidMobileNumberException("Invalid Configuration for Filtering by VaccinationCentreNames: \"IsSearchToBeDoneForVaccinationCentreName\": " + userEnteredSearchByVaccinationCentreNameDto.IsSearchToBeDoneByVaccinationCentreName.ToString() + ", \"VaccinationCentreNames\": [ " + string.Join(", ", vaccinationCentresToSearch) + " ] found in your config file. If you want to search for only specific Vaccination Centres, please set IsSearchToBeDoneForVaccinationCentreName as true and provide proper valid values for VaccinationCentreNames from CoWIN Portal");
            }
        }

        private void VaccinationCentreNameValidation()
        {
            if (Convert.ToBoolean(_configuration["CoWinAPI:IsSearchToBeDoneForVaccinationCentreName"]))
            {
                foreach (var vaccinationCentre in vaccinationCentresToSearch)
                {
                    if (!_vaccinationCentreNameValidator.IsValid(vaccinationCentre))
                    {
                        throw new InvalidVaccinationCentreException("Invalid VaccinationCentreName: " + vaccinationCentre + " found in your config file");
                    }
                }
            }
        }

        private void ValidateAuthCriteria()
        {

            if (!_mobileNumberValidator.IsValid(_configuration["CoWinAPI:Auth:Mobile"]))
            {
                throw new InvalidMobileNumberException("Invalid Mobile Number: " + _configuration["CoWinAPI:Auth:Mobile"] + " found in your config file");
            };

            foreach (var beneficiary in _configuration.GetSection("CoWinAPI:ProtectedAPI:BeneficiaryIds").Get<List<string>>())
            {
                if (!_beneficiaryValidator.IsValid(beneficiary))
                {
                    throw new InvalidBeneficiaryException("Invalid BeneficiaryId: " + beneficiary + " found in your config file.");
                }
            }
        }

        private void ValidateSearchCriteria()
        {
            DistrictValidation();

            PINCodeValidation();

            SearchByDistrictValidation();

            SearchByPINCodeValidation();

        }

        private void SearchByPINCodeValidation()
        {
            var userEnteredSearchByPINCodeDto = new SearchByPINCodeModel
            {
                IsSearchToBeDoneByPINCode = Convert.ToBoolean(_configuration["CoWinAPI:IsSearchToBeDoneByPINCode"]),
                PINCodes = pinCodesToSearch
            };

            if (!_searchByPINCodeValidator.IsValid(userEnteredSearchByPINCodeDto))
            {
                throw new InvalidMobileNumberException("Invalid Configuration for Searching by PINCode: \"IsSearchToBeDoneByPINCode\": " + userEnteredSearchByPINCodeDto.IsSearchToBeDoneByPINCode.ToString() + ", \"PINCodes\": [ " + string.Join(", ", pinCodesToSearch) + " ] found in your config file. If you want to search by PINCode, please set IsSearchToBeDoneByPINCode as true and provide proper valid values for PINCodes");
            }
        }

        private void SearchByDistrictValidation()
        {
            var userEnteredSearchByDistrictDto = new SearchByDistrictModel
            {
                IsSearchToBeDoneByDistrict = Convert.ToBoolean(_configuration["CoWinAPI:IsSearchToBeDoneByDistrict"]),
                Districts = districtsToSearch

            };

            if (!_searchByDistrictValidator.IsValid(userEnteredSearchByDistrictDto))
            {
                throw new InvalidMobileNumberException("Invalid Configuration for Searching by District: \"IsSearchToBeDoneByDistrict\": " + userEnteredSearchByDistrictDto.IsSearchToBeDoneByDistrict.ToString() + ", \"Districts\": [ " + string.Join(", ", districtsToSearch) + " ] found in your config file. If you want to search by District, please set IsSearchToBeDoneByDistrict as true and provide proper valid values for Districts");
            }
        }

        private void PINCodeValidation()
        {
            if (Convert.ToBoolean(_configuration["CoWinAPI:IsSearchToBeDoneByPINCode"]))
            {
                totalSearchObjects += pinCodesToSearch.Count;
                foreach (var pinCode in pinCodesToSearch)
                {
                    if (!_pinCodeValidator.IsValid(pinCode))
                    {
                        throw new InvalidDistrictException("Invalid PINCode: " + pinCode + " found in your config file");
                    }
                }

            }
        }

        private void DistrictValidation()
        {
            if (Convert.ToBoolean(_configuration["CoWinAPI:IsSearchToBeDoneByDistrict"]))
            {
                totalSearchObjects += districtsToSearch.Count;
                foreach (var district in districtsToSearch)
                {
                    if (!_districtValidator.IsValid(district))
                    {
                        throw new InvalidDistrictException("Invalid District: " + district + " found in your config file");
                    }
                }
            }
        }


        private void AuthenticateUser()
        {
            new OTPAuthenticator(_configuration).ValidateUser();
        }

        private void SearchForAvailableSlots()
        {
            for (int i = 1; i < Convert.ToInt32(_configuration["CoWinAPI:TotalIterations"]); i++)
            {
                if (CovidVaccinationCenter.IS_BOOKING_SUCCESSFUL == true)
                {
                    ShowFeedback();
                    return;
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\nFinding Available Slots as per your request, Try #{i}\n");
                Console.ResetColor();

                /* Seaching with be either by PIN or District or Both; By Default by PIN.
                 * If Both are selected for searching, PIN will be given Preference Over District
                 */
                if (Convert.ToBoolean(_configuration["CoWinAPI:IsSearchToBeDoneByPINCode"]))
                {
                    foreach (var pinCode in pinCodesToSearch)
                    {
                        new CovidVaccinationCenter(_configuration, vaccinationCentresToSearch).GetSlotsByPINCode(pinCode, searchDate, vaccineType);

                        if (CovidVaccinationCenter.IS_BOOKING_SUCCESSFUL == true)
                        {
                            ShowFeedback();
                            return;
                        }

                    }
                }
                if (Convert.ToBoolean(_configuration["CoWinAPI:IsSearchToBeDoneByDistrict"]))
                {
                    foreach (var district in districtsToSearch)
                    {
                        new CovidVaccinationCenter(_configuration, vaccinationCentresToSearch).GetSlotsByDistrictId(district, searchDate, vaccineType);

                        if (CovidVaccinationCenter.IS_BOOKING_SUCCESSFUL == true)
                        {
                            ShowFeedback();
                            return;
                        }

                    }
                }
                Thread.Sleep(Convert.ToInt32(sleepInterval));
            }
        }

        private void SetSleepInterval()
        {

            if (Convert.ToBoolean(_configuration["CoWinAPI:IsThrottlingToBeUsed"]))
            {
                if (totalSearchObjects == 1)
                {
                    sleepInterval = Convert.ToInt32(_configuration["CoWinAPI:SleepIntervalInMilliseconds"]);
                }
                else
                {
                    int throttlingThreshold = Convert.ToInt32(_configuration["CoWinAPI:ThrottlingThreshold"]);
                    var throttlingIntervalInMilliSeconds = (Convert.ToInt32(_configuration["CoWinAPI:ThrottlingIntervalInMinutes"])) * 60 * 1000.0;
                    int jitter = 100;
                    sleepInterval = (Math.Ceiling(throttlingIntervalInMilliSeconds / throttlingThreshold) * totalSearchObjects) + jitter;
                }
            }
            else
            {
                sleepInterval = Convert.ToInt32(_configuration["CoWinAPI:SleepIntervalInMilliseconds"]);
            }            
        }

        private void ConfigureSearchCriteria()
        {
            /* Seaching with be either by PIN or District or Both; By Default by PIN.
            * If Both are selected for searching, PIN will be given Preference Over District
            */
            foreach (var item in _configuration.GetSection("CoWinAPI:Districts").GetChildren())
            {
                districtsToSearch.Add(item.Value);
            }

            foreach (var item in _configuration.GetSection("CoWinAPI:PINCodes").GetChildren())
            {
                pinCodesToSearch.Add(item.Value);
            }

            foreach (var item in _configuration.GetSection("CoWinAPI:VaccinationCentreNames").GetChildren())
            {
                vaccinationCentresToSearch.Add(item.Value.ToUpper().Trim());
            }
            

            if (!string.IsNullOrEmpty(_configuration["CoWinAPI:DateToSearch"]))
            {
                searchDate = DateTime.ParseExact(_configuration["CoWinAPI:DateToSearch"], "dd-MM-yyyy", new CultureInfo("en-US")).ToString("dd-MM-yyyy");
            }
            else
            {
                // BY DEFAULT, When CoWinAPI:DateToSearch is Blank, Next Day is chosen as the default date to search for Vaccine
                searchDate = DateTime.Now.AddDays(1).ToString("dd-MM-yyyy");
            }
            vaccineType = _configuration["CoWinAPI:VaccineType"];
        }

        private void ShowFeedback()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n**************************************************************************************************************************************************************************");
            Console.WriteLine("\nGREETINGS!");
            Console.ResetColor();
            Console.WriteLine("Glad that we were able to help you book your slot!");
            Console.WriteLine("We’d love to know about your experience with CoWinVaccineSlotFinder.");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("If you appreciate our work and are willing to share your feedback and word of mouth, please do like, share and post a comment in LinkedIn/Facebook/Twitter/Instagram/Social Media of your Choice with #CoWinVaccineSlotFinder, and star our Github Repository. We’d love to check that out!");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Also, if you would like to support us, you may like to Buy us some Coffee as we are converting Coffee into Code for you through #OpenSource Contributions!");
            Console.ResetColor();
            Console.WriteLine("Thank you in advance for helping us out! Feel free to share the Application with your friends/family/colleagues/circles so that it helps others as well to get the vaccine #covid19help");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n*************************************************************************************************************************************************************************");
            Console.WriteLine("\nPress Any Key to Open the Links in your Browser for supporting us through LinkedIn, Github, BuyMeACoffee and Exit the Application after providing your feedback");
            var input = Console.ReadLine();
            if (input != null)
            {
                Process.Start(new ProcessStartInfo("https://shawt.io/r/sYx") { UseShellExecute = true });
                Process.Start(new ProcessStartInfo("https://shawt.io/r/sYy") { UseShellExecute = true });
                Process.Start(new ProcessStartInfo("https://shawt.io/r/sYG") { UseShellExecute = true });
                Environment.Exit(0);
            }
        }

    }
}

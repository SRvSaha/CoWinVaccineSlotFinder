using CoWiN.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using CoWin.Auth;
using System.Globalization;

namespace CoWin.Models
{
    public class CovidVaccinationCenterFinder
    {
        private IConfiguration _configuration;
        private List<string> districtsToSearch = new List<string>();
        private List<string> pinCodesToSearch = new List<string>();
        private string searchDate;
        private string vaccineType;
        
        public CovidVaccinationCenterFinder()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .Build();
        }
        public void FindSlot()
        {
            AuthenticateUser();
            ConfigureSearchCriteria();
            SearchForAvailableSlots();
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
                    return;
                }

                Console.ResetColor();
                Console.WriteLine($"Fetching Resources, Try #{i}");

                /* Seaching with be either by PIN or District or Both; By Default by PIN.
                 * If Both are selected for searching, PIN will be given Preference Over District
                 */
                if (Convert.ToBoolean(_configuration["CoWinAPI:IsSearchToBeDoneByPINCode"]))
                {
                    foreach (var pinCode in pinCodesToSearch)
                    {
                        new CovidVaccinationCenter(_configuration).GetSlotsByPINCode(pinCode, searchDate, vaccineType);

                        if (CovidVaccinationCenter.IS_BOOKING_SUCCESSFUL == true)
                        {
                            return;
                        }

                    }
                }
                if (Convert.ToBoolean(_configuration["CoWinAPI:IsSearchToBeDoneByDistrict"]))
                {
                    foreach (var district in districtsToSearch)
                    {
                        new CovidVaccinationCenter(_configuration).GetSlotsByDistrictId(district, searchDate, vaccineType);
                        
                        if (CovidVaccinationCenter.IS_BOOKING_SUCCESSFUL == true)
                        {
                            return;
                        }

                    }
                }
                
                Thread.Sleep(Convert.ToInt32(_configuration["CoWinAPI:SleepIntervalInMilliseconds"]));
            }
        }

        private void ConfigureSearchCriteria()
        {
            /* Seaching with be either by PIN or District or Both; By Default by PIN.
            * If Both are selected for searching, PIN will be given Preference Over District
            */

            if (Convert.ToBoolean(_configuration["CoWinAPI:IsSearchToBeDoneByDistrict"]))
            {
                foreach (var item in _configuration.GetSection("CoWinAPI:Districts").GetChildren())
                {
                    districtsToSearch.Add(item.Value);
                }
            }

            if (Convert.ToBoolean(_configuration["CoWinAPI:IsSearchToBeDoneByPINCode"]))
            {
                foreach (var item in _configuration.GetSection("CoWinAPI:PINCodes").GetChildren())
                {
                    pinCodesToSearch.Add(item.Value);
                }
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

    }
}

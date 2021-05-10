using CoWiN.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using CoWin.Auth;

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
                    break;
                }
                Console.ResetColor();
                Console.WriteLine($"Fetching Resources, Try #{i}");

                if (Convert.ToBoolean(_configuration["CoWinAPI:IsSearchToBeDoneByDistrict"]))
                {
                    foreach (var district in districtsToSearch)
                    {
                        new CovidVaccinationCenter(_configuration).GetSlotsByDistrictId(district, searchDate, vaccineType);
                    }
                }
                else if (Convert.ToBoolean(_configuration["CoWinAPI:IsSearchToBeDoneByPINCode"]))
                {
                    foreach (var pinCode in pinCodesToSearch)
                    {
                        new CovidVaccinationCenter(_configuration).GetSlotsByPINCode(pinCode, searchDate, vaccineType);
                    }
                }

                Thread.Sleep(Convert.ToInt32(_configuration["CoWinAPI:SleepIntervalInMilliseconds"]));
            }
        }

        private void ConfigureSearchCriteria()
        {
            if (Convert.ToBoolean(_configuration["CoWinAPI:IsSearchToBeDoneByDistrict"]))
            {
                foreach (var item in _configuration.GetSection("Districts").GetChildren())
                {
                    districtsToSearch.Add(item.Value);
                }
            }
            else if (Convert.ToBoolean(_configuration["CoWinAPI:IsSearchToBeDoneByPINCode"]))
            {
                foreach (var item in _configuration.GetSection("PINCodes").GetChildren())
                {
                    pinCodesToSearch.Add(item.Value);
                }
            }
            if (!string.IsNullOrEmpty(_configuration["CoWinAPI:DateToSearch"]))
            {
                searchDate = DateTime.Parse(_configuration["CoWinAPI:DateToSearch"]).ToString("dd-MM-yyyy");
            }
            else
            {
                searchDate = DateTime.Now.ToString("dd-MM-yyyy");
            }
            vaccineType = _configuration["CoWinAPI:VaccineType"];
        }

    }
}

using CoWin.Providers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace CoWin.Core.Models
{
    public class VersionChecker
    {
        private readonly IConfiguration _configuration;
        private readonly string versionCheckingEndpoint = "https://api.github.com/repos/srvsaha/CoWINVaccineSlotFinder/releases/latest";
        public VersionChecker(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool EvaluateCurrentSoftwareVersion()
        {
            var latestVersionDto = GetLatestVersionDetails();
            
            if (latestVersionDto is null)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"\n[FATAL] SORRY! OUR SERVICE/APPLICATION IS DOWN, SO WE WON'T BE ABLE TO FIND YOUR SLOT AT THE MOMENT. THANKS FOR YOUR TIME. APPLICATION WILL EXIT NOW. PLEASE TRY AGAIN LATER!");
                Console.ResetColor();
                return false;
            }

            var serverVersion = GetVersionInfoFromServer(latestVersionDto);
            var localVersion = GetCurrentVersionFromSystem();

            if (IsUpdatedVersionAvailable(serverVersion, localVersion))
            {
                if (IsVersionUpdateMandatory(serverVersion.Major, localVersion.Major, serverVersion.Minor, localVersion.Minor))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[FATAL] Your Software Version is outdated. You MUST update the software, your current version is {localVersion}");
                    ShowLatestVersionFeatureInfo(latestVersionDto, serverVersion);
                    Console.WriteLine($"Please press Y for Downloading the Latest Version {serverVersion}, any other key to exit the app");
                    var input = Console.ReadLine();
                    if (input == "Y" || input == "y")
                    {
                        DownloadLatestVersion(latestVersionDto);
                        return false;
                    }
                    else
                    {
                        Environment.Exit(0);
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[INFO] New Version of the Software is Available, your current version is {localVersion}");
                    ShowLatestVersionFeatureInfo(latestVersionDto, serverVersion);
                    Console.WriteLine($"Please press Y for Downloading the Latest Version {serverVersion}, any other key to continue using the current version the app");
                    var input = Console.ReadLine();
                    if (input == "Y" || input == "y")
                    {
                        DownloadLatestVersion(latestVersionDto);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return true; 
        }

        private static void DownloadLatestVersion(VersionModel latestVersionDto)
        {
            string downloadUrl = latestVersionDto.Assets[0].BrowserDownloadUrl.AbsoluteUri;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                downloadUrl = downloadUrl.Replace("windows", "linux");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                downloadUrl = downloadUrl.Replace("windows", "osx");
            }

            Process.Start(new ProcessStartInfo(downloadUrl) { UseShellExecute = true });
        }

        private Version GetVersionInfoFromServer(VersionModel latestVersionDto)
        {
            string processedVersion;
            if (latestVersionDto.TagName.Contains("-"))
            {
                processedVersion = latestVersionDto.TagName[1..latestVersionDto.TagName.IndexOf("-")] + ".0";
            }
            else 
            {
                processedVersion = latestVersionDto.TagName[1..] + ".0";
            }
            var lastestVersionOnServer = new Version(processedVersion);
            return lastestVersionOnServer;
        }

        private bool IsUpdatedVersionAvailable(Version serverVersion, Version localVersion)
        {
            if (serverVersion.CompareTo(localVersion) > 0)
                return true;
            return false;
        }

        public Version GetCurrentVersionFromSystem()
        {
            var localVersion = Assembly.GetExecutingAssembly().GetName().Version;
            return localVersion;
        }
        private bool IsVersionUpdateMandatory(int serverMajorVersion, int localMajorVersion, int serverMinorVersion, int localMinorVersion)
        {
            if (serverMajorVersion != localMajorVersion)
                return true;

            if (serverMinorVersion != localMinorVersion)
                return true;

            return false;
        }

        private VersionModel GetLatestVersionDetails()
        {
            var endpoint = versionCheckingEndpoint;
            IRestResponse response = new APIFacade(_configuration).Get(endpoint, isCowinRelatedHeadersToBeUsed: false);
            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var versionDto  = JsonConvert.DeserializeObject<VersionModel>(response.Content);
                return versionDto;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[ERROR] GITHUB FETCH LATEST VERSION ResponseStatus: {response.StatusDescription}, ResponseContent: {response.Content}\n");
                Console.ResetColor();
                return null;
            }

        }

        private void ShowLatestVersionFeatureInfo(VersionModel latestVersionDto, Version serverVersion)
        {
            string applicationName = latestVersionDto.Name;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                applicationName = applicationName.Replace("windows", "linux");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                applicationName = applicationName.Replace("windows", "osx");
            }

            Console.WriteLine($"*************************************************************************************************************************************************************");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Latest Version of the Software { applicationName } is { serverVersion }, Downloaded #{latestVersionDto.Assets[0].DownloadCount} times, Released on { latestVersionDto.PublishedAt.LocalDateTime} \n\nFeatures of the Updated Version:\n{latestVersionDto.Body}");
            Console.WriteLine($"*************************************************************************************************************************************************************");
            Console.ResetColor();

        }
    }
}

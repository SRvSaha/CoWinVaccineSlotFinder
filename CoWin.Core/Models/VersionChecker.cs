using CoWin.Providers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace CoWin.Core.Models
{
    public class VersionChecker
    {
        private readonly IConfiguration _configuration;
        public VersionChecker(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool EvaluateCurrentSoftwareVersion()
        {
            var latestVersionDto = GetLatestVersionDetails();
            // Allow App to Run if there is any issue in fetching the Relases info from Github for Update Checking so that Core functionality doesn't stop
            if (latestVersionDto is null)
                return true;

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
            Process.Start(new ProcessStartInfo(latestVersionDto.Assets[0].BrowserDownloadUrl.AbsoluteUri) { UseShellExecute = true });
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
            var endpoint = _configuration["App:LatestVersion:FetchDetailsAPIEndpoint"];
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
            Console.WriteLine($"*************************************************************************************************************************************************************");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Latest Version of the Software { latestVersionDto.Name} is { serverVersion }, Downloaded #{latestVersionDto.Assets[0].DownloadCount} times, Released on { latestVersionDto.PublishedAt.LocalDateTime} \n\nFeatures of the Updated Version:\n{latestVersionDto.Body}");
            Console.WriteLine($"*************************************************************************************************************************************************************");
            Console.ResetColor();

        }
    }
}

using CoWin.Core.Exceptions;
using CoWin.Models;
using System;
using System.Diagnostics;
using System.Reflection;

namespace CoWin
{
    public class Program
    {   
        public static void Main()
        {
            try
            {
                new CovidVaccinationCenterFinder().FindSlot();
            }
            catch (ConfigurationNotInitializedException e)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[WARNING] Configuration needs to be changed with your personal details. Please read the one time instructions at https://shawt.io/r/sYt for initial setup.");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR] Details of the Issue: " + e.Message + " " + e.InnerException?.Message + " " + e.InnerException?.InnerException.Message);
                Process.Start(new ProcessStartInfo("https://shawt.io/r/sYt") { UseShellExecute = true });
            }
            Console.ResetColor();
            Console.WriteLine("\nPress Enter Key to Exit The Application .....");
            Console.ReadLine();
        }
    }

}

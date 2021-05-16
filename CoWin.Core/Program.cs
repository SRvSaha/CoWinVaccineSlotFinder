using CoWin.Core.Exceptions;
using CoWin.Models;
using System;
using System.Diagnostics;

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
            catch (ConfigurationNotInitializedException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Configuration needs to be changed with your personal details. Please read the one time instructions at https://shawt.io/r/sYt for initial setup.");            
                Process.Start(new ProcessStartInfo("https://shawt.io/r/sYt") { UseShellExecute = true });
            }
            Console.ResetColor();
            Console.WriteLine("\nPress Enter Key to Exit The Application .....");
            Console.ReadLine();
        }
    }

}

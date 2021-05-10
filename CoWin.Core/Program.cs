using CoWin.Models;
using System;

namespace CoWin
{
    public class Program
    {   
        public static void Main()
        {
            
            new CovidVaccinationCenterFinder().FindSlot();
            Console.ResetColor();
            Console.WriteLine("Press Enter to Exit");
            Console.ReadKey();
        }
    }

}

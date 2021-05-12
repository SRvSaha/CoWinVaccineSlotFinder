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
            Console.WriteLine("\nPress Any Key to Exit The Application .....");
            Console.ReadLine();
        }
    }

}

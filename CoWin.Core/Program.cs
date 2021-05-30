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
                if (!IsDisclaimerAccepted())
                    Environment.Exit(0);

                new CovidVaccinationCenterFinder().FindSlot();
            }
            catch (ConfigurationNotInitializedException e)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[WARNING] Configuration needs to be changed with your personal details. Please read the one time instructions at https://shawt.io/r/sYt for initial setup.");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[FATAL] Details of the Issue: " + e.Message + " " + e.InnerException?.Message + " " + e.InnerException?.InnerException.Message);
                Process.Start(new ProcessStartInfo("https://shawt.io/r/sYt") { UseShellExecute = true });
            }
            Console.ResetColor();
            Console.WriteLine("\nPress Enter Key to Exit The Application .....");
            Console.ReadLine();
        }
        private static bool IsDisclaimerAccepted()
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("######################################################################## DISCLAIMER ###################################################################################");
            Console.WriteLine("For the purposes of this End - User License Agreement:");
            Console.WriteLine("Agreement means this End - User License Agreement that forms the entire agreement between You and the Company regarding the use of the Application.");
            Console.WriteLine("Company (referred to as either \"the Company\", \"We\", \"Us\" or \"Our\" in this Disclaimer) refers to CoWinVaccineSlotFinder.");
            Console.WriteLine("Service refers to the Application.");
            Console.WriteLine("Application means the software program provided by the Company downloaded by You to a Device, named CoWinVaccineSlotFinder");
            Console.WriteLine("Content refers to content such as text, images, or other information that can be posted, uploaded, linked to or otherwise made available by You, regardless of the form of that content.");
            Console.WriteLine("Country refers to: India");
            Console.WriteLine("Device means any device that can access the Application such as a computer, a laptop, a cellphone or a digital tablet.");
            Console.WriteLine("Third - Party Services means any services or content(including data, information, applications and other products services) provided by a third - party that may be displayed, included or made available by the Application.");
            Console.WriteLine("You means the individual accessing or using the Application or the company, or other legal entity on behalf of which such individual is accessing or using the Application, as applicable.");
            Console.WriteLine("The Application is provided to You \"AS IS\" and \"AS AVAILABLE\" and with all faults and defects without warranty of any kind. To the maximum extent permitted under applicable law, the Company, on its own behalf and on behalf of its affiliates and its and their respective licensors and service providers, expressly disclaims all warranties, whether express, implied, statutory or otherwise, with respect to the Application, including all implied warranties of merchantability, fitness for a particular purpose, title and non-infringement, and warranties that may arise out of course of dealing, course of performance, usage or trade practice. Without limitation to the foregoing, the Company provides no warranty or undertaking, and makes no representation of any kind that the Application will meet your requirements, achieve any intended results, be compatible or work with any other software, applications, systems or services, operate without interruption, meet any performance or reliability standards or be error free or that any errors or defects can or will be corrected." +
                             "Without limiting the foregoing, neither the Company nor any of the company's provider makes any representation or warranty of any kind, express or implied: (i) as to the operation or availability of the Application, or the information, content, and materials or products included thereon; (ii) that the Application will be uninterrupted or error-free; (iii) as to the accuracy, reliability, or currency of any information or content provided through the Application; or (iv) that the Application, its servers, the content, or e-mails sent from or on behalf of the Company are free of viruses, scripts, trojan horses, worms, malware, timebombs or other harmful components." +
                             "Some jurisdictions do not allow the exclusion of certain types of warranties or limitations on applicable statutory rights of a consumer, so some or all of the above exclusions and limitations may not apply to You.But in such a case the exclusions and limitations set forth in this section shall be applied to the greatest extent enforceable under applicable law. To the extent any warranty exists under law that cannot be disclaimed, the Company shall be solely responsible for such warranty." +
                             "All information in the Service is provided \"AS IS\", with no guarantee of completeness, accuracy, timeliness or of the results obtained from the use of this information, and without warranty of any kind, express or implied, including, but not limited to warranties of performance, merchantability and fitness for a particular purpose." +
                              "We will not be liable to You or anyone else for any decision made or action taken in reliance on the information given by the Service or for any consequential, special or similar damages, even if advised of the possibility of such damages.");
            Console.WriteLine(" We don't collect any of your personally identifiable information. We collect some metadata for App Usage Statistics which doesn't include any of personal information to help us improve our Service. " +
                              "For further details, please go through the Disclaimer, End User Lisence Agreement & the Privacy Policy, proceed further only if you accept the Terms and Conditions" +
                              " available at https://shawt.io/r/sYw use the Application/Software ");
            Console.WriteLine("DUE TO RECENT CHANGES IN THE API ACCESS POLICY BY MOHFW AVAILABLE AT https://shawt.io/r/sYF THERE WON'T BE ANY FURTHER IMPROVEMENTS/UPDATES/MODIFICATIONS/RELEASES IN THE APPLICATION. " +
                "GRATEFUL TO EVERY CONTRIBUTORS, SUPPORTERS, USERS THAT WE COULD HELP YOU IN THE CRISIS. WE RECOMMEND THAT YOU DON'T MISUSE THE APP TO SAVE YOURSELF YOUR LEGAL ISSUES. " +
                "WHILE WE DON'T NOT COLLECT OR STORE YOUR PERSONAL DATA, YET IN ORDER TO DO BOOKING USING PROTECTED APIS, APIKEY WOULD BE NEEDED, WHICH CAN BE MISUSED BY USERS. ALSO, API HITS CAN'T BE QUANTIFIED SINCE APPLICATION IS USED FROM MULTIPLE PLACES. THAT BEING SAID, THE APPLICATION WILL BE AVAILABLE FOR USE, BUT NO MAINTENANCE WILL BE DONE OF THE APPLICATION. " +
                "USE IT AT YOUR OWN RISK.");
            Console.ResetColor();
            Console.WriteLine("########################################################################################################################################################################\n");
            Console.WriteLine("By pressing the Y/y key for using the Application, you are agreeing to be bound by the terms and conditions of this Agreement. If You do not agree to the terms of this Agreement, you may exit the Application.");
            Console.WriteLine("\nPress Y/y to Accept the Terms and Conditions, Any other Key to Exit the Application");
            Console.ResetColor();
            var input = Console.ReadLine();
            if (input == "Y" || input == "y")
            {
                return true;
            }
            return false;
        }
    }

}

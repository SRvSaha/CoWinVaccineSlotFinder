using System;
using System.Collections.Generic;
using System.Text;

namespace CoWin.Core.Exceptions
{
    public class InvalidVaccinationCentreException : ConfigurationNotInitializedException
    {
        public InvalidVaccinationCentreException(string message) : base(message)
        {
        }
    }
}

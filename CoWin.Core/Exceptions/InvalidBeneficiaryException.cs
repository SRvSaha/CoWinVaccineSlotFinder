using System;
using System.Collections.Generic;
using System.Text;

namespace CoWin.Core.Exceptions
{
    public class InvalidBeneficiaryException : ConfigurationNotInitializedException
    {
        public InvalidBeneficiaryException(string message) : base(message)
        {
        }
    }
}

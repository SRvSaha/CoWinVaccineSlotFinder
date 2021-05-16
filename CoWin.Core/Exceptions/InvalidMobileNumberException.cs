using System;
using System.Collections.Generic;
using System.Text;

namespace CoWin.Core.Exceptions
{
    public class InvalidMobileNumberException : ConfigurationNotInitializedException
    {
        public InvalidMobileNumberException(string message) : base(message)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace CoWin.Core.Exceptions
{
    public class InvalidPinCodeException : ConfigurationNotInitializedException
    {
        public InvalidPinCodeException(string message) : base(message)
        {
        }
    }
}

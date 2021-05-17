using System;
using System.Collections.Generic;
using System.Text;

namespace CoWin.Core.Exceptions
{
    public class InvalidSearchByPINCodeException : ConfigurationNotInitializedException
    {
        public InvalidSearchByPINCodeException(string message) : base(message)
        {
        }
    }
}

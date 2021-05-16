using System;
using System.Collections.Generic;
using System.Text;

namespace CoWin.Core.Exceptions
{
    public class InvalidDistrictException : ConfigurationNotInitializedException
    {
        public InvalidDistrictException(string message) : base(message)
        {
        }
    }
}

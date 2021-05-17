using System;
using System.Collections.Generic;
using System.Text;

namespace CoWin.Core.Exceptions
{
    public class InvalidSearchByDistrictException : ConfigurationNotInitializedException
    {
        public InvalidSearchByDistrictException(string message) : base(message)
        {
        }
    }
}

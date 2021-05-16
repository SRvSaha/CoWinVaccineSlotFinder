using System;
using System.Collections.Generic;
using System.Text;

namespace CoWin.Core.Exceptions
{
    public class ConfigurationNotInitializedException : Exception
    {
        public ConfigurationNotInitializedException(string message) : base(message)
        {

        }

        public ConfigurationNotInitializedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

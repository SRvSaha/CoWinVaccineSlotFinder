using System;
using System.Collections.Generic;
using System.Text;

namespace CoWin.Core.Validators
{
    public class MobileNumberValidator : IValidator<string>
    {
        private const int IndianValidMobileNumber = 10;
        private readonly string _defaultMobileNumber = "REPLACE_WITH_YOUR_REGISTERED_MOBILE_NO";
        public bool IsValid(string value)
        {
            return value != _defaultMobileNumber && long.TryParse(value, out _) && value.Length == IndianValidMobileNumber;
        }

    }
}

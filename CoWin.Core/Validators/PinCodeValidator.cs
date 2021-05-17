using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoWin.Core.Validators
{
    public class PINCodeValidator : IValidator<string>
    {
        private readonly string[] _defaultPINCodes = new string[] { "REPLACE_ME_WITH_YOUR_PIN_CODE_1", "REPLACE_ME_WITH_YOUR_PIN_CODE_2" };

        public bool IsValid(string value)
        {
            return !_defaultPINCodes.Contains(value) && int.TryParse(value, out _);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoWin.Core.Validators
{
    public class DistrictValidator : IValidator<string>
    {
        private readonly string[] _defaultDistrictCodes = new string[] { "REPLACE_ME_WITH_YOUR_DISTRICT_CODE_1", "REPLACE_ME_WITH_YOUR_DISTRICT_CODE_2" };
        public bool IsValid(string value)
        {
            return !_defaultDistrictCodes.Contains(value) && long.TryParse(value, out _);
        }
    }
}

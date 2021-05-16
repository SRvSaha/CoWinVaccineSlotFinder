using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoWin.Core.Validators
{
    public class BeneficiaryValidator : IValidator<string>
    {
        private const int BeneficiaryIdLength = 14;
        private readonly string[] _defaultBeneficiaries = new string[] { "REPLACE_WITH_YOUR_BENEFICIARY_ID_1", "REPLACE_WITH_YOUR_BENEFICIARY_ID_2" };
        public bool IsValid(string value)
        {
            return !_defaultBeneficiaries.Contains(value) && value.Length == BeneficiaryIdLength && long.TryParse(value, out _);
        }
    }
}

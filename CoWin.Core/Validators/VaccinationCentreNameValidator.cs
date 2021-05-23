using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoWin.Core.Validators
{
    public class VaccinationCentreNameValidator : IValidator<string>
    {
        private readonly string[] _defaultVaccinationCentreNames = new string[] { "REPLACE_ME_WITH_YOUR_VACCINATION_CENTER_NAME_1", "REPLACE_ME_WITH_YOUR_VACCINATION_CENTER_NAME_2" };
        public bool IsValid(string value)
        {
            return !_defaultVaccinationCentreNames.Contains(value);
        }
    }
}

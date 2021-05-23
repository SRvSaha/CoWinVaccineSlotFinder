using CoWin.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoWin.Core.Validators
{
    public class SearchByVaccinationCentreNameValidator : IValidator<SearchByVaccinationCentreNameModel>
    {
        private readonly IValidator<string> _vaccinationCentreNameValidator;

        public SearchByVaccinationCentreNameValidator(IValidator<string> vaccinationCentreNameValidator)
        {
            _vaccinationCentreNameValidator = vaccinationCentreNameValidator;
        }

        public bool IsValid(SearchByVaccinationCentreNameModel value)
        {
            if (value.IsSearchToBeDoneByVaccinationCentreName == false)
            {
                if (value.VaccinationCentreNames.Any(x => !_vaccinationCentreNameValidator.IsValid(x)))
                    return true;
                else return false;
            }
            else
            {
                if (value.VaccinationCentreNames.Any(x => !_vaccinationCentreNameValidator.IsValid(x)))
                    return false;
                else return true;
            }
        }
    }
}

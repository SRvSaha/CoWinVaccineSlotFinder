using CoWin.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoWin.Core.Validators
{
    public class SearchByDistrictValidator : IValidator<SearchByDistrictModel>
    {
        private readonly IValidator<string> _districtValidator;

        public SearchByDistrictValidator(IValidator<string> districtValidator)
        {
            _districtValidator = districtValidator;
        }

        public bool IsValid(SearchByDistrictModel value)
        {
            if (value.IsSearchToBeDoneByDistrict == false)
            {
                if (value.Districts.Any(x => !_districtValidator.IsValid(x)))
                    return true;
                else return false;
            }
            else
            {
                if (value.Districts.Any(x => !_districtValidator.IsValid(x)))
                    return false;
                else return true;
            }
        }
    }
}

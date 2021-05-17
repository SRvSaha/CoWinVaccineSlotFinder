using CoWin.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoWin.Core.Validators
{
    public class SearchByPINCodeValidator : IValidator<SearchByPINCodeModel>
    {
        private readonly IValidator<string> _pinCodeValidator;

        public SearchByPINCodeValidator(IValidator<string> pinCodeValidator)
        {
            _pinCodeValidator = pinCodeValidator;
        }

        public bool IsValid(SearchByPINCodeModel value)
        {
            if (value.IsSearchToBeDoneByPINCode == false)
            {
                if (value.PINCodes.Any(x => !_pinCodeValidator.IsValid(x)))
                    return true;
                else return false;
            }
            else
            {
                if (value.PINCodes.Any(x => !_pinCodeValidator.IsValid(x)))
                    return false;
                else return true;
            }
        }
    }
}

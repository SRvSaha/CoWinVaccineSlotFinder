using System;
using System.Collections.Generic;
using System.Text;

namespace CoWin.Core.Validators
{
    public class PinCodeValidator : IValidator<string>
    {
        public bool IsValid(string value) => int.TryParse(value, out _);
    }
}

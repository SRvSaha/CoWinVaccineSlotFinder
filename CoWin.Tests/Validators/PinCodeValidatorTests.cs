using Microsoft.VisualStudio.TestTools.UnitTesting;
using CoWin.Core.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoWin.Core.Validators.Tests
{
    [TestClass()]
    public class PinCodeValidatorTests
    {
        private readonly IValidator<string> _validator;
        public PinCodeValidatorTests(IValidator<string> validator)
        {
            _validator = validator;
        }
        public PinCodeValidatorTests()
        {
            _validator = new PinCodeValidator();
        }

        [TestMethod()]
        [DataRow("REPLACE_ME_PIN_CODE_1")]
        [DataRow("SomeRandomPin")]
        public void IsValid_Should_ReturnFalse_When_PinCodeIsDefaultOrNonInteger(string userEnteredPinCode)
        {            
            var isValid = _validator.IsValid(userEnteredPinCode);

            Assert.IsFalse(isValid);
        }

        [TestMethod()]
        public void IsValid_Should_ReturnTrue_When_PinCodeIsNonDefault()
        {
            var userEnteredPinCode = "400001";

            var isValid = _validator.IsValid(userEnteredPinCode);

            Assert.IsTrue(isValid);
        }
    }
}
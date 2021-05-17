using Microsoft.VisualStudio.TestTools.UnitTesting;
using CoWin.Core.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoWin.Core.Validators.Tests
{
    [TestClass()]
    public class PINCodeValidatorTests
    {
        private readonly IValidator<string> _validator;
        public PINCodeValidatorTests(IValidator<string> validator)
        {
            _validator = validator;
        }
        public PINCodeValidatorTests()
        {
            _validator = new PINCodeValidator();
        }

        [TestMethod()]
        [DataRow("REPLACE_ME_WITH_YOUR_PIN_CODE_1>")]
        [DataRow("REPLACE_ME_WITH_YOUR_PIN_CODE_2")]
        public void IsValid_Should_ReturnFalse_When_PinCodeIsDefaultOrNonInteger(string userEnteredPinCode)
        {            
            var isValid = _validator.IsValid(userEnteredPinCode);

            Assert.IsFalse(isValid);
        }

        [TestMethod()]
        [DataRow("SomeRandomPin")]
        [DataRow("Mumbai-400001")]
        [DataRow("Mumbai: 400001")]
        public void IsValid_Should_ReturnTrue_When_PinCodeIsNonDefaultAndNotInteger(string userEnteredPinCode)
        {
            var isValid = _validator.IsValid(userEnteredPinCode);

            Assert.IsFalse(isValid);
        }

        [TestMethod()]
        public void IsValid_Should_ReturnTrue_When_PinCodeIsNonDefaultAndInteger()
        {
            var userEnteredPinCode = "400001";

            var isValid = _validator.IsValid(userEnteredPinCode);

            Assert.IsTrue(isValid);
        }
    }
}
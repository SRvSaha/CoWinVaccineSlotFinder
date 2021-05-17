using CoWin.Core.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoWin.Core.Validators.Tests
{
    [TestClass()]
    public class MobileNumberTests
    {
        private readonly IValidator<string> _validator;
        public MobileNumberTests(IValidator<string> validator)
        {
            _validator = validator;
        }

        public MobileNumberTests()
        {
            _validator = new MobileNumberValidator();
        }
        [TestMethod()]
        public void IsValid_Should_ReturnFalse_When_MobileNumberIsDefault()
        {
            var userEnteredMobileNumber = "REPLACE_WITH_YOUR_REGISTERED_MOBILE_NO";

            var isValid = _validator.IsValid(userEnteredMobileNumber);

            Assert.IsFalse(isValid);
        }

        [TestMethod()]
        [DataRow("+919999999999")]
        [DataRow("999999999999")]
        [DataRow("99999999")]
        public void IsValid_Should_ReturnFalse_When_MobileNumberIsNotDefaultButNot10DigitsLong(string userEnteredMobileNumber)
        {

            var isValid = _validator.IsValid(userEnteredMobileNumber);

            Assert.IsFalse(isValid);
        }

        [TestMethod()]
        [DataRow("9999999999")]
        public void IsValid_Should_ReturnTrue_When_MobileNumberIsNotDefaultAnd10DigitsLong(string userEnteredMobileNumber)
        {

            var isValid = _validator.IsValid(userEnteredMobileNumber);

            Assert.IsTrue(isValid);
        }
    }
}

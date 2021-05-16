using Microsoft.VisualStudio.TestTools.UnitTesting;
using CoWin.Core.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoWin.Core.Validators.Tests
{
    [TestClass()]
    public class DistrictValidatorTests
    {
        private readonly IValidator<string> _validator;
        public DistrictValidatorTests(IValidator<string> validator)
        {
            _validator = validator;
        }
        public DistrictValidatorTests()
        {
            _validator = new DistrictValidator();
        }

        [TestMethod()]
        public void IsValid_Should_ReturnFalse_When_DistrictIsDefault()
        {
            var userEnteredDistrict = "<REPLACE_ME_DISTRICT_CODE_1>";

            var isValid = _validator.IsValid(userEnteredDistrict);

            Assert.IsFalse(isValid);
        }

        [TestMethod()]
        public void IsValid_Should_ReturnTrue_When_DistrictIsNonDefault()
        {
            var userEnteredDistrict = "Mumbai";

            var isValid = _validator.IsValid(userEnteredDistrict);

            Assert.IsTrue(isValid);
        }
    }
}
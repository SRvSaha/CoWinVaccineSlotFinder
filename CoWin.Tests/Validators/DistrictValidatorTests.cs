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
        [DataRow("REPLACE_ME_WITH_YOUR_DISTRICT_CODE_1")]
        [DataRow("REPLACE_ME_WITH_YOUR_DISTRICT_CODE_2")]
        public void IsValid_Should_ReturnFalse_When_DistrictIsDefault(string userEnteredDistrict)
        {
            var isValid = _validator.IsValid(userEnteredDistrict);

            Assert.IsFalse(isValid);
        }

        [TestMethod()]
        [DataRow("Mumbai")]
        [DataRow("Mumbai-395")]
        public void IsValid_Should_ReturnFalse_When_DistrictIsNonDefaultAndNotInteger(string userEnteredDistrict)
        {

            var isValid = _validator.IsValid(userEnteredDistrict);

            Assert.IsFalse(isValid);
        }

        [TestMethod()]
        [DataRow("395")]
        public void IsValid_Should_ReturnTrue_When_DistrictIsNonDefaultAndInteger(string userEnteredDistrict)
        {

            var isValid = _validator.IsValid(userEnteredDistrict);

            Assert.IsTrue(isValid);
        }
    }
}
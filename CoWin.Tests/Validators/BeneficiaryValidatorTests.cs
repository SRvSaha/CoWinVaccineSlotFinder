using CoWin.Core.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoWin.Core.Validators.Tests
{
    [TestClass()]
    public class BeneficiaryValidatorTests
    {
        private readonly IValidator<string> _validator;
        public BeneficiaryValidatorTests(IValidator<string> validator)
        {
            _validator = validator;
        }
        public BeneficiaryValidatorTests()
        {
            _validator = new BeneficiaryValidator();
        }

        [TestMethod()]
        [DataRow("REPLACE_WITH_YOUR_BENEFICIARY_ID_1")]
        [DataRow("REPLACE_WITH_YOUR_BENEFICIARY_ID_2")]
        public void IsValid_Should_ReturnFalse_When_BeneficiaryIdIsDefault(string userEnteredDistrict)
        {
            var isValid = _validator.IsValid(userEnteredDistrict);

            Assert.IsFalse(isValid);
        }

        [TestMethod()]
        [DataRow("395")]
        [DataRow("Mumbai")]
        [DataRow("njn8893jjkfnsljdn")]
        public void IsValid_Should_ReturnFalse_When_BeneficiaryIdIsNonDefaultButNotOf14Characters(string userEnteredDistrict)
        {

            var isValid = _validator.IsValid(userEnteredDistrict);

            Assert.IsFalse(isValid);
        }

        [TestMethod()]
        [DataRow("99999999999999")]
        [DataRow("12121212121212")]
        public void IsValid_Should_ReturnFalse_When_BeneficiaryIdIsNonDefaultAnd14Characters(string userEnteredDistrict)
        {

            var isValid = _validator.IsValid(userEnteredDistrict);

            Assert.IsTrue(isValid);
        }
    }
}

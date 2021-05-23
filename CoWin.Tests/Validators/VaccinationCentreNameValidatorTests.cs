using Microsoft.VisualStudio.TestTools.UnitTesting;
using CoWin.Core.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoWin.Core.Validators.Tests
{
    [TestClass()]
    public class VaccinationCentreNameValidatorTests
    {
        private readonly IValidator<string> _validator;
        public VaccinationCentreNameValidatorTests(IValidator<string> validator)
        {
            _validator = validator;
        }
        public VaccinationCentreNameValidatorTests()
        {
            _validator = new VaccinationCentreNameValidator();
        }

        [TestMethod()]
        [DataRow("REPLACE_ME_WITH_YOUR_VACCINATION_CENTER_NAME_1")]
        [DataRow("REPLACE_ME_WITH_YOUR_VACCINATION_CENTER_NAME_2")]
        public void IsValid_Should_ReturnFalse_When_VaccinationCentreNameIsDefault(string userEnteredVaccinationCentreName)
        {
            var isValid = _validator.IsValid(userEnteredVaccinationCentreName);

            Assert.IsFalse(isValid);
        }

        [TestMethod()]
        [DataRow("Mumbai CHC")]
        public void IsValid_Should_ReturnTrue_When_VaccinationCentreNameIsNon(string userEnteredVaccinationCentreName)
        {

            var isValid = _validator.IsValid(userEnteredVaccinationCentreName);

            Assert.IsTrue(isValid);
        }
    }
}
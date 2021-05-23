using CoWin.Core.Models;
using CoWin.Core.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoWin.Core.Validators.Tests
{
    [TestClass()]
    public class SearchByVaccinationCentreNameValidatorTests
    {
        private readonly IValidator<SearchByVaccinationCentreNameModel> _searchByVaccinationCentreNameValidator;
        private readonly IValidator<string> _vaccinationCentreNameValidator;
        public SearchByVaccinationCentreNameValidatorTests(IValidator<SearchByVaccinationCentreNameModel> searchByVaccinationCentreNameValidator)
        {
            _searchByVaccinationCentreNameValidator = searchByVaccinationCentreNameValidator;
        }
        public SearchByVaccinationCentreNameValidatorTests()
        {
            _vaccinationCentreNameValidator = new VaccinationCentreNameValidator();
            _searchByVaccinationCentreNameValidator = new SearchByVaccinationCentreNameValidator(_vaccinationCentreNameValidator);
        }

        [TestMethod()]
        public void IsValid_Should_ReturnTrue_When_IsSearchToBeDoneForVaccinationCentreNameIsFalseAndBothVaccinationCentreNamesAreDefault()
        {
            var userEnteredVaccinationCentreName = new List<string> { "REPLACE_ME_WITH_YOUR_VACCINATION_CENTER_NAME_1", "REPLACE_ME_WITH_YOUR_VACCINATION_CENTER_NAME_2" };
            var userEnteredSearchByVaccinationCentreNameDto = new SearchByVaccinationCentreNameModel
            {
                IsSearchToBeDoneByVaccinationCentreName = false,
                VaccinationCentreNames = userEnteredVaccinationCentreName

            };
            var isValid = _searchByVaccinationCentreNameValidator.IsValid(userEnteredSearchByVaccinationCentreNameDto);

            Assert.IsTrue(isValid);
        }

        [TestMethod()]
        public void IsValid_Should_ReturnTrue_When_IsSearchToBeDoneByVaccinationCentreNameIsFalseAndOneVaccinationCentreNameIsDefault()
        {
            var userEnteredVaccinationCentreName = new List<string> { "Maharashtra CHC", "REPLACE_ME_WITH_YOUR_VACCINATION_CENTER_NAME_2" };
            var userEnteredSearchByVaccinationCentreNameDto = new SearchByVaccinationCentreNameModel
            {
                IsSearchToBeDoneByVaccinationCentreName = false,
                VaccinationCentreNames = userEnteredVaccinationCentreName

            };
            var isValid = _searchByVaccinationCentreNameValidator.IsValid(userEnteredSearchByVaccinationCentreNameDto);

            Assert.IsTrue(isValid);
        }


        [TestMethod()]
        public void IsValid_Should_ReturnFalse_When_IsSearchToBeDoneByVaccinationCentreNameIsFalseAndBothVaccinationCentreNamesAreValid()
        {
            var userEnteredVaccinationCentreName = new List<string> { "Mumbai Raheja", "Thane" };

            var userEnteredSearchByVaccinationCentreNameDto = new SearchByVaccinationCentreNameModel
            {
                IsSearchToBeDoneByVaccinationCentreName = false,
                VaccinationCentreNames = userEnteredVaccinationCentreName

            };
            var isValid = _searchByVaccinationCentreNameValidator.IsValid(userEnteredSearchByVaccinationCentreNameDto);

            Assert.IsFalse(isValid);
        }

        [TestMethod()]
        public void IsValid_Should_ReturnFalse_When_IsSearchToBeDoneByVaccinationCentreNameIsTrueAndBothVaccinationCentreNamesAreDefault()
        {
            var userEnteredVaccinationCentreName = new List<string> { "REPLACE_ME_WITH_YOUR_VACCINATION_CENTER_NAME_1", "REPLACE_ME_WITH_YOUR_VACCINATION_CENTER_NAME_2" };
            var userEnteredSearchByVaccinationCentreNameDto = new SearchByVaccinationCentreNameModel
            {
                IsSearchToBeDoneByVaccinationCentreName = true,
                VaccinationCentreNames = userEnteredVaccinationCentreName

            };
            var isValid = _searchByVaccinationCentreNameValidator.IsValid(userEnteredSearchByVaccinationCentreNameDto);

            Assert.IsFalse(isValid);
        }

        [TestMethod()]
        public void IsValid_Should_ReturnFalse_When_IsSearchToBeDoneByVaccinationCentreNameIsTrueAndOneVaccinationCentreNameIsDefault()
        {
            var userEnteredVaccinationCentreName = new List<string> { "Udaipur CHC", "REPLACE_ME_WITH_YOUR_VACCINATION_CENTER_NAME_2" };
            var userEnteredSearchByVaccinationCentreNameDto = new SearchByVaccinationCentreNameModel
            {
                IsSearchToBeDoneByVaccinationCentreName = true,
                VaccinationCentreNames = userEnteredVaccinationCentreName

            };
            var isValid = _searchByVaccinationCentreNameValidator.IsValid(userEnteredSearchByVaccinationCentreNameDto);

            Assert.IsFalse(isValid);
        }


        [TestMethod()]
        public void IsValid_Should_ReturnTrue_When_IsSearchToBeDoneByVaccinationCentreNameIsTrueAndBothVaccinationCentreNamesAreValid()
        {
            var userEnteredVaccinationCentreName = new List<string> { "Jharakhand Hospital", "Karnataka CHC" };

            var userEnteredSearchByVaccinationCentreNameDto = new SearchByVaccinationCentreNameModel
            {
                IsSearchToBeDoneByVaccinationCentreName = true,
                VaccinationCentreNames = userEnteredVaccinationCentreName

            };
            var isValid = _searchByVaccinationCentreNameValidator.IsValid(userEnteredSearchByVaccinationCentreNameDto);

            Assert.IsTrue(isValid);
        }
    }
}

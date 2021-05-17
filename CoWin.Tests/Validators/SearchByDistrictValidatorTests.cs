using CoWin.Core.Models;
using CoWin.Core.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoWin.Core.Validators.Tests
{
    [TestClass()]
    public class SearchByDistrictValidatorTests
    {
        private readonly IValidator<SearchByDistrictModel> _searchByDistrictValidator;
        private readonly IValidator<string> _districtValidator;
        public SearchByDistrictValidatorTests(IValidator<SearchByDistrictModel> searchByDistrictValidator)
        {
            _searchByDistrictValidator = searchByDistrictValidator;
        }
        public SearchByDistrictValidatorTests()
        {
            _districtValidator = new DistrictValidator();
            _searchByDistrictValidator = new SearchByDistrictValidator(_districtValidator);
        }

        [TestMethod()]
        public void IsValid_Should_ReturnTrue_When_IsSearchToBeDoneByDistrictIsFalseAndBothDistrictsAreDefault()
        {
            var userEnteredDistrict = new List<string> { "REPLACE_ME_WITH_YOUR_DISTRICT_CODE_1", "REPLACE_ME_WITH_YOUR_DISTRICT_CODE_2" };
            var userEnteredSearchByDistrictDto = new SearchByDistrictModel
            {
                IsSearchToBeDoneByDistrict = false,
                Districts = userEnteredDistrict

            };
            var isValid = _searchByDistrictValidator.IsValid(userEnteredSearchByDistrictDto);

            Assert.IsTrue(isValid);
        }

        [TestMethod()]
        public void IsValid_Should_ReturnTrue_When_IsSearchToBeDoneByDistrictIsFalseAndOneDistrictIsDefault()
        {
            var userEnteredDistrict = new List<string> { "395", "REPLACE_ME_WITH_YOUR_DISTRICT_CODE_2" };
            var userEnteredSearchByDistrictDto = new SearchByDistrictModel
            {
                IsSearchToBeDoneByDistrict = false,
                Districts = userEnteredDistrict

            };
            var isValid = _searchByDistrictValidator.IsValid(userEnteredSearchByDistrictDto);

            Assert.IsTrue(isValid);
        }


        [TestMethod()]
        public void IsValid_Should_ReturnFalse_When_IsSearchToBeDoneByDistrictIsFalseAndBothDistrictsAreValid()
        {
            var userEnteredDistrict = new List<string> { "395", "392" };

            var userEnteredSearchByDistrictDto = new SearchByDistrictModel
            {
                IsSearchToBeDoneByDistrict = false,
                Districts = userEnteredDistrict

            };
            var isValid = _searchByDistrictValidator.IsValid(userEnteredSearchByDistrictDto);

            Assert.IsFalse(isValid);
        }

        [TestMethod()]
        public void IsValid_Should_ReturnFalse_When_IsSearchToBeDoneByDistrictIsFalseAndBothDistrictsAreDefault()
        {
            var userEnteredDistrict = new List<string> { "REPLACE_ME_WITH_YOUR_DISTRICT_CODE_1", "REPLACE_ME_WITH_YOUR_DISTRICT_CODE_2" };
            var userEnteredSearchByDistrictDto = new SearchByDistrictModel
            {
                IsSearchToBeDoneByDistrict = true,
                Districts = userEnteredDistrict

            };
            var isValid = _searchByDistrictValidator.IsValid(userEnteredSearchByDistrictDto);

            Assert.IsFalse(isValid);
        }

        [TestMethod()]
        public void IsValid_Should_ReturnFalse_When_IsSearchToBeDoneByDistrictIsFalseAndOneDistrictIsDefault()
        {
            var userEnteredDistrict = new List<string> { "395", "REPLACE_ME_WITH_YOUR_DISTRICT_CODE_2" };
            var userEnteredSearchByDistrictDto = new SearchByDistrictModel
            {
                IsSearchToBeDoneByDistrict = true,
                Districts = userEnteredDistrict

            };
            var isValid = _searchByDistrictValidator.IsValid(userEnteredSearchByDistrictDto);

            Assert.IsFalse(isValid);
        }


        [TestMethod()]
        public void IsValid_Should_ReturnTrue_When_IsSearchToBeDoneByDistrictIsFalseAndBothDistrictsAreValid()
        {
            var userEnteredDistrict = new List<string> { "395", "392" };

            var userEnteredSearchByDistrictDto = new SearchByDistrictModel
            {
                IsSearchToBeDoneByDistrict = true,
                Districts = userEnteredDistrict

            };
            var isValid = _searchByDistrictValidator.IsValid(userEnteredSearchByDistrictDto);

            Assert.IsTrue(isValid);
        }
    }
}

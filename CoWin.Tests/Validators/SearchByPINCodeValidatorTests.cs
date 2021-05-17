using CoWin.Core.Models;
using CoWin.Core.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoWin.Core.Validators.Tests
{
    [TestClass()]
    public class SearchByPINCodeValidatorTests
    {
        private readonly IValidator<SearchByPINCodeModel> _searchByPINCodeValidator;
        private readonly IValidator<string> _pinCodeValidator;
        public SearchByPINCodeValidatorTests(IValidator<SearchByPINCodeModel> searchByPINCodeValidator)
        {
            _searchByPINCodeValidator = searchByPINCodeValidator;
        }
        public SearchByPINCodeValidatorTests()
        {
            _pinCodeValidator = new PINCodeValidator();
            _searchByPINCodeValidator = new SearchByPINCodeValidator(_pinCodeValidator);
        }

        [TestMethod()]
        public void IsValid_Should_ReturnTrue_When_IsSearchToBeDoneByPINCodeIsFalseAndBothPINCodesAreDefault()
        {
            var userEnteredDistrict = new List<string> { "REPLACE_ME_WITH_YOUR_PIN_CODE_1", "REPLACE_ME_WITH_YOUR_PIN_CODE_2" };
            var userEnteredSearchByPINCodeDto = new SearchByPINCodeModel
            {
                IsSearchToBeDoneByPINCode = false,
                PINCodes = userEnteredDistrict

            };
            var isValid = _searchByPINCodeValidator.IsValid(userEnteredSearchByPINCodeDto);

            Assert.IsTrue(isValid);
        }

        [TestMethod()]
        public void IsValid_Should_ReturnTrue_When_IsSearchToBeDoneByPINCodeIsFalseAndOneDistrictIsDefault()
        {
            var userEnteredDistrict = new List<string> { "400001", "REPLACE_ME_WITH_YOUR_PIN_CODE_1" };
            var userEnteredSearchByPINCodeDto = new SearchByPINCodeModel
            {
                IsSearchToBeDoneByPINCode = false,
                PINCodes = userEnteredDistrict

            };
            var isValid = _searchByPINCodeValidator.IsValid(userEnteredSearchByPINCodeDto);

            Assert.IsTrue(isValid);
        }


        [TestMethod()]
        public void IsValid_Should_ReturnFalse_When_IsSearchToBeDoneByPINCodeIsFalseAndBothPINCodesAreValid()
        {
            var userEnteredDistrict = new List<string> { "400001", "400006" };

            var userEnteredSearchByPINCodeDto = new SearchByPINCodeModel
            {
                IsSearchToBeDoneByPINCode = false,
                PINCodes = userEnteredDistrict

            };
            var isValid = _searchByPINCodeValidator.IsValid(userEnteredSearchByPINCodeDto);

            Assert.IsFalse(isValid);
        }

        [TestMethod()]
        public void IsValid_Should_ReturnFalse_When_IsSearchToBeDoneByPINCodeIsFalseAndBothPINCodesAreDefault()
        {
            var userEnteredDistrict = new List<string> { "REPLACE_ME_WITH_YOUR_PIN_CODE_1", "REPLACE_ME_WITH_YOUR_PIN_CODE_2" };
            var userEnteredSearchByPINCodeDto = new SearchByPINCodeModel
            {
                IsSearchToBeDoneByPINCode = true,
                PINCodes = userEnteredDistrict

            };
            var isValid = _searchByPINCodeValidator.IsValid(userEnteredSearchByPINCodeDto);

            Assert.IsFalse(isValid);
        }

        [TestMethod()]
        public void IsValid_Should_ReturnFalse_When_IsSearchToBeDoneByPINCodeIsFalseAndOneDistrictIsDefault()
        {
            var userEnteredDistrict = new List<string> { "400001", "REPLACE_ME_WITH_YOUR_PIN_CODE_2" };
            var userEnteredSearchByPINCodeDto = new SearchByPINCodeModel
            {
                IsSearchToBeDoneByPINCode = true,
                PINCodes = userEnteredDistrict

            };
            var isValid = _searchByPINCodeValidator.IsValid(userEnteredSearchByPINCodeDto);

            Assert.IsFalse(isValid);
        }


        [TestMethod()]
        public void IsValid_Should_ReturnTrue_When_IsSearchToBeDoneByPINCodeIsFalseAndBothPINCodesAreValid()
        {
            var userEnteredDistrict = new List<string> { "400001", "400002" };

            var userEnteredSearchByPINCodeDto = new SearchByPINCodeModel
            {
                IsSearchToBeDoneByPINCode = true,
                PINCodes = userEnteredDistrict

            };
            var isValid = _searchByPINCodeValidator.IsValid(userEnteredSearchByPINCodeDto);

            Assert.IsTrue(isValid);
        }
    }
}

using System;
using Xunit;
using Cursed.Models.Entities.Data;
using Cursed.Models.Services;
using System.Collections.Generic;
using System.Text;

namespace Cursed.Tests.Tests.Services
{
    public class LicenseValidationTests
    {
        private readonly LicenseValidation licenseValidation;
        public LicenseValidationTests()
        {
            licenseValidation = new LicenseValidation();
        }

        [Fact]
        public void IsValidLicense_LicenseIsNotValid_ReturnFalse()
        {
            // arrange
            var invalidLicense = new License { Date = DateTime.UtcNow.AddDays(-1) };
            // act
            var compareResult = licenseValidation.IsValid(invalidLicense);
            // assert
            Assert.False(compareResult);
        }

        [Fact]
        public void IsValidLicense_LicenseIsValid_ReturnTrue()
        {
            // arrange
            var validLicense = new License { Date = DateTime.UtcNow.AddDays(1) };
            // act
            var compareResult = licenseValidation.IsValid(validLicense);
            // assert
            Assert.True(compareResult);
        }
    }
}

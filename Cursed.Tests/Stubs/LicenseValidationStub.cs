using System;
using System.Collections.Generic;
using Cursed.Models.Services;
using Cursed.Models.Entities;

namespace Cursed.Tests.Stubs
{
    /// <summary>
    /// Stub for using instead of ILicenseValidation. Returns true to any input
    /// </summary>
    public class LicenseValidationStub : ILicenseValidation
    {
        /// <returns>true</returns>
        public bool IsValid(License license)
        {
            return true;
        }
    }
}

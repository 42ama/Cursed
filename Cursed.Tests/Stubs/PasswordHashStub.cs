using System;
using System.Collections.Generic;
using Cursed.Models.Services;
using Cursed.Models.Entities;

namespace Cursed.Tests.Stubs
{
    /// <summary>
    /// Stub for using instead of ILicenseValidation. Returns true to any input
    /// </summary>
    public class PasswordHashStub : IGenPasswordHash
    {
        /// <returns>"hash"</returns>
        public string GenerateHash(string password)
        {
            return "hash";
        }

        public bool IsPasswordMathcingHash(string password, string savedPasswordHash)
        {
            if(password == "tetriandoh")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

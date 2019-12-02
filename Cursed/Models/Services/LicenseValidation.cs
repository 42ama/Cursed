using System;
using Cursed.Models.Entities;

namespace Cursed.Models.Services
{
    public class LicenseValidation : ILicenseValidation
    {
        public bool IsValid(DateTime date)
        {
            if (date > DateTime.UtcNow)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsValid(License license)
        {
            return IsValid(license.Date);
        }
    }
}

using System;
using Cursed.Models.Entities;

namespace Cursed.Models.Services
{
    public class LicenseValidation : ILicenseValidation
    {
        // validate licenses by date
        public bool IsValid(License license)
        {
            return IsDateValid(license.Date);
        }

        private bool IsDateValid(DateTime date)
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

        
    }
}

using System;
using Cursed.Models.Entities.Data;

namespace Cursed.Models.Services
{
    /// <summary>
    /// Validates License instance
    /// </summary>
    public class LicenseValidation : ILicenseValidation
    {
        /// <summary>
        /// Returns true if <c>license</c> is valid and false othewise. Validation is completed by license date
        /// </summary>
        /// <param name="license">License instance, which being validated</param>
        /// <returns>True if <c>license</c> is valid and false othewise.</returns>
        public bool IsValid(License license)
        {
            return IsDateValid(license.Date);
        }

        /// <summary>
        /// Returns true if <c>date</c> not yet come, and false otherwise.
        /// </summary>
        /// <param name="date">Date to which current date compared</param>
        /// <returns>True if <c>date</c> not yet come, and false otherwise.</returns>
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

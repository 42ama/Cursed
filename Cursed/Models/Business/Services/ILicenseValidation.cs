using Cursed.Models.Entities.Data;

namespace Cursed.Models.Services
{
    /// <summary>
    /// Provides means to do License validation
    /// </summary>
    public interface ILicenseValidation
    {
        /// <summary>
        /// Returns true if <c>license</c> is valid and false othewise.
        /// </summary>
        /// <param name="license">License instance, which being validated</param>
        /// <returns>True if <c>license</c> is valid and false othewise.</returns>
        bool IsValid(License license);
    }
}

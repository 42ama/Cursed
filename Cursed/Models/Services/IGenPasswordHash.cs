namespace Cursed.Models.Services
{
    /// <summary>
    /// Provide means to generate hash and compare existing hash to generated
    /// </summary>
    public interface IGenPasswordHash
    {
        /// <summary>
        /// Generate hash based on password string
        /// </summary>
        /// <param name="password">Hash is based on this string</param>
        /// <returns>Hash based on password string</returns>
        string GenerateHash(string password);
        /// <summary>
        /// Compare hash generated from password string versus hash string
        /// </summary>
        /// <param name="password">Hash is based on this string</param>
        /// <param name="savedPasswordHash">Saved hash to be compared</param>
        /// <returns>True if hash mathches, false otherwise</returns>
        bool IsPasswordMathcingHash(string password, string savedPasswordHash);
    }
}

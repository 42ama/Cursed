using System;
using System.Security.Cryptography;

namespace Cursed.Models.Services
{
    /// <summary>
    /// Realize <c>IGenPasswordHash</c> - provide means to generate hash and compare existing hash to generated
    /// </summary>
    public class PasswordHash : IGenPasswordHash
    {
        const int nCycles = 5000;
        /// <summary>
        /// Generate hash based on password string
        /// </summary>
        /// <param name="password">Hash is based on this string</param>
        /// <returns>Hash based on password string</returns>
        public string GenerateHash(string password)
        {

            //STEP 1 Create the salt value with a cryptographic PRNG
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            //STEP 2 Create the Rfc2898DeriveBytes and get the hash value
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, nCycles);
            byte[] hash = pbkdf2.GetBytes(20);

            //STEP 3 Combine the salt and password bytes for later use
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            //STEP 4 Turn the combined salt+hash into a string for storage
            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            //delegate STEP 5 Store it
            return savedPasswordHash;
        }

        /// <summary>
        /// Compare hash generated from password string versus hash string
        /// </summary>
        /// <param name="password">Hash is based on this string</param>
        /// <param name="savedPasswordHash">Saved hash to be compared</param>
        /// <returns>True if hash mathches, false otherwise</returns>
        public bool IsPasswordMathcingHash(string password, string savedPasswordHash)
        {
            /* Extract the bytes */
            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
            /* Get the salt */
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            /* Compute the hash on the password the user entered */
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, nCycles);
            byte[] hash = pbkdf2.GetBytes(20);
            /* Compare the results */
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    return false;
            return true;
        }
    }
}

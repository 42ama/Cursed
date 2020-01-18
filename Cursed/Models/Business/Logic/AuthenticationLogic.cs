using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Context;
using Cursed.Models.Entities.Authentication;

namespace Cursed.Models.Logic
{
    /// <summary>
    /// Authetication section logic. Consists of read action for user data.
    /// </summary>
    public class AuthenticationLogic
    {
        private readonly CursedAuthenticationContext db;

        public AuthenticationLogic(CursedAuthenticationContext db)
        {
            this.db = db;
        }

        /// <summary>
        /// Gather user data for user found by <c>login</c>.
        /// </summary>
        /// <param name="login">User is found by this login</param>
        /// <returns>Single UserData instance</returns>
        public async Task<UserData> GetUserData(string login)
        {
            return await db.UserData.FirstOrDefaultAsync(u => u.Login == login);
        }
    }
}

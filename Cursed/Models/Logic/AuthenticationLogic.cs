using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Context;
using Cursed.Models.StaticReferences.Routing;
using Cursed.Models.Services;
using Cursed.Models.Entities.Authentication;

namespace Cursed.Models.Logic
{
    public class AuthenticationLogic
    {
        private readonly CursedAuthenticationContext db;

        public AuthenticationLogic(CursedAuthenticationContext db)
        {
            this.db = db;
        }

        public async Task<UserData> GetUserData(string login)
        {
            return await db.UserData.FirstOrDefaultAsync(u => u.Login == login);
        }
    }
}

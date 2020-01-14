using Cursed.Models.Context;
using Cursed.Models.Entities.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursed.Models.Services
{
    public class LogProvider : ILogProvider<CursedAuthenticationContext>
    {
        private readonly CursedAuthenticationContext db;
        private readonly IHttpContextAccessor httpContext;

        public LogProvider(CursedAuthenticationContext db, [FromServices] IHttpContextAccessor httpContext)
        {
            this.db = db;
            this.httpContext = httpContext;
        }

        public async Task AddToLogAsync(string message)
        {
            var userLogin = httpContext.HttpContext.User.FindFirst(System.Security.Claims.ClaimsIdentity.DefaultNameClaimType).Value;
            await AddToLogAsync(message, userLogin);
        }

        public async Task AddToLogAsync(string message, string userLogin)
        {
            var userIP = httpContext.HttpContext.Connection.RemoteIpAddress.ToString();
            db.LogRecord.Add(new LogRecord
            {
                UserLogin = userLogin,
                Record = message,
                UserIP = userIP
            });
            // date recorded at instancing class
            // id recorded at writing to db
            await db.SaveChangesAsync();
        }
    }
}

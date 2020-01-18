using Cursed.Models.Context;
using Cursed.Models.Entities.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cursed.Models.Services
{
    /// <summary>
    /// Logging service, which stores logs in provided database
    /// </summary>
    /// <typeparam name="T">Specific database context class</typeparam>
    public class LogProvider : ILogProvider<CursedAuthenticationContext>
    {
        private readonly CursedAuthenticationContext db;
        private readonly IHttpContextAccessor httpContext;

        public LogProvider(CursedAuthenticationContext db, [FromServices] IHttpContextAccessor httpContext)
        {
            this.db = db;
            this.httpContext = httpContext;
        }

        /// <summary>
        /// Add record to log
        /// </summary>
        /// <param name="message">Record message</param>
        public async Task AddToLogAsync(string message)
        {
            // gather user login from context
            var userLogin = httpContext.HttpContext.User.FindFirst(System.Security.Claims.ClaimsIdentity.DefaultNameClaimType).Value;
            await AddToLogAsync(message, userLogin);
        }

        /// <summary>
        /// Add record to log, used when user name is not defined in http-context
        /// (due to async sign-in), but already known at controller.
        /// </summary>
        /// <param name="message">Record message</param>
        /// <param name="userLogin">User name</param>
        public async Task AddToLogAsync(string message, string userLogin)
        {
            // gather user ip from context
            var userIP = httpContext.HttpContext.Connection.RemoteIpAddress.ToString();
            db.LogRecord.Add(new LogRecord
            {
                UserLogin = userLogin,
                Record = message,
                UserIP = userIP
            });
            // date recorded at instancing LogRecord class
            // id recorded at writing to database

            await db.SaveChangesAsync();
        }
    }
}

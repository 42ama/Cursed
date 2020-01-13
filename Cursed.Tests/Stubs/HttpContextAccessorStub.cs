using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Cursed.Tests.Stubs
{
    public class HttpContextAccessorStub : IHttpContextAccessor
    {
        public HttpContextAccessorStub()
        {
            HttpContext = new DefaultHttpContext();
        }
        public HttpContext HttpContext { get; set; }

        public async Task SignIn(string userName, string roleName)
        {
            // create list of user claims
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, roleName.ToUpperInvariant())
            };

            // create new identity object, contains info about claims and auth scheme
            var claimsIdentity = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            // register identity object in clients cookie's
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        }

        public async Task SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}

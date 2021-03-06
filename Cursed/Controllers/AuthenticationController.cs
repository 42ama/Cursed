﻿using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Context;
using Cursed.Models.StaticReferences.Routing;
using Cursed.Models.Services;
using Cursed.Models.DataModel.Authentication;
using Cursed.Models.Logic;
using Cursed.Models.LogicValidation;
using Microsoft.AspNetCore.Authorization;

namespace Cursed.Controllers
{
    /// <summary>
    /// Used for log-in and log-out of user. Also provides AccesDenied page, which accessed
    /// in cases user doesn't have required role to access action.
    /// </summary>
    public class AuthenticationController : Controller
    {
        private readonly AuthenticationLogic logic;
        private readonly AuthenticationLogicValidation logicValidation;
        private readonly ILogProvider<CursedAuthenticationContext> logProvider;

        public AuthenticationController(
            CursedAuthenticationContext db, 
            [FromServices] IErrorHandlerFactory errorHandlerFactory, 
            [FromServices] IGenPasswordHash genPassHash,
            [FromServices] ILogProvider<CursedAuthenticationContext> logProvider)
        {
            logic = new AuthenticationLogic(db);
            logicValidation = new AuthenticationLogicValidation(db, errorHandlerFactory, genPassHash);
            this.logProvider = logProvider;
        }

        /// <summary>
        /// Display login page
        /// </summary>
        [HttpGet("login", Name = AuthenticationRouting.Login)]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Process <c>loginModel</c>, check if user exist in DataBase and authenticate him
        /// </summary>
        [HttpPost("login", Name = AuthenticationRouting.Login)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginModel);
            }

            var statusMessage = await logicValidation.CheckLogin(loginModel);
            if (statusMessage.IsCompleted)
            {
                var userData = await logic.GetUserData(loginModel.Login);
                await Authenticate(userData.Login, userData.RoleName);
                await logProvider.AddToLogAsync("User logged in.", loginModel.Login);
                return RedirectToRoute(HubRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        /// <summary>
        /// View redirected to, when user doesn't have correct role
        /// </summary>
        [HttpGet("access-denied", Name = AuthenticationRouting.AccessDenied)]
        public IActionResult AccessDenied()
        {
            return View();
        }


        /// <summary>
        /// Logout user and redirect to hub page
        /// </summary>
        [HttpGet("logout", Name = AuthenticationRouting.Logout)]
        public async Task<IActionResult> Logout()
        {
            await logProvider.AddToLogAsync("User logged out.");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToRoute(HubRouting.Index);
        }


        /// <summary>
        /// Authehticate user in system. <c>HttpContext.User</c> now will contain <c>userName</c> and <c>roleName</c>
        /// of user
        /// </summary>
        /// <param name="userName">Username to be stored in user identity</param>
        /// <param name="roleName">Role to be stored in user identity</param>
        private async Task Authenticate(string userName, string roleName)
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
    }
}
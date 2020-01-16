using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Context;
using Cursed.Models.Routing;
using Cursed.Models.Services;
using Cursed.Models.DataModel.Authentication;
using Cursed.Models.DataModel.Utility.ErrorHandling;

namespace Cursed.Models.LogicValidation
{
    public class AuthenticationLogicValidation
    {
        private readonly CursedAuthenticationContext db;
        private readonly IErrorHandlerFactory errorHandlerFactory;
        private readonly IGenPasswordHash genPassHash;

        public AuthenticationLogicValidation(CursedAuthenticationContext db, IErrorHandlerFactory errorHandlerFactory, IGenPasswordHash genPassHash)
        {
            this.db = db;
            this.errorHandlerFactory = errorHandlerFactory;
            this.genPassHash = genPassHash;
        }

        public async Task<IErrorHandler> CheckLogin(LoginModel model)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = "Login process.",
                RedirectRoute = AuthenticationRouting.Login,
                UseKeyWithRoute = false
            });

            var userAuth = await db.UserAuth.FirstOrDefaultAsync(u => u.Login == model.Login);
            if (userAuth != null)
            {
                if (!genPassHash.IsPasswordMathcingHash(model.Password, userAuth.PasswordHash))
                {
                    statusMessage.AddProblem(new Problem
                    {
                        Entity = "Password incorrect.",
                        Message = "User with this password isn't found",
                        RedirectRoute = AuthenticationRouting.Login,
                        UseKeyWithRoute = false
                    });
                }
            }
            else
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = "Username incorrect.",
                    Message = "User with this username isn't found",
                    RedirectRoute = AuthenticationRouting.Login,
                    UseKeyWithRoute = false
                });
            }

            return statusMessage;
        }
    }
}

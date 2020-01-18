using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Context;
using Cursed.Models.StaticReferences.Routing;
using Cursed.Models.Services;
using Cursed.Models.DataModel.Authentication;
using Cursed.Models.DataModel.ErrorHandling;

namespace Cursed.Models.LogicValidation
{
    /// <summary>
    /// Authentication section logic validation. Contains of methods used to validate authentication
    /// in specific situations.
    /// </summary>
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

        /// <summary>
        /// Checks if <c>model</c> is valid
        /// </summary>
        /// <param name="model">Login model</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckLogin(LoginModel model)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = "Login process.",
                RedirectRoute = AuthenticationRouting.Login,
                UseKeyWithRoute = false
            });

            // check if user exists
            var userAuth = await db.UserAuth.FirstOrDefaultAsync(u => u.Login == model.Login);
            if (userAuth != null)
            {
                // check if password correct with using of IGenPasswordHash service
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

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Cursed.Models.Context;
using Cursed.Models.DataModel.ErrorHandling;
using Cursed.Models.StaticReferences.Routing;
using Cursed.Models.Services;


namespace Cursed.Models.LogicValidation
{
    /// <summary>
    /// User managment section logic validation. Contains of methods used to validate user managment actions
    /// in specific situations.
    /// </summary>
    public class UserManagmentLogicValidation
    {
        private readonly CursedAuthenticationContext db;
        private readonly IErrorHandlerFactory errorHandlerFactory;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IGenPasswordHash genPasswordHash;
        public UserManagmentLogicValidation(CursedAuthenticationContext db, IErrorHandlerFactory errorHandlerFactory, IHttpContextAccessor contextAccessor, IGenPasswordHash genPasswordHash)
        {
            this.db = db;
            this.errorHandlerFactory = errorHandlerFactory;
            this.contextAccessor = contextAccessor;
            this.genPasswordHash = genPasswordHash;
        }

        /// <summary>
        /// Checks if user is valid, to be gathered
        /// </summary>
        /// <param name="key">Login of user to be found</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckGetSingleDataModelAsync(object key)
        {
            return await CheckUserExists(key);
        }

        /// <summary>
        /// Checks if user is valid, to be gathered for update and check if role exists
        /// </summary>
        /// <param name="key">Login of user to be found</param>
        /// <param name="roleName">Role to be checked</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckUpdateUserDataUpdateModelAsync(object key, string roleName)
        {
            var statusMessage = await CheckUserExists(key);
            return await CheckRoleExists(statusMessage, roleName);
        }

        /// <summary>
        /// Checks if user is valid, to be added and check if role exists
        /// </summary>
        /// <param name="key">Login of user to be found</param>
        /// <param name="roleName">Role to be checked</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckAddSingleDataModelAsync(object key, string roleName)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = "User.",
                EntityKey = (string)key,
                RedirectRoute = UserManagmentRouting.SingleItem
            });
            return await CheckRoleExists(statusMessage, roleName);
        }

        /// <summary>
        /// Checks if user is valid password matching <c>passwordOld</c>
        /// </summary>
        /// <param name="key">Login of user to be found</param>
        /// <param name="passwordOld">Password to be compared</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckUpdateUserAuthUpdateModelAsync(object key, string passwordOld)
        {
            var statusMessage = await CheckUserExists(key);
            if(statusMessage.IsCompleted)
            {
                // compare password using IGenPasswordHash
                var userAuth = await db.UserAuth.SingleAsync(i => i.Login == (string)key);
                if(!genPasswordHash.IsPasswordMathcingHash(passwordOld, userAuth.PasswordHash))
                {
                    statusMessage.AddProblem(new Problem
                    {
                        Entity = "User.",
                        EntityKey = statusMessage.ProblemStatus.EntityKey,
                        Message = "Old password incorrect.",
                        RedirectRoute = UserManagmentRouting.Index,
                        UseKeyWithRoute = false
                    });
                }
            }
            
            return statusMessage;
        }

        /// <summary>
        /// Checks if user is valid, to be gathered for update
        /// </summary>
        /// <param name="key">Login of user to be found</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckGetSingleUpdateModelAsync(object key)
        {
            return await CheckUserExists(key);
        }

        /// <summary>
        /// Checks if user is valid, to be removed
        /// </summary>
        /// <param name="key">Login of user to be found</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckRemoveDataModelAsync(object key)
        {
            var statusMessage = await CheckUserExists(key);
            
            // cannot remove current user, that action must be done by another admin
            return CheckIfUserCurrentUser(statusMessage, key);
        }

        /// <summary>
        /// Checks if user exists
        /// </summary>
        /// <param name="key">Login of user to be found</param>
        /// <returns>Status message with validaton information</returns>
        private async Task<IErrorHandler> CheckUserExists(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = "User.",
                EntityKey = (string)key,
                RedirectRoute = UserManagmentRouting.SingleItem
            });

            // check if user data exists
            if (await db.UserData.FirstOrDefaultAsync(i => i.Login == (string)key) == null)
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = "User (data).",
                    EntityKey = (string)key,
                    Message = "User (data) with this is not found. Probably user just doesn't exist.",
                    RedirectRoute = UserManagmentRouting.Index,
                    UseKeyWithRoute = false
                });
            }

            // check if user auth exists
            if (await db.UserAuth.FirstOrDefaultAsync(i => i.Login == (string)key) == null)
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = "User (auth).",
                    EntityKey = (string)key,
                    Message = "User (auth) with this is not found. Probably somewhere error occured, check DB for reference.",
                    RedirectRoute = UserManagmentRouting.Index,
                    UseKeyWithRoute = false
                });
            }

            return statusMessage;
        }

        /// <summary>
        /// Checks if user found by <c>key</c> is current user
        /// </summary>
        /// <param name="statusMessage">Status message to which problem will be added</param>
        /// <param name="key">Login of user to be found</param>
        /// <returns>Status message with validaton information</returns>
        private IErrorHandler CheckIfUserCurrentUser(IErrorHandler statusMessage, object key)
        {
            if(contextAccessor.HttpContext.User.FindFirst(System.Security.Claims.ClaimsIdentity.DefaultNameClaimType)?.Value == (string)key)
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = "User.",
                    EntityKey = (string)key,
                    Message = "You cannot do this type of actions to your account while still been log in into it.",
                    RedirectRoute = UserManagmentRouting.Index,
                    UseKeyWithRoute = false
                });
            }
            return statusMessage;
        }

        /// <summary>
        /// Checks if role exists
        /// </summary>
        /// <param name="statusMessage">Status message to which problem will be added</param>
        /// <param name="key">Role name to be found</param>
        /// <returns>Status message with validaton information</returns>
        private async Task<IErrorHandler> CheckRoleExists(IErrorHandler statusMessage, object key)
        {
            string roleName = (string)key;

            // check if role exists
            var dbRole = await db.Role.FirstOrDefaultAsync(i => i.Name == roleName);
            if (dbRole == null)
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = "User.",
                    EntityKey = statusMessage.ProblemStatus.EntityKey,
                    Message = $"Such role: {roleName}, don't exist in DataBase.",
                    RedirectRoute = UserManagmentRouting.Index,
                    UseKeyWithRoute = false
                });
            }
            return statusMessage;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Cursed.Models.Context;
using Cursed.Models.DataModel.Companies;
using Cursed.Models.Entities;
using Cursed.Models.Interfaces.LogicCRUD;
using Cursed.Models.DataModel.Utility;
using Cursed.Models.DataModel.Utility.ErrorHandling;
using Cursed.Models.StaticReferences.Routing;
using Cursed.Models.Services;


namespace Cursed.Models.LogicValidation
{
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
        
        public async Task<IErrorHandler> CheckGetSingleDataModelAsync(object key)
        {
            return await CheckUserExists(key);
        }

        public async Task<IErrorHandler> CheckUpdateUserDataUpdateModelAsync(object key, string roleName)
        {
            var statusMessage = await CheckUserExists(key);
            return await CheckRoleExists(statusMessage, roleName);
        }

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

        public async Task<IErrorHandler> CheckUpdateUserAuthUpdateModelAsync(object key, string passwordOld)
        {
            var statusMessage = await CheckUserExists(key);
            if(statusMessage.IsCompleted)
            {
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

        public async Task<IErrorHandler> CheckGetSingleUpdateModelAsync(object key)
        {
            return await CheckUserExists(key);
        }

        public async Task<IErrorHandler> CheckRemoveDataModelAsync(object key)
        {
            var statusMessage = await CheckUserExists(key);
            
            return CheckIfUserCurrentUser(statusMessage, key);
        }
        private async Task<IErrorHandler> CheckUserExists(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = "User.",
                EntityKey = (string)key,
                RedirectRoute = UserManagmentRouting.SingleItem
            });

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

        private async Task<IErrorHandler> CheckRoleExists(IErrorHandler statusMessage, object key)
        {
            string roleName = (string)key;
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

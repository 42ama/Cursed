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
using Cursed.Models.Data.Companies;
using Cursed.Models.Entities;
using Cursed.Models.Interfaces.LogicCRUD;
using Cursed.Models.Data.Utility;
using Cursed.Models.Data.Utility.ErrorHandling;
using Cursed.Models.Routing;
using Cursed.Models.Services;


namespace Cursed.Models.LogicValidation
{
    public class UserManagmentLogicValidation
    {
        private readonly CursedAuthenticationContext db;
        private readonly IErrorHandlerFactory errorHandlerFactory;

        public UserManagmentLogicValidation(CursedAuthenticationContext db, IErrorHandlerFactory errorHandlerFactory)
        {
            this.db = db;
            this.errorHandlerFactory = errorHandlerFactory;
        }
        // add checks for user auth
        public async Task<IErrorHandler> CheckGetSingleDataModelAsync(object key)
        {
            return await CheckUserExists(key);
        }

        public async Task<IErrorHandler> CheckGetSingleUserDataUpdateModelAsync(object key)
        {
            return await CheckUserExists(key);
        }

        public async Task<IErrorHandler> CheckGetSingleUserAuthUpdateModelAsync(object key)
        {
            return await CheckUserExists(key);
        }

        public async Task<IErrorHandler> CheckUpdateDataModelAsync(object key)
        {
            return await CheckUserExists(key);
        }

        public async Task<IErrorHandler> CheckRemoveDataModelAsync(object key)
        {
            return await CheckUserExists(key);
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
                    Message = "User (auth) with this is not found. Probably somewhere error occused, check DB for reference.",
                    RedirectRoute = UserManagmentRouting.Index,
                    UseKeyWithRoute = false
                });
            }

            return statusMessage;
        }
    }
}

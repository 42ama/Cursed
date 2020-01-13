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
using Cursed.Models.Entities.Authentication;
using Cursed.Models.Interfaces.LogicCRUD;
using Cursed.Models.Data.Utility;
using Cursed.Models.Data.Utility.ErrorHandling;
using Cursed.Models.Data.Authentication;
using Cursed.Models.Services;

namespace Cursed.Models.Logic
{
    public class UserManagmentLogic : IReadColection<UserData>, IReadSingle<UserData>, IReadUpdateForm<UserData>, ICreate<RegistrationModel>, IUpdate<UserData>, IUpdate<UserAuthUpdateModel>, IDeleteByKey
    {
        private readonly CursedAuthenticationContext db;
        private readonly IGenPasswordHash genPasswordHash;
        public UserManagmentLogic(CursedAuthenticationContext db, IGenPasswordHash genPasswordHash)
        {
            this.db = db;
            this.genPasswordHash = genPasswordHash;
        }


        public async Task<IEnumerable<UserData>> GetAllDataModelAsync()
        {
            return db.UserData;
        }

        public async Task<UserData> GetSingleDataModelAsync(object key)
        {
            return await db.UserData.SingleAsync(i => i.Login == (string)key);
        }

        public async Task<UserData> GetSingleUpdateModelAsync(object key)
        {
            return await db.UserData.SingleAsync(i => i.Login == (string)key);
        }

        public async Task AddDataModelAsync(RegistrationModel model)
        {
            var userData = new UserData
            {
                Login = model.Login,
                RoleName = model.RoleName
            };
            var userAuth = new UserAuth
            {
                Login = model.Login,
                PasswordHash = genPasswordHash.GenerateHash(model.Password)
            };

            db.Add(userData);
            db.Add(userAuth);
            await db.SaveChangesAsync();
        }

        public async Task UpdateDataModelAsync(UserData model)
        {
            var currentModel = await db.UserData.FirstOrDefaultAsync(i => i.Login == model.Login);
            db.Entry(currentModel).CurrentValues.SetValues(model);
            await db.SaveChangesAsync();
        }

        public async Task UpdateDataModelAsync(UserAuthUpdateModel model)
        {
            var currentModel = await db.UserAuth.FirstOrDefaultAsync(i => i.Login == model.Login);
            var userAuth = new UserAuth
            {
                Login = model.Login,
                PasswordHash = genPasswordHash.GenerateHash(model.PasswordNew)
            };
            db.Entry(currentModel).CurrentValues.SetValues(userAuth);
            await db.SaveChangesAsync();
        }

        public async Task RemoveDataModelAsync(object key)
        {
            var userData = await db.UserData.FirstOrDefaultAsync(i => i.Login == (string)key);
            var userAuth = await db.UserAuth.FirstOrDefaultAsync(i => i.Login == (string)key);
            db.UserData.Remove(userData);
            db.UserAuth.Remove(userAuth);
            await db.SaveChangesAsync();
        }
    }
}

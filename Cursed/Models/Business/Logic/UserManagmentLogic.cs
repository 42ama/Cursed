using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Context;
using Cursed.Models.Entities.Authentication;
using Cursed.Models.Interfaces.LogicCRUD;
using Cursed.Models.DataModel.Authentication;
using Cursed.Models.Services;

namespace Cursed.Models.Logic
{
    /// <summary>
    /// User managment section logic. Consists of CRUD actions for users, including gathering methods for
    /// both single user and collection of all users.
    /// </summary>
    public class UserManagmentLogic : IReadColection<UserData>, IReadSingle<UserData>, IReadUpdateForm<UserData>, IUpdate<UserData>, IUpdate<UserAuthUpdateModel>, IDeleteByKey
    {
        private readonly CursedAuthenticationContext db;
        private readonly IGenPasswordHash genPasswordHash;
        public UserManagmentLogic(CursedAuthenticationContext db, IGenPasswordHash genPasswordHash)
        {
            this.db = db;
            this.genPasswordHash = genPasswordHash;
        }

        /// <summary>
        /// Gather all users data from database.
        /// </summary>
        /// <returns>All users data from database</returns>
        public async Task<IEnumerable<UserData>> GetAllDataModelAsync()
        {
            return db.UserData;
        }

        /// <summary>
        /// Gather single user data, which found by <c>key</c>.
        /// </summary>
        /// <param name="key">Login of user data to be found</param>
        /// <returns>Single user data, which found by <c>key</c></returns>
        public async Task<UserData> GetSingleDataModelAsync(object key)
        {
            return await db.UserData.SingleAsync(i => i.Login == (string)key);
        }

        /// <summary>
        /// Gather single user data, which found by <c>key</c>.
        /// </summary>
        /// <param name="key">Login of user data to be found</param>
        /// <returns>Single user data, which found by <c>key</c></returns>
        public async Task<UserData> GetSingleUpdateModelAsync(object key)
        {
            return await db.UserData.SingleAsync(i => i.Login == (string)key);
        }

        /// <summary>
        /// Add new user.
        /// </summary>
        /// <param name="model">User to be added</param>
        /// <returns>Added user data with correct key(Id) value</returns>
        public async Task<UserData> AddDataModelAsync(RegistrationModel model)
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

            var entity = db.Add(userData);
            db.Add(userAuth);
            await db.SaveChangesAsync();
            return entity.Entity;
        }

        /// <summary>
        /// Update user data.
        /// </summary>
        /// <param name="model">Updated user data information</param>
        public async Task UpdateDataModelAsync(UserData model)
        {
            var currentModel = await db.UserData.FirstOrDefaultAsync(i => i.Login == model.Login);
            db.Entry(currentModel).CurrentValues.SetValues(model);
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Update user security data.
        /// </summary>
        /// <param name="model">Updated user security data information</param>
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

        /// <summary>
        /// Delete user.
        /// </summary>
        /// <param name="key">Login of user to be deleted</param>
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

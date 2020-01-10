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

namespace Cursed.Models.Logic
{
    public class UserManagmentLogic : IReadColection<UserData>, IReadSingle<UserData>, IReadUpdateForm<UserData>, ICreate<RegistrationModel>, IUpdate<UserData>, IUpdate<UserAuthUpdateModel>, IDeleteByKey
    {
        private readonly CursedAuthenticationContext db;
        public UserManagmentLogic(CursedAuthenticationContext db)
        {
            this.db = db;
        }


        public async Task<IEnumerable<UserData>> GetAllDataModelAsync()
        {
            return new List<UserData>();
        }

        public async Task<UserData> GetSingleDataModelAsync(object key)
        {
            return new UserData();
        }

        public async Task<UserData> GetSingleUpdateModelAsync(object key)
        {
            return new UserData();
        }

        public async Task AddDataModelAsync(RegistrationModel model)
        {

        }

        public async Task UpdateDataModelAsync(UserData model)
        {

        }

        public async Task UpdateDataModelAsync(UserAuthUpdateModel model)
        {

        }

        public async Task RemoveDataModelAsync(object key)
        {

        }
    }
}

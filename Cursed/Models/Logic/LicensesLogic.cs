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
using Cursed.Models.Data.Licenses;
using Cursed.Models.Entities;
using Cursed.Models.Data.Shared;
using Cursed.Models.Interfaces.LogicCRUD;

namespace Cursed.Models.Logic
{
    public class LicensesLogic : IReadColection<LicensesDataModel>, IReadSingle<LicensesDataModel>, IReadUpdateForm<License>, ICUD<License>
    {
        private readonly CursedContext db;
        private readonly IQueryable<LicensesDataModel> basicDataModelQuery;
        public LicensesLogic(CursedContext db)
        {
            this.db = db;

            // probably will remain static between calls, and not will be updated after db changes. Dig into
            // got an answer, that linq querys are delayed to each call, so it will process normaly
            basicDataModelQuery = db.License.Join
                (
                    db.ProductCatalog,
                    lt => lt.ProductId,
                    rt => rt.Id,
                    (x, y) => new LicensesDataModel
                    {
                        Id = x.Id,
                        GovermentNum = x.GovermentNum,
                        Date = x.Date,
                        ProductId = x.ProductId,
                        ProductName = y.Name,
                        ProductCAS = y.Cas
                    }
                );
        }

        public async Task<IEnumerable<LicensesDataModel>> GetAllDataModelAsync()
        {
            return basicDataModelQuery;
        }

        public async Task<LicensesDataModel> GetSingleDataModelAsync(object key)
        {
            return basicDataModelQuery.Single(i => i.Id == (int)key);
        }

        public async Task<License> GetSingleUpdateModelAsync(object key)
        {
            return await db.License.SingleAsync(i => i.Id == (int)key);
        }

        public async Task AddDataModelAsync(License model)
        {
            model.Id = default;
            db.Add(model);
            await db.SaveChangesAsync();
        }

        public async Task UpdateDataModelAsync(License model)
        {
            var currentModel = await db.License.FirstOrDefaultAsync(i => i.Id == model.Id);
            db.Entry(currentModel).CurrentValues.SetValues(model);
            await db.SaveChangesAsync();
        }

        public async Task RemoveDataModelAsync(object key)
        {
            var entity = await db.License.FindAsync((int)key);

            db.License.Remove(entity);

            await db.SaveChangesAsync();
        }
    }
}

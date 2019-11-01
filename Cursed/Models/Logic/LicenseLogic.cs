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
using Cursed.Models.Data.Utility;

namespace Cursed.Models.Logic
{
    public class LicenseLogic : IRESTAsync<LicensesDM, LicensesDM, License>
    {
        private readonly CursedContext db;
        private readonly IQueryable<LicensesDM> basicDataModelQuery;
        public LicenseLogic(CursedContext db)
        {
            this.db = db;

            // probably will remain static between calls, and not will be updated after db changes. Dig into
            // got an answer, that linq querys are delayed to each call, so it will process normaly
            basicDataModelQuery = db.License.Join
                (
                    db.ProductCatalog,
                    lt => lt.ProductId,
                    rt => rt.Id,
                    (x, y) => new LicensesDM
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

        public async Task<IEnumerable<LicensesDM>> GetAllDataModelAsync()
        {
            return basicDataModelQuery;
        }

        public async Task<LicensesDM> GetSingleDataModelAsync(object UId)
        {
            return basicDataModelQuery.Single(i => i.Id == (int)UId);
        }

        public async Task<License> GetSingleUpdateModelAsync(object UId)
        {
            return await db.License.SingleAsync(i => i.Id == (int)UId);
        }

        public async Task AddDataModelAsync(License dataModel)
        {
            dataModel.Id = default;
            db.Add(dataModel);
            await db.SaveChangesAsync();
        }

        public async Task UpdateDataModelAsync(License updatedDataModel)
        {
            var currentModel = await db.License.FirstOrDefaultAsync(i => i.Id == updatedDataModel.Id);
            currentModel = updatedDataModel;
            await db.SaveChangesAsync();
        }

        public async Task RemoveDataModelAsync(License dataModel)
        {
            var entity = await db.ProductCatalog.FindAsync(dataModel);

            db.ProductCatalog.Remove(entity);

            await db.SaveChangesAsync();
        }
    }
}

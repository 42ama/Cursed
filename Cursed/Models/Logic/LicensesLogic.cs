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
using Cursed.Models.Data.Utility.ErrorHandling;

namespace Cursed.Models.Logic
{
    public class LicensesLogic : IReadColection<LicensesDataModel>, IReadSingle<LicensesDataModel>, IReadUpdateForm<License>, ICUD<License>
    {
        private readonly CursedContext db;
        private readonly IQueryable<LicensesDataModel> basicDataModelQuery;
        private readonly AbstractErrorHandlerFactory errorHandlerFactory;
        public LicensesLogic(CursedContext db)
        {
            this.db = db;
            errorHandlerFactory = new StatusMessageFactory();
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

        public async Task<AbstractErrorHandler<IEnumerable<LicensesDataModel>>> GetAllDataModelAsync()
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler<IEnumerable<LicensesDataModel>>("All licenses.");

            statusMessage.ReturnValue = basicDataModelQuery;

            return statusMessage;
        }

        public async Task<AbstractErrorHandler<LicensesDataModel>> GetSingleDataModelAsync(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler<LicensesDataModel>("License.", key);

            statusMessage.ReturnValue = basicDataModelQuery.Single(i => i.Id == (int)key);

            return statusMessage;
        }

        public async Task<AbstractErrorHandler<License>> GetSingleUpdateModelAsync(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler<License>("License.", key);

            statusMessage.ReturnValue = await db.License.SingleAsync(i => i.Id == (int)key);

            return statusMessage;
        }

        public async Task<AbstractErrorHandler> AddDataModelAsync(License model)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler("License.");

            model.Id = default;
            db.Add(model);
            await db.SaveChangesAsync();

            return statusMessage;
        }

        public async Task<AbstractErrorHandler> UpdateDataModelAsync(License model)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler("License.", model.Id);

            var currentModel = await db.License.FirstOrDefaultAsync(i => i.Id == model.Id);
            db.Entry(currentModel).CurrentValues.SetValues(model);
            await db.SaveChangesAsync();

            return statusMessage;
        }

        public async Task<AbstractErrorHandler> RemoveDataModelAsync(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler<License>("License.", key);

            var entity = await db.License.FindAsync((int)key);
            db.License.Remove(entity);
            await db.SaveChangesAsync();

            return statusMessage;
        }
    }
}

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
using Cursed.Models.Data.Storages;
using Cursed.Models.Entities;
using Cursed.Models.Data.Shared;
using Cursed.Models.Data.Utility;
using Cursed.Models.Interfaces.LogicCRUD;
using Cursed.Models.Data.Utility.ErrorHandling;

namespace Cursed.Models.Logic
{
    public class StoragesLogic : IReadColection<StoragesModel>, IReadSingle<StorageModel>, IReadUpdateForm<Storage>, ICUD<Storage>
    {
        private readonly CursedContext db;
        private readonly AbstractErrorHandlerFactory errorHandlerFactory;
        public StoragesLogic(CursedContext db)
        {
            this.db = db;
            errorHandlerFactory = new StatusMessageFactory();
        }

        public async Task<AbstractErrorHandler<IEnumerable<StoragesModel>>> GetAllDataModelAsync()
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler<IEnumerable<StoragesModel>>("Storages.");

            var storages = await db.Storage.ToListAsync();
            var query = from s in storages
                        join p in (from s in storages
                                   join p in db.Product on s.Id equals p.StorageId into tble
                                   group tble by s.Id) on s.Id equals p.Key into tble
                        from t in tble.DefaultIfEmpty()
                        join c in db.Company on s.CompanyId equals c.Id into companies
                        from comp in companies.DefaultIfEmpty()
                        select new StoragesModel
                        {
                            Id = s.Id,
                            Name = s.Name,
                            Latitude = s.Latitude,
                            Longitude = s.Longitude,
                            Company = comp != null ? new TitleIdContainer { Id = comp.Id, Title = comp.Name } : null,
                            ProductsCount = t.Single().Count()
                        };

            statusMessage.ReturnValue = query;

            return statusMessage;
        }

        public async Task<AbstractErrorHandler<StorageModel>> GetSingleDataModelAsync(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler<StorageModel>("Storage.", key);

            var storages = await db.Storage.ToListAsync();
            var query = from s in storages
                        where s.Id == (int)key
                        join p in (from s in storages
                                    join p in db.Product on s.Id equals p.StorageId into products
                                    from prod in products
                                    join pc in db.ProductCatalog on prod.Uid equals pc.Id into productsCat
                                    from prodCat in productsCat
                                    group new ProductContainer
                                    {
                                        Id = prod.Id,
                                        Uid = prodCat.Id,
                                        Name = prodCat.Name,
                                        Price = prod.Price,
                                        Quantity = prod.Quantity,
                                        QuantityUnit = prod.QuantityUnit
                                    } by s.Id) on s.Id equals p.Key into tble
                        from t in tble.DefaultIfEmpty()
                        join c in db.Company on s.CompanyId equals c.Id into companies
                        from comp in companies.DefaultIfEmpty()
                        select new StorageModel
                        {
                            Id = s.Id,
                            Name = s.Name,
                            Latitude = s.Latitude,
                            Longitude = s.Longitude,
                            Company = comp != null ? new TitleIdContainer { Id = comp.Id, Title = comp.Name } : null,
                            Products = t.ToList()
                        };

            statusMessage.ReturnValue = query.Single();

            return statusMessage;
        }
    

        public async Task<AbstractErrorHandler<Storage>> GetSingleUpdateModelAsync(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler<Storage>("Storage.", key);

            statusMessage.ReturnValue = await db.Storage.SingleOrDefaultAsync(i => i.Id == (int)key);

            return statusMessage;
        }

        public async Task<AbstractErrorHandler> AddDataModelAsync(Storage model)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler("Storage.");

            model.Id = default;
            db.Add(model);
            await db.SaveChangesAsync();

            return statusMessage;
        }

        public async Task<AbstractErrorHandler> UpdateDataModelAsync(Storage model)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler("Storage.", model.Id);

            var currentModel = await db.Storage.FirstOrDefaultAsync(i => i.Id == model.Id);
            db.Entry(currentModel).CurrentValues.SetValues(model);
            await db.SaveChangesAsync();

            return statusMessage;
        }

        public async Task<AbstractErrorHandler> RemoveDataModelAsync(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler("Storage.", key);

            var entity = await db.Storage.FindAsync((int)key);
            db.Storage.Remove(entity);
            await db.SaveChangesAsync();

            return statusMessage;
        }
    }
}

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
        private readonly CursedDataContext db;
        public StoragesLogic(CursedDataContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<StoragesModel>> GetAllDataModelAsync()
        {
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

            return query;
        }

        public async Task<StorageModel> GetSingleDataModelAsync(object key)
        {
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

            return query.Single();
        }
    

        public async Task<Storage> GetSingleUpdateModelAsync(object key)
        {
            return await db.Storage.SingleOrDefaultAsync(i => i.Id == (int)key);
        }

        public async Task AddDataModelAsync(Storage model)
        {
            model.Id = default;
            db.Add(model);
            await db.SaveChangesAsync();
        }

        public async Task UpdateDataModelAsync(Storage model)
        {
            var currentModel = await db.Storage.FirstOrDefaultAsync(i => i.Id == model.Id);
            db.Entry(currentModel).CurrentValues.SetValues(model);
            await db.SaveChangesAsync();
        }

        public async Task RemoveDataModelAsync(object key)
        {
            var entity = await db.Storage.FindAsync((int)key);
            db.Storage.Remove(entity);
            await db.SaveChangesAsync();
        }
    }
}

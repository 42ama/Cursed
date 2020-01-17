using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Context;
using Cursed.Models.DataModel.Storages;
using Cursed.Models.Entities.Data;
using Cursed.Models.Interfaces.LogicCRUD;
using Cursed.Models.DataModel.Products;

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
                            Company = comp != null ? new ValueTuple<string, int> { Item2 = comp.Id, Item1 = comp.Name } : new ValueTuple<string, int>(),
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
                            Company = comp != null ? new ValueTuple<string, int> { Item2 = comp.Id, Item1 = comp.Name } : new ValueTuple<string, int>(),
                            Products = t.ToList()
                        };

            return query.Single();
        }
    

        public async Task<Storage> GetSingleUpdateModelAsync(object key)
        {
            return await db.Storage.SingleOrDefaultAsync(i => i.Id == (int)key);
        }

        public async Task<Storage> AddDataModelAsync(Storage model)
        {
            model.Id = default;
            var entity = db.Add(model);
            await db.SaveChangesAsync();
            return entity.Entity;
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

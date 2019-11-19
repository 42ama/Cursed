﻿using System;
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

namespace Cursed.Models.Logic
{
    public class StoragesLogic : IRESTAsync<StoragesModel, StorageModel, Storage>
    {
        private readonly CursedContext db;
        public StoragesLogic(CursedContext db)
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

        public async Task AddDataModelAsync(Storage dataModel)
        {
            dataModel.Id = default;
            db.Add(dataModel);
            await db.SaveChangesAsync();
        }

        public async Task UpdateDataModelAsync(Storage updatedDataModel)
        {
            var currentModel = await db.Storage.FirstOrDefaultAsync(i => i.Id == updatedDataModel.Id);
            db.Entry(currentModel).CurrentValues.SetValues(updatedDataModel);
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
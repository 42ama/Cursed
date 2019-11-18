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
using Cursed.Models.Data.ProductsCatalog;
using Cursed.Models.Entities;
using Cursed.Models.Data.Shared;
using Cursed.Models.Data.Utility;

namespace Cursed.Models.Logic
{
    public class ProductsCatalogLogic : IRESTAsync<ProductsCatalogModel, ProductCatalogModel, ProductCatalog>
    {
        private readonly CursedContext db;
        public ProductsCatalogLogic(CursedContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<ProductsCatalogModel>> GetAllDataModelAsync()
        {
            // probably need testing with different db connection combination
            var productCatalogs = await db.ProductCatalog.ToListAsync();

            var dataModels = from pc in productCatalogs
                             join l in db.License on pc.Id equals l.ProductId into groupC
                             from c in groupC.DefaultIfEmpty()
                             join gA in (from pc in productCatalogs
                                         join p in db.Product on pc.Id equals p.Uid into t
                                         group t by pc.Id).Select(x => new { Id = x.Key, Count = x.Single().Count() })
                                         on pc.Id equals gA.Id into groupD
                             from d in groupD.DefaultIfEmpty()
                             join gB in (from pc in productCatalogs
                                         join r in db.RecipeProductChanges on pc.Id equals r.ProductId into t
                                         group t by pc.Id).Select(x => new { Id = x.Key, Count = x.Single().Count() })
                                         on pc.Id equals gB.Id into groupE
                             from e in groupE.DefaultIfEmpty()
                             select new ProductsCatalogModel
                             {
                                 ProductId = pc.Id,
                                 Name = pc.Name,
                                 CAS = pc.Cas,
                                 Type = pc.Type,
                                 LicenseRequired = pc.LicenseRequired ?? false,
                                 License = c?.Id != null
                                 ? new LicenseValid( new License
                                 {
                                     Id = c.Id,
                                     GovermentNum = c.GovermentNum,
                                     Date = c.Date
                                 })
                                 : null,
                                 StoragesCount = d.Count,
                                 RecipesCount = e.Count
                             };

            return dataModels;
        }

        public async Task<ProductCatalogModel> GetSingleDataModelAsync(object key)
        {
            int Uid = (int)key;

            var productCatalog = await db.ProductCatalog.SingleOrDefaultAsync(i => i.Id == Uid);
            var licenses = await db.License.Where(i => i.ProductId == Uid).ToListAsync();
            var recipes = await db.RecipeProductChanges
                .Where(i => i.ProductId == Uid)
                .Select(x => new TitleIdContainer { Id = x.RecipeId, Title = x.Recipe.Content.Substring(0,45) + "..." }).ToListAsync();
            var storages = await db.Product
                .Where(i => i.Uid == Uid)
                .Select(x => new TitleIdContainer { Id = x.StorageId, Title = x.Storage.Name }).ToListAsync();
            bool licenseRequired = productCatalog.LicenseRequired ?? false;
            List<LicenseValid> validLicenses = new List<LicenseValid>();
            foreach (var license in licenses)
            {
                if (licenseRequired)
                {
                    validLicenses.Add(new LicenseValid(license));
                }
                else
                {
                    validLicenses.Add(new LicenseValid(license, true));
                }
            }

            var dataModel = new ProductCatalogModel
            {
                ProductId = Uid,
                CAS = productCatalog.Cas,
                Name = productCatalog.Name,
                Type = productCatalog.Type,
                LicenseRequired = licenseRequired,
                Licenses = validLicenses,
                Recipes = recipes,
                Storages = storages
            };
            return dataModel;
        }

        //Add to interface
        public async Task<ProductCatalog> GetSingleUpdateModelAsync(object key)
        {
            var productCatalog = await db.ProductCatalog.SingleOrDefaultAsync(i => i.Id == (int)key);
            return productCatalog;
        }

        public async Task AddDataModelAsync(ProductCatalog dataModel)
        {
            dataModel.Id = default;
            db.Add(dataModel);
            await db.SaveChangesAsync();
        }

        public async Task UpdateDataModelAsync(ProductCatalog updatedDataModel)
        {
            var currentModel = await db.ProductCatalog.FirstOrDefaultAsync(i => i.Id == updatedDataModel.Id);
            db.Entry(currentModel).CurrentValues.SetValues(updatedDataModel);
            await db.SaveChangesAsync();
        }

        public async Task RemoveDataModelAsync(object key)
        {
            var entity = await db.ProductCatalog.FindAsync((int)key);

            //catch execption if related entites are exist and display error message
            db.ProductCatalog.Remove(entity);

            await db.SaveChangesAsync();
        }
    }
}
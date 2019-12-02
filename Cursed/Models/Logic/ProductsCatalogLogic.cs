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
using Cursed.Models.Data.ProductsCatalog;
using Cursed.Models.Entities;
using Cursed.Models.Data.Shared;
using Cursed.Models.Data.Utility;
using Cursed.Models.Services;
using Cursed.Models.Interfaces.LogicCRUD;

namespace Cursed.Models.Logic
{
    public class ProductsCatalogLogic : IReadColection<ProductsCatalogModel>, IReadSingle<ProductCatalogModel>, IReadUpdateForm<ProductCatalog>, ICUD<ProductCatalog>
    {
        private readonly CursedContext db;
        private readonly ILicenseValidation licenseValidation;
        public ProductsCatalogLogic(CursedContext db, ILicenseValidation licenseValidation)
        {
            this.db = db;
            this.licenseValidation = licenseValidation;
        }

        public async Task<IEnumerable<ProductsCatalogModel>> GetAllDataModelAsync()
        {
            // probably need testing with different db connection combination
            var productCatalogs = await db.ProductCatalog.ToListAsync();

            var dataModels = (from pc in productCatalogs
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
                                 StoragesCount = d.Count,
                                 RecipesCount = e.Count
                             }).ToList();

            // setup validation state to each of products, using ILicenseValidation
            foreach (var item in dataModels)
            {
                if(item.LicenseRequired == true)
                {
                    item.IsValid = false;
                    foreach (var license in db.License.Where(i => i.ProductId == item.ProductId))
                    {
                        if(licenseValidation.IsValid(license))
                        {
                            item.IsValid = true;
                            break;
                        }
                    }
                }
                else
                {
                    item.IsValid = true;
                }
            }

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
            var validLicenses = new List<(License license, bool isValid)>();
            foreach (var license in licenses)
            {
                if (!licenseRequired)
                {
                    validLicenses.Add((license, true));
                }
                else
                {
                    validLicenses.Add((license, licenseValidation.IsValid(license)));
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

        public async Task AddDataModelAsync(ProductCatalog model)
        {
            model.Id = default;
            db.Add(model);
            await db.SaveChangesAsync();
        }

        public async Task UpdateDataModelAsync(ProductCatalog model)
        {
            var currentModel = await db.ProductCatalog.FirstOrDefaultAsync(i => i.Id == model.Id);
            db.Entry(currentModel).CurrentValues.SetValues(model);
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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Context;
using Cursed.Models.DataModel.ProductsCatalog;
using Cursed.Models.Entities.Data;
using Cursed.Models.Services;
using Cursed.Models.Interfaces.LogicCRUD;

namespace Cursed.Models.Logic
{
    /// <summary>
    /// Products catalog section logic. Consists of CRUD actions for products in catalog, including gathering methods for
    /// both single product and collection of all products.
    /// </summary>
    public class ProductsCatalogLogic : IReadColection<ProductsCatalogModel>, IReadSingle<ProductCatalogModel>, IReadUpdateForm<ProductCatalog>, ICUD<ProductCatalog>
    {
        private readonly CursedDataContext db;
        private readonly ILicenseValidation licenseValidation;
        public ProductsCatalogLogic(CursedDataContext db, ILicenseValidation licenseValidation)
        {
            this.db = db;
            this.licenseValidation = licenseValidation;
        }

        /// <summary>
        /// Gather all products from catalog from database.
        /// </summary>
        /// <returns>All products from catalog from database. Each product contains more information than ProductCatalog entity.</returns>
        public async Task<IEnumerable<ProductsCatalogModel>> GetAllDataModelAsync()
        {
            // gather products catalog, to have unblocking call when grouping
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
                                 LicenseRequired = pc.LicenseRequired ?? false,
                                 StoragesCount = d.Count,
                                 RecipesCount = e.Count
                             }).ToList();

            // for each product connect license from licenses list and 
            // based on validation set IsValid property
            foreach (var item in dataModels)
            {
                if(item.LicenseRequired == true)
                {
                    // set default to false
                    item.IsValid = false;
                    foreach (var license in db.License.Where(i => i.ProductId == item.ProductId))
                    {
                        // if valid license for current proudct if found, set IsValid to true
                        if (licenseValidation.IsValid(license))
                        {
                            item.IsValid = true;
                            break;
                        }
                    }
                }
                else
                {
                    // if license isn't required, consider product valid
                    item.IsValid = true;
                }
            }

            return dataModels;
        }

        /// <summary>
        /// Gather single product from catalog, which found by <c>key</c>.
        /// </summary>
        /// <param name="key">Id of product from catalog to be found</param>
        /// <returns>Single product from catalog, which found by <c>key</c>. Contains more information than ProductCatalog entity.</returns>
        public async Task<ProductCatalogModel> GetSingleDataModelAsync(object key)
        {
            int Uid = (int)key;

            // gather products catalog, to have unblocking call when grouping
            var productCatalogs = await db.ProductCatalog.ToListAsync();
            // gather licenses, to have validation information on each license
            var licenses = await db.License.Where(i => i.ProductId == Uid).ToListAsync();

            // gather basic data required for display product
            var dataModel = (from pc in productCatalogs
                         where pc.Id == Uid
                         join gA in (from rpc in db.RecipeProductChanges
                                     where rpc.ProductId == Uid
                                     join r in db.Recipe on rpc.RecipeId equals r.Id
                                     select Tuple.Create(Uid, rpc.RecipeId, r.Content)
                                    ) on pc.Id equals gA.Item1 into groupA
                         from A in groupA.DefaultIfEmpty()
                         join gB in (from p in db.Product
                                     where p.Uid == Uid
                                     join s in db.Storage on p.StorageId equals s.Id
                                     select Tuple.Create(Uid, p.StorageId, s.Name)
                                    ) on pc.Id equals gB.Item1 into groupB
                         from B in groupB.DefaultIfEmpty()
                         select new ProductCatalogModel
                         {
                             ProductId = Uid,
                             CAS = pc.Cas,
                             Name = pc.Name,
                             LicenseRequired = pc.LicenseRequired ?? false,
                             Recipes = groupA?.Select(x => new ValueTuple<string, int> { Item2 = x.Item2, Item1 = x.Item3 }).ToList() ?? new List<ValueTuple<string, int>>(),
                             Storages = groupB?.Select(x => new ValueTuple<string, int> { Item2 = x.Item2, Item1 = x.Item3 }).ToList() ?? new List<ValueTuple<string, int>>()
                         }).First();
            //used first instead of single, cause query results contains several
            //identical entities by number of 1 + Recipes.Count + Storages.Count

            // for each product connect license from licenses list and 
            // based on validation set IsValid property
            var validLicenses = new List<(License license, bool isValid)>();
            foreach (var license in licenses)
            {
                if (!dataModel.LicenseRequired)
                {
                    validLicenses.Add((license, true));
                }
                else
                {
                    validLicenses.Add((license, licenseValidation.IsValid(license)));
                }
            }

            // shorten each recipe content to acceptble length
            for (int i = 0; i < dataModel.Recipes.Count; i++)
            {
                if (dataModel.Recipes[i].Item1.Length > 45)
                {
                    dataModel.Recipes[i] = new ValueTuple<string, int>(dataModel.Recipes[i].Item1.Substring(0, 45) + "...", dataModel.Recipes[i].Item2);
                }
            }

            dataModel.Licenses = validLicenses;

            return dataModel;
        }

        /// <summary>
        /// Gather single product from catalog, which found by <c>key</c>.
        /// </summary>
        /// <param name="key">Id of product from catalog to be found</param>
        /// <returns>Single product from catalog, which found by <c>key</c>.</returns>
        public async Task<ProductCatalog> GetSingleUpdateModelAsync(object key)
        {
            return await db.ProductCatalog.SingleOrDefaultAsync(i => i.Id == (int)key);
        }

        /// <summary>
        /// Add new product to catalog.
        /// </summary>
        /// <param name="model">Product to be added to catalog</param>
        /// <returns>Added product from catalog with correct key(Id) value</returns>
        public async Task<ProductCatalog> AddDataModelAsync(ProductCatalog model)
        {
            model.Id = default;
            var entity = db.Add(model);
            await db.SaveChangesAsync();
            return entity.Entity;
        }

        /// <summary>
        /// Update product from catalog.
        /// </summary>
        /// <param name="model">Updated product from catalog information</param>
        public async Task UpdateDataModelAsync(ProductCatalog model)
        {
            var currentModel = await db.ProductCatalog.FirstOrDefaultAsync(i => i.Id == model.Id);
            db.Entry(currentModel).CurrentValues.SetValues(model);
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Delete product from catalgo.
        /// </summary>
        /// <param name="key">Id of product from catalog to be deleted</param>
        public async Task RemoveDataModelAsync(object key)
        {
            var entity = await db.ProductCatalog.FindAsync((int)key);
            db.ProductCatalog.Remove(entity);
            await db.SaveChangesAsync();
        }
    }
}

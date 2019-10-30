using System.Globalization;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Cursed.Models.Context;
using Cursed.Models.Data;
using Cursed.Models.Entities;

namespace Cursed.Models.Logic
{
    public class ProductCatalogLogic : IRESTAsync<ProductCatalogDataModel, ProductCatalog>
    {
        private readonly CursedContext db;
        public ProductCatalogLogic(CursedContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<ProductCatalogDataModel>> GetDataModelsAsync()
        {
            // probably need testing with different db connection combination
            var productCatalogs = await db.ProductCatalog.ToListAsync();
            var licenses = await db.License.ToListAsync();



            // Count version does not work due EF implementation of Group
            /*var recipeCountCollection = db.ProductCatalog
                .GroupJoin(
                db.RecipeProductChanges,
                lt => lt.Id,
                rt => rt.ProductId,
                (lt, os) => new
                {
                    Id = lt.Id,
                    RecipeCount = os.Select(i => i.RecipeId).Distinct().Count()
                });

            var recipeCountCollection = (from pc in productCatalogs
                                         join rpc in db.RecipeProductChanges on pc.Id equals rpc.ProductId into t
                                         group t by pc.Id).Select(i => new
                                         {
                                             Id = i.Key,
                                             RecipeCount = i.Select(x => x.Select(y => y.RecipeId).Distinct()).Count()
                                         });

            var storageCountCollection = db.Product
                .GroupBy(x => x.Uid)
                .Select(g => new
                {
                    Id = g.Key,
                    StorageCount = 1//g.Select(x => x.StorageId).Distinct().Count()
                });

            var dataModels = from pc in productCatalogs
                             join l in licenses on pc.Id equals l.ProductId into aGroup
                             from a in aGroup.DefaultIfEmpty()
                             join rcc in recipeCountCollection on pc.Id equals rcc.Id into bGroup
                             from b in bGroup.DefaultIfEmpty()
                             join scc in storageCountCollection on pc.Id equals scc.Id into cGroup
                             from c in cGroup.DefaultIfEmpty()
                             select new ProductCatalogDataModel
                             {
                                 ProductId = pc.Id,
                                 Name = pc.Name,
                                 CAS = pc.Cas,
                                 Type = pc.Type,
                                 LicenseRequired = pc?.LicenseRequired,
                                 GovermentNum = a?.GovermentNum,
                                 LicensedUntil = a?.Date
                             };*/
            var dataModels = from pc in productCatalogs
                             join l in licenses on pc.Id equals l.ProductId into aGroup
                             from a in aGroup.DefaultIfEmpty()
                             select new ProductCatalogDataModel
                             {
                                 ProductId = pc.Id,
                                 Name = pc.Name,
                                 CAS = pc.Cas,
                                 Type = pc.Type,
                                 LicenseRequired = pc?.LicenseRequired,
                                 GovermentNum = a?.GovermentNum,
                                 LicensedUntil = a?.Date
                             }; 
            // listed version does not work for unknown reason
            foreach (var item in dataModels)
            {
                item.Recipes = (from rpc in db.RecipeProductChanges
                                where rpc.ProductId == item.ProductId
                                select rpc.RecipeId).ToList();
                item.Storages = (from p in db.Product
                                 where p.Uid == item.ProductId
                                select p.StorageId).ToList();
            }

            return dataModels;
        }

        public async Task<ProductCatalogDataModel> GetDataModelAsync(object UId)
        {
            int Uid = (int)UId;
            var productCatalog = await db.ProductCatalog.SingleOrDefaultAsync(i => i.Id == Uid);
            var license = await db.License.Where(i => i.ProductId == Uid).OrderByDescending(i => i.Date).FirstOrDefaultAsync();
            var recipes = await db.RecipeProductChanges.Where(i => i.ProductId == Uid).Select(x => x.RecipeId).ToListAsync();
            var storages = await db.Product.Where(i => i.Uid == Uid).Select(x => x.StorageId).ToListAsync();
            var dataModel = new ProductCatalogDataModel
            {
                ProductId = Uid,
                CAS = productCatalog.Cas,
                Name = productCatalog.Name,
                Type = productCatalog.Type,
                LicenseRequired = productCatalog.LicenseRequired,
                Recipes = recipes,
                Storages = storages
            };
            if(license != null)
            {
                dataModel.GovermentNum = license.GovermentNum;
                dataModel.LicensedUntil = license.Date;
            }
            return dataModel;
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
            currentModel = updatedDataModel;
            await db.SaveChangesAsync();
        }

        public async Task RemoveDataModelAsync(ProductCatalog dataModel)
        {
            var entity = await db.ProductCatalog.FindAsync(dataModel);

            //catch execption if related entites are exist and display error message
            db.ProductCatalog.Remove(entity);

            await db.SaveChangesAsync();
        }
    }
}

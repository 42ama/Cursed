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
            var productCatalogs = await db.ProductCatalog.ToListAsync();
            var licenses = await db.License.ToListAsync();
            var dataModels =
                from p in productCatalogs
                join l in licenses on p.Id equals l.ProductId into pl
                from _l in pl.DefaultIfEmpty()
                select new ProductCatalogDataModel { ProductId = p.Id, CAS = p.Cas, Name = p.Name, Type = p.Type, LicenseRequired = p.LicenseRequired, GovermentNum = _l?.GovermentNum, LicensedUntil = _l?.Date };
            return dataModels;
        }

        public async Task<ProductCatalogDataModel> GetDataModelAsync(object UId)
        {
            var productCatalog = await db.ProductCatalog.SingleOrDefaultAsync(i => i.Id == (int)UId);
            var license = await db.License.Where(i => i.ProductId == (int)UId).OrderByDescending(i => i.Date).FirstOrDefaultAsync();
            var dataModel = new ProductCatalogDataModel { ProductId = productCatalog.Id, CAS = productCatalog.Cas, Name = productCatalog.Name, Type = productCatalog.Type, LicenseRequired = productCatalog.LicenseRequired };
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

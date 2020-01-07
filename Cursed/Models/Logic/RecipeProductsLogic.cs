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
using Cursed.Models.Data.RecipeProducts;
using Cursed.Models.Entities;
using Cursed.Models.Interfaces.LogicCRUD;
using Cursed.Models.Data.Shared;
using Cursed.Models.Data.Utility;
using Cursed.Models.Data.Utility.ErrorHandling;

namespace Cursed.Models.Logic
{
    public class RecipeProductsLogic : IReadCollectionByParam<RecipeProductsDataModel>, ICreate<RecipeProductChanges>, IUpdate<RecipeProductChanges>, IDeleteByModel<RecipeProductChanges>
    {
        private readonly CursedDataContext db;

        public RecipeProductsLogic(CursedDataContext db)
        {
            this.db = db;

        }

        public async Task<IEnumerable<RecipeProductsDataModel>> GetAllDataModelAsync(object key)
        {
            int recipeId = (int)key;
            var rpcList = await db.RecipeProductChanges.ToListAsync();
            var query = from rpcOut in rpcList
                        where rpcOut.RecipeId == recipeId
                        join pc in db.ProductCatalog on rpcOut.ProductId equals pc.Id into products
                        from product in products
                        select  new RecipeProductsDataModel
                        {
                            RecipeId = recipeId,
                            ProductId = rpcOut.ProductId,
                            Type = rpcOut.Type,
                            Quantity = rpcOut.Quantity,
                            ProductName = product.Name,
                            Cas = product.Cas,
                            LicenseRequired = product.LicenseRequired                            
                        };

            return query;
        }

        // doesn't fit into interfaces
        public IEnumerable<ProductCatalog> GetProductsFromCatalog(IEnumerable<int> productIdsIgnore)
        {
            var query = from pc in db.ProductCatalog where !productIdsIgnore.Contains(pc.Id) select pc;
            return query;
        }

        public async Task AddDataModelAsync(RecipeProductChanges model)
        {
            db.Add(model);
            await db.SaveChangesAsync();
        }

        public async Task UpdateDataModelAsync(RecipeProductChanges model)
        {
            var currentModel = await db.RecipeProductChanges.FirstOrDefaultAsync(i => i.RecipeId == model.RecipeId && i.ProductId == model.ProductId);
            db.Entry(currentModel).CurrentValues.SetValues(model);
            await db.SaveChangesAsync();
        }

        public async Task RemoveDataModelAsync(RecipeProductChanges model)
        {
            var entity = await db.RecipeProductChanges.SingleAsync(i => i.RecipeId == model.RecipeId && i.ProductId == model.ProductId);
            db.RecipeProductChanges.Remove(entity);
            await db.SaveChangesAsync();
        }
    }
}

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
using Cursed.Models.Data.Shared;
using Cursed.Models.Data.Utility;

namespace Cursed.Models.Logic
{
    public class RecipeProductsLogic
    {
        private readonly CursedContext db;
        public RecipeProductsLogic(CursedContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<RecipeProductsDataModel>> GetAllDataModelAsync(int recipeId)
        {
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
            return query.ToArray();
        }

        public async Task<IEnumerable<ProductCatalog>> GetProductsFromCatalog(IEnumerable<int> productIdsIgnore)
        {
            var query = from pc in db.ProductCatalog where !productIdsIgnore.Contains(pc.Id) select pc;
            return query;
        }

        public async Task AddToRecipeProduct(RecipeProductChanges recipeProduct)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Context;
using Cursed.Models.DataModel.RecipeProducts;
using Cursed.Models.Entities.Data;
using Cursed.Models.Interfaces.LogicCRUD;

namespace Cursed.Models.Logic
{
    /// <summary>
    /// Recipes products section logic. Consists of CRUD actions for product recipe relations.
    /// </summary>
    public class RecipeProductsLogic : IReadCollectionByParam<RecipeProductsDataModel>, ICreate<RecipeProductChanges>, IUpdate<RecipeProductChanges>, IDeleteByModel<RecipeProductChanges>
    {
        private readonly CursedDataContext db;

        public RecipeProductsLogic(CursedDataContext db)
        {
            this.db = db;

        }

        /// <summary>
        /// Gather all product recipe relations for specific recipe from database.
        /// </summary>
        /// <param name="key">Id of recipe to which product recipe relations belongs</param>
        /// <returns>All product recipe relations from database. Each relation contains more information than RecipeProductChanges entity.</returns>
        public async Task<IEnumerable<RecipeProductsDataModel>> GetAllDataModelAsync(object key)
        {
            int recipeId = (int)key;
            // gather relations, to have unblocking call when grouping
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

        /// <summary>
        /// Gather all products from catalog from database, without ignored
        /// </summary>
        /// <param name="productIdsIgnore">Products from catalog to be ignored</param>
        /// <returns>All products from catalog from database, without ignored</returns>
        public IEnumerable<ProductCatalog> GetProductsFromCatalog(IEnumerable<int> productIdsIgnore)
        {
            var query = from pc in db.ProductCatalog where !productIdsIgnore.Contains(pc.Id) select pc;
            return query;
        }

        /// <summary>
        /// Add new product recipe relation.
        /// </summary>
        /// <param name="model">Product recipe relation to be added</param>
        /// <returns>Added product recipe relation with correct key(ProductId, RecipeId) value</returns>
        public async Task<RecipeProductChanges> AddDataModelAsync(RecipeProductChanges model)
        {
            var entity = db.Add(model);
            await db.SaveChangesAsync();
            return entity.Entity;
        }

        /// <summary>
        /// Update product recipe relation.
        /// </summary>
        /// <param name="model">Updated product recipe relation information</param>
        public async Task UpdateDataModelAsync(RecipeProductChanges model)
        {
            var currentModel = await db.RecipeProductChanges.FirstOrDefaultAsync(i => i.RecipeId == model.RecipeId && i.ProductId == model.ProductId);
            db.Entry(currentModel).CurrentValues.SetValues(model);
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Delete product recipe relation.
        /// </summary>
        /// <param name="model">Model of recipe product relation containing key information (ProductId and RecipeId) to find recipe product changes</param>
        public async Task RemoveDataModelAsync(RecipeProductChanges model)
        {
            var entity = await db.RecipeProductChanges.SingleAsync(i => i.RecipeId == model.RecipeId && i.ProductId == model.ProductId && i.Type == model.Type);
            db.RecipeProductChanges.Remove(entity);
            await db.SaveChangesAsync();
        }
    }
}

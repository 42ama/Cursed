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
        private readonly CursedContext db;
        private readonly AbstractErrorHandlerFactory errorHandlerFactory;
        public RecipeProductsLogic(CursedContext db)
        {
            this.db = db;
            errorHandlerFactory = new StatusMessageFactory();
        }

        public async Task<AbstractErrorHandler<IEnumerable<RecipeProductsDataModel>>> GetAllDataModelAsync(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler<IEnumerable<RecipeProductsDataModel>>("Recipe products.", key);

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

            statusMessage.ReturnValue = query;

            return statusMessage;
        }

        // doesn't fit into interfaces
        public IEnumerable<ProductCatalog> GetProductsFromCatalog(IEnumerable<int> productIdsIgnore)
        {
            var query = from pc in db.ProductCatalog where !productIdsIgnore.Contains(pc.Id) select pc;
            return query;
        }

        public async Task<AbstractErrorHandler> AddDataModelAsync(RecipeProductChanges model)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler("Recipe products.", (recipeId: model.RecipeId, productId: model.ProductId));

            db.Add(model);
            await db.SaveChangesAsync();

            return statusMessage;
        }

        public async Task<AbstractErrorHandler> UpdateDataModelAsync(RecipeProductChanges model)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler("Recipe products.", (recipeId: model.RecipeId, productId: model.ProductId));

            var currentModel = await db.RecipeProductChanges.FirstOrDefaultAsync(i => i.RecipeId == model.RecipeId && i.ProductId == model.ProductId);
            db.Entry(currentModel).CurrentValues.SetValues(model);
            await db.SaveChangesAsync();

            return statusMessage;
        }

        public async Task<AbstractErrorHandler> RemoveDataModelAsync(RecipeProductChanges model)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler("Recipe products.", (recipeId: model.RecipeId, productId: model.ProductId));

            var entity = await db.RecipeProductChanges.SingleAsync(i => i.RecipeId == model.RecipeId && i.ProductId == model.ProductId);
            db.RecipeProductChanges.Remove(entity);
            await db.SaveChangesAsync();

            return statusMessage;
        }
    }
}

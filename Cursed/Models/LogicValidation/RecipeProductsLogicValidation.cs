using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Context;
using Cursed.Models.DataModel.ErrorHandling;
using Cursed.Models.StaticReferences.Routing;
using Cursed.Models.Services;

namespace Cursed.Models.LogicValidation
{
    public class RecipeProductsLogicValidation
    {
        private readonly CursedDataContext db;
        private readonly IErrorHandlerFactory errorHandlerFactory;

        public RecipeProductsLogicValidation(CursedDataContext db, IErrorHandlerFactory errorHandlerFactory)
        {
            this.db = db;
            this.errorHandlerFactory = errorHandlerFactory;
        }

        public async Task<IErrorHandler> CheckUpdateDataModelAsync(object key)
        {
            return await CheckExists(key);
        }

        public async Task<IErrorHandler> CheckRemoveDataModelAsync(object key)
        {
            return await CheckExists(key);
        }
        private async Task<IErrorHandler> CheckExists(object key)
        {
            // recipeId and productId
            var tupleKey = (ValueTuple<int, int>)key;
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = "Recipe products changes.",
                EntityKey = tupleKey.Item1.ToString(),
                RedirectRoute = RecipeProductsRouting.Index
            });
            
            if (await db.RecipeProductChanges.FirstOrDefaultAsync(i => i.RecipeId == tupleKey.Item1 &&
            i.ProductId == tupleKey.Item2) == null)
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = $"Recipe products changes. Proudct Id: {tupleKey.Item2}.",
                    EntityKey = tupleKey.Item1.ToString(),
                    Message = "Product Changes process with this Id's is not found.",
                    RedirectRoute = RecipeProductsRouting.Index
                });
            }

            return statusMessage;
        }
    }
}

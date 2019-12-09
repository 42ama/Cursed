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
using Cursed.Models.Data.Companies;
using Cursed.Models.Entities;
using Cursed.Models.Interfaces.LogicCRUD;
using Cursed.Models.Data.Utility;
using Cursed.Models.Data.Utility.ErrorHandling;

namespace Cursed.Models.LogicValidation
{
    public class RecipeProductsLogicValidation
    {
        private readonly CursedContext db;
        private readonly AbstractErrorHandlerFactory errorHandlerFactory;

        public RecipeProductsLogicValidation(CursedContext db)
        {
            this.db = db;
            errorHandlerFactory = new StatusMessageFactory();
        }

        public async Task<AbstractErrorHandler> CheckUpdateDataModelAsync(object key)
        {
            return await CheckExists(key);
        }

        public async Task<AbstractErrorHandler> CheckRemoveDataModelAsync(object key)
        {
            var statusMessage = await CheckExists(key);

            if (!statusMessage.IsCompleted)
            {
                return statusMessage;
            }

            // check related entities
            var recipes = db.Recipe.Where(i => i.Id == ((ValueTuple<int, int>)key).Item1);
            var productsCatalog = db.ProductCatalog.Where(i => i.Id == ((ValueTuple<int, int>)key).Item2);
            

            if (productsCatalog.Any())
            {
                foreach (var productCatalog in productsCatalog)
                {
                    statusMessage.Problems.Add(new Problem
                    {
                        Entity = "Product in catalog.",
                        EntityKey = productCatalog.Id,
                        Message = "Recipe products changes have related product in catalog."
                    });
                }
            }

            if (recipes.Any())
            {
                foreach (var recipe in recipes)
                {
                    statusMessage.Problems.Add(new Problem
                    {
                        Entity = "Recipe.",
                        EntityKey = recipe.Id,
                        Message = "Recipe products changes have related recipe."
                    });
                }
            }

            return statusMessage;
        }
        private async Task<AbstractErrorHandler> CheckExists(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler("Tech process.", key);

            if (await db.RecipeProductChanges.FirstOrDefaultAsync(i => i.RecipeId == ((ValueTuple<int, int>)key).Item1 &&
            i.ProductId == ((ValueTuple<int, int>)key).Item2) == null)
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = "Recipe products changes.",
                    EntityKey = key,
                    Message = "No recipe products changes with such key's found."
                });
            }

            return statusMessage;
        }
    }
}

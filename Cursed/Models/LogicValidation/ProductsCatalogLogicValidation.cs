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
    public class ProductsCatalogLogicValidation
    {
        private readonly CursedContext db;
        private readonly AbstractErrorHandlerFactory errorHandlerFactory;

        public ProductsCatalogLogicValidation(CursedContext db)
        {
            this.db = db;
            errorHandlerFactory = new StatusMessageFactory();
        }

        public async Task<AbstractErrorHandler> CheckGetSingleDataModelAsync(object key)
        {
            return await CheckExists(key);
        }

        public async Task<AbstractErrorHandler> CheckGetSingleUpdateModelAsync(object key)
        {
            return await CheckExists(key);
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
            var recipeProductsChanges = db.RecipeProductChanges.Where(i => i.ProductId == (int)key);
            var products = db.Product.Where(i => i.Uid == (int)key);
            var licenses = db.License.Where(i => i.ProductId == (int)key);

            if (recipeProductsChanges.Any())
            {
                foreach (var recipeProductChanges in recipeProductsChanges)
                {
                    statusMessage.Problems.Add(new Problem
                    {
                        Entity = "Recipe product changes.",
                        EntityKey = (productId: recipeProductChanges.ProductId, recipeId: recipeProductChanges.RecipeId),
                        Message = "Product in catalog have related recipe product changes."
                    });
                }
            }
            if (products.Any())
            {
                foreach (var product in products)
                {
                    statusMessage.Problems.Add(new Problem
                    {
                        Entity = "Product.",
                        EntityKey = product.Id,
                        Message = "Product in catalog have related product in storage."
                    });
                }
            }
            if (licenses.Any())
            {
                foreach (var license in licenses)
                {
                    statusMessage.Problems.Add(new Problem
                    {
                        Entity = "License.",
                        EntityKey = license.Id,
                        Message = "Product in catalog have related license."
                    });
                }
            }

            return statusMessage;
        }
        private async Task<AbstractErrorHandler> CheckExists(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler("Products in catalog.", key);

            if (await db.ProductCatalog.FirstOrDefaultAsync(i => i.Id == (int)key) == null)
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = "Products in catalog.",
                    EntityKey = key,
                    Message = "No products in catalog with such key found."
                });
            }

            return statusMessage;
        }
    }
}

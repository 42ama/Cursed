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
using Cursed.Models.Routing;
using Cursed.Models.Services;

namespace Cursed.Models.LogicValidation
{
    public class ProductsCatalogLogicValidation
    {
        private readonly CursedDataContext db;
        private readonly IErrorHandlerFactory errorHandlerFactory;

        public ProductsCatalogLogicValidation(CursedDataContext db, IErrorHandlerFactory errorHandlerFactory)
        {
            this.db = db;
            this.errorHandlerFactory = errorHandlerFactory;
        }

        public async Task<IErrorHandler> CheckGetSingleDataModelAsync(object key)
        {
            return await CheckExists(key);
        }

        public async Task<IErrorHandler> CheckGetSingleUpdateModelAsync(object key)
        {
            return await CheckExists(key);
        }

        public async Task<IErrorHandler> CheckUpdateDataModelAsync(object key)
        {
            return await CheckExists(key);
        }

        public async Task<IErrorHandler> CheckRemoveDataModelAsync(object key)
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
                        Entity = $"Product changes. Recipe Id: {recipeProductChanges.RecipeId}.",
                        EntityKey = recipeProductChanges.RecipeId.ToString(),
                        Message = "You must remove dependent Product Changes in Recipe first.",
                        RedirectRoute = RecipeProductsRouting.Index
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
                        EntityKey = product.Id.ToString(),
                        Message = "You must remove dependent Product in Storage first.",
                        RedirectRoute = ProductsRouting.SingleItem
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
                        EntityKey = license.Id.ToString(),
                        Message = "You must remove dependent License first.",
                        RedirectRoute = LicensesRouting.SingleItem
                    });
                }
            }

            return statusMessage;
        }
        private async Task<IErrorHandler> CheckExists(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = "Products in catalog.",
                EntityKey = ((int)key).ToString(),
                RedirectRoute = ProductsCatalogRouting.SingleItem
            });

            if (await db.ProductCatalog.FirstOrDefaultAsync(i => i.Id == (int)key) == null)
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = "Products in catalog.",
                    EntityKey = ((int)key).ToString(),
                    Message = "Products in catalog with this Id is not found.",
                    RedirectRoute = ProductsCatalogRouting.Index,
                    UseKeyWithRoute = false
                });
            }

            return statusMessage;
        }
    }
}

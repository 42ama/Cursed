﻿using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Context;
using Cursed.Models.DataModel.ErrorHandling;
using Cursed.Models.StaticReferences.Routing;
using Cursed.Models.Services;

namespace Cursed.Models.LogicValidation
{
    /// <summary>
    /// Transactions section logic validation. Contains of methods used to validate transactions actions
    /// in specific situations.
    /// </summary>
    public class ProductsCatalogLogicValidation
    {
        private readonly CursedDataContext db;
        private readonly IErrorHandlerFactory errorHandlerFactory;

        public ProductsCatalogLogicValidation(CursedDataContext db, IErrorHandlerFactory errorHandlerFactory)
        {
            this.db = db;
            this.errorHandlerFactory = errorHandlerFactory;
        }

        /// <summary>
        /// Checks if product from catalog is valid, to be gathered
        /// </summary>
        /// <param name="key">Id of product from catalog to be found</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckGetSingleDataModelAsync(object key)
        {
            return await CheckExists(key);
        }

        /// <summary>
        /// Checks if product from catalog is valid, to be gathered for update
        /// </summary>
        /// <param name="key">Id of product from catalog to be found</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckGetSingleUpdateModelAsync(object key)
        {
            return await CheckExists(key);
        }

        /// <summary>
        /// Checks if product from catalog is valid, to be updated
        /// </summary>
        /// <param name="key">Id of product from catalog to be found</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckUpdateDataModelAsync(object key)
        {
            return await CheckExists(key);
        }

        /// <summary>
        /// Checks if product from catalog is valid, to be removed
        /// </summary>
        /// <param name="key">Id of product from catalog to be found</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckRemoveDataModelAsync(object key)
        {
            var statusMessage = await CheckExists(key);

            if (!statusMessage.IsCompleted)
            {
                return statusMessage;
            }

            // there must be no related entites to delete product from catalog
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
                        RedirectRoute = ProductsRouting.Index,
                        UseKeyWithRoute = false
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

        /// <summary>
        /// Checks if product exists in catalog
        /// </summary>
        /// <param name="key">Id of product from catalog to be found</param>
        /// <returns>Status message with validaton information</returns>
        private async Task<IErrorHandler> CheckExists(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = "Products in catalog.",
                EntityKey = ((int)key).ToString(),
                RedirectRoute = ProductsCatalogRouting.SingleItem
            });

            // check if product exists in catalog
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

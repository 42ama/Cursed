﻿using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Context;
using Cursed.Models.DataModel.ErrorHandling;
using Cursed.Models.StaticReferences.Routing;
using Cursed.Models.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace Cursed.Models.LogicValidation
{
    /// <summary>
    /// Recipe products section logic validation. Contains of methods used to validate recipe product actions
    /// in specific situations.
    /// </summary>
    public class RecipeProductsLogicValidation
    {
        private readonly CursedDataContext db;
        private readonly IErrorHandlerFactory errorHandlerFactory;

        public RecipeProductsLogicValidation(CursedDataContext db, IErrorHandlerFactory errorHandlerFactory)
        {
            this.db = db;
            this.errorHandlerFactory = errorHandlerFactory;
        }

        /// <summary>
        /// Validates product to recipe relation model
        /// </summary>
        /// <param name="modelState">Model state with validation problems</param>
        /// <returns></returns>
        public IErrorHandler ValidateModel(ModelStateDictionary modelState)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = "Recipe products changes.",
                EntityKey = "",
                RedirectRoute = RecipesRouting.Index,
                UseKeyWithRoute = false
            });

            return ValidateModel(statusMessage, modelState);
        }

        /// <summary>
        /// Validates product to recipe relation model
        /// </summary>
        /// <param name="statusMessage">Error handler to which found problems will be added</param>
        /// <param name="modelState">Model state with validation problems</param>
        /// <returns></returns>
        public IErrorHandler ValidateModel(IErrorHandler statusMessage, ModelStateDictionary modelState)
        {
            if (!modelState.IsValid)
            {
                var errors = modelState.Values.SelectMany(i => i.Errors);

                foreach (var error in errors)
                {
                    statusMessage.AddProblem(new Problem
                    {
                        Entity = "Recipe product changes.",
                        EntityKey = "",
                        Message = error.ErrorMessage,
                        RedirectRoute = RecipesRouting.Index,
                        UseKeyWithRoute = false
                    });
                }
            }

            return statusMessage;
        }

        /// <summary>
        /// Checks if product recipe relation is valid, to be updated
        /// </summary>
        /// <param name="key">ProductId and RecipeId of product recipe relation to be found</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckUpdateDataModelAsync(object key)
        {
            return await CheckExists(key);
        }

        /// <summary>
        /// Checks if product recipe relation is valid, to be removed
        /// </summary>
        /// <param name="key">ProductId and RecipeId of product recipe relation to be found</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckRemoveDataModelAsync(object key)
        {
            return await CheckExists(key);
        }

        /// <summary>
        /// Checks if product recipe relation exists
        /// </summary>
        /// <param name="key">ProductId and RecipeId of product recipe relation to be found</param>
        /// <returns>Status message with validaton information</returns>
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

            // check if product recipe relation exists
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

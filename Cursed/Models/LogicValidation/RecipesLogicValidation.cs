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
    public class RecipesLogicValidation
    {
        private readonly CursedContext db;
        private readonly IErrorHandlerFactory errorHandlerFactory;

        public RecipesLogicValidation(CursedContext db, IErrorHandlerFactory errorHandlerFactory)
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
            var childs = db.RecipeInheritance.Where(i => i.ParentId == (int)key);
            var techProcesses = db.TechProcess.Where(i => i.RecipeId == (int)key);
            var productsChanges = db.RecipeProductChanges.Where(i => i.RecipeId == (int)key);

            if (childs.Any())
            {
                foreach (var child in childs)
                {
                    statusMessage.Problems.Add(new Problem
                    {
                        Entity = "Recipe.",
                        EntityKey = child.ChildId,
                        Message = "Recipe have childs.",
                        RedirectRoute = RecipesRouting.SingleItem
                    });
                }
            }
            if (techProcesses.Any())
            {
                foreach (var techProcess in techProcesses)
                {
                    statusMessage.Problems.Add(new Problem
                    {
                        Entity = "Tech process.",
                        EntityKey = techProcess.FacilityId,
                        Message = "Recipe have related tech process.",
                        RedirectRoute = FacilityTechProcessesRouting.Index
                    });
                }
            }
            if (productsChanges.Any())
            {
                foreach (var productChanges in productsChanges)
                {
                    statusMessage.Problems.Add(new Problem
                    {
                        Entity = "Recipe product changes.",
                        EntityKey = productChanges.RecipeId,
                        Message = "Recipe have related recipe product changes.",
                        RedirectRoute = RecipeProductsRouting.Index
                    });
                }
            }

            return statusMessage;
        }
        private async Task<IErrorHandler> CheckExists(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = "Recipe.",
                EntityKey = (int)key,
                RedirectRoute = RecipesRouting.SingleItem
            });

            if (await db.Recipe.FirstOrDefaultAsync(i => i.Id == (int)key) == null)
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = "Recipe.",
                    EntityKey = (int)key,
                    Message = "No recipe with such key found.",
                    RedirectRoute = RecipesRouting.Index,
                    UseKeyWithRoute = false
                });
            }

            return statusMessage;
        }
    }
}

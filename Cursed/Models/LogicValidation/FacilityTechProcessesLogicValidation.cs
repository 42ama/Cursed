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

namespace Cursed.Models.LogicValidation
{
    public class FacilityTechProcessesLogicValidation
    {
        private readonly CursedContext db;
        private readonly AbstractErrorHandlerFactory errorHandlerFactory;

        public FacilityTechProcessesLogicValidation(CursedContext db)
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
            var facilities = db.Facility.Where(i => i.Id == ((ValueTuple<int, int>)key).Item1);
            var recipes = db.Recipe.Where(i => i.Id == ((ValueTuple<int, int>)key).Item2);

            if (facilities.Any())
            {
                foreach (var facility in facilities)
                {
                    statusMessage.Problems.Add(new Problem
                    {
                        Entity = "Facility.",
                        EntityKey = facility.Id,
                        Message = "Tech process have related facility.",
                        RedirectRoute = FacilitiesRouting.SingleItem
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
                        Message = "Tech process have related recipe.",
                        RedirectRoute = RecipesRouting.SingleItem
                    });
                }
            }

            return statusMessage;
        }
        private async Task<AbstractErrorHandler> CheckExists(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler("Tech process.", key);
            // facility id and recipe id
            var tupleKey = ((ValueTuple<int, int>)key);
            if (await db.TechProcess.FirstOrDefaultAsync(i => i.FacilityId == tupleKey.Item1 &&
            i.RecipeId == tupleKey.Item2) == null)
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = "Tech process.",
                    EntityKey = tupleKey.Item1,
                    Message = $"No tech proccess with such Recipe Id: {tupleKey.Item2} found.",
                    RedirectRoute = FacilityTechProcessesRouting.Index
                });
            }

            return statusMessage;
        }
    }
}

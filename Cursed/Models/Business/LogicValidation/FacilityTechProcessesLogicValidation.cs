using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Context;
using Cursed.Models.DataModel.ErrorHandling;
using Cursed.Models.StaticReferences.Routing;
using Cursed.Models.Services;
using Cursed.Models.Entities.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace Cursed.Models.LogicValidation
{
    /// <summary>
    /// Tech processes section logic validation. Contains of methods used to validate tech process actions
    /// in specific situations.
    /// </summary>
    public class FacilityTechProcessesLogicValidation
    {
        private readonly CursedDataContext db;
        private readonly IErrorHandlerFactory errorHandlerFactory;

        public FacilityTechProcessesLogicValidation(CursedDataContext db, IErrorHandlerFactory errorHandlerFactory)
        {
            this.db = db;
            this.errorHandlerFactory = errorHandlerFactory;
        }

        /// <summary>
        /// Validates tech process model
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
                        Entity = "Technological process.",
                        EntityKey = "",
                        Message = error.ErrorMessage,
                        RedirectRoute = FacilitiesRouting.Index,
                        UseKeyWithRoute = false
                    });
                }
            }

            return statusMessage;
        }

        /// <summary>
        /// Checks if tech process is valid, to be updated
        /// </summary>
        /// <param name="model">Technological process model to be validated</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckUpdateDataModelAsync(TechProcess model)
        {
            var statusMessage = await CheckExists((model.FacilityId, model.RecipeId));

            return await CheckRelatedEntitiesExists(model, statusMessage);
        }

        /// <summary>
        /// Checks if technological process is valid, to be added
        /// </summary>
        /// <param name="model">Technological process model to be validated</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckAddDataModelAsync(TechProcess model)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = "Technological procces.",
                EntityKey = "",
                RedirectRoute = HubRouting.Index,
                UseKeyWithRoute = false
            });

            return await CheckRelatedEntitiesExists(model, statusMessage);
        }

        /// <summary>
        /// Checks if tech process is valid, to be removed
        /// </summary>
        /// <param name="key">RecipeId and FacilityId of tech process to be found</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckRemoveDataModelAsync(object key)
        {
            return await CheckExists(key);
        }

        /// <summary>
        /// Checks if related entities exitst
        /// </summary>
        /// <param name="model">Technological process model to be validated</param>
        /// <param name="statusMessage">Error handler to which problem will be added</param>
        /// <returns>Status message with validaton information</returns>
        private async Task<IErrorHandler> CheckRelatedEntitiesExists(TechProcess model, IErrorHandler statusMessage)
        {
            // check if related facility exists
            if (await db.Facility.FirstOrDefaultAsync(i => i.Id == model.FacilityId) == null)
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = "Facility.",
                    EntityKey = (model.FacilityId).ToString(),
                    Message = "Facility with this Id isn't found",
                    RedirectRoute = FacilitiesRouting.Index,
                    UseKeyWithRoute = false
                });
            }

            // check if related product from catalog exists
            if (await db.Recipe.FirstOrDefaultAsync(i => i.Id == model.RecipeId) == null)
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = "Recipe.",
                    EntityKey = (model.RecipeId).ToString(),
                    Message = "Recipe with this Id isn't found",
                    RedirectRoute = RecipesRouting.Index,
                    UseKeyWithRoute = false
                });
            }

            return statusMessage;
        }

        /// <summary>
        /// Checks if tech process exists
        /// </summary>
        /// <param name="key">RecipeId and FacilityId of tech process to be found</param>
        /// <returns>Status message with validaton information</returns>
        private async Task<IErrorHandler> CheckExists(object key)
        {
            // facility id and recipe id
            var tupleKey = (ValueTuple<int, int>)key;

            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = $"Technological process. Recipe Id: {tupleKey.Item2}.",
                EntityKey = tupleKey.Item1.ToString(),
                RedirectRoute = FacilityTechProcessesRouting.Index
            });

            // check if tech process exists
            if (await db.TechProcess.FirstOrDefaultAsync(i => i.FacilityId == tupleKey.Item1 &&
            i.RecipeId == tupleKey.Item2) == null)
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = $"Technological process. Recipe Id: {tupleKey.Item2}.",
                    EntityKey = tupleKey.Item1.ToString(),
                    Message = "Technological process with this Id's is not found.",
                    RedirectRoute = FacilityTechProcessesRouting.Index
                });
            }

            return statusMessage;
        }
    }
}

﻿using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Context;
using Cursed.Models.DataModel.ErrorHandling;
using Cursed.Models.StaticReferences.Routing;
using Cursed.Models.Services;

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
        /// Checks if tech process is valid, to be updated
        /// </summary>
        /// <param name="key">RecipeId and FacilityId of tech process to be found</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckUpdateDataModelAsync(object key)
        {
            return await CheckExists(key);
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

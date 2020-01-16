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
using Cursed.Models.DataModel.Companies;
using Cursed.Models.Entities;
using Cursed.Models.Interfaces.LogicCRUD;
using Cursed.Models.DataModel.Utility;
using Cursed.Models.DataModel.Utility.ErrorHandling;
using Cursed.Models.StaticReferences.Routing;
using Cursed.Models.Services;

namespace Cursed.Models.LogicValidation
{
    public class FacilitiesLogicValidation
    {
        private readonly CursedDataContext db;
        private readonly IErrorHandlerFactory errorHandlerFactory;

        public FacilitiesLogicValidation(CursedDataContext db, IErrorHandlerFactory errorHandlerFactory)
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
            var techProcesses = db.TechProcess.Where(i => i.FacilityId == (int)key);

            if (techProcesses.Any())
            {
                foreach (var techProcess in techProcesses)
                {
                    statusMessage.Problems.Add(new Problem
                    {
                        Entity = $"Technological process. Recipe Id: {techProcess.RecipeId}.",
                        EntityKey = techProcess.FacilityId.ToString(),
                        Message = "You must remove dependent Technological Process first.",
                        RedirectRoute = FacilityTechProcessesRouting.Index
                    });
                }
            }

            return statusMessage;
        }
        private async Task<IErrorHandler> CheckExists(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = "Facility.",
                EntityKey = ((int)key).ToString(),
                RedirectRoute = FacilitiesRouting.SingleItem
            });

            if (await db.Facility.FirstOrDefaultAsync(i => i.Id == (int)key) == null)
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = "Facility.",
                    EntityKey = ((int)key).ToString(),
                    Message = "Facility with this Id is not found.",
                    RedirectRoute = FacilitiesRouting.Index,
                    UseKeyWithRoute = false
                });
            }

            return statusMessage;
        }
    }
}

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
using Cursed.Models.Routing;
using Cursed.Models.Services;

namespace Cursed.Models.LogicValidation
{
    public class FacilityTechProcessesLogicValidation
    {
        private readonly CursedDataContext db;
        private readonly IErrorHandlerFactory errorHandlerFactory;

        public FacilityTechProcessesLogicValidation(CursedDataContext db, IErrorHandlerFactory errorHandlerFactory)
        {
            this.db = db;
            this.errorHandlerFactory = errorHandlerFactory;
        }
                
        public async Task<IErrorHandler> CheckUpdateDataModelAsync(object key)
        {
            return await CheckExists(key);
        }

        public async Task<IErrorHandler> CheckRemoveDataModelAsync(object key)
        {
            return await CheckExists(key);
        }
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

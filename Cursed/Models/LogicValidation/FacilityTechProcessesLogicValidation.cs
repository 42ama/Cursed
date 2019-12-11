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
    public class FacilityTechProcessesLogicValidation
    {
        private readonly CursedContext db;
        private readonly IErrorHandlerFactory errorHandlerFactory;

        public FacilityTechProcessesLogicValidation(CursedContext db, IErrorHandlerFactory errorHandlerFactory)
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
                Entity = "Tech process.",
                EntityKey = tupleKey.Item1,
                RedirectRoute = FacilityTechProcessesRouting.Index
            });
            

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

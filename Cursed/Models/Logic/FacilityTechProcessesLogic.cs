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
using Cursed.Models.Data.FacilityTechProcesses;
using Cursed.Models.Entities;
using Cursed.Models.Data.Shared;
using Cursed.Models.Interfaces.LogicCRUD;
using Cursed.Models.Data.Utility.ErrorHandling;

namespace Cursed.Models.Logic
{
    public class FacilityTechProcessesLogic// : IReadCollectionByParam<FacilityTechProcessesDataModel>, ICreate<TechProcess>, IUpdate<TechProcess>, IDeleteByModel<TechProcess>
    {
        private readonly CursedContext db;
        private readonly AbstractErrorHandlerFactory errorHandlerFactory;
        public FacilityTechProcessesLogic(CursedContext db)
        {
            this.db = db;
            errorHandlerFactory = new StatusMessageFactory();
        }

        public async Task<AbstractErrorHandler<IEnumerable<FacilityTechProcessesDataModel>>> GetAllDataModelAsync(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler<IEnumerable<FacilityTechProcessesDataModel>>("Tech processes.", key);

            int facilityId = (int)key;
            var query = from f in db.TechProcess
                        where f.FacilityId == facilityId
                        join fac in db.Facility on f.FacilityId equals fac.Id into facilities
                        from facs in facilities
                        join r in db.Recipe on f.RecipeId equals r.Id into recipes
                        from rs in recipes
                        select new FacilityTechProcessesDataModel
                        {
                            FacilityId = f.FacilityId,
                            FacilityName = facs.Name,
                            DayEffiency = f.DayEffiency,
                            RecipeId = f.RecipeId,
                            RecipeContent = rs.Content,
                            RecipeGovApprov = rs.GovermentApproval ?? false,
                            RecipeTechApprov = rs.TechApproval ?? false
                        };

            statusMessage.ReturnValue = query;

            return statusMessage;
        }

        public async Task<AbstractErrorHandler> AddDataModelAsync(TechProcess model)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler("Tech process.", (facilityId: model.FacilityId, recipeId: model.RecipeId));

            db.Add(model);
            await db.SaveChangesAsync();

            return statusMessage;
        }

        public async Task<AbstractErrorHandler> UpdateDataModelAsync(TechProcess model)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler("Tech process.", (facilityId: model.FacilityId, recipeId: model.RecipeId));

            var currentModel = await db.TechProcess.FirstOrDefaultAsync(i => i.RecipeId == model.RecipeId && i.FacilityId == model.FacilityId);
            db.Entry(currentModel).CurrentValues.SetValues(model);
            await db.SaveChangesAsync();

            return statusMessage;
        }

        public async Task<AbstractErrorHandler> RemoveDataModelAsync(TechProcess model)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler("Tech process.", (facilityId: model.FacilityId, recipeId: model.RecipeId));

            var entity = await db.TechProcess.SingleAsync(i => i.RecipeId == model.RecipeId && i.FacilityId == model.FacilityId);
            db.TechProcess.Remove(entity);
            await db.SaveChangesAsync();

            return statusMessage;
        }
    }
}

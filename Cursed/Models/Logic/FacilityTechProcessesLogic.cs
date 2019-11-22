﻿using System;
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

namespace Cursed.Models.Logic
{
    public class FacilityTechProcessesLogic : IReadCollectionByParam<FacilityTechProcessesDataModel>, ICreate<TechProcess>, IUpdate<TechProcess>, IDeleteByModel<TechProcess>
    {
        private readonly CursedContext db;
        public FacilityTechProcessesLogic(CursedContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<FacilityTechProcessesDataModel>> GetAllDataModelAsync(object key)
        {
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
            return query;
        }

        public async Task AddDataModelAsync(TechProcess model)
        {
            db.Add(model);
            await db.SaveChangesAsync();
        }

        public async Task UpdateDataModelAsync(TechProcess model)
        {
            var currentModel = await db.TechProcess.FirstOrDefaultAsync(i => i.RecipeId == model.RecipeId && i.FacilityId == model.FacilityId);
            db.Entry(currentModel).CurrentValues.SetValues(model);
            await db.SaveChangesAsync();
        }

        public async Task RemoveDataModelAsync(TechProcess model)
        {
            var entity = await db.TechProcess.SingleAsync(i => i.RecipeId == model.RecipeId && i.FacilityId == model.FacilityId);

            db.TechProcess.Remove(entity);

            await db.SaveChangesAsync();
        }
    }
}
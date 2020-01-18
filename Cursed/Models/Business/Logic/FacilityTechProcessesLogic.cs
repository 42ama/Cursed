using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Context;
using Cursed.Models.DataModel.FacilityTechProcesses;
using Cursed.Models.Entities.Data;
using Cursed.Models.Interfaces.LogicCRUD;

namespace Cursed.Models.Logic
{
    /// <summary>
    /// Tech process section logic. Consists of CRUD actions for tech processes.
    /// </summary>
    public class FacilityTechProcessesLogic : IReadCollectionByParam<FacilityTechProcessesDataModel>, ICreate<TechProcess>, IUpdate<TechProcess>, IDeleteByModel<TechProcess>
    {
        private readonly CursedDataContext db;
        public FacilityTechProcessesLogic(CursedDataContext db)
        {
            this.db = db;
        }

        /// <summary>
        /// Gather all tech processes at specific facility from database.
        /// </summary>
        /// <param name="key">Id of facility to which tech processes belongs</param>
        /// <returns>All tech processes from database. Each tech process contains more information than TechProcess entity.</returns>
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
                            DayEfficiency = f.DayEfficiency,
                            RecipeId = f.RecipeId,
                            RecipeContent = rs.Content,
                            RecipeGovApprov = rs.GovermentApproval ?? false,
                            RecipeTechApprov = rs.TechApproval ?? false
                        };

            return query;
        }

        /// <summary>
        /// Add new tech process.
        /// </summary>
        /// <param name="model">Tech process to be added</param>
        /// <returns>Added tech process with correct key(FacilityId, RecipeId) value</returns>
        public async Task<TechProcess> AddDataModelAsync(TechProcess model)
        {
            var entity = db.Add(model);
            await db.SaveChangesAsync();
            return entity.Entity;
        }

        /// <summary>
        /// Update tech process.
        /// </summary>
        /// <param name="model">Updated company information</param>
        public async Task UpdateDataModelAsync(TechProcess model)
        {
            var currentModel = await db.TechProcess.FirstOrDefaultAsync(i => i.RecipeId == model.RecipeId && i.FacilityId == model.FacilityId);
            db.Entry(currentModel).CurrentValues.SetValues(model);
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Delete tech process.
        /// </summary>
        /// <param name="model">Model of tech process containing key information (FacilityId and RecipeId) to find tech process</param>
        public async Task RemoveDataModelAsync(TechProcess model)
        {
            var entity = await db.TechProcess.SingleAsync(i => i.RecipeId == model.RecipeId && i.FacilityId == model.FacilityId);
            db.TechProcess.Remove(entity);
            await db.SaveChangesAsync();
        }
    }
}

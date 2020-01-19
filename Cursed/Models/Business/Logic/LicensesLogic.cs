using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Context;
using Cursed.Models.DataModel.Licenses;
using Cursed.Models.Entities.Data;
using Cursed.Models.Interfaces.LogicCRUD;

namespace Cursed.Models.Logic
{
    /// <summary>
    /// Licenses section logic. Consists of CRUD actions for licenses, including gathering methods for
    /// both single license and collection of all licenses.
    /// </summary>
    public class LicensesLogic : IReadColection<LicensesDataModel>, IReadSingle<LicensesDataModel>, IReadUpdateForm<License>, ICUD<License>
    {
        private readonly CursedDataContext db;
        private readonly IQueryable<LicensesDataModel> basicDataModelQuery;

        public LicensesLogic(CursedDataContext db)
        {
            this.db = db;
            
            // basic query, which used to access licenses data
            basicDataModelQuery = db.License.Join
                (
                    db.ProductCatalog,
                    lt => lt.ProductId,
                    rt => rt.Id,
                    (x, y) => new LicensesDataModel
                    {
                        Id = x.Id,
                        GovermentNum = x.GovermentNum,
                        Date = x.Date,
                        ProductId = x.ProductId,
                        ProductName = y.Name,
                        ProductCAS = y.Cas
                    }
                );
        }

        /// <summary>
        /// Gather all licenses from database.
        /// </summary>
        /// <returns>All licenses from database. Each license contains more information than License entity.</returns>
        public async Task<IEnumerable<LicensesDataModel>> GetAllDataModelAsync()
        {
            return basicDataModelQuery;
        }

        /// <summary>
        /// Gather single license, which found by <c>key</c>.
        /// </summary>
        /// <param name="key">Id of license to be found</param>
        /// <returns>Single license, which found by <c>key</c>. Contains more information than License entity.</returns>
        public async Task<LicensesDataModel> GetSingleDataModelAsync(object key)
        {
            return basicDataModelQuery.Single(i => i.Id == (int)key);
        }

        /// <summary>
        /// Returns a collection of entities that shares common <c>key</c> - Product Id
        /// </summary>
        /// <param name="relatedKey">Product Id</param>
        /// <returns>Collection of entities that shares common <c>key</c> - Product Id</returns>
        public async Task<IEnumerable<LicensesDataModel>> GetRelatedEntitiesByKeyAsync(object relatedKey, object exceptKey)
        {
            return basicDataModelQuery.Where(i => i.ProductId == (int)relatedKey && i.Id != (int)exceptKey);
        }

        /// <summary>
        /// Gather single license, which found by <c>key</c>.
        /// </summary>
        /// <param name="key">Id of license to be found</param>
        /// <returns>Single license, which found by <c>key</c>.</returns>
        public async Task<License> GetSingleUpdateModelAsync(object key)
        {
            return await db.License.SingleAsync(i => i.Id == (int)key);
        }

        /// <summary>
        /// Add new license.
        /// </summary>
        /// <param name="model">License to be added</param>
        /// <returns>Added license with correct key(Id) value</returns>
        public async Task<License> AddDataModelAsync(License model)
        {
            model.Id = default;
            var entity = db.Add(model);
            await db.SaveChangesAsync();
            return entity.Entity;
        }

        /// <summary>
        /// Update license.
        /// </summary>
        /// <param name="model">Updated license information</param>
        public async Task UpdateDataModelAsync(License model)
        {
            var currentModel = await db.License.FirstOrDefaultAsync(i => i.Id == model.Id);
            db.Entry(currentModel).CurrentValues.SetValues(model);
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Delete license.
        /// </summary>
        /// <param name="key">Id of license to be deleted</param>
        public async Task RemoveDataModelAsync(object key)
        {
            var entity = await db.License.FindAsync((int)key);
            db.License.Remove(entity);
            await db.SaveChangesAsync();
        }
    }
}

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Context;
using Cursed.Models.Entities.Data;
using Cursed.Models.Interfaces.LogicCRUD;

namespace Cursed.Models.Logic
{
    /// <summary>
    /// Operations section logic. Consists of CRUD actions for operations. Part of operations 
    /// read actions located in transactions logic.
    /// </summary>
    public class OperationsLogic : IReadUpdateForm<Operation>, ICreate<Operation>, IUpdate<Operation>, IDeleteByModel<Operation>
    {
        private readonly CursedDataContext db;

        public OperationsLogic(CursedDataContext db)
        {
            this.db = db;
        }

        /// <summary>
        /// Gather single operation, which found by <c>key</c>.
        /// </summary>
        /// <param name="key">Id of operation to be found</param>
        /// <returns>Single operation, which found by <c>key</c>.</returns>
        public async Task<Operation> GetSingleUpdateModelAsync(object key)
        {
            return await db.Operation.SingleAsync(i => i.Id == (int)key);
        }

        /// <summary>
        /// Add new operation.
        /// </summary>
        /// <param name="model">Operation to be added</param>
        /// <returns>Added operation with correct key(Id) value</returns>
        public async Task<Operation> AddDataModelAsync(Operation model)
        {
            model.Id = default;
            var entity = db.Add(model);
            await db.SaveChangesAsync();
            return entity.Entity;
        }

        /// <summary>
        /// Update operation.
        /// </summary>
        /// <param name="model">Updated operation information</param>
        public async Task UpdateDataModelAsync(Operation model)
        {
            var currentModel = await db.Operation.FirstOrDefaultAsync(i => i.Id == model.Id);
            db.Entry(currentModel).CurrentValues.SetValues(model);
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Delete operation.
        /// </summary>
        /// <param name="model">Model of operation containing key information (Id) to find operation</param>
        public async Task RemoveDataModelAsync(Operation model)
        {
            var entity = await db.Operation.FindAsync(model.Id);
            db.Operation.Remove(entity);
            await db.SaveChangesAsync();
        }
    }
}

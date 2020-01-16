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
using Cursed.Models.DataModel.Transactions;
using Cursed.Models.Entities.Data;
using Cursed.Models.Interfaces.LogicCRUD;

namespace Cursed.Models.Logic
{
    public class OperationsLogic : IReadUpdateForm<Operation>, ICreate<Operation>, IUpdate<Operation>, IDeleteByModel<Operation>
    {
        private readonly CursedDataContext db;

        public OperationsLogic(CursedDataContext db)
        {
            this.db = db;
        }

        public async Task<Operation> GetSingleUpdateModelAsync(object key)
        {
            return await db.Operation.SingleAsync(i => i.Id == (int)key);
        }

        public async Task<Operation> AddDataModelAsync(Operation model)
        {
            model.Id = default;
            var entity = db.Add(model);
            await db.SaveChangesAsync();
            return entity.Entity;
        }

        public async Task UpdateDataModelAsync(Operation model)
        {
            var currentModel = await db.Operation.FirstOrDefaultAsync(i => i.Id == model.Id);
            db.Entry(currentModel).CurrentValues.SetValues(model);
            await db.SaveChangesAsync();
        }

        public async Task RemoveDataModelAsync(Operation model)
        {
            var entity = await db.Operation.FindAsync(model.Id);
            db.Operation.Remove(entity);
            await db.SaveChangesAsync();
        }
    }
}

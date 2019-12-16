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
using Cursed.Models.Data.Transactions;
using Cursed.Models.Entities;
using Cursed.Models.Data.Shared;
using Cursed.Models.Data.Utility;
using Cursed.Models.Services;
using Cursed.Models.Data.Utility.ErrorHandling;
using Cursed.Models.Interfaces.LogicCRUD;

namespace Cursed.Models.Logic
{
    public class OperationsLogic : IReadUpdateForm<Operation>, ICreate<Operation>, IUpdate<Operation>, IDeleteByModel<Operation>
    {
        private readonly CursedContext db;

        public OperationsLogic(CursedContext db)
        {
            this.db = db;
        }

        public async Task<Operation> GetSingleUpdateModelAsync(object key)
        {
            return await db.Operation.SingleAsync(i => i.Id == (int)key);
        }

        public async Task AddDataModelAsync(Operation model)
        {
            model.Id = default;
            db.Add(model);
            await db.SaveChangesAsync();
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
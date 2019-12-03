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
using Cursed.Models.Data.Transactions;
using Cursed.Models.Entities;
using Cursed.Models.Data.Shared;
using Cursed.Models.Data.Utility;
using Cursed.Models.Services;
using Cursed.Models.Interfaces.LogicCRUD;

namespace Cursed.Models.Logic
{
    public class TransactionsLogic
    {
        private readonly CursedContext db;
        public TransactionsLogic(CursedContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<TransactionsModel>> GetAllDataModelAsync()
        {
            var transactions = await db.TransactionBatch.ToListAsync();
            var query = from t in transactions
                        join c in db.Company on t.CompanyId equals c.Id into companies
                        from comp in companies
                        join o in (from t in transactions
                                   join o in db.Operation on t.Id equals o.TransactionId into operations
                                   group operations by t.Id) on t.Id equals o.Key
                        select new TransactionsModel
                        {
                            Id = t.Id,
                            Date = t.Date,
                            Type = t.Type,
                            Comment = t.Comment,
                            IsOpen = t.IsOpen,
                            CompanyId = t.CompanyId,
                            CompanyName = comp.Name,
                            OperationsCount = o.Single().Count()
                        };

            return query;
        }

        public async Task<TransactionModel> GetSingleDataModelAsync(object key)
        {
            var transactions = await db.TransactionBatch.ToListAsync();
            var query = from t in transactions
                        where t.Id == (int)key
                        join c in db.Company on t.CompanyId equals c.Id into companies
                        from comp in companies
                        join o in (from t in transactions
                                   join o in db.Operation on t.Id equals o.TransactionId into operations
                                   group operations by t.Id) on t.Id equals o.Key
                        select new TransactionModel
                        {
                            Id = t.Id,
                            Date = t.Date,
                            Type = t.Type,
                            Comment = t.Comment,
                            IsOpen = t.IsOpen,
                            CompanyId = t.CompanyId,
                            CompanyName = comp.Name,
                            Operations = o.Single().ToList()
                        };

            return query.Single();
        }

        public async Task<TransactionBatch> GetSingleUpdateModelAsync(object key)
        {
            return await db.TransactionBatch.SingleAsync(i => i.Id == (int)key);
        }

        public async Task CloseTransactionAsync(object key)
        {

        }

        public async Task AddDataModelAsync(TransactionBatch model)
        {
            model.Id = default;
            db.Add(model);
            await db.SaveChangesAsync();
        }

        public async Task UpdateDataModelAsync(TransactionBatch model)
        {
            var currentModel = await db.TransactionBatch.FirstOrDefaultAsync(i => i.Id == model.Id);
            if(!currentModel.IsOpen)
            {
                throw new Exception();
            }
            db.Entry(currentModel).CurrentValues.SetValues(model);
            await db.SaveChangesAsync();
        }

        public async Task RemoveDataModelAsync(object key)
        {
            int transactionId = (int)key;
            var entity = await db.TransactionBatch.FindAsync(transactionId);
            if (!entity.IsOpen)
            {
                throw new Exception();
            }

            db.Operation.RemoveRange(db.Operation.Where(i => i.TransactionId == transactionId));
            db.TransactionBatch.Remove(entity);

            await db.SaveChangesAsync();
        }
    }
}

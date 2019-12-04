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

namespace Cursed.Models.Logic
{
    public class CompaniesLogic// : IReadColection<CompaniesModel>, IReadSingle<CompanyModel>, IReadUpdateForm<Company>, ICUD<Company>
    {
        private readonly CursedContext db;
        public CompaniesLogic(CursedContext db)
        {
            this.db = db;
        }

        public async Task<StatusMessage<IEnumerable<CompaniesModel>>> GetAllDataModelAsync()
        {
            var statusMessage = new StatusMessage<IEnumerable<CompaniesModel>>
            {
                Entity = "All companies."
            };
            var companies = await db.Company.ToListAsync();
            var query = from c in companies
                        join s in (from c in companies
                                   join s in db.Storage on c.Id equals s.CompanyId into tab
                                   group tab by c.Id) on c.Id equals s.Key into storages
                        from cs in storages.DefaultIfEmpty()
                        join t in (from c in companies
                                   join t in db.TransactionBatch on c.Id equals t.CompanyId into tab
                                   group tab by c.Id) on c.Id equals t.Key into transcts
                        from cst in transcts.DefaultIfEmpty()
                        select new CompaniesModel
                        {
                            Id = c.Id,
                            Name = c.Name,
                            StoragesCount = cs.Single().Count(),
                            TransactionsCount = cst.Single().Count()
                        };
            statusMessage.ReturnValue = query;
            return statusMessage;
        }

        public async Task<StatusMessage<CompanyModel>> GetSingleDataModelAsync(object key)
        {
            var statusMessage = new StatusMessage<CompanyModel>
            {
                Entity = $"Company. Id: {key}.",
                EntityKey = key
            };
            var companies = await db.Company.Where(i => i.Id == (int)key).ToListAsync();
            var query = from c in companies
                        join s in (from c in companies
                                   join s in db.Storage on c.Id equals s.CompanyId into tab
                                   group tab by c.Id) on c.Id equals s.Key into storages
                        from cs in storages.DefaultIfEmpty()
                        join t in (from c in companies
                                   join t in db.TransactionBatch on c.Id equals t.CompanyId into tab
                                   group tab by c.Id) on c.Id equals t.Key into transcts
                        from cst in transcts.DefaultIfEmpty()
                        select new CompanyModel
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Storages = cs.Single().Select(i => new TitleIdContainer { Id = i.Id, Title = i.Name }).ToList(),
                            Transactions = cst.Single().Select(i => new TitleIdContainer { Id = i.Id, Title = i.Date.ToShortDateString() }).ToList()
                        };

            statusMessage.ReturnValue = query.Single();
            return statusMessage;
        }

        public async Task<StatusMessage<Company>> GetSingleUpdateModelAsync(object key)
        {
            var statusMessage = new StatusMessage<Company>
            {
                Entity = $"Company. Id: {key}.",
                EntityKey = key
            };

            var company = await db.Company.SingleOrDefaultAsync(i => i.Id == (int)key);
            statusMessage.ReturnValue = company;

            return statusMessage;
        }

        public async Task<StatusMessage> AddDataModelAsync(Company model)
        {
            var statusMessage = new StatusMessage
            {
                Entity = $"Company. Id: <NotSetuped>."
            };

            model.Id = default;
            db.Add(model);
            await db.SaveChangesAsync();

            return statusMessage;
        }

        public async Task<StatusMessage> UpdateDataModelAsync(Company model)
        {
            var statusMessage = new StatusMessage
            {
                Entity = $"Company. Id: {model.Id}.",
                EntityKey = model.Id
            };

            var currentModel = await db.Company.FirstOrDefaultAsync(i => i.Id == model.Id);
            db.Entry(currentModel).CurrentValues.SetValues(model);
            await db.SaveChangesAsync();

            return statusMessage;
        }

        public async Task<StatusMessage> RemoveDataModelAsync(object key)
        {
            var statusMessage = new StatusMessage
            {
                Entity = $"Company. Id: {key}.",
                EntityKey = key
            };

            var entity = await db.Company.FindAsync((int)key);
            db.Company.Remove(entity);
            await db.SaveChangesAsync();

            return statusMessage;
        }
    }
}

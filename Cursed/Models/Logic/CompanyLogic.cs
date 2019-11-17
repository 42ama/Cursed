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
using Cursed.Models.Data.Company;
using Cursed.Models.Entities;
using Cursed.Models.Data.Shared;
using Cursed.Models.Data.Utility;

namespace Cursed.Models.Logic
{
    public class CompanyLogic : IRESTAsync<CompanyAllModel, CompanySingleModel, Company>
    {
        private readonly CursedContext db;
        public CompanyLogic(CursedContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<CompanyAllModel>> GetAllDataModelAsync()
        {
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
                        select new CompanyAllModel
                        {
                            Id = c.Id,
                            Name = c.Name,
                            StoragesCount = cs.Single().Count(),
                            TransactionsCount = cst.Single().Count()
                        };
            return query;
        }

        public async Task<CompanySingleModel> GetSingleDataModelAsync(object key)
        {
            var companies = await db.Company.ToListAsync();
            var query = from c in companies
                        where c.Id == (int)key
                        join s in (from c in companies
                                   join s in db.Storage on c.Id equals s.CompanyId into tab
                                   group tab by c.Id) on c.Id equals s.Key into storages
                        from cs in storages.DefaultIfEmpty()
                        join t in (from c in companies
                                   join t in db.TransactionBatch on c.Id equals t.CompanyId into tab
                                   group tab by c.Id) on c.Id equals t.Key into transcts
                        from cst in transcts.DefaultIfEmpty()
                        select new CompanySingleModel
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Storages = cs.Single().Select(i => new TitleIdContainer { Id = i.Id, Title = i.Name }).ToList(),
                            Transactions = cst.Single().Select(i => new TitleIdContainer { Id = i.Id, Title = i.Date.ToShortDateString() }).ToList()
                        };
            return query.Single();
        }

        public async Task<Company> GetSingleUpdateModelAsync(object key)
        {
            return await db.Company.SingleOrDefaultAsync(i => i.Id == (int)key);
        }

        public async Task AddDataModelAsync(Company dataModel)
        {
            dataModel.Id = default;
            db.Add(dataModel);
            await db.SaveChangesAsync();
        }

        public async Task UpdateDataModelAsync(Company updatedDataModel)
        {
            var currentModel = await db.Company.FirstOrDefaultAsync(i => i.Id == updatedDataModel.Id);
            db.Entry(currentModel).CurrentValues.SetValues(updatedDataModel);
            await db.SaveChangesAsync();
        }

        public async Task RemoveDataModelAsync(object key)
        {
            var entity = await db.Company.FindAsync((int)key);

            db.Company.Remove(entity);

            await db.SaveChangesAsync();
        }
    }
}

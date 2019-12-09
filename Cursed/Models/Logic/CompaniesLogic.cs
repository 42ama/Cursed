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
using Cursed.Models.Data.Companies;
using Cursed.Models.Entities;
using Cursed.Models.Interfaces.LogicCRUD;
using Cursed.Models.Data.Utility;
using Cursed.Models.Data.Utility.ErrorHandling;

namespace Cursed.Models.Logic
{
    public class CompaniesLogic : IReadColection<CompaniesModel>, IReadSingle<CompanyModel>, IReadUpdateForm<Company>, ICUD<Company>
    {
        private readonly CursedContext db;
        public CompaniesLogic(CursedContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<CompaniesModel>> GetAllDataModelAsync()
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
                        select new CompaniesModel
                        {
                            Id = c.Id,
                            Name = c.Name,
                            StoragesCount = cs.Single().Count(),
                            TransactionsCount = cst.Single().Count()
                        };

            return query;
        }

        public async Task<CompanyModel> GetSingleDataModelAsync(object key)
        {
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

            return query.Single();
        }

        public async Task<Company> GetSingleUpdateModelAsync(object key)
        {
            return await db.Company.SingleOrDefaultAsync(i => i.Id == (int)key);
        }

        public async Task AddDataModelAsync(Company model)
        {
            model.Id = default;
            db.Add(model);
            await db.SaveChangesAsync();
        }

        public async Task UpdateDataModelAsync(Company model)
        {
            var currentModel = await db.Company.FirstOrDefaultAsync(i => i.Id == model.Id);
            db.Entry(currentModel).CurrentValues.SetValues(model);
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

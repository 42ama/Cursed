using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Context;
using Cursed.Models.DataModel.Companies;
using Cursed.Models.Entities.Data;
using Cursed.Models.Interfaces.LogicCRUD;

namespace Cursed.Models.Logic
{
    /// <summary>
    /// Companies section logic. Consists of CRUD actions for companies, including gathering methods for
    /// both single company and collection of all companies.
    /// </summary>
    public class CompaniesLogic : IReadColection<CompaniesModel>, IReadSingle<CompanyModel>, IReadUpdateForm<Company>, ICUD<Company>
    {
        private readonly CursedDataContext db;
        public CompaniesLogic(CursedDataContext db)
        {
            this.db = db;
        }

        /// <summary>
        /// Gather all companies from database.
        /// </summary>
        /// <returns>All companies from database. Each company contains more information than Company entity.</returns>
        public async Task<IEnumerable<CompaniesModel>> GetAllDataModelAsync()
        {
            // gather companies, to have unblocking call when grouping
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

        /// <summary>
        /// Gather single company, which found by <c>key</c>.
        /// </summary>
        /// <param name="key">Id of company to be found</param>
        /// <returns>Single company, which found by <c>key</c>. Contains more information than Company entity.</returns>
        public async Task<CompanyModel> GetSingleDataModelAsync(object key)
        {
            // gather companies, to have unblocking call when grouping
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
                            Storages = cs.Single().Select(i => new ValueTuple<string, int> { Item2 = i.Id, Item1 = i.Name }).ToList(),
                            Transactions = cst.Single().Select(i => new ValueTuple<string, int> { Item2 = i.Id, Item1 = i.Date.ToShortDateString() }).ToList()
                        };

            return query.Single();
        }

        /// <summary>
        /// Gather single company, which found by <c>key</c>.
        /// </summary>
        /// <param name="key">Id of company to be found</param>
        /// <returns>Single company, which found by <c>key</c>.</returns>
        public async Task<Company> GetSingleUpdateModelAsync(object key)
        {
            return await db.Company.SingleOrDefaultAsync(i => i.Id == (int)key);
        }

        /// <summary>
        /// Add new company.
        /// </summary>
        /// <param name="model">Company to be added</param>
        /// <returns>Added company with correct key(Id) value</returns>
        public async Task<Company> AddDataModelAsync(Company model)
        {
            model.Id = default;
            var entity = db.Add(model);
            await db.SaveChangesAsync();
            return entity.Entity;
        }

        /// <summary>
        /// Update company.
        /// </summary>
        /// <param name="model">Updated company information</param>
        public async Task UpdateDataModelAsync(Company model)
        {
            var currentModel = await db.Company.FirstOrDefaultAsync(i => i.Id == model.Id);
            db.Entry(currentModel).CurrentValues.SetValues(model);
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Delete company.
        /// </summary>
        /// <param name="key">Id of company to be deleted</param>
        public async Task RemoveDataModelAsync(object key)
        {
            var entity = await db.Company.FindAsync((int)key);
            db.Company.Remove(entity);
            await db.SaveChangesAsync();
        }
    }
}

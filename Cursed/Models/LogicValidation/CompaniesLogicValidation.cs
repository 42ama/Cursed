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

namespace Cursed.Models.LogicValidation
{
    public class CompaniesLogicValidation
    {
        private readonly CursedContext db;
        private readonly AbstractErrorHandlerFactory errorHandlerFactory;

        public CompaniesLogicValidation(CursedContext db)
        {
            this.db = db;
            errorHandlerFactory = new StatusMessageFactory();
        }

        public async Task<AbstractErrorHandler> CheckGetSingleDataModelAsync(object key)
        {
            return await CheckExists(key);
        }

        public async Task<AbstractErrorHandler> CheckGetSingleUpdateModelAsync(object key)
        {
            return await CheckExists(key);
        }

        public async Task<AbstractErrorHandler> CheckUpdateDataModelAsync(object key)
        {
            return await CheckExists(key);
        }

        public async Task<AbstractErrorHandler> CheckRemoveDataModelAsync(object key)
        {
            var statusMessage = await CheckExists(key);

            if(!statusMessage.IsCompleted)
            {
                return statusMessage;
            }

            // check related entities
            var storages = db.Storage.Where(i => i.CompanyId == (int)key);
            var transactions = db.TransactionBatch.Where(i => i.CompanyId == (int)key);

            if (storages.Any())
            {
                foreach (var storage in storages)
                {
                    statusMessage.Problems.Add(new Problem
                    {
                        Entity = "Storage.",
                        EntityKey = storage.Id,
                        Message = "Company have related storage."
                    });
                }
            }
            if (transactions.Any())
            {
                foreach (var transaction in transactions)
                {
                    statusMessage.Problems.Add(new Problem
                    {
                        Entity = "Transaction.",
                        EntityKey = transaction.Id,
                        Message = "Company have related transaction."
                    });
                }
            }

            return statusMessage;
        }
        private async Task<AbstractErrorHandler> CheckExists(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler("Company.", key);

            if (await db.Company.FirstOrDefaultAsync(i => i.Id == (int)key) == null)
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = "Company.",
                    EntityKey = key,
                    Message = "No company with such key found."
                });
            }

            return statusMessage;
        }
    }
}

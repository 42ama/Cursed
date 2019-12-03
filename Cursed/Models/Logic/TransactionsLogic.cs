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
            return null;
        }

        public async Task<TransactionModel> GetSingleDataModelAsync(object key)
        {
            return null;
        }

        public async Task<TransactionModel> GetSingleUpdateModelAsync(object key)
        {
            return null;
        }

        public async Task CloseTransactionAsync(object key)
        {

        }

        public async Task AddDataModelAsync(TransactionBatch model)
        {

        }

        public async Task UpdateDataModelAsync(TransactionBatch model)
        {

        }

        public async Task RemoveDataModelAsync(object key)
        {

        }
    }
}

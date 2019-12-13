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
using Cursed.Models.Services;
using Cursed.Models.Data.Utility;
using Cursed.Models.Data.Utility.ErrorHandling;
using Cursed.Models.Routing;
using Cursed.Models.Services;

namespace Cursed.Models.LogicValidation
{
    public class TransactionsLogicValidation
    {
        private readonly CursedContext db;
        private readonly IOperationDataValidation operationValidation;
        private readonly IErrorHandlerFactory errorHandlerFactory;

        public TransactionsLogicValidation(CursedContext db, IOperationDataValidation operationValidation, IErrorHandlerFactory errorHandlerFactory)
        {
            this.db = db;
            this.operationValidation = operationValidation;
            this.errorHandlerFactory = errorHandlerFactory;

        }

        public async Task<IErrorHandler> CheckGetSingleDataModelAsync(object key)
        {
            return await CheckExists(key);
        }

        public async Task<IErrorHandler> CheckGetSingleUpdateModelAsync(object key)
        {
            return await CheckExists(key);
        }

        public async Task<IErrorHandler> CheckUpdateDataModelAsync(object key)
        {
            var statusMessage = await CheckExists(key);

            if (!statusMessage.IsCompleted)
            {
                return statusMessage;
            }

            return await CheckClosed(key);
        }

        public async Task<IErrorHandler> CheckOpenTransactionAsync(object key)
        {
            var statusMessage = await CheckExists(key);

            if (!statusMessage.IsCompleted)
            {
                return statusMessage;
            }

            var lastTransaction = await db.TransactionBatch.OrderByDescending(i => i.Date).FirstAsync();
            if (lastTransaction.Id != (int)key)
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = "Go to last transaction.",
                    EntityKey = lastTransaction.Id,
                    Message = "Only last transaction can be open.",
                    RedirectRoute = TransactionsRouting.SingleItem
                });
            }

            return statusMessage;
        }

        public async Task<IErrorHandler> CheckRemoveDataModelAsync(object key)
        {
            var statusMessage = await CheckExists(key);

            if (!statusMessage.IsCompleted)
            {
                return statusMessage;
            }

            return await CheckClosed(key);
        }

        public async Task<IErrorHandler> CheckCloseTransactionAsync(object key)
        {
            var statusMessage = await CheckExists(key);

            if (!statusMessage.IsCompleted)
            {
                return statusMessage;
            }

            // operation validation
            var transaction = await db.TransactionBatch.SingleAsync(i => i.Id == (int)key);
            foreach (var operation in transaction.Operation)
            {
                var operationMessage = await operationValidation.IsValidAsync(operation);
                if (!operationMessage.IsCompleted)
                {
                    foreach (var problem in operationMessage?.Problems)
                    {
                        statusMessage.Problems.Add(problem);
                    }
                }
            }

            return statusMessage;
        }

        private async Task<IErrorHandler> CheckExists(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = "Transaction.",
                EntityKey = (int)key,
                RedirectRoute = TransactionsRouting.SingleItem
            });
            var transaction = await db.TransactionBatch.FirstOrDefaultAsync(i => i.Id == (int)key);
            if (transaction == null)
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = "Transaction.",
                    EntityKey = (int)key,
                    Message = "Transaction with this Id is not found.",
                    RedirectRoute = TransactionsRouting.Index,
                    UseKeyWithRoute = false
                });
            }
            
            return statusMessage;
        }

        private async Task<IErrorHandler> CheckClosed(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = "Transaction.",
                EntityKey = (int)key,
                RedirectRoute = TransactionsRouting.SingleItem
            });
            var transaction = await db.TransactionBatch.FirstOrDefaultAsync(i => i.Id == (int)key);

            if (!transaction.IsOpen)
            {
                statusMessage.Problems.Add(new Problem
                {
                    Entity = "Transaction open status.",
                    Message = "Can't change transaction, when it closed.",
                    RedirectRoute = TransactionsRouting.SingleItem,
                    EntityKey = (int)key
                });
            }

            return statusMessage;
        }
    }
}

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

namespace Cursed.Models.LogicValidation
{
    public class TransactionsLogicValidation
    {
        private readonly CursedContext db;
        private readonly IOperationValidation operationValidation;
        private readonly AbstractErrorHandlerFactory errorHandlerFactory;

        public TransactionsLogicValidation(CursedContext db, IOperationValidation operationValidation)
        {
            this.db = db;
            this.operationValidation = operationValidation;
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
            var statusMessage = await CheckExists(key);

            if (!statusMessage.IsCompleted)
            {
                return statusMessage;
            }

            return await CheckClosed(key);
        }

        public async Task<AbstractErrorHandler> CheckRemoveDataModelAsync(object key)
        {
            var statusMessage = await CheckExists(key);

            if (!statusMessage.IsCompleted)
            {
                return statusMessage;
            }

            return await CheckClosed(key);
        }

        public async Task<AbstractErrorHandler> CheckCloseTransactionAsync(object key)
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
                        statusMessage.Problems.Add(new Problem
                        {
                            Entity = operationMessage.Entity + " " + problem.Entity,
                            EntityKey = problem.EntityKey,
                            Message = problem.Message
                        });
                    }
                }
            }

            return statusMessage;
        }

        private async Task<AbstractErrorHandler> CheckExists(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler("Transaction.", key);
            var transaction = await db.TransactionBatch.FirstOrDefaultAsync(i => i.Id == (int)key);
            if (transaction == null)
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = "Transaction.",
                    EntityKey = key,
                    Message = "No transaction with such key found."
                });
            }
            
            return statusMessage;
        }

        private async Task<AbstractErrorHandler> CheckClosed(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler("Transaction.", key);
            var transaction = await db.TransactionBatch.FirstOrDefaultAsync(i => i.Id == (int)key);

            if (!transaction.IsOpen)
            {
                statusMessage.Problems.Add(new Problem
                {
                    Entity = "Transaction open status.",
                    Message = "Can't change transaction, when it closed."
                });
            }

            return statusMessage;
        }
    }
}

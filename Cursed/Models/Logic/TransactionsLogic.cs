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
using Cursed.Models.Data.Utility.ErrorHandling;
using Cursed.Models.Interfaces.LogicCRUD;

namespace Cursed.Models.Logic
{
    public class TransactionsLogic
    {
        private readonly CursedContext db;
        private readonly IOperationValidation operationValidation;
        private readonly AbstractErrorHandlerFactory errorHandlerFactory;
        public TransactionsLogic(CursedContext db, IOperationValidation operationValidation)
        {
            this.db = db;
            this.operationValidation = operationValidation;
            errorHandlerFactory = new StatusMessageFactory();
        }

        public async Task<AbstractErrorHandler<IEnumerable<TransactionsModel>>> GetAllDataModelAsync()
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler<IEnumerable<TransactionsModel>>("All transactions.");

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
            statusMessage.ReturnValue = query;

            return statusMessage;
        }

        public async Task<AbstractErrorHandler<TransactionModel>> GetSingleDataModelAsync(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler<TransactionModel>("Transaction.", key);

            var transactions = await db.TransactionBatch.Where(i => i.Id == (int)key).ToListAsync();
            var query = from t in transactions
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
            statusMessage.ReturnValue = query.Single();

            return statusMessage;
        }

        public async Task<AbstractErrorHandler<TransactionBatch>> GetSingleUpdateModelAsync(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler<TransactionBatch>("Transaction.", key);
            var transaction = await db.TransactionBatch.SingleAsync(i => i.Id == (int)key);
            statusMessage.ReturnValue = transaction;
            return statusMessage;
        }

        public async Task<AbstractErrorHandler> CloseTransactionAsync(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler("Transaction.", key);

            // operation validation
            var transaction = db.TransactionBatch.Single(i => i.Id == (int)key);
            foreach (var operation in transaction.Operation)
            {
                var operationMessage = await operationValidation.IsValidAsync(operation);
                if(!operationMessage.IsCompleted)
                {
                    foreach (var problem in operationMessage?.Problems)
                    {
                        statusMessage.Problems.Add(new Problem
                        {
                            Entity = operationMessage.Entity + problem.Entity,
                            Message = problem.Entity
                        });
                    }
                }
            }

            // applying each operation to db
            foreach(var operation in transaction.Operation)
            {
                var productFrom = await db.Product.FirstOrDefaultAsync(i => i.Uid == operation.ProductId && i.StorageId == operation.StorageFromId);
                var productTo = await db.Product.FirstOrDefaultAsync(i => i.Uid == operation.ProductId && i.StorageId == operation.StorageToId);

                if (productTo == null)
                {
                    db.Add(new Product
                    {
                        Uid = operation.ProductId,
                        Quantity = operation.Quantity,
                        QuantityUnit = "mg.",
                        Price = operation.Price,
                        StorageId = operation.StorageToId
                    });
                }
                else
                {
                    var updatedProductTo = productTo;
                    updatedProductTo.Quantity += operation.Quantity;
                    updatedProductTo.Price = ((productTo.Quantity * productTo.Price) + (operation.Quantity * operation.Price)) / 2;
                    db.Entry(productTo).CurrentValues.SetValues(updatedProductTo);
                }

                if(productFrom.Quantity == operation.Quantity)
                {
                    db.Remove(productFrom);
                }
                else
                {
                    var updatedProductFrom = productFrom;
                    updatedProductFrom.Quantity -= operation.Quantity;
                    db.Entry(productFrom).CurrentValues.SetValues(updatedProductFrom);
                }

                db.SaveChanges();
            }

            return statusMessage;
        }

        public async Task<AbstractErrorHandler> AddDataModelAsync(TransactionBatch model)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler("Transaction.");

            model.Id = default;
            db.Add(model);
            await db.SaveChangesAsync();

            return statusMessage;
        }

        public async Task<AbstractErrorHandler> UpdateDataModelAsync(TransactionBatch model)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler("Transaction.", model.Id);

            var currentModel = await db.TransactionBatch.FirstOrDefaultAsync(i => i.Id == model.Id);
            if(!currentModel.IsOpen)
            {
                statusMessage.Problems.Add(new Problem
                {
                    Entity = "Transaction open status.",
                    Message = "Can't update transaction, when it closed."
                });
            }
            db.Entry(currentModel).CurrentValues.SetValues(model);
            await db.SaveChangesAsync();

            return statusMessage;
        }

        public async Task<AbstractErrorHandler> RemoveDataModelAsync(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler("Transaction.", key);

            int transactionId = (int)key;

            var entity = await db.TransactionBatch.FindAsync(transactionId);
            if (!entity.IsOpen)
            {
                statusMessage.Problems.Add(new Problem
                {
                    Entity = "Transaction open status.",
                    Message = "Can't delete transaction, when it closed."
                });
            }

            db.Operation.RemoveRange(db.Operation.Where(i => i.TransactionId == transactionId));
            db.TransactionBatch.Remove(entity);

            await db.SaveChangesAsync();

            return statusMessage;
        }
    }
}

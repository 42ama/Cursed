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
using Cursed.Models.Data.Transactions;
using Cursed.Models.Entities;
using Cursed.Models.Data.Shared;
using Cursed.Models.Data.Utility;
using Cursed.Models.Services;
using Cursed.Models.Data.Utility.ErrorHandling;
using Cursed.Models.Interfaces.LogicCRUD;

namespace Cursed.Models.Logic
{
    public class TransactionsLogic : IReadColection<TransactionsModel>, IReadSingle<TransactionModel>, IReadUpdateForm<TransactionBatch>, ICUD<TransactionBatch>
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

            return query.Single();
        }


        public async Task<TransactionBatch> GetSingleUpdateModelAsync(object key)
        {
            return await db.TransactionBatch.SingleAsync(i => i.Id == (int)key); 
        }

        public async Task CloseTransactionAsync(object key)
        {
            var transaction = db.TransactionBatch.Single(i => i.Id == (int)key);

            // applying each operation to db
            foreach(var operation in transaction.Operation)
            {
                var productFrom = await db.Product.FirstOrDefaultAsync(i => i.Uid == operation.ProductId && i.StorageId == operation.StorageFromId);
                var productTo = await db.Product.FirstOrDefaultAsync(i => i.Uid == operation.ProductId && i.StorageId == operation.StorageToId);

                await IncreaseOrAddProduct(productTo, operation, operation.StorageToId);
                await DecreaseOrRemoveProduct(productFrom, operation); 
            }
        }

        public async Task OpenTransactionAsync(object key)
        {
            var transaction = db.TransactionBatch.Single(i => i.Id == (int)key);

            // applying each operation to db
            foreach (var operation in transaction.Operation)
            {
                var productFrom = await db.Product.FirstOrDefaultAsync(i => i.Uid == operation.ProductId && i.StorageId == operation.StorageFromId);
                var productTo = await db.Product.FirstOrDefaultAsync(i => i.Uid == operation.ProductId && i.StorageId == operation.StorageToId);

                await IncreaseOrAddProduct(productFrom, operation, operation.StorageFromId);
                await DecreaseOrRemoveProduct(productTo, operation);
            }
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
            db.Entry(currentModel).CurrentValues.SetValues(model);
            await db.SaveChangesAsync();
        }

        public async Task RemoveDataModelAsync(object key)
        {
            int transactionId = (int)key;
            var entity = await db.TransactionBatch.FindAsync(transactionId);
            db.Operation.RemoveRange(db.Operation.Where(i => i.TransactionId == transactionId));
            db.TransactionBatch.Remove(entity);
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Add product to db or increase it's quantity
        /// </summary>
        /// <param name="product">Product to be added</param>
        /// <param name="operation">Operation in which product is added</param>
        /// <param name="storageId">Storage Id at which product is stored</param>
        private async Task IncreaseOrAddProduct(Product product, Operation operation, int storageId)
        {
            if (product == null)
            {
                db.Add(new Product
                {
                    Uid = operation.ProductId,
                    Quantity = operation.Quantity,
                    QuantityUnit = "mg.",
                    Price = operation.Price,
                    StorageId = storageId
                });
            }
            else
            {
                var updatedProductTo = product;
                updatedProductTo.Quantity += operation.Quantity;
                updatedProductTo.Price = ((product.Quantity * product.Price) + (operation.Quantity * operation.Price)) / 2;
                db.Entry(product).CurrentValues.SetValues(updatedProductTo);
            }

            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Remove product from db or decrease it's quantity
        /// </summary>
        /// <param name="product">Product to be removed</param>
        /// <param name="operation">Operation in which product is removed</param>
        private async Task DecreaseOrRemoveProduct(Product product, Operation operation)
        {
            if (product.Quantity == operation.Quantity)
            {
                db.Remove(product);
            }
            else
            {
                var updatedProduct = product;
                updatedProduct.Quantity -= operation.Quantity;
                db.Entry(product).CurrentValues.SetValues(updatedProduct);
            }

            await db.SaveChangesAsync();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Context;
using Cursed.Models.DataModel.Transactions;
using Cursed.Models.Entities.Data;
using Cursed.Models.Interfaces.LogicCRUD;

namespace Cursed.Models.Logic
{
    /// <summary>
    /// Transactions section logic. Consists of CRUD actions for transactions, including gathering methods for
    /// both single transaction and collection of all transactions.
    /// </summary>
    public class TransactionsLogic : IReadColection<TransactionsModel>, IReadSingle<TransactionModel>, IReadUpdateForm<TransactionBatch>, ICUD<TransactionBatch>
    {
        private readonly CursedDataContext db;

        public TransactionsLogic(CursedDataContext db)
        {
            this.db = db;

        }

        /// <summary>
        /// Gather all transactions from database.
        /// </summary>
        /// <returns>All transactions from database. Each transaction contains more information than TransactionBatch entity.</returns>
        public async Task<IEnumerable<TransactionsModel>> GetAllDataModelAsync()
        {
            // gather transactions, to have unblocking call when grouping
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

        /// <summary>
        /// Gather single transaction, which found by <c>key</c>. Also contains related Operations data.
        /// </summary>
        /// <param name="key">Id of transaction to be found</param>
        /// <returns>Single transaction, which found by <c>key</c>. Contains more information than TransactionBatch entity.</returns>
        public async Task<TransactionModel> GetSingleDataModelAsync(object key)
        {
            // gather realated to transaction operations
            var operationsQuery = from o in db.Operation
                                  where o.TransactionId == (int)key
                                  join pc in db.ProductCatalog on o.ProductId equals pc.Id into productsCatalog
                                  from prodsCat in productsCatalog
                                  join s in db.Storage on o.StorageFromId equals s.Id into storagesFrom
                                  from storsF in storagesFrom
                                  join s in db.Storage on o.StorageToId equals s.Id into storagesTo
                                  from storsT in storagesTo
                                  select new OperationModel
                                  {
                                      Id = o.Id,
                                      Price = o.Price,
                                      ProductId = o.ProductId,
                                      Quantity = o.Quantity,
                                      StorageFromId = o.StorageFromId,
                                      StorageToId = o.StorageToId,
                                      TransactionId = o.TransactionId,
                                      CAS = prodsCat.Cas,
                                      ProductName = prodsCat.Name,
                                      StorageFromName = storsF.Name,
                                      StorageToName = storsT.Name
                                  };

            // aggregate data to final model
            var query = from t in db.TransactionBatch
                        where t.Id == (int)key
                        join c in db.Company on t.CompanyId equals c.Id into companies
                        from comp in companies
                        select new TransactionModel
                        {
                            Id = t.Id,
                            Date = t.Date,
                            Type = t.Type,
                            Comment = t.Comment,
                            IsOpen = t.IsOpen,
                            CompanyId = t.CompanyId,
                            CompanyName = comp.Name,
                            Operations = operationsQuery.ToList()
                        };

            return query.Single();
        }

        /// <summary>
        /// Gather single transaction, which found by <c>key</c>.
        /// </summary>
        /// <param name="key">Id of transaction to be found</param>
        /// <returns>Single transaction, which found by <c>key</c>.</returns>
        public async Task<TransactionBatch> GetSingleUpdateModelAsync(object key)
        {
            return await db.TransactionBatch.SingleAsync(i => i.Id == (int)key); 
        }

        /// <summary>
        /// Closes transaction. Which mean apply all operations record in it to database.
        /// </summary>
        /// <param name="key">Id of transaction to be found</param>
        public async Task CloseTransactionAsync(object key)
        {
            await ChangeTransactionOpenStateAsync((int)key);
        }

        /// <summary>
        /// Opend transaction. Which mean undo all operations record in it to database.
        /// </summary>
        /// <param name="key">Id of transaction to be found</param>
        public async Task OpenTransactionAsync(object key)
        {
            await ChangeTransactionOpenStateAsync((int)key);
        }

        /// <summary>
        /// Applys/undos all operations recorded in specific transaction. Is applying done or undoing, is
        /// determined by transaction status (closed/open)
        /// </summary>
        /// <param name="id">Id of transaction to be found</param>
        private async Task ChangeTransactionOpenStateAsync(int id)
        {
            // gather current transaction information
            var transaction = db.TransactionBatch.Single(i => i.Id == id);

            // apply/undo each operation in current tansaction
            foreach (var operation in await db.Operation.Where(i => i.TransactionId == id).ToListAsync())
            {
                // product, which quantity will be increased
                Product productInc;
                // product, which quantity will be decrased
                Product productDec;
                // id of storage at which product quantity will be increased
                int storageIdForInc;

                // initialized delcared variables, depending on transaction status
                if (transaction.IsOpen)
                {
                    // if we applying operations, products come from From to To storage
                    productDec = await db.Product.FirstOrDefaultAsync(i => i.Uid == operation.ProductId && i.StorageId == operation.StorageFromId);
                    productInc = await db.Product.FirstOrDefaultAsync(i => i.Uid == operation.ProductId && i.StorageId == operation.StorageToId);

                    storageIdForInc = operation.StorageToId;
                }
                else
                {
                    // if we undoing operations, product come from To to From storage
                    productInc = await db.Product.FirstOrDefaultAsync(i => i.Uid == operation.ProductId && i.StorageId == operation.StorageFromId);
                    productDec = await db.Product.FirstOrDefaultAsync(i => i.Uid == operation.ProductId && i.StorageId == operation.StorageToId);

                    storageIdForInc = operation.StorageFromId;
                }
                
                IncreaseOrAddProduct(productInc, operation, storageIdForInc);
                DecreaseOrRemoveProduct(productDec, operation);

                await db.SaveChangesAsync();
            }

            // set transaction last updated date to current
            transaction.Date = DateTime.UtcNow;
            // and reverse open status
            transaction.IsOpen = !transaction.IsOpen;

            await UpdateDataModelAsync(transaction);
        }

        /// <summary>
        /// Add new transaction.
        /// </summary>
        /// <param name="model">Transaction to be added</param>
        /// <returns>Added transaction with correct key(Id) value</returns>
        public async Task<TransactionBatch> AddDataModelAsync(TransactionBatch model)
        {
            model.Id = default;
            var entity = db.Add(model);
            await db.SaveChangesAsync();
            return entity.Entity;
        }

        /// <summary>
        /// Update transaction.
        /// </summary>
        /// <param name="model">Updated transaction information</param>
        public async Task UpdateDataModelAsync(TransactionBatch model)
        {
            var currentModel = await db.TransactionBatch.FirstOrDefaultAsync(i => i.Id == model.Id);
            db.Entry(currentModel).CurrentValues.SetValues(model);
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Delete transaction.
        /// </summary>
        /// <param name="key">Id of transaction to be deleted</param>
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
        private void IncreaseOrAddProduct(Product product, Operation operation, int storageId)
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
                // recalculate product quantity and price
                var updatedProductTo = product;
                updatedProductTo.Quantity += operation.Quantity;

                // prepare percents for calculate average
                var originalProductQuantityPercent = operation.Quantity / (operation.Quantity + updatedProductTo.Quantity);
                var updatedProductQuantityPercent = updatedProductTo.Quantity / (operation.Quantity + updatedProductTo.Quantity);

                // record average to price field
                updatedProductTo.Price = (updatedProductQuantityPercent * product.Price) + (originalProductQuantityPercent * operation.Price);

                db.Entry(product).CurrentValues.SetValues(updatedProductTo);
            }
        }

        /// <summary>
        /// Remove product from db or decrease it's quantity
        /// </summary>
        /// <param name="product">Product to be removed</param>
        /// <param name="operation">Operation in which product is removed</param>
        private void DecreaseOrRemoveProduct(Product product, Operation operation)
        {
            if (product.Quantity == operation.Quantity)
            {
                db.Remove(product);
            }
            else
            {
                // recalculate product quantity
                var updatedProduct = product;
                updatedProduct.Quantity -= operation.Quantity;
                db.Entry(product).CurrentValues.SetValues(updatedProduct);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Cursed.Models.Entities;
using Cursed.Models.Context;
using Cursed.Models.Data.Utility.ErrorHandling;
using Cursed.Models.Routing;

namespace Cursed.Models.Services
{
    public class OperationValidation : IOperationValidation
    {
        private readonly CursedContext db;
        public OperationValidation(CursedContext context)
        {
            db = context;
        }

        public async Task<IErrorHandler> IsValidAsync(Operation operation)
        {
            IErrorHandler statusMessage = new StatusMessage
            {
                ProblemStatus = new Problem
                {
                    Entity = $"Operation.",
                    EntityKey = operation.Id,
                    RedirectRoute = OperationsRouting.SingleItem
                }
            };
            var product = await db.Product.SingleOrDefaultAsync(i => i.Uid == operation.ProductId && i.StorageId == operation.StorageFromId);
            var storageFrom = await db.Storage.SingleOrDefaultAsync(i => i.Id == operation.StorageFromId);
            var storageTo = await db.Storage.SingleOrDefaultAsync(i => i.Id == operation.StorageToId);
            var transaction = await db.TransactionBatch.SingleOrDefaultAsync(i => i.Id == operation.TransactionId);


            if(product == null)
            {
                statusMessage.Problems.Add(new Problem
                {
                    Entity = $"Product at storage from.",
                    EntityKey = operation.ProductId,
                    Message = "Product isn't found.",
                    RedirectRoute = ProductsCatalogRouting.Index,
                    UseKeyWithRoute = false
                });
            }

            if (storageFrom == null)
            {
                statusMessage.Problems.Add(new Problem
                {
                    Entity = $"Storage from product coming.",
                    EntityKey = operation.StorageFromId,
                    Message = "Storage isn't found.",
                    RedirectRoute = StoragesRouting.Index,
                    UseKeyWithRoute = false
                });
            }

            if (storageTo == null)
            {
                statusMessage.Problems.Add(new Problem
                {
                    Entity = $"Storage to product coming.",
                    EntityKey = operation.StorageToId,
                    Message = "Storage isn't found.",
                    RedirectRoute = StoragesRouting.Index,
                    UseKeyWithRoute = false
                });
            }

            if (transaction == null)
            {
                statusMessage.Problems.Add(new Problem
                {
                    Entity = $"Transaction.",
                    EntityKey = operation.TransactionId,
                    Message = "Transaction isn't found.",
                    RedirectRoute = TransactionsRouting.Index,
                    UseKeyWithRoute = false
                });
            }

            if(product != null && storageFrom != null)
            {
                if (product.Quantity < operation.Quantity)
                {
                    statusMessage.Problems.Add(new Problem
                    {
                        Entity = "Product at storage from.",
                        EntityKey = product.Id,
                        Message = $"Quantity of product at storage from ({product.Quantity}) is lesser, then " +
                        $"operation is trying to withdraw ({operation.Quantity}).",
                        RedirectRoute = ProductsRouting.SingleItem
                    });
                }
            }
            

            return statusMessage;
        }
    }
}

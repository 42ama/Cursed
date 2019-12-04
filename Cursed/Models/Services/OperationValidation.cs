﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Cursed.Models.Entities;
using Cursed.Models.Context;
using Cursed.Models.Data.Utility.ErrorHandling;

namespace Cursed.Models.Services
{
    public class OperationValidation : IOperationValidation
    {
        private readonly CursedContext db;
        public OperationValidation(CursedContext context)
        {
            db = context;
        }

        public async Task<StatusMessage> IsValidAsync(Operation operation)
        {
            var statusMessage = new StatusMessage
            {
                Entity = $"Operation. Id: {operation.Id}",
                EntityKey = operation.Id
            };
            var product = await db.Product.SingleOrDefaultAsync(i => i.Uid == operation.ProductId && i.StorageId == operation.StorageFromId);
            var storageFrom = await db.Storage.SingleOrDefaultAsync(i => i.Id == operation.StorageFromId);
            var storageTo = await db.Storage.SingleOrDefaultAsync(i => i.Id == operation.StorageToId);
            var transaction = await db.TransactionBatch.SingleOrDefaultAsync(i => i.Id == operation.TransactionId);


            if(product == null)
            {
                statusMessage.Problems.Add(new Problem
                {
                    Entity = $"Product at storage from. Id: {operation.ProductId}",
                    EntityKey = operation.ProductId,
                    Message = "Product isn't found."
                });
            }

            if (storageFrom == null)
            {
                statusMessage.Problems.Add(new Problem
                {
                    Entity = $"Storage from product coming. Id: {operation.StorageFromId}",
                    EntityKey = operation.StorageFromId,
                    Message = "Storage isn't found."
                });
            }

            if (storageTo == null)
            {
                statusMessage.Problems.Add(new Problem
                {
                    Entity = $"Storage to product coming. Id: {operation.StorageToId}",
                    EntityKey = operation.StorageToId,
                    Message = "Storage isn't found."
                });
            }

            if (transaction == null)
            {
                statusMessage.Problems.Add(new Problem
                {
                    Entity = $"Transaction. Id: {operation.TransactionId}",
                    EntityKey = operation.TransactionId,
                    Message = "Transaction isn't found."
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
                        $"operation is trying to withdraw ({operation.Quantity})."
                    });
                }
            }
            

            return statusMessage;
        }
    }
}

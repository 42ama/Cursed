using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Cursed.Models.Entities;
using Cursed.Models.Context;

namespace Cursed.Models.Services
{
    public class OperationValidation : IOperationValidation
    {
        private readonly CursedContext db;
        public OperationValidation(CursedContext context)
        {
            db = context;
        }

        public async Task<bool> IsValidAsync(Operation operation)
        {
            await db.Product.FirstAsync(i => i.Uid == operation.ProductId);
            await db.Storage.SingleAsync(i => i.Id == operation.StorageFromId);
            await db.Storage.SingleAsync(i => i.Id == operation.StorageToId);
            await db.TransactionBatch.SingleAsync(i => i.Id == operation.TransactionId);

            var proudctAtStorageFrom = await db.Product.SingleAsync(i => i.Uid == operation.ProductId && i.StorageId == operation.StorageFromId);
            if(proudctAtStorageFrom.Quantity >= operation.Quantity)
            {
                return true;
            }
            return false;
        }
    }
}

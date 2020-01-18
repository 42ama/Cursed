using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Cursed.Models.Entities.Data;
using Cursed.Models.Context;
using Cursed.Models.DataModel.ErrorHandling;
using Cursed.Models.StaticReferences.Routing;

namespace Cursed.Models.Services
{
    /// <summary>
    /// Provides means to validatie Operation
    /// </summary>
    public class OperationDataValidation : IOperationDataValidation
    {
        private readonly CursedDataContext db;
        public OperationDataValidation(CursedDataContext context)
        {
            db = context;
        }

        /// <summary>
        /// Validate <c>operation</c>
        /// </summary>
        /// <param name="operation">Operation which will be validated</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> IsValidAsync(Operation operation)
        {
            IErrorHandler statusMessage = new StatusMessage
            {
                ProblemStatus = new Problem
                {
                    Entity = "Operation.",
                    EntityKey = operation.Id.ToString(),
                    RedirectRoute = OperationsRouting.SingleItem
                }
            };

            // all related to operation entities must exist
            var product = await db.Product.SingleOrDefaultAsync(i => i.Uid == operation.ProductId && i.StorageId == operation.StorageFromId);
            var storageFrom = await db.Storage.SingleOrDefaultAsync(i => i.Id == operation.StorageFromId);
            var storageTo = await db.Storage.SingleOrDefaultAsync(i => i.Id == operation.StorageToId);
            var transaction = await db.TransactionBatch.SingleOrDefaultAsync(i => i.Id == operation.TransactionId);


            if(product == null)
            {
                statusMessage.Problems.Add(new Problem
                {
                    Entity = "Product at storage from.",
                    EntityKey = operation.ProductId.ToString(),
                    Message = "Product with this Id is not found.",
                    RedirectRoute = ProductsCatalogRouting.Index,
                    UseKeyWithRoute = false
                });
            }

            if (storageFrom == null)
            {
                statusMessage.Problems.Add(new Problem
                {
                    Entity = "Storage from product coming.",
                    EntityKey = operation.StorageFromId.ToString(),
                    Message = "Storage with this Id is not found.",
                    RedirectRoute = StoragesRouting.Index,
                    UseKeyWithRoute = false
                });
            }

            if (storageTo == null)
            {
                statusMessage.Problems.Add(new Problem
                {
                    Entity = "Storage to product coming.",
                    EntityKey = operation.StorageToId.ToString(),
                    Message = "Storage with this Id is not found.",
                    RedirectRoute = StoragesRouting.Index,
                    UseKeyWithRoute = false
                });
            }

            if (transaction == null)
            {
                statusMessage.Problems.Add(new Problem
                {
                    Entity = "Transaction.",
                    EntityKey = operation.TransactionId.ToString(),
                    Message = "Transaction with this Id is not found.",
                    RedirectRoute = TransactionsRouting.Index,
                    UseKeyWithRoute = false
                });
            }

            // quantity of product at storage from must be greater or equal to
            // quantity of product at storage to
            if(product != null && storageFrom != null)
            {
                if (product.Quantity < operation.Quantity)
                {
                    statusMessage.Problems.Add(new Problem
                    {
                        Entity = "Product at storage from.",
                        EntityKey = product.Id.ToString(),
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

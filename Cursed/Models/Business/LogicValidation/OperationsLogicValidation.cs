using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Context;
using Cursed.Models.Services;
using Cursed.Models.DataModel.ErrorHandling;
using Cursed.Models.StaticReferences.Routing;
using Cursed.Models.Entities.Data;

namespace Cursed.Models.LogicValidation
{
    /// <summary>
    /// Operations section logic validation. Contains of methods used to validate operations actions
    /// in specific situations.
    /// </summary>
    public class OperationsLogicValidation
    {
        private readonly CursedDataContext db;
        private readonly IErrorHandlerFactory errorHandlerFactory;

        public OperationsLogicValidation(CursedDataContext db, IErrorHandlerFactory errorHandlerFactory)
        {
            this.db = db;
            this.errorHandlerFactory = errorHandlerFactory;

        }

        /// <summary>
        /// Checks if operation is valid, to be gatherd for update
        /// </summary>
        /// <param name="key">Id of operation to be found</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckGetSingleUpdateModelAsync(object key)
        {
            return await CheckExistsAndTransactionOpen(key);
        }

        /// <summary>
        /// Checks if operation is valid, to be updated
        /// </summary>
        /// <param name="key">Id of operation to be found</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckUpdateDataModelAsync(Operation model)
        {
            var statusMessage = await CheckExistsAndTransactionOpen(model.Id);
            return await CheckRelatedEntitiesExists(model, statusMessage);
        }

        /// <summary>
        /// Checks if operation is valid, to be added
        /// </summary>
        /// <param name="model">Operation model to be validated</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckAddDataModelAsync(Operation model)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = "Operation in a transaction.",
                EntityKey = "",
                RedirectRoute = TransactionsRouting.Index,
                UseKeyWithRoute = false
            });

            return await CheckRelatedEntitiesExists(model, statusMessage);
        }

        /// <summary>
        /// Checks if operation is valid, to be removed
        /// </summary>
        /// <param name="key">Id of operation to be found</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckRemoveDataModelAsync(object key)
        {
            return await CheckExistsAndTransactionOpen(key);
        }

        /// <summary>
        /// Checks if related entities exitst
        /// </summary>
        /// <param name="model">Operation model with key properties of related entities</param>
        /// <param name="statusMessage">Error handler to which problem will be added</param>
        /// <returns>Status message with validaton information</returns>
        private async Task<IErrorHandler> CheckRelatedEntitiesExists(Operation model, IErrorHandler statusMessage)
        {
            // check if related product from catalog exists
            if (await db.ProductCatalog.FirstOrDefaultAsync(i => i.Id == model.ProductId) == null)
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = "Products from catalog.",
                    EntityKey = (model.ProductId).ToString(),
                    Message = "Product with this Id isn't found",
                    RedirectRoute = ProductsCatalogRouting.Index,
                    UseKeyWithRoute = false
                });
            }

            // check if related transaction exists
            if (await db.TransactionBatch.FirstOrDefaultAsync(i => i.Id == model.TransactionId) == null)
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = "Transaction.",
                    EntityKey = (model.TransactionId).ToString(),
                    Message = "Transaction with this Id isn't found",
                    RedirectRoute = TransactionsRouting.Index,
                    UseKeyWithRoute = false
                });
            }

            // check if related transaction exists
            if (await db.Storage.FirstOrDefaultAsync(i => i.Id == model.StorageFromId) == null)
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = "Storage.",
                    EntityKey = (model.StorageFromId).ToString(),
                    Message = "Storage with this Id isn't found",
                    RedirectRoute = StoragesRouting.Index,
                    UseKeyWithRoute = false
                });
            }

            // check if related transaction exists
            if (await db.Storage.FirstOrDefaultAsync(i => i.Id == model.StorageToId) == null)
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = "Storage.",
                    EntityKey = (model.StorageToId).ToString(),
                    Message = "Storage with this Id isn't found",
                    RedirectRoute = StoragesRouting.Index,
                    UseKeyWithRoute = false
                });
            }


            return statusMessage;
        }

        /// <summary>
        /// Checks if operation exists and transaction is open
        /// </summary>
        /// <param name="key">Id of operation to be found</param>
        /// <returns>Status message with validaton information</returns>
        private async Task<IErrorHandler> CheckExistsAndTransactionOpen(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = "Operation.",
                EntityKey = ((int)key).ToString(),
                RedirectRoute = TransactionsRouting.Index,
                UseKeyWithRoute = false
            });

            // check if operation exists
            var opertaion = await db.Operation.FirstOrDefaultAsync(i => i.Id == (int)key);
            if (opertaion == null)
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = "Operation.",
                    EntityKey = ((int)key).ToString(),
                    Message = "Operation with this Id is not found.",
                    RedirectRoute = TransactionsRouting.Index,
                    UseKeyWithRoute = false
                });

                return statusMessage;
            }

            // check if transaction is open
            var transaction = await db.TransactionBatch.FirstOrDefaultAsync(i => i.Id == opertaion.TransactionId);
            if (!transaction.IsOpen)
            {
                statusMessage.Problems.Add(new Problem
                {
                    Entity = "Transaction close status.",
                    Message = "Can't change operation when related transaction is closed.",
                    RedirectRoute = TransactionsRouting.SingleItem,
                    EntityKey = transaction.Id.ToString()
                });
            }

            return statusMessage;
        }
    }
}

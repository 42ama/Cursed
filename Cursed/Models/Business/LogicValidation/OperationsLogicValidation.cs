using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Context;
using Cursed.Models.Services;
using Cursed.Models.DataModel.ErrorHandling;
using Cursed.Models.StaticReferences.Routing;

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
        public async Task<IErrorHandler> CheckUpdateDataModelAsync(object key)
        {
            return await CheckExistsAndTransactionOpen(key);
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

using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Context;
using Cursed.Models.Services;
using Cursed.Models.DataModel.ErrorHandling;
using Cursed.Models.StaticReferences.Routing;
using Cursed.Models.Entities.Data;

namespace Cursed.Models.LogicValidation
{
    /// <summary>
    /// Transactions section logic validation. Contains of methods used to validate transactions actions
    /// in specific situations.
    /// </summary>
    public class TransactionsLogicValidation
    {
        private readonly CursedDataContext db;
        private readonly IOperationDataValidation operationValidation;
        private readonly IErrorHandlerFactory errorHandlerFactory;

        public TransactionsLogicValidation(CursedDataContext db, IOperationDataValidation operationValidation, IErrorHandlerFactory errorHandlerFactory)
        {
            this.db = db;
            this.operationValidation = operationValidation;
            this.errorHandlerFactory = errorHandlerFactory;
        }

        /// <summary>
        /// Checks if transaction is valid, to be gathered
        /// </summary>
        /// <param name="key">Id of transaction to be found</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckGetSingleDataModelAsync(object key)
        {
            return await CheckExists(key);
        }

        /// <summary>
        /// Checks if transaction is valid, to be gathered for update
        /// </summary>
        /// <param name="key">Id of transaction to be found</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckGetSingleUpdateModelAsync(object key)
        {
            return await CheckExists(key);
        }

        /// <summary>
        /// Checks if transaction is valid, to be added
        /// </summary>
        /// <param name="model">Transaction model to be validated</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckAddDataModelAsync(TransactionBatch model)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = "Transaction.",
                EntityKey = "",
                RedirectRoute = TransactionsRouting.Index,
                UseKeyWithRoute = false
            });

            return await CheckRelatedEntitiesExists(model, statusMessage);
        }

        /// <summary>
        /// Checks if transaction is valid, to be updated
        /// </summary>
        /// <param name="model">Transaction model to be validated</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckUpdateDataModelAsync(TransactionBatch model)
        {
            var statusMessage = await CheckExists(model.Id);

            if (!statusMessage.IsCompleted)
            {
                return statusMessage;
            }

            statusMessage = await CheckExists(model.Id);

            if (!statusMessage.IsCompleted)
            {
                return statusMessage;
            }

            return await CheckRelatedEntitiesExists(model, statusMessage);
        }

        /// <summary>
        /// Checks if transaction is open
        /// </summary>
        /// <param name="key">Id of transaction to be found</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckOpenTransactionAsync(object key)
        {
            var statusMessage = await CheckExists(key);

            if (!statusMessage.IsCompleted)
            {
                return statusMessage;
            }

            // check if this last closed transaction (only last closed transaction can be opened)
            var lastTransaction = await db.TransactionBatch.OrderByDescending(i => i.Date).FirstAsync();
            if (lastTransaction.Id != (int)key)
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = "Go to last transaction.",
                    EntityKey = lastTransaction.Id.ToString(),
                    Message = "Only last transaction can be open.",
                    RedirectRoute = TransactionsRouting.SingleItem
                });
            }

            return statusMessage;
        }

        /// <summary>
        /// Checks if transaction is valid, to be removed
        /// </summary>
        /// <param name="key">Id of transaction to be found</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckRemoveDataModelAsync(object key)
        {
            var statusMessage = await CheckExists(key);

            if (!statusMessage.IsCompleted)
            {
                return statusMessage;
            }

            return await CheckClosed(key);
        }

        /// <summary>
        /// Checks if transaction is valid to close
        /// </summary>
        /// <param name="key">Id of transaction to be found</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckCloseTransactionAsync(object key)
        {
            var statusMessage = await CheckExists(key);

            if (!statusMessage.IsCompleted)
            {
                return statusMessage;
            }

            // check if all related operations are valid
            var transaction = await db.TransactionBatch.SingleAsync(i => i.Id == (int)key);
            foreach (var operation in transaction.Operation)
            {
                var operationMessage = await operationValidation.IsValidAsync(operation);
                if (!operationMessage.IsCompleted)
                {
                    foreach (var problem in operationMessage?.Problems)
                    {
                        statusMessage.Problems.Add(problem);
                    }
                }
            }

            return statusMessage;
        }

        /// <summary>
        /// Checks if related entities exitst
        /// </summary>
        /// <param name="model">Transaction model with key properties of related entities</param>
        /// <param name="statusMessage">Error handler to which problem will be added</param>
        /// <returns>Status message with validaton information</returns>
        private async Task<IErrorHandler> CheckRelatedEntitiesExists(TransactionBatch model, IErrorHandler statusMessage)
        {
            // check if related company exists
            if (await db.Company.FirstOrDefaultAsync(i => i.Id == model.CompanyId) == null)
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = "Company.",
                    EntityKey = (model.CompanyId).ToString(),
                    Message = "Company with this Id isn't found",
                    RedirectRoute = CompaniesRouting.Index,
                    UseKeyWithRoute = false
                });
            }
            return statusMessage;
        }

        /// <summary>
        /// Checks if transaction exists
        /// </summary>
        /// <param name="key">Id of transaction to be found</param>
        /// <returns>Status message with validaton information</returns>
        private async Task<IErrorHandler> CheckExists(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = "Transaction.",
                EntityKey = ((int)key).ToString(),
                RedirectRoute = TransactionsRouting.SingleItem
            });

            // check if transaction exists
            var transaction = await db.TransactionBatch.FirstOrDefaultAsync(i => i.Id == (int)key);
            if (transaction == null)
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = "Transaction.",
                    EntityKey = ((int)key).ToString(),
                    Message = "Transaction with this Id is not found.",
                    RedirectRoute = TransactionsRouting.Index,
                    UseKeyWithRoute = false
                });
            }
            
            return statusMessage;
        }


        /// <summary>
        /// Checks if transaction is closed
        /// </summary>
        /// <param name="key">Id of transaction to be found</param>
        /// <returns>Status message with validaton information</returns>
        private async Task<IErrorHandler> CheckClosed(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = "Transaction.",
                EntityKey = ((int)key).ToString(),
                RedirectRoute = TransactionsRouting.SingleItem
            });
            var transaction = await db.TransactionBatch.FirstOrDefaultAsync(i => i.Id == (int)key);

            // check if transaction is closed
            if (!transaction.IsOpen)
            {
                statusMessage.Problems.Add(new Problem
                {
                    Entity = "Transaction open status.",
                    Message = "Can't change transaction, when it closed.",
                    RedirectRoute = TransactionsRouting.SingleItem,
                    EntityKey = ((int)key).ToString()
                });
            }

            return statusMessage;
        }
    }
}

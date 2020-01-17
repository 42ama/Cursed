using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Context;
using Cursed.Models.Services;
using Cursed.Models.DataModel.ErrorHandling;
using Cursed.Models.StaticReferences.Routing;

namespace Cursed.Models.LogicValidation
{
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

        public async Task<IErrorHandler> CheckGetSingleDataModelAsync(object key)
        {
            return await CheckExists(key);
        }

        public async Task<IErrorHandler> CheckGetSingleUpdateModelAsync(object key)
        {
            return await CheckExists(key);
        }

        public async Task<IErrorHandler> CheckUpdateDataModelAsync(object key)
        {
            var statusMessage = await CheckExists(key);

            if (!statusMessage.IsCompleted)
            {
                return statusMessage;
            }

            return await CheckClosed(key);
        }

        public async Task<IErrorHandler> CheckOpenTransactionAsync(object key)
        {
            var statusMessage = await CheckExists(key);

            if (!statusMessage.IsCompleted)
            {
                return statusMessage;
            }

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

        public async Task<IErrorHandler> CheckRemoveDataModelAsync(object key)
        {
            var statusMessage = await CheckExists(key);

            if (!statusMessage.IsCompleted)
            {
                return statusMessage;
            }

            return await CheckClosed(key);
        }

        public async Task<IErrorHandler> CheckCloseTransactionAsync(object key)
        {
            var statusMessage = await CheckExists(key);

            if (!statusMessage.IsCompleted)
            {
                return statusMessage;
            }

            // operation validation
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

        private async Task<IErrorHandler> CheckExists(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = "Transaction.",
                EntityKey = ((int)key).ToString(),
                RedirectRoute = TransactionsRouting.SingleItem
            });
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

        private async Task<IErrorHandler> CheckClosed(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = "Transaction.",
                EntityKey = ((int)key).ToString(),
                RedirectRoute = TransactionsRouting.SingleItem
            });
            var transaction = await db.TransactionBatch.FirstOrDefaultAsync(i => i.Id == (int)key);

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

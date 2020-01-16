using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Context;
using Cursed.Models.Services;
using Cursed.Models.DataModel.ErrorHandling;
using Cursed.Models.StaticReferences.Routing;

namespace Cursed.Models.LogicValidation
{
    public class OperationsLogicValidation
    {
        private readonly CursedDataContext db;
        private readonly IErrorHandlerFactory errorHandlerFactory;

        public OperationsLogicValidation(CursedDataContext db, IErrorHandlerFactory errorHandlerFactory)
        {
            this.db = db;
            this.errorHandlerFactory = errorHandlerFactory;

        }

        public async Task<IErrorHandler> CheckGetSingleUpdateModelAsync(object key)
        {
            return await CheckExistsAndTransactionOpen(key);
        }

        public async Task<IErrorHandler> CheckUpdateDataModelAsync(object key)
        {
            return await CheckExistsAndTransactionOpen(key);
        }

        public async Task<IErrorHandler> CheckRemoveDataModelAsync(object key)
        {
            return await CheckExistsAndTransactionOpen(key);
        }

        private async Task<IErrorHandler> CheckExistsAndTransactionOpen(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = "Operation.",
                EntityKey = ((int)key).ToString(),
                RedirectRoute = TransactionsRouting.Index,
                UseKeyWithRoute = false
            });

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

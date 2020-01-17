using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Context;
using Cursed.Models.StaticReferences.Routing;
using Cursed.Models.DataModel.ErrorHandling;
using Cursed.Models.Services;

namespace Cursed.Models.LogicValidation
{
    public class CompaniesLogicValidation
    {
        private readonly CursedDataContext db;
        private readonly IErrorHandlerFactory errorHandlerFactory;

        public CompaniesLogicValidation(CursedDataContext db, IErrorHandlerFactory errorHandlerFactory)
        {
            this.db = db;
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
            return await CheckExists(key);
        }

        public async Task<IErrorHandler> CheckRemoveDataModelAsync(object key)
        {
            var statusMessage = await CheckExists(key);

            if(!statusMessage.IsCompleted)
            {
                return statusMessage;
            }

            // check related entities
            var storages = db.Storage.Where(i => i.CompanyId == (int)key);
            var transactions = db.TransactionBatch.Where(i => i.CompanyId == (int)key);

            if (storages.Any())
            {
                foreach (var storage in storages)
                {
                    statusMessage.Problems.Add(new Problem
                    {
                        Entity = "Storage.",
                        EntityKey = storage.Id.ToString(),
                        Message = "You must remove dependent Storage first.",
                        RedirectRoute = StoragesRouting.SingleItem
                    });
                }
            }
            if (transactions.Any())
            {
                foreach (var transaction in transactions)
                {
                    statusMessage.Problems.Add(new Problem
                    {
                        Entity = "Transaction.",
                        EntityKey = transaction.Id.ToString(),
                        Message = "You must remove dependent Transaction first.",
                        RedirectRoute = TransactionsRouting.SingleItem
                    });
                }
            }

            return statusMessage;
        }
        private async Task<IErrorHandler> CheckExists(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
                {
                    Entity = "Company.",
                    EntityKey = ((int)key).ToString(),
                    RedirectRoute = CompaniesRouting.SingleItem
                });

            if (await db.Company.FirstOrDefaultAsync(i => i.Id == (int)key) == null)
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = "Company.",
                    EntityKey = ((int)key).ToString(),
                    Message = "Company with this Id is not found.",
                    RedirectRoute = CompaniesRouting.Index,
                    UseKeyWithRoute = false
                });
            }

            return statusMessage;
        }
    }
}

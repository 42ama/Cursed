using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Context;
using Cursed.Models.DataModel.ErrorHandling;
using Cursed.Models.StaticReferences.Routing;
using Cursed.Models.Services;
using Cursed.Models.Entities.Data;

namespace Cursed.Models.LogicValidation
{
    /// <summary>
    /// Storages section logic validation. Contains of methods used to validate storages actions
    /// in specific situations.
    /// </summary>
    public class StoragesLogicValidation
    {
        private readonly CursedDataContext db;
        private readonly IErrorHandlerFactory errorHandlerFactory;

        public StoragesLogicValidation(CursedDataContext db, IErrorHandlerFactory errorHandlerFactory)
        {
            this.db = db;
            this.errorHandlerFactory = errorHandlerFactory;
        }

        /// <summary>
        /// Checks if storage is valid, to be gathered
        /// </summary>
        /// <param name="key">Id of storage to be found</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckGetSingleDataModelAsync(object key)
        {
            return await CheckExists(key);
        }


        /// <summary>
        /// Checks if storage is valid, to be gathered for update
        /// </summary>
        /// <param name="key">Id of storage to be found</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckGetSingleUpdateModelAsync(object key)
        {
            return await CheckExists(key);
        }

        /// <summary>
        /// Checks if storage is valid, to be updated
        /// </summary>
        /// <param name="model">Storage model to be validated</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckUpdateDataModelAsync(Storage model)
        {
            var statusMessage = await CheckExists(model.Id);

            return await CheckRelatedEntitiesExists(model, statusMessage);
        }

        /// <summary>
        /// Checks if storage is valid, to be added
        /// </summary>
        /// <param name="model">Storage model to be validated</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckAddDataModelAsync(Storage model)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = "Storage.",
                EntityKey = "",
                RedirectRoute = StoragesRouting.Index,
                UseKeyWithRoute = false
            });

            return await CheckRelatedEntitiesExists(model, statusMessage);
        }

        /// <summary>
        /// Checks if storage is valid, to be removed
        /// </summary>
        /// <param name="key">Id of storage to be found</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckRemoveDataModelAsync(object key)
        {
            var statusMessage = await CheckExists(key);

            if (!statusMessage.IsCompleted)
            {
                return statusMessage;
            }

            // there must be no related entites to delete storage
            var products = db.Product.Where(i => i.StorageId == (int)key);
            var operations = db.Operation.Where(i => i.StorageToId == (int)key || i.StorageFromId == (int)key);

            if (products.Any())
            {
                foreach (var product in products)
                {
                    statusMessage.Problems.Add(new Problem
                    {
                        Entity = "Product.",
                        EntityKey = product.Id.ToString(),
                        Message = "You must remove dependent Product in Storage first.",
                        RedirectRoute = ProductsRouting.Index,
                        UseKeyWithRoute = false
                    });
                }
            }
            if (operations.Any())
            {
                foreach (var operation in operations)
                {
                    statusMessage.Problems.Add(new Problem
                    {
                        Entity = "Operation.",
                        EntityKey = operation.Id.ToString(),
                        Message = "You must remove dependent Operation first.",
                        RedirectRoute = OperationsRouting.SingleItem
                    });
                }
            }

            return statusMessage;
        }

        /// <summary>
        /// Checks if related entities exitst
        /// </summary>
        /// <param name="model">Storage model with key properties of related entities</param>
        /// <param name="statusMessage">Error handler to which problem will be added</param>
        /// <returns>Status message with validaton information</returns>
        private async Task<IErrorHandler> CheckRelatedEntitiesExists(Storage model, IErrorHandler statusMessage)
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
        /// Checks if storage exists
        /// </summary>
        /// <param name="key">Id of storage to be found</param>
        /// <returns>Status message with validaton information</returns>
        private async Task<IErrorHandler> CheckExists(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = "Storage.",
                EntityKey = ((int)key).ToString(),
                RedirectRoute = StoragesRouting.SingleItem
            });

            // checl if storage exists
            if (await db.Storage.FirstOrDefaultAsync(i => i.Id == (int)key) == null)
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = "Storage.",
                    EntityKey = ((int)key).ToString(),
                    Message = "Storage with this Id is not found.",
                    RedirectRoute = StoragesRouting.Index,
                    UseKeyWithRoute = false
                });
            }

            return statusMessage;
        }
    }
}

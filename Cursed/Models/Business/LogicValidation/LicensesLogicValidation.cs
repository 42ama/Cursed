using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Context;
using Cursed.Models.DataModel.ErrorHandling;
using Cursed.Models.StaticReferences.Routing;
using Cursed.Models.Services;
using Cursed.Models.Entities.Data;

namespace Cursed.Models.LogicValidation
{
    /// <summary>
    /// Licenses section logic validation. Contains of methods used to validate licenses actions
    /// in specific situations.
    /// </summary>
    public class LicensesLogicValidation
    {
        private readonly CursedDataContext db;
        private readonly IErrorHandlerFactory errorHandlerFactory;

        public LicensesLogicValidation(CursedDataContext db, IErrorHandlerFactory errorHandlerFactory)
        {
            this.db = db;
            this.errorHandlerFactory = errorHandlerFactory;
        }

        /// <summary>
        /// Checks if license is valid, to be gathered
        /// </summary>
        /// <param name="key">Id of license to be found</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckGetSingleDataModelAsync(object key)
        {
            return await CheckExists(key);
        }

        /// <summary>
        /// Checks if license is valid, to be gathered for update
        /// </summary>
        /// <param name="key">Id of license to be found</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckGetSingleUpdateModelAsync(object key)
        {
            return await CheckExists(key);
        }

        /// <summary>
        /// Checks if license is valid, to be updated
        /// </summary>
        /// <param name="model">License model to be validated</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckUpdateDataModelAsync(License model)
        {
            var statusMessage = await CheckExists(model.Id);
            return await CheckRelatedEntitiesExists(model, statusMessage);
        }

        /// <summary>
        /// Checks if license is valid, to be added
        /// </summary>
        /// <param name="model">License model to be validated</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckAddDataModelAsync(License model)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = "License.",
                EntityKey = "",
                RedirectRoute = LicensesRouting.Index,
                UseKeyWithRoute = false
            });

            return await CheckRelatedEntitiesExists(model, statusMessage);
        }

        /// <summary>
        /// Checks if license is valid, to be removed
        /// </summary>
        /// <param name="key">Id of license to be found</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckRemoveDataModelAsync(object key)
        {
            return await CheckExists(key);
        }

        /// <summary>
        /// Checks if related entities exitst
        /// </summary>
        /// <param name="model">License model with key properties of related entities</param>
        /// <param name="statusMessage">Error handler to which problem will be added</param>
        /// <returns>Status message with validaton information</returns>
        private async Task<IErrorHandler> CheckRelatedEntitiesExists(License model, IErrorHandler statusMessage)
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
            return statusMessage;
        }

        /// <summary>
        /// Checks if license exists
        /// </summary>
        /// <param name="key">Id of license to be found</param>
        /// <returns>Status message with validaton information</returns>
        private async Task<IErrorHandler> CheckExists(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = "License.",
                EntityKey = ((int)key).ToString(),
                RedirectRoute = LicensesRouting.SingleItem
            });

            // check if license exists
            if (await db.License.FirstOrDefaultAsync(i => i.Id == (int)key) == null)
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = "License.",
                    EntityKey = ((int)key).ToString(),
                    Message = "License with this Id is not found.",
                    RedirectRoute = LicensesRouting.Index,
                    UseKeyWithRoute = false
                });
            }

            return statusMessage;
        }
    }
}

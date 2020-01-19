using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Context;
using Cursed.Models.DataModel.ErrorHandling;
using Cursed.Models.StaticReferences.Routing;
using Cursed.Models.Services;

namespace Cursed.Models.LogicValidation
{
    /// <summary>
    /// Facilities section logic validation. Contains of methods used to validate facility actions
    /// in specific situations.
    /// </summary>
    public class FacilitiesLogicValidation
    {
        private readonly CursedDataContext db;
        private readonly IErrorHandlerFactory errorHandlerFactory;

        public FacilitiesLogicValidation(CursedDataContext db, IErrorHandlerFactory errorHandlerFactory)
        {
            this.db = db;
            this.errorHandlerFactory = errorHandlerFactory;
        }

        /// <summary>
        /// Checks if facility is valid, to be gathered
        /// </summary>
        /// <param name="key">Id of facility to be found</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckGetSingleDataModelAsync(object key)
        {
            return await CheckExists(key);
        }

        /// <summary>
        /// Checks if facility is valid, to be gathered for update
        /// </summary>
        /// <param name="key">Id of facility to be found</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckGetSingleUpdateModelAsync(object key)
        {
            return await CheckExists(key);
        }

        /// <summary>
        /// Checks if facility is valid, to be updated
        /// </summary>
        /// <param name="key">Id of facility to be found</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckUpdateDataModelAsync(object key)
        {
            return await CheckExists(key);
        }

        /// <summary>
        /// Checks if facility is valid, to be removed
        /// </summary>
        /// <param name="key">Id of facility to be found</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckRemoveDataModelAsync(object key)
        {
            var statusMessage = await CheckExists(key);

            if (!statusMessage.IsCompleted)
            {
                return statusMessage;
            }

            // there must be no related entites to delete facility
            var techProcesses = db.TechProcess.Where(i => i.FacilityId == (int)key);

            if (techProcesses.Any())
            {
                foreach (var techProcess in techProcesses)
                {
                    statusMessage.Problems.Add(new Problem
                    {
                        Entity = $"Technological process. Recipe Id: {techProcess.RecipeId}.",
                        EntityKey = techProcess.FacilityId.ToString(),
                        Message = "You must remove dependent Technological Process first.",
                        RedirectRoute = FacilityTechProcessesRouting.Index
                    });
                }
            }

            return statusMessage;
        }

        /// <summary>
        /// Checks if facility exists
        /// </summary>
        /// <param name="key">Id of facility to be found</param>
        /// <returns>Status message with validaton information</returns>
        private async Task<IErrorHandler> CheckExists(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = "Facility.",
                EntityKey = ((int)key).ToString(),
                RedirectRoute = FacilitiesRouting.SingleItem
            });

            // check if facility exists
            if (await db.Facility.FirstOrDefaultAsync(i => i.Id == (int)key) == null)
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = "Facility.",
                    EntityKey = ((int)key).ToString(),
                    Message = "Facility with this Id is not found.",
                    RedirectRoute = FacilitiesRouting.Index,
                    UseKeyWithRoute = false
                });
            }

            return statusMessage;
        }
    }
}

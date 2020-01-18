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
    /// Recipes section logic validation. Contains of methods used to validate recipes actions
    /// in specific situations.
    /// </summary>
    public class RecipesLogicValidation
    {
        private readonly CursedDataContext db;
        private readonly IErrorHandlerFactory errorHandlerFactory;

        public RecipesLogicValidation(CursedDataContext db, IErrorHandlerFactory errorHandlerFactory)
        {
            this.db = db;
            this.errorHandlerFactory = errorHandlerFactory;
        }

        /// <summary>
        /// Checks if recipe is valid, to be gathered
        /// </summary>
        /// <param name="key">Id of recipe to be found</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckGetSingleDataModelAsync(object key)
        {
            return await CheckExists(key);
        }

        /// <summary>
        /// Checks if recipe is valid, to be gathered for update
        /// </summary>
        /// <param name="key">Id of recipe to be found</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckGetSingleUpdateModelAsync(object key)
        {
            return await CheckExists(key);
        }

        /// <summary>
        /// Checks if recipe is valid, to be updated
        /// </summary>
        /// <param name="key">Id of recipe to be found</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckUpdateDataModelAsync(object key)
        {
            return await CheckExists(key);
        }

        /// <summary>
        /// Checks if recipe is valid, to be removed
        /// </summary>
        /// <param name="key">Id of recipe to be found</param>
        /// <returns>Status message with validaton information</returns>
        public async Task<IErrorHandler> CheckRemoveDataModelAsync(object key)
        {
            var statusMessage = await CheckExists(key);

            if (!statusMessage.IsCompleted)
            {
                return statusMessage;
            }

            // there must be no related entites to delete recipe
            var childs = db.RecipeInheritance.Where(i => i.ParentId == (int)key);
            var techProcesses = db.TechProcess.Where(i => i.RecipeId == (int)key);
            var productsChanges = db.RecipeProductChanges.Where(i => i.RecipeId == (int)key);

            if (childs.Any())
            {
                foreach (var child in childs)
                {
                    statusMessage.Problems.Add(new Problem
                    {
                        Entity = "Recipe.",
                        EntityKey = child.ChildId.ToString(),
                        Message = "You must remove dependent Recipe Childs first.",
                        RedirectRoute = RecipesRouting.SingleItem
                    });
                }
            }
            if (techProcesses.Any())
            {
                foreach (var techProcess in techProcesses)
                {
                    statusMessage.Problems.Add(new Problem
                    {
                        Entity = "Technological process.",
                        EntityKey = techProcess.FacilityId.ToString(),
                        Message = "You must remove dependent Technological Process first.",
                        RedirectRoute = FacilityTechProcessesRouting.Index
                    });
                }
            }
            if (productsChanges.Any())
            {
                foreach (var productChanges in productsChanges)
                {
                    statusMessage.Problems.Add(new Problem
                    {
                        Entity = $"Product changes. Product Id: {productChanges.ProductId}.",
                        EntityKey = productChanges.RecipeId.ToString(),
                        Message = "You must remove dependent Product Changes in Recipe first.",
                        RedirectRoute = RecipeProductsRouting.Index
                    });
                }
            }

            return statusMessage;
        }

        /// <summary>
        /// Checks if recipe exists
        /// </summary>
        /// <param name="key">Id of recipe to be found</param>
        /// <returns>Status message with validaton information</returns>
        private async Task<IErrorHandler> CheckExists(object key)
        {
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = "Recipe.",
                EntityKey = ((int)key).ToString(),
                RedirectRoute = RecipesRouting.SingleItem
            });

            // check if recipe exists
            if (await db.Recipe.FirstOrDefaultAsync(i => i.Id == (int)key) == null)
            {
                statusMessage.AddProblem(new Problem
                {
                    Entity = "Recipe.",
                    EntityKey = ((int)key).ToString(),
                    Message = "Recipe with this Id is not found.",
                    RedirectRoute = RecipesRouting.Index,
                    UseKeyWithRoute = false
                });
            }

            return statusMessage;
        }
    }
}

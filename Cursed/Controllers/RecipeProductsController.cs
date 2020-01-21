using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cursed.Models.DataModel.RecipeProducts;
using Cursed.Models.Context;
using Cursed.Models.Entities.Data;
using Cursed.Models.Logic;
using Cursed.Models.DataModel.Pagenation;
using Cursed.Models.Interfaces.ControllerCRUD;
using Cursed.Models.StaticReferences.Routing;
using Cursed.Models.LogicValidation;
using Cursed.Models.Services;
using Cursed.Models.DataModel.Authorization;

namespace Cursed.Controllers
{
    /// <summary>
    /// Tech processes section controller. Consists of CRUD actions for tech processes.
    /// </summary>
    [Route("recipes/products")]
    public class RecipeProductsController : Controller, IReadCollectionByParam, ICreate<RecipeProductChanges>, IUpdate<RecipeProductChanges>, IDeleteByModel<RecipeProductChanges>
    {
        private readonly RecipeProductsLogic logic;
        private readonly RecipeProductsLogicValidation logicValidation;
        private readonly ILogProvider<CursedAuthenticationContext> logProvider;
        public RecipeProductsController(CursedDataContext db, 
            [FromServices] IErrorHandlerFactory errorHandlerFactory,
            [FromServices] ILogProvider<CursedAuthenticationContext> logProvider)
        {
            logic = new RecipeProductsLogic(db);
            logicValidation = new RecipeProductsLogicValidation(db, errorHandlerFactory);
            this.logProvider = logProvider;
        }

        /// <summary>
        /// Main page of section, contains consolidated collection of products in recipe. 
        /// Can be navigated through pagenation.
        /// </summary>
        /// <param name="key">Id of recipe to which product recipe relations belongs</param>
        /// <param name="currentPage">Defines which portion of items from collection, will be shown</param>
        /// <param name="itemsOnPage">Defines how many item there will be in a portion</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpGet("", Name = RecipeProductsRouting.Index)]
        public async Task<IActionResult> Index(string key, int currentPage = 1, int itemsOnPage = 20)
        {
            int recipeId = Int32.Parse(key);
            var addedProducts = await logic.GetAllDataModelAsync(recipeId);
            // collect products already used in recipe
            var ignoreProducts = addedProducts.Select(i => i.ProductId).Distinct();
            // and collect all other products from catalog, which can be added
            var notAddedProducts = logic.GetProductsFromCatalog(ignoreProducts);
            // id of recipe in which these products used
            ViewData["RecipeId"] = recipeId;
            // form pagenation model, with added collection of unsed products from catalog
            var model = new CollectionPlusPagenation<RecipeProductsDataModel, ProductCatalog>
            {
                Collection = addedProducts,
                Pagenation = new Pagenation<ProductCatalog>(notAddedProducts, itemsOnPage, currentPage)
            };
            return View(model);
        }

        /// <summary>
        /// Post action to add new recipe product relation.
        /// </summary>
        /// <param name="model">Recipe product relation to be added</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpPost("add", Name = RecipeProductsRouting.AddSingleItem)]
        public async Task<IActionResult> AddSingleItem(RecipeProductChanges model)
        {
            var recipeProductChanges = await logic.AddDataModelAsync(model);
            var statusMessage = logicValidation.ValidateModel(ModelState);
            if (statusMessage.IsCompleted)
            {
                await logProvider.AddToLogAsync($"Added new recipe product relations (Recipe Id: {recipeProductChanges.RecipeId}; Product Id: {recipeProductChanges.ProductId}).");
                return RedirectToRoute(RecipeProductsRouting.Index, new { key = model.RecipeId });
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        /// <summary>
        /// Post action to update recipe product relation.
        /// </summary>
        /// <param name="model">Updated recipe product relation information</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpPost("edit", Name = RecipeProductsRouting.EditSingleItem)]
        public async Task<IActionResult> EditSingleItem(RecipeProductChanges model)
        {
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync((model.RecipeId, model.ProductId));
            statusMessage = logicValidation.ValidateModel(statusMessage, ModelState);
            if (statusMessage.IsCompleted)
            {
                await logic.UpdateDataModelAsync(model);
                await logProvider.AddToLogAsync($"Updated recipe product relation information (Recipe Id: {model.RecipeId}; Product Id: {model.ProductId}).");
                return RedirectToRoute(RecipeProductsRouting.Index, new { key = model.RecipeId });
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        /// <summary>
        /// Post action to delete recipe product relation.
        /// </summary>
        /// <param name="model">Model of recipe product relation containing key information (ProductId and RecipeId) to find recipe product changes</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpPost("delete", Name = RecipeProductsRouting.DeleteSingleItem)]
        public async Task<IActionResult> DeleteSingleItem(RecipeProductChanges model)
        {
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync((model.RecipeId, model.ProductId));
            if (statusMessage.IsCompleted)
            {
                await logic.RemoveDataModelAsync(model);
                await logProvider.AddToLogAsync($"Removed recipe product relation (Recipe Id: {model.RecipeId}; Product Id: {model.ProductId}).");
                return RedirectToRoute(RecipeProductsRouting.Index, new { key = model.RecipeId });
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }
    }
}
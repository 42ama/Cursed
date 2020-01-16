using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cursed.Models.DataModel.RecipeProducts;
using Cursed.Models.Context;
using Cursed.Models.Entities.Data;
using Cursed.Models.Logic;
using Cursed.Models.DataModel.Pagenation;
using Cursed.Models.Interfaces.ControllerCRUD;
using Cursed.Models.StaticReferences.Routing;
using Cursed.Models.LogicValidation;
using Cursed.Models.Services;
using Cursed.Models.DataModel.Utility.Authorization;

namespace Cursed.Controllers
{
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

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpGet("", Name = RecipeProductsRouting.Index)]
        public async Task<IActionResult> Index(string key, int currentPage = 1, int itemsOnPage = 20)
        {
            int recipeId = Int32.Parse(key);
            var addedProducts = await logic.GetAllDataModelAsync(recipeId);
            var ignoreProducts = addedProducts.Select(i => i.ProductId).Distinct();
            var notAddedProducts = logic.GetProductsFromCatalog(ignoreProducts);
            ViewData["RecipeId"] = recipeId;
            var model = new CollectionPlusPagenation<RecipeProductsDataModel, ProductCatalog>
            {
                Collection = addedProducts,
                Pagenation = new Pagenation<ProductCatalog>(notAddedProducts, itemsOnPage, currentPage)
            };
            return View(model);
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpPost("add", Name = RecipeProductsRouting.AddSingleItem)]
        public async Task<IActionResult> AddSingleItem(RecipeProductChanges model)
        {
            var recipeProductChanges = await logic.AddDataModelAsync(model);
            await logProvider.AddToLogAsync($"Added new recipe product relations (Recipe Id: {recipeProductChanges.RecipeId}; Product Id: {recipeProductChanges.ProductId}).");
            return RedirectToRoute(RecipeProductsRouting.Index, new { key = model.RecipeId });
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpPost("edit", Name = RecipeProductsRouting.EditSingleItem)]
        public async Task<IActionResult> EditSingleItem(RecipeProductChanges model)
        {
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync((model.RecipeId, model.ProductId));
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
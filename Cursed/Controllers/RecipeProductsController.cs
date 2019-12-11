using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cursed.Models.Data.RecipeProducts;
using Cursed.Models.Context;
using Cursed.Models.Entities;
using Cursed.Models.Logic;
using Cursed.Models.Data.Utility;
using Cursed.Models.Interfaces.ControllerCRUD;
using Cursed.Models.Routing;
using Cursed.Models.LogicValidation;
using Cursed.Models.Services;

namespace Cursed.Controllers
{
    [Route("recipes/products")]
    public class RecipeProductsController : Controller, IReadCollectionByParam, ICreate<RecipeProductChanges>, IUpdate<RecipeProductChanges>, IDeleteByModel<RecipeProductChanges>
    {
        private readonly RecipeProductsLogic logic;
        private readonly RecipeProductsLogicValidation logicValidation;
        public RecipeProductsController(CursedContext db, [FromServices] IErrorHandlerFactory errorHandlerFactory)
        {
            logic = new RecipeProductsLogic(db);
            logicValidation = new RecipeProductsLogicValidation(db, errorHandlerFactory);
        }

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

        [HttpPost("add", Name = RecipeProductsRouting.AddSingleItem)]
        public async Task<IActionResult> AddSingleItem(RecipeProductChanges model)
        {
            await logic.AddDataModelAsync(model);
            return RedirectToRoute(RecipeProductsRouting.Index, new { key = model.RecipeId });
        }

        [HttpPost("edit", Name = RecipeProductsRouting.EditSingleItem)]
        public async Task<IActionResult> EditSingleItem(RecipeProductChanges model)
        {
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync((model.RecipeId, model.ProductId));
            if (statusMessage.IsCompleted)
            {
                await logic.UpdateDataModelAsync(model);
                return RedirectToRoute(RecipeProductsRouting.Index, new { key = model.RecipeId });
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        [HttpPost("delete", Name = RecipeProductsRouting.DeleteSingleItem)]
        public async Task<IActionResult> DeleteSingleItem(RecipeProductChanges model)
        {
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync((model.RecipeId, model.ProductId));
            if (statusMessage.IsCompleted)
            {
                await logic.RemoveDataModelAsync(model);
                return RedirectToRoute(RecipeProductsRouting.Index, new { key = model.RecipeId });
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }
    }
}
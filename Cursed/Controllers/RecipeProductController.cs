﻿using System;
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
using Cursed.Models;

namespace Cursed.Controllers
{
    [Route("recipes/products")]
    public class RecipeProductController : Controller
    {
        private readonly RecipeProductsLogic logic;
        public RecipeProductController(CursedContext db)
        {
            logic = new RecipeProductsLogic(db);
        }

        [HttpGet("", Name = "RecipeProducts")]
        public async Task<IActionResult> Index(int recipeId, int currentPage = 1, int itemsOnPage = 20)
        {
            var addedProducts = await logic.GetAllDataModelAsync(recipeId);
            var ignoreProducts = addedProducts.Select(i => i.ProductId).Distinct();
            var notAddedProducts = await logic.GetProductsFromCatalog(ignoreProducts);
            ViewData["RecipeId"] = recipeId;
            var model = new CollectionPlusPagenation<RecipeProductsDataModel, ProductCatalog>
            {
                Collection = addedProducts,
                Pagenation = new Pagenation<ProductCatalog>(notAddedProducts, itemsOnPage, currentPage)
            };
            return View(model);
        }

        [HttpPost("add", Name = "AddRecipeProduct")]
        public async Task<IActionResult> Add(int recipeId, int productId, string type, decimal quantity)
        {
            await logic.AddToRecipeProduct(new RecipeProductChanges
            {
                RecipeId = recipeId,
                ProductId = productId,
                Type = type,
                Quantity = quantity
            });
            return RedirectToRoute("RecipeProducts", new { recipeId = recipeId });
        }
    }
}
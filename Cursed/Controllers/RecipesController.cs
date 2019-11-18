using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cursed.Models.Data.Recipes;
using Cursed.Models.Data.Shared;
using Cursed.Models.Context;
using Cursed.Models.Entities;
using Cursed.Models.Logic;
using Cursed.Models;
using Cursed.Models.Data.Utility;
using Cursed.Models.Routing;

namespace Cursed.Controllers
{
    [Route("recipes")]
    public class RecipesController : Controller, IControllerRESTAsync<Recipe>
    {
        private readonly RecipeLogic logic;
        public RecipesController(CursedContext db)
        {
            logic = new RecipeLogic(db);
        }

        [HttpGet("", Name = RecipesRouting.Index)]
        public async Task<IActionResult> Index(int currentPage = 1, int itemsOnPage = 20)
        {
            var model = await logic.GetAllDataModelAsync();
            var pagenationModel = new Pagenation<RecipesModel>(model, itemsOnPage, currentPage);
            return View(pagenationModel);
        }

        [HttpGet("recipe", Name = RecipesRouting.SingleItem)]
        public async Task<IActionResult> SingleItem(string key)
        {
            int id = Int32.Parse(key);
            return View(await logic.GetSingleDataModelAsync(id));
        }

        [HttpGet("recipe/edit", Name = RecipesRouting.GetEditSingleItem)]
        public async Task<IActionResult> GetEditSingleItem(string key)
        {
            if (key != null)
            {
                int id = Int32.Parse(key);
                ViewData["SaveRoute"] = RecipesRouting.EditSingleItem;
                var model = await logic.GetSingleUpdateModelAsync(id);
                return View("EditSingleItem", model);
            }
            else
            {
                ViewData["SaveRoute"] = RecipesRouting.AddSingleItem;
                return View("EditSingleItem");
            }
        }

        [HttpPost("recipe/add", Name = RecipesRouting.AddSingleItem)]
        public async Task<IActionResult> AddSingleItem(Recipe model)
        {
            await logic.AddDataModelAsync(model);
            return RedirectToRoute(RecipesRouting.Index);
        }

        [HttpPost("recipe/edit", Name = RecipesRouting.EditSingleItem)]
        public async Task<IActionResult> EditSingleItem(Recipe model)
        {
            await logic.UpdateDataModelAsync(model);
            return RedirectToRoute(RecipesRouting.Index);
        }

        [HttpPost("recipe/delete", Name = RecipesRouting.DeleteSingleItem)]
        public async Task<IActionResult> DeleteSingleItem(string key)
        {
            int id = Int32.Parse(key);
            await logic.RemoveDataModelAsync(id);
            return RedirectToRoute(RecipesRouting.Index);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cursed.Models.Data.Recipe;
using Cursed.Models.Data.Shared;
using Cursed.Models.Context;
using Cursed.Models.Entities;
using Cursed.Models.Logic;
using Cursed.Models;
using Cursed.Models.Data.Utility;

namespace Cursed.Controllers
{
    [Route("recipes")]
    public class RecipeController : Controller, IControllerRESTAsync<Recipe>
    {
        private readonly RecipeLogic logic;
        public RecipeController(CursedContext db)
        {
            logic = new RecipeLogic(db);
        }

        [HttpGet("", Name = "RecipeAll")]
        public async Task<IActionResult> Index(int currentPage = 1, int itemsOnPage = 20)
        {
            var model = await logic.GetAllDataModelAsync();
            var pagenationModel = new Pagenation<RecipeDM>(model, itemsOnPage, currentPage);
            return View(pagenationModel);
        }

        [HttpGet("recipe", Name = "RecipeSingle")]
        public async Task<IActionResult> SingleItem(int id)
        {
            return View(await logic.GetSingleDataModelAsync(id));
        }

        [HttpGet("recipe/edit", Name = "GetForEditRecipeSingle")]
        public async Task<IActionResult> GetEditSingleItem(int? id)
        {
            if (id.HasValue)
            {
                ViewData["SaveRoute"] = "EditRecipeSingle";
                var model = await logic.GetSingleUpdateModelAsync(id);
                return View("EditSingleItem", model);
            }
            else
            {
                ViewData["SaveRoute"] = "AddRecipeSingle";
                return View("EditSingleItem");
            }
        }

        [HttpPost("recipe/add", Name = "AddRecipeSingle")]
        public async Task<IActionResult> AddSingleItem(Recipe model)
        {
            await logic.AddDataModelAsync(model);
            return RedirectToRoute("LicensesAll");
        }

        [HttpPost("recipe/edit", Name = "EditRecipeSingle")]
        public async Task<IActionResult> EditSingleItem(Recipe model)
        {
            await logic.UpdateDataModelAsync(model);
            return RedirectToRoute("LicensesAll");
        }

        [HttpPost("recipe/delete", Name = "DeleteRecipeSingle")]
        public async Task<IActionResult> DeleteSingleItem(int id)
        {
            await logic.RemoveDataModelAsync(new Recipe { Id = id });
            return RedirectToRoute("LicensesAll");
        }
    }
}
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
                var model = await logic.GetSingleUpdateModelAsync(id);
                return View("EditSingleItem", model);
            }
            else
            {
                return View("EditSingleItem");
            }
        }

        [HttpPost("recipe", Name = "RecipeSingle")]
        public async Task<IActionResult> AddSingleItem(Recipe model)
        {
            return RedirectToRoute("LicensesAll");
        }

        [HttpPut("recipe", Name = "RecipeSingle")]
        public async Task<IActionResult> EditSingleItem(Recipe model)
        {
            return RedirectToRoute("LicensesAll");
        }

        [HttpDelete("recipe", Name = "RecipeSingle")]
        public async Task<IActionResult> DeleteSingleItem(int id)
        {
            return RedirectToRoute("LicensesAll");
        }
    }
}
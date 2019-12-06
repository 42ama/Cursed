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
using Cursed.Models.Interfaces.ControllerCRUD;
using Cursed.Models.Data.Utility;
using Cursed.Models.Routing;

namespace Cursed.Controllers
{
    [Route("recipes")]
    public class RecipesController : Controller, ICUD<Recipe>, IReadColection, IReadSingle, IReadUpdateForm
    {
        private readonly RecipesLogic logic;
        public RecipesController(CursedContext db)
        {
            logic = new RecipesLogic(db);
        }

        [HttpGet("", Name = RecipesRouting.Index)]
        public async Task<IActionResult> Index(int currentPage = 1, int itemsOnPage = 20)
        {
            var statusMessage = await logic.GetAllDataModelAsync();
            if(statusMessage.IsCompleted)
            {
                var pagenationModel = new Pagenation<RecipesModel>(statusMessage.ReturnValue, itemsOnPage, currentPage);
                return View(pagenationModel);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        [HttpGet("recipe", Name = RecipesRouting.SingleItem)]
        public async Task<IActionResult> SingleItem(string key)
        {
            int id = Int32.Parse(key);
            var statusMessage = await logic.GetSingleDataModelAsync(id);
            if (statusMessage.IsCompleted)
            {
                return View(statusMessage.ReturnValue);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
            
        }

        [HttpGet("recipe/edit", Name = RecipesRouting.GetEditSingleItem)]
        public async Task<IActionResult> GetEditSingleItem(string key)
        {
            if (key != null)
            {
                int id = Int32.Parse(key);
                ViewData["SaveRoute"] = RecipesRouting.EditSingleItem;
                var statusMessage = await logic.GetSingleUpdateModelAsync(id);
                if (statusMessage.IsCompleted)
                {
                    return View("EditSingleItem", statusMessage.ReturnValue);
                }
                else
                {
                    return View("CustomError", statusMessage);
                }
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
            var statusMessage = await logic.AddDataModelAsync(model);
            if(statusMessage.IsCompleted)
            {
                return RedirectToRoute(RecipesRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        [HttpPost("recipe/edit", Name = RecipesRouting.EditSingleItem)]
        public async Task<IActionResult> EditSingleItem(Recipe model)
        {
            var statusMessage = await logic.UpdateDataModelAsync(model);
            if (statusMessage.IsCompleted)
            {
                return RedirectToRoute(RecipesRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        [HttpPost("recipe/delete", Name = RecipesRouting.DeleteSingleItem)]
        public async Task<IActionResult> DeleteSingleItem(string key)
        {
            int id = Int32.Parse(key);
            var statusMessage = await logic.RemoveDataModelAsync(id);
            if (statusMessage.IsCompleted)
            {
                return RedirectToRoute(RecipesRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }
    }
}
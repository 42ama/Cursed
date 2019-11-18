using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cursed.Models.Data.Storages;
using Cursed.Models.Data.Shared;
using Cursed.Models.Context;
using Cursed.Models.Entities;
using Cursed.Models.Logic;
using Cursed.Models.Routing;
using Cursed.Models;
using Cursed.Models.Data.Utility;

namespace Cursed.Controllers
{
    [Route("storages")]
    public class StoragesController : Controller, IControllerRESTAsync<Storage>
    {
        private readonly StoragesLogic logic;

        public StoragesController(CursedContext db)
        {
            logic = new StoragesLogic(db);
        }
        [HttpGet("", Name = StoragesRouting.Index)]
        public async Task<IActionResult> Index(int currentPage = 1, int itemsOnPage = 20)
        {
            var model = await logic.GetAllDataModelAsync();
            var pagenationModel = new Pagenation<StoragesModel>(model, itemsOnPage, currentPage);

            return View(pagenationModel);
        }

        [HttpGet("storage", Name = StoragesRouting.SingleItem)]
        public async Task<IActionResult> SingleItem(string key)
        {
            int id = Int32.Parse(key);
            var model = await logic.GetSingleDataModelAsync(id);
            return View(model);
        }

        [HttpGet("storage/edit", Name = StoragesRouting.GetEditSingleItem)]
        public async Task<IActionResult> GetEditSingleItem(string key)
        {
            if (key != null)
            {
                int id = Int32.Parse(key);
                ViewData["SaveRoute"] = StoragesRouting.EditSingleItem;
                var model = await logic.GetSingleUpdateModelAsync(id);
                return View("EditSingleItem", model);
            }
            else
            {
                ViewData["SaveRoute"] = StoragesRouting.AddSingleItem;
                return View("EditSingleItem");
            }
        }

        [HttpPost("storage/add", Name = StoragesRouting.AddSingleItem)]
        public async Task<IActionResult> AddSingleItem(Storage model)
        {
            await logic.AddDataModelAsync(model);
            return RedirectToRoute(StoragesRouting.Index);
        }

        [HttpPost("storage/edit", Name = StoragesRouting.EditSingleItem)]
        public async Task<IActionResult> EditSingleItem(Storage model)
        {
            await logic.UpdateDataModelAsync(model);
            return RedirectToRoute(StoragesRouting.Index);
        }

        [HttpPost("storage/delete", Name = StoragesRouting.DeleteSingleItem)]
        public async Task<IActionResult> DeleteSingleItem(string key)
        {
            int id = Int32.Parse(key);
            await logic.RemoveDataModelAsync(id);
            return RedirectToRoute(StoragesRouting.Index);
        }
    }
}
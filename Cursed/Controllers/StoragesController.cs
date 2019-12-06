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
using Cursed.Models.Interfaces.ControllerCRUD;
using Cursed.Models.Data.Utility;

namespace Cursed.Controllers
{
    [Route("storages")]
    public class StoragesController : Controller, ICUD<Storage>, IReadColection, IReadSingle, IReadUpdateForm
    {
        private readonly StoragesLogic logic;

        public StoragesController(CursedContext db)
        {
            logic = new StoragesLogic(db);
        }
        [HttpGet("", Name = StoragesRouting.Index)]
        public async Task<IActionResult> Index(int currentPage = 1, int itemsOnPage = 20)
        {
            var statusMessage = await logic.GetAllDataModelAsync();
            if(statusMessage.IsCompleted)
            {
                var pagenationModel = new Pagenation<StoragesModel>(statusMessage.ReturnValue, itemsOnPage, currentPage);

                return View(pagenationModel);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
            
        }

        [HttpGet("storage", Name = StoragesRouting.SingleItem)]
        public async Task<IActionResult> SingleItem(string key)
        {
            int id = Int32.Parse(key);
            var statusMessage = await logic.GetSingleDataModelAsync(id);
            if(statusMessage.IsCompleted)
            {
                return View(statusMessage.ReturnValue);
            }
            else
            {
                return View("CustomError", statusMessage);
            }

        }

        [HttpGet("storage/edit", Name = StoragesRouting.GetEditSingleItem)]
        public async Task<IActionResult> GetEditSingleItem(string key)
        {
            if (key != null)
            {
                int id = Int32.Parse(key);
                ViewData["SaveRoute"] = StoragesRouting.EditSingleItem;
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
                ViewData["SaveRoute"] = StoragesRouting.AddSingleItem;
                return View("EditSingleItem");
            }
        }

        [HttpPost("storage/add", Name = StoragesRouting.AddSingleItem)]
        public async Task<IActionResult> AddSingleItem(Storage model)
        {
            var statusMessage = await logic.AddDataModelAsync(model);
            if(statusMessage.IsCompleted)
            {
                return RedirectToRoute(StoragesRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        [HttpPost("storage/edit", Name = StoragesRouting.EditSingleItem)]
        public async Task<IActionResult> EditSingleItem(Storage model)
        {
            var statusMessage = await logic.UpdateDataModelAsync(model);
            if (statusMessage.IsCompleted)
            {
                return RedirectToRoute(StoragesRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        [HttpPost("storage/delete", Name = StoragesRouting.DeleteSingleItem)]
        public async Task<IActionResult> DeleteSingleItem(string key)
        {
            int id = Int32.Parse(key);
            var statusMessage = await logic.RemoveDataModelAsync(id);
            if (statusMessage.IsCompleted)
            {
                return RedirectToRoute(StoragesRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }
    }
}
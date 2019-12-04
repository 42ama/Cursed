using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cursed.Models.Data.Companies;
using Cursed.Models.Data.Shared;
using Cursed.Models.Context;
using Cursed.Models.Entities;
using Cursed.Models.Logic;
using Cursed.Models.Routing;
using Cursed.Models.Interfaces.ControllerCRUD;
using Cursed.Models.Data.Utility;
using Cursed.Models.Data.Utility.ErrorHandling;

namespace Cursed.Controllers
{
    [Route("companies")]
    public class CompaniesController : Controller//, ICUD<Company>, IReadColection, IReadSingle, IReadUpdateForm
    {
        private readonly CompaniesLogic logic;

        public CompaniesController(CursedContext db)
        {
            logic = new CompaniesLogic(db);
        }
        [HttpGet("", Name = CompaniesRouting.Index)]
        public async Task<IActionResult> Index(int currentPage = 1, int itemsOnPage = 20)
        {
            var statusMessage = await logic.GetAllDataModelAsync();
            if (statusMessage.IsCompleted)
            {
                var pagenationModel = new Pagenation<CompaniesModel>(statusMessage.ReturnValue, itemsOnPage, currentPage);

                return View(pagenationModel);
            }
            else
            {
                return View("CustomError");
            }
        }

        [HttpGet("company", Name = CompaniesRouting.SingleItem)]
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
                return View("CustomError");
            }
        }

        [HttpGet("company/edit", Name = CompaniesRouting.GetEditSingleItem)]
        public async Task<IActionResult> GetEditSingleItem(string key)
        {
            if (key != null)
            {
                int id = Int32.Parse(key);
                ViewData["SaveRoute"] = CompaniesRouting.EditSingleItem;
                var statusMessage = await logic.GetSingleUpdateModelAsync(id);
                if(statusMessage.IsCompleted)
                {
                    return View("EditSingleItem", statusMessage);
                }
                else
                {
                    return View("CustomError");
                }
            }
            else
            {
                ViewData["SaveRoute"] = CompaniesRouting.AddSingleItem;
                return View("EditSingleItem");
            }
        }

        [HttpPost("company/add", Name = CompaniesRouting.AddSingleItem)]
        public async Task<IActionResult> AddSingleItem(Company model)
        {
            var statusMessage = await logic.AddDataModelAsync(model);
            if(statusMessage.IsCompleted)
            {
                return RedirectToRoute(CompaniesRouting.Index);
            }
            else
            {
                return View("CustomError");
            }
        }

        [HttpPost("company/edit", Name = CompaniesRouting.EditSingleItem)]
        public async Task<IActionResult> EditSingleItem(Company model)
        {
            var statusMessage = await logic.UpdateDataModelAsync(model);
            if (statusMessage.IsCompleted)
            {
                return RedirectToRoute(CompaniesRouting.Index);
            }
            else
            {
                return View("CustomError");
            }
        }

        [HttpPost("company/delete", Name = CompaniesRouting.DeleteSingleItem)]
        public async Task<IActionResult> DeleteSingleItem(string key)
        {
            int id = Int32.Parse(key);
            var statusMessage = await logic.RemoveDataModelAsync(id);
            if(statusMessage.IsCompleted)
            {
                return RedirectToRoute(CompaniesRouting.Index);
            }
            else
            {
                return View("CustomError");
            }
        }
    }
}
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
using Cursed.Models;
using Cursed.Models.Data.Utility;

namespace Cursed.Controllers
{
    [Route("companies")]
    public class CompaniesController : Controller, IControllerRESTAsync<Company>
    {
        private readonly CompanyLogic logic;

        public CompaniesController(CursedContext db)
        {
            logic = new CompanyLogic(db);
        }
        [HttpGet("", Name = CompanyRouting.Index)]
        public async Task<IActionResult> Index(int currentPage = 1, int itemsOnPage = 20)
        {
            var model = await logic.GetAllDataModelAsync();
            var pagenationModel = new Pagenation<CompaniesModel>(model, itemsOnPage, currentPage);

            return View(pagenationModel);
        }

        [HttpGet("company", Name = CompanyRouting.SingleItem)]
        public async Task<IActionResult> SingleItem(string key)
        {
            int id = Int32.Parse(key);
            var model = await logic.GetSingleDataModelAsync(id);
            return View(model);
        }

        [HttpGet("company/edit", Name = CompanyRouting.GetEditSingleItem)]
        public async Task<IActionResult> GetEditSingleItem(string key)
        {
            if (key != null)
            {
                int id = Int32.Parse(key);
                ViewData["SaveRoute"] = CompanyRouting.EditSingleItem;
                var model = await logic.GetSingleUpdateModelAsync(id);
                return View("EditSingleItem", model);
            }
            else
            {
                ViewData["SaveRoute"] = CompanyRouting.AddSingleItem;
                return View("EditSingleItem");
            }
        }

        [HttpPost("company/add", Name = CompanyRouting.AddSingleItem)]
        public async Task<IActionResult> AddSingleItem(Company model)
        {
            await logic.AddDataModelAsync(model);
            return RedirectToRoute(CompanyRouting.Index);
        }

        [HttpPost("company/edit", Name = CompanyRouting.EditSingleItem)]
        public async Task<IActionResult> EditSingleItem(Company model)
        {
            await logic.UpdateDataModelAsync(model);
            return RedirectToRoute(CompanyRouting.Index);
        }

        [HttpPost("company/delete", Name = CompanyRouting.DeleteSingleItem)]
        public async Task<IActionResult> DeleteSingleItem(string key)
        {
            int id = Int32.Parse(key);
            await logic.RemoveDataModelAsync(id);
            return RedirectToRoute(CompanyRouting.Index);
        }
    }
}
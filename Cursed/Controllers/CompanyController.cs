using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cursed.Models.Data.Company;
using Cursed.Models.Data.Shared;
using Cursed.Models.Context;
using Cursed.Models.Entities;
using Cursed.Models.Logic;
using Cursed.Models;
using Cursed.Models.Data.Utility;

namespace Cursed.Controllers
{
    [Route("companies")]
    public class CompanyController : Controller, IControllerRESTAsync<Company>
    {
        private readonly CompanyLogic logic;
        public CompanyController(CursedContext db)
        {
            logic = new CompanyLogic(db);
        }
        [HttpGet("", Name = "CompaniesAll")]
        public async Task<IActionResult> Index(int currentPage = 1, int itemsOnPage = 20)
        {
            var model = await logic.GetAllDataModelAsync();
            var pagenationModel = new Pagenation<CompanyAllModel>(model, itemsOnPage, currentPage);

            return View(pagenationModel);
        }

        [HttpGet("company", Name = "CompaniesSingle")]
        public async Task<IActionResult> SingleItem(string key)
        {
            int id = Int32.Parse(key);
            var model = await logic.GetSingleDataModelAsync(id);
            return View(model);
        }

        [HttpGet("company/edit", Name = "GetForEditCompaniesSingle")]
        public async Task<IActionResult> GetEditSingleItem(string key)
        {
            if (key != null)
            {
                int id = Int32.Parse(key);
                ViewData["SaveRoute"] = "EditCompaniesSingle";
                var model = await logic.GetSingleUpdateModelAsync(id);
                return View("EditSingleItem", model);
            }
            else
            {
                ViewData["SaveRoute"] = "AddCompaniesSingle";
                return View("EditSingleItem");
            }
        }

        [HttpPost("company/add", Name = "AddCompaniesSingle")]
        public async Task<IActionResult> AddSingleItem(Company model)
        {
            await logic.AddDataModelAsync(model);
            return RedirectToRoute("CompaniesAll");
        }

        [HttpPost("company/edit", Name = "EditCompaniesSingle")]
        public async Task<IActionResult> EditSingleItem(Company model)
        {
            await logic.UpdateDataModelAsync(model);
            return RedirectToRoute("CompaniesAll");
        }

        [HttpPost("company/delete", Name = "DeleteCompaniesSingle")]
        public async Task<IActionResult> DeleteSingleItem(string key)
        {
            int id = Int32.Parse(key);
            await logic.RemoveDataModelAsync(id);
            return RedirectToRoute("CompaniesAll");
        }
    }
}
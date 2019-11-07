using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cursed.Models.Data.Licenses;
using Cursed.Models.Data.Shared;
using Cursed.Models.Context;
using Cursed.Models.Entities;
using Cursed.Models.Logic;
using Cursed.Models;
using Cursed.Models.Data.Utility;


namespace Cursed.Controllers
{
    [Route("licenses")]
    public class LicensesController : Controller, IControllerRESTAsync<License>
    {
        private readonly LicenseLogic logic;
        public LicensesController(CursedContext db)
        {
            logic = new LicenseLogic(db);
        }

        [HttpGet("", Name = "LicensesAll")]
        public async Task<IActionResult> Index(int currentPage = 1, int itemsOnPage = 20)
        {
            var dataModel = await logic.GetAllDataModelAsync();
            var viewModel = new List<LicensesVM>();
            foreach (var item in dataModel)
            {
                viewModel.Add(new LicensesVM
                {
                    Id = item.Id,
                    GovermentNum = item.GovermentNum,
                    Date = item.Date.ToShortDateString(),
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductCAS = item.ProductCAS != null ? item.ProductCAS.Value.ToString() : "None",
                    IsValid = LicenseValid.Validate(item.Date)
                });
            }
            var pagenationModel = new Pagenation<LicensesVM>(viewModel, itemsOnPage, currentPage);

            return View(pagenationModel);
        }

        [HttpGet("license", Name = "LicensesSingle")]
        public async Task<IActionResult> SingleItem(int id)
        {
            var dataModel = await logic.GetSingleDataModelAsync(id);
            var viewModel = new LicensesVM
            {
                Id = dataModel.Id,
                GovermentNum = dataModel.GovermentNum,
                Date = dataModel.Date.ToShortDateString(),
                ProductId = dataModel.ProductId,
                ProductName = dataModel.ProductName,
                ProductCAS = dataModel.ProductCAS != null ? dataModel.ProductCAS.Value.ToString() : "None",
                IsValid = LicenseValid.Validate(dataModel.Date)
            };
            return View(viewModel);
        }

        [HttpGet("license/edit", Name = "GetForEditLicensesSingle")]
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

        [HttpPost("license", Name = "LicensesSingle")]
        public async Task<IActionResult> AddSingleItem(License model)
        {
            return RedirectToRoute("LicensesAll");
        }

        [HttpPut("license", Name = "LicensesSingle")]
        public async Task<IActionResult> EditSingleItem(License model)
        {
            return RedirectToRoute("LicensesAll");
        }

        [HttpDelete("license", Name = "LicensesSingle")]
        public async Task<IActionResult> DeleteSingleItem(int id)
        {
            return RedirectToRoute("LicensesAll");
        }
    }
}
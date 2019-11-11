﻿using System;
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
            var viewModel = new List<LicensesViewModel>();
            foreach (var item in dataModel)
            {
                viewModel.Add(new LicensesViewModel
                {
                    Id = item.Id,
                    GovermentNum = item.GovermentNum,
                    Date = item.Date.ToShortDateString(),
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductCAS = item.ProductCAS,
                    IsValid = LicenseValid.Validate(item.Date)
                });
            }
            var pagenationModel = new Pagenation<LicensesViewModel>(viewModel, itemsOnPage, currentPage);

            return View(pagenationModel);
        }

        [HttpGet("license", Name = "LicensesSingle")]
        public async Task<IActionResult> SingleItem(string key)
        {
            int id = Int32.Parse(key);
            var dataModel = await logic.GetSingleDataModelAsync(id);
            var viewModel = new LicensesViewModel
            {
                Id = dataModel.Id,
                GovermentNum = dataModel.GovermentNum,
                Date = dataModel.Date.ToShortDateString(),
                ProductId = dataModel.ProductId,
                ProductName = dataModel.ProductName,
                ProductCAS = dataModel.ProductCAS,
                IsValid = LicenseValid.Validate(dataModel.Date)
            };
            return View(viewModel);
        }

        [HttpGet("license/edit", Name = "GetForEditLicensesSingle")]
        public async Task<IActionResult> GetEditSingleItem(string key)
        {
            if (key != null)
            {
                int id = Int32.Parse(key);
                ViewData["SaveRoute"] = "EditLicensesSingle";
                var model = await logic.GetSingleUpdateModelAsync(id);
                return View("EditSingleItem", model);
            }
            else
            {
                ViewData["SaveRoute"] = "AddLicensesSingle";
                return View("EditSingleItem");
            }
        }

        [HttpPost("license/add", Name = "AddLicensesSingle")]
        public async Task<IActionResult> AddSingleItem(License model)
        {
            await logic.AddDataModelAsync(model);
            return RedirectToRoute("LicensesAll");
        }

        [HttpPost("license/edit", Name = "EditLicensesSingle")]
        public async Task<IActionResult> EditSingleItem(License model)
        {
            await logic.UpdateDataModelAsync(model);
            return RedirectToRoute("LicensesAll");
        }

        [HttpPost("license/delete", Name = "DeleteLicensesSingle")]
        public async Task<IActionResult> DeleteSingleItem(string key)
        {
            int id = Int32.Parse(key);
            await logic.RemoveDataModelAsync(id);
            return RedirectToRoute("LicensesAll");
        }
    }
}
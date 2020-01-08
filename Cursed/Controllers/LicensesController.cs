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
using Cursed.Models.Interfaces.ControllerCRUD;
using Cursed.Models.Data.Utility;
using Cursed.Models.Routing;
using Cursed.Models.Services;
using Cursed.Models.LogicValidation;
using Cursed.Models.Data.Utility.Authorization;

namespace Cursed.Controllers
{
    [Route("licenses")]
    public class LicensesController : Controller, ICUD<License>, IReadColection, IReadSingle, IReadUpdateForm
    {
        private readonly LicensesLogic logic;
        private readonly LicensesLogicValidation logicValidation;
        private readonly ILicenseValidation licenseValidation;
        public LicensesController(CursedDataContext db, [FromServices] ILicenseValidation licenseValidation, [FromServices] IErrorHandlerFactory errorHandlerFactory)
        {
            logic = new LicensesLogic(db);
            logicValidation = new LicensesLogicValidation(db, errorHandlerFactory);
            this.licenseValidation = licenseValidation;
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist, AuthorizeRoles.GovermentAgent)]
        [HttpGet("", Name = LicensesRouting.Index)]
        public async Task<IActionResult> Index(int currentPage = 1, int itemsOnPage = 20)
        {
            var model = await logic.GetAllDataModelAsync();

            var viewModel = new List<LicensesViewModel>();
            foreach (var item in model)
            {
                viewModel.Add(new LicensesViewModel
                {
                    Id = item.Id,
                    GovermentNum = item.GovermentNum,
                    Date = item.Date.ToShortDateString(),
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductCAS = item.ProductCAS,
                    IsValid = licenseValidation.IsValid(new License { Date = item.Date })
                });
            }
            var pagenationModel = new Pagenation<LicensesViewModel>(viewModel, itemsOnPage, currentPage);

            return View(pagenationModel);
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist, AuthorizeRoles.GovermentAgent)]
        [HttpGet("license", Name = LicensesRouting.SingleItem)]
        public async Task<IActionResult> SingleItem(string key)
        {
            int id = Int32.Parse(key);
            var statusMessage = await logicValidation.CheckGetSingleDataModelAsync(id);
            if (statusMessage.IsCompleted)
            {
                var dataModel = await logic.GetSingleDataModelAsync(id);
                var viewModel = new LicensesViewModel
                {
                    Id = dataModel.Id,
                    GovermentNum = dataModel.GovermentNum,
                    Date = dataModel.Date.ToShortDateString(),
                    ProductId = dataModel.ProductId,
                    ProductName = dataModel.ProductName,
                    ProductCAS = dataModel.ProductCAS,
                    IsValid = licenseValidation.IsValid(new License { Date = dataModel.Date })
                };
                return View(viewModel);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.GovermentAgent)]
        [HttpGet("license/edit", Name = LicensesRouting.GetEditSingleItem)]
        public async Task<IActionResult> GetEditSingleItem(string key)
        {
            if (key != null)
            {
                int id = Int32.Parse(key);
                ViewData["SaveRoute"] = LicensesRouting.EditSingleItem;
                var statusMessage = await logicValidation.CheckGetSingleUpdateModelAsync(id);
                if (statusMessage.IsCompleted)
                {
                    var model = await logic.GetSingleUpdateModelAsync(id);
                    return View("EditSingleItem", model);
                }
                else
                {
                    return View("CustomError", statusMessage);
                }
            }
            else
            {
                ViewData["SaveRoute"] = LicensesRouting.AddSingleItem;
                return View("EditSingleItem");
            }
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.GovermentAgent)]
        [HttpPost("license/add", Name = LicensesRouting.AddSingleItem)]
        public async Task<IActionResult> AddSingleItem(License model)
        {
            await logic.AddDataModelAsync(model);
            return RedirectToRoute(LicensesRouting.Index);
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.GovermentAgent)]
        [HttpPost("license/edit", Name = LicensesRouting.EditSingleItem)]
        public async Task<IActionResult> EditSingleItem(License model)
        {
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(model.Id);
            if (statusMessage.IsCompleted)
            {
                await logic.UpdateDataModelAsync(model);
                return RedirectToRoute(LicensesRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.GovermentAgent)]
        [HttpPost("license/delete", Name = LicensesRouting.DeleteSingleItem)]
        public async Task<IActionResult> DeleteSingleItem(string key)
        {
            int id = Int32.Parse(key);
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync(id);
            if (statusMessage.IsCompleted)
            {
                await logic.RemoveDataModelAsync(id);
                return RedirectToRoute(LicensesRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }
    }
}
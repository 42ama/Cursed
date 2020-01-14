using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cursed.Models.Data.Facilities;
using Cursed.Models.Data.Shared;
using Cursed.Models.Context;
using Cursed.Models.Entities;
using Cursed.Models.Logic;
using Cursed.Models.Routing;
using Cursed.Models.Interfaces.ControllerCRUD;
using Cursed.Models.Data.Utility;
using Cursed.Models.Services;
using Cursed.Models.LogicValidation;
using Cursed.Models.Data.Utility.Authorization;

namespace Cursed.Controllers
{
    [Route("facilities")]
    public class FacilitiesController : Controller, ICUD<Facility>, IReadColection, IReadSingle, IReadUpdateForm
    {
        private readonly FacilitiesLogic logic;
        private readonly FacilitiesLogicValidation logicValidation;
        private readonly ILogProvider<CursedAuthenticationContext> logProvider;

        public FacilitiesController(CursedDataContext db, 
            [FromServices] ILicenseValidation licenseValidation, 
            [FromServices] IErrorHandlerFactory errorHandlerFactory,
            [FromServices] ILogProvider<CursedAuthenticationContext> logProvider)
        {
            logic = new FacilitiesLogic(db, licenseValidation);
            logicValidation = new FacilitiesLogicValidation(db, errorHandlerFactory);
            this.logProvider = logProvider;
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpGet("", Name = FacilitiesRouting.Index)]
        public async Task<IActionResult> Index(int currentPage = 1, int itemsOnPage = 20)
        {
            var model = await logic.GetAllDataModelAsync();

            var pagenationModel = new Pagenation<FacilitiesModel>(model, itemsOnPage, currentPage);

            return View(pagenationModel);
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpGet("facility", Name = FacilitiesRouting.SingleItem)]
        public async Task<IActionResult> SingleItem(string key)
        {
            int id = Int32.Parse(key);
            var statusMessage = await logicValidation.CheckGetSingleDataModelAsync(id);
            if (statusMessage.IsCompleted)
            {
                var model = await logic.GetSingleDataModelAsync(id);
                return View(model);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager)]
        [HttpGet("facility/edit", Name = FacilitiesRouting.GetEditSingleItem)]
        public async Task<IActionResult> GetEditSingleItem(string key)
        {
            if (key != null)
            {
                int id = Int32.Parse(key);
                ViewData["SaveRoute"] = FacilitiesRouting.EditSingleItem;
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
                ViewData["SaveRoute"] = FacilitiesRouting.AddSingleItem;
                return View("EditSingleItem");
            }
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager)]
        [HttpPost("facility/add", Name = FacilitiesRouting.AddSingleItem)]
        public async Task<IActionResult> AddSingleItem(Facility model)
        {
            var facility = await logic.AddDataModelAsync(model);
            await logProvider.AddToLogAsync($"Added new facility (Id: {facility.Id}).");
            return RedirectToRoute(FacilitiesRouting.Index);
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager)]
        [HttpPost("facility/edit", Name = FacilitiesRouting.EditSingleItem)]
        public async Task<IActionResult> EditSingleItem(Facility model)
        {
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(model.Id);
            if (statusMessage.IsCompleted)
            {
                await logic.UpdateDataModelAsync(model);
                await logProvider.AddToLogAsync($"Updated facility information (Id: {model.Id}).");
                return RedirectToRoute(FacilitiesRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager)]
        [HttpPost("facility/delete", Name = FacilitiesRouting.DeleteSingleItem)]
        public async Task<IActionResult> DeleteSingleItem(string key)
        {
            int id = Int32.Parse(key);
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync(id);
            if (statusMessage.IsCompleted)
            {
                await logic.RemoveDataModelAsync(id);
                await logProvider.AddToLogAsync($"Removed facility (Id: {key}).");
                return RedirectToRoute(FacilitiesRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }
    }
}
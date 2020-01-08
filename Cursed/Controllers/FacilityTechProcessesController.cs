﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cursed.Models.Data.FacilityTechProcesses;
using Cursed.Models.Data.Shared;
using Cursed.Models.Context;
using Cursed.Models.Entities;
using Cursed.Models.Logic;
using Cursed.Models.Routing;
using Cursed.Models.Interfaces.ControllerCRUD;
using Cursed.Models.Data.Utility;
using Cursed.Models.LogicValidation;
using Cursed.Models.Services;
using Cursed.Models.Data.Utility.Authorization;

namespace Cursed.Controllers
{
    [Route("facilities/tech-processes")]
    public class FacilityTechProcessesController : Controller
    {
        private readonly FacilityTechProcessesLogic logic;
        private readonly FacilityTechProcessesLogicValidation logicValidation;
        public FacilityTechProcessesController(CursedDataContext db, [FromServices] IErrorHandlerFactory errorHandlerFactory)
        {
            logic = new FacilityTechProcessesLogic(db);
            logicValidation = new FacilityTechProcessesLogicValidation(db, errorHandlerFactory);
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpGet("", Name = FacilityTechProcessesRouting.Index)]
        public async Task<IActionResult> Index(string key, int currentPage = 1, int itemsOnPage = 20)
        {
            int facilityId = Int32.Parse(key);
            ViewData["FacilityId"] = facilityId;
            var model = await logic.GetAllDataModelAsync(facilityId);

            var pagenationModel = new Pagenation<FacilityTechProcessesDataModel>(model, itemsOnPage, currentPage);
            return View(pagenationModel);
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpPost("add", Name = FacilityTechProcessesRouting.AddSingleItem)]
        public async Task<IActionResult> AddSingleItem(TechProcess model)
        {
            await logic.AddDataModelAsync(model);
            return RedirectToRoute(FacilityTechProcessesRouting.Index, new { key = model.FacilityId });
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpPost("edit", Name = FacilityTechProcessesRouting.EditSingleItem)]
        public async Task<IActionResult> EditSingleItem(TechProcess model)
        {
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync((model.FacilityId, model.RecipeId));
            if (statusMessage.IsCompleted)
            {
                await logic.UpdateDataModelAsync(model);
                return RedirectToRoute(FacilityTechProcessesRouting.Index, new { key = model.FacilityId });
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpPost("delete", Name = FacilityTechProcessesRouting.DeleteSingleItem)]
        public async Task<IActionResult> DeleteSingleItem(TechProcess model)
        {
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync((model.FacilityId, model.RecipeId));
            if (statusMessage.IsCompleted)
            {
                await logic.RemoveDataModelAsync(model);
                return RedirectToRoute(FacilityTechProcessesRouting.Index, new { key = model.FacilityId });
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }
    }
}
using System;
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

namespace Cursed.Controllers
{
    [Route("facilities/tech-processes")]
    public class FacilityTechProcessesController : Controller
    {
        private readonly FacilityTechProcessesLogic logic;
        public FacilityTechProcessesController(CursedContext db)
        {
            logic = new FacilityTechProcessesLogic(db);
        }

        [HttpGet("", Name = FacilityTechProcessesRouting.Index)]
        public async Task<IActionResult> Index(string key, int currentPage = 1, int itemsOnPage = 20)
        {
            int facilityId = Int32.Parse(key);
            ViewData["FacilityId"] = facilityId;
            var statusMessage = await logic.GetAllDataModelAsync(facilityId);
            if(statusMessage.IsCompleted)
            {
                var pagenationModel = new Pagenation<FacilityTechProcessesDataModel>(statusMessage.ReturnValue, itemsOnPage, currentPage);
                return View(pagenationModel);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        [HttpPost("add", Name = FacilityTechProcessesRouting.AddSingleItem)]
        public async Task<IActionResult> AddSingleItem(TechProcess model)
        {
            var statusMessage = await logic.AddDataModelAsync(model);
            if(statusMessage.IsCompleted)
            {
                return RedirectToRoute(FacilityTechProcessesRouting.Index, new { key = model.FacilityId });
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        [HttpPost("edit", Name = FacilityTechProcessesRouting.EditSingleItem)]
        public async Task<IActionResult> EditSingleItem(TechProcess model)
        {
            var statusMessage = await logic.UpdateDataModelAsync(model);
            if (statusMessage.IsCompleted)
            {
                return RedirectToRoute(FacilityTechProcessesRouting.Index, new { key = model.FacilityId });
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        [HttpPost("delete", Name = FacilityTechProcessesRouting.DeleteSingleItem)]
        public async Task<IActionResult> DeleteSingleItem(TechProcess model)
        {
            var statusMessage = await logic.RemoveDataModelAsync(model);
            if (statusMessage.IsCompleted)
            {
                return RedirectToRoute(FacilityTechProcessesRouting.Index, new { key = model.FacilityId });
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }
    }
}
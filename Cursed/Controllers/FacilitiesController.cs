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

namespace Cursed.Controllers
{
    [Route("facilities")]
    public class FacilitiesController : Controller, ICUD<Facility>, IReadColection, IReadSingle, IReadUpdateForm
    {
        private readonly FacilitiesLogic logic;

        public FacilitiesController(CursedContext db, [FromServices] ILicenseValidation licenseValidation)
        {
            logic = new FacilitiesLogic(db, licenseValidation);
        }
        [HttpGet("", Name = FacilitiesRouting.Index)]
        public async Task<IActionResult> Index(int currentPage = 1, int itemsOnPage = 20)
        {
            var model = await logic.GetAllDataModelAsync();
            var pagenationModel = new Pagenation<FacilitiesModel>(model, itemsOnPage, currentPage);

            return View(pagenationModel);
        }

        [HttpGet("facility", Name = FacilitiesRouting.SingleItem)]
        public async Task<IActionResult> SingleItem(string key)
        {
            int id = Int32.Parse(key);
            var model = await logic.GetSingleDataModelAsync(id);
            return View(model);
        }

        [HttpGet("facility/edit", Name = FacilitiesRouting.GetEditSingleItem)]
        public async Task<IActionResult> GetEditSingleItem(string key)
        {
            if (key != null)
            {
                int id = Int32.Parse(key);
                ViewData["SaveRoute"] = FacilitiesRouting.EditSingleItem;
                var model = await logic.GetSingleUpdateModelAsync(id);
                return View("EditSingleItem", model);
            }
            else
            {
                ViewData["SaveRoute"] = FacilitiesRouting.AddSingleItem;
                return View("EditSingleItem");
            }
        }

        [HttpPost("facility/add", Name = FacilitiesRouting.AddSingleItem)]
        public async Task<IActionResult> AddSingleItem(Facility model)
        {
            await logic.AddDataModelAsync(model);
            return RedirectToRoute(FacilitiesRouting.Index);
        }

        [HttpPost("facility/edit", Name = FacilitiesRouting.EditSingleItem)]
        public async Task<IActionResult> EditSingleItem(Facility model)
        {
            await logic.UpdateDataModelAsync(model);
            return RedirectToRoute(FacilitiesRouting.Index);
        }

        [HttpPost("facility/delete", Name = FacilitiesRouting.DeleteSingleItem)]
        public async Task<IActionResult> DeleteSingleItem(string key)
        {
            int id = Int32.Parse(key);
            await logic.RemoveDataModelAsync(id);
            return RedirectToRoute(FacilitiesRouting.Index);
        }
    }
}
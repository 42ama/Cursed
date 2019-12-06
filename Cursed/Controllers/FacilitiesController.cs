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
            var statusMessage = await logic.GetAllDataModelAsync();
            if(statusMessage.IsCompleted)
            {
                var pagenationModel = new Pagenation<FacilitiesModel>(statusMessage.ReturnValue, itemsOnPage, currentPage);

                return View(pagenationModel);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        [HttpGet("facility", Name = FacilitiesRouting.SingleItem)]
        public async Task<IActionResult> SingleItem(string key)
        {
            int id = Int32.Parse(key);
            var statusMessage = await logic.GetSingleDataModelAsync(id);
            if (statusMessage.IsCompleted)
            {
                return View(statusMessage.ReturnValue);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        [HttpGet("facility/edit", Name = FacilitiesRouting.GetEditSingleItem)]
        public async Task<IActionResult> GetEditSingleItem(string key)
        {
            if (key != null)
            {
                int id = Int32.Parse(key);
                ViewData["SaveRoute"] = FacilitiesRouting.EditSingleItem;
                var statusMessage = await logic.GetSingleUpdateModelAsync(id);
                if (statusMessage.IsCompleted)
                {
                    return View("EditSingleItem", statusMessage.ReturnValue);
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

        [HttpPost("facility/add", Name = FacilitiesRouting.AddSingleItem)]
        public async Task<IActionResult> AddSingleItem(Facility model)
        {
            var statusMessage = await logic.AddDataModelAsync(model);
            if (statusMessage.IsCompleted)
            {
                return RedirectToRoute(FacilitiesRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        [HttpPost("facility/edit", Name = FacilitiesRouting.EditSingleItem)]
        public async Task<IActionResult> EditSingleItem(Facility model)
        {
            var statusMessage = await logic.UpdateDataModelAsync(model);
            if (statusMessage.IsCompleted)
            {
                return RedirectToRoute(FacilitiesRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        [HttpPost("facility/delete", Name = FacilitiesRouting.DeleteSingleItem)]
        public async Task<IActionResult> DeleteSingleItem(string key)
        {
            int id = Int32.Parse(key);
            var statusMessage = await logic.RemoveDataModelAsync(id);
            if (statusMessage.IsCompleted)
            {
                return RedirectToRoute(FacilitiesRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }
    }
}
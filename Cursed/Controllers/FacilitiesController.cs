using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cursed.Models.DataModel.Facilities;
using Cursed.Models.Context;
using Cursed.Models.Entities.Data;
using Cursed.Models.Logic;
using Cursed.Models.StaticReferences.Routing;
using Cursed.Models.Interfaces.ControllerCRUD;
using Cursed.Models.DataModel.Pagenation;
using Cursed.Models.Services;
using Cursed.Models.LogicValidation;
using Cursed.Models.DataModel.Authorization;

namespace Cursed.Controllers
{
    /// <summary>
    /// Facilities section controller. Consists of CRUD actions for facilities, including read action for
    /// both single facility and collection of all facilities.
    /// </summary>
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

        /// <summary>
        /// Main page of section, contains consolidated collection of facilities. 
        /// Can be navigated through pagenation.
        /// </summary>
        /// <param name="currentPage">Defines which portion of items from collection, will be shown</param>
        /// <param name="itemsOnPage">Defines how many item there will be in a portion</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpGet("", Name = FacilitiesRouting.Index)]
        public async Task<IActionResult> Index(int currentPage = 1, int itemsOnPage = 20)
        {
            var model = await logic.GetAllDataModelAsync();

            // form pagenation model
            var pagenationModel = new Pagenation<FacilitiesModel>(model, itemsOnPage, currentPage);

            return View(pagenationModel);
        }

        /// <summary>
        /// Displays a single facility, which found by <c>key</c>.
        /// </summary>
        /// <param name="key">Id of facility to be found</param>
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

        /// <summary>
        /// Display a page with form to update/add new facility.
        /// </summary>
        /// <param name="key">Id of facility to be edited, if null - considered that facility added insted of edited</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager)]
        [HttpGet("facility/edit", Name = FacilitiesRouting.GetEditSingleItem)]
        public async Task<IActionResult> GetEditSingleItem(string key)
        {
            // add distincted from edit, by presence of key parameter
            // further on they distincted by ViewData[SaveRoute]
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

        /// <summary>
        /// Post action to add new facility.
        /// </summary>
        /// <param name="model">Company to be added</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager)]
        [HttpPost("facility/add", Name = FacilitiesRouting.AddSingleItem)]
        public async Task<IActionResult> AddSingleItem(Facility model)
        {
            var facility = await logic.AddDataModelAsync(model);
            await logProvider.AddToLogAsync($"Added new facility (Id: {facility.Id}).");
            return RedirectToRoute(FacilitiesRouting.Index);
        }

        /// <summary>
        /// Post action to update facility.
        /// </summary>
        /// <param name="model">Updated facility information</param>
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

        /// <summary>
        /// Post action to delete facility.
        /// </summary>
        /// <param name="key">Id of facility to be deleted</param>
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
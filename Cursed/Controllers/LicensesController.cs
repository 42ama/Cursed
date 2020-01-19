using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cursed.Models.DataModel.Licenses;
using Cursed.Models.Context;
using Cursed.Models.Entities.Data;
using Cursed.Models.Logic;
using Cursed.Models.Interfaces.ControllerCRUD;
using Cursed.Models.DataModel.Pagenation;
using Cursed.Models.StaticReferences.Routing;
using Cursed.Models.Services;
using Cursed.Models.LogicValidation;
using Cursed.Models.DataModel.Authorization;

namespace Cursed.Controllers
{
    /// <summary>
    /// Licenses section controller. Consists of CRUD actions for licenses, including read action for
    /// both single license and collection of all licenses.
    /// </summary>
    [Route("licenses")]
    public class LicensesController : Controller, ICUD<License>, IReadColection, IReadSingle, IReadUpdateForm
    {
        private readonly LicensesLogic logic;
        private readonly LicensesLogicValidation logicValidation;
        private readonly ILicenseValidation licenseValidation;
        private readonly ILogProvider<CursedAuthenticationContext> logProvider;
        public LicensesController(CursedDataContext db, 
            [FromServices] ILicenseValidation licenseValidation, 
            [FromServices] IErrorHandlerFactory errorHandlerFactory,
            [FromServices] ILogProvider<CursedAuthenticationContext> logProvider)
        {
            logic = new LicensesLogic(db);
            logicValidation = new LicensesLogicValidation(db, errorHandlerFactory);
            this.licenseValidation = licenseValidation;
            this.logProvider = logProvider;
        }

        /// <summary>
        /// Main page of section, contains consolidated collection of licenses. 
        /// Can be navigated through pagenation.
        /// </summary>
        /// <param name="currentPage">Defines which portion of items from collection, will be shown</param>
        /// <param name="itemsOnPage">Defines how many item there will be in a portion</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist, AuthorizeRoles.GovermentAgent)]
        [HttpGet("", Name = LicensesRouting.Index)]
        public async Task<IActionResult> Index(int currentPage = 1, int itemsOnPage = 20)
        {
            var model = await logic.GetAllDataModelAsync();

            var viewModel = new List<LicensesViewModel>();
            foreach (var item in model)
            {
                viewModel.Add(LicensesDataModelToViewModel(item));
            }
            // form pagenation model
            var pagenationModel = new Pagenation<LicensesViewModel>(viewModel, itemsOnPage, currentPage);

            return View(pagenationModel);
        }

        /// <summary>
        /// Displays a single license, which found by <c>key</c>
        /// </summary>
        /// <param name="key">Id of license to be found</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist, AuthorizeRoles.GovermentAgent)]
        [HttpGet("license", Name = LicensesRouting.SingleItem)]
        public async Task<IActionResult> SingleItem(string key)
        {
            int id = Int32.Parse(key);
            var statusMessage = await logicValidation.CheckGetSingleDataModelAsync(id);
            if (statusMessage.IsCompleted)
            {
                var dataModel = await logic.GetSingleDataModelAsync(id);
                var viewModel = LicensesDataModelToViewModel(dataModel);

                // gather licenses related to current license by product id
                var relatedLicensesDataModel = await logic.GetRelatedEntitiesByKeyAsync(dataModel.ProductId, id);
                var relatedLicensesViewModel = new List<LicensesViewModel>();
                foreach (var licenseDataModel in relatedLicensesDataModel)
                {
                    relatedLicensesViewModel.Add(LicensesDataModelToViewModel(licenseDataModel));
                }
                viewModel.RelatedLicenses = relatedLicensesViewModel;

                return View(viewModel);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        /// <summary>
        /// Display a page with form to update/add new license.
        /// </summary>
        /// <param name="key">Id of license to be edited, if null - considered that license added insted of edited</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.GovermentAgent)]
        [HttpGet("license/edit", Name = LicensesRouting.GetEditSingleItem)]
        public async Task<IActionResult> GetEditSingleItem(string key)
        {
            // add distincted from edit, by presence of key parameter
            // further on they distincted by ViewData[SaveRoute]
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

        /// <summary>
        /// Post action to add new license.
        /// </summary>
        /// <param name="model">License to be added</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.GovermentAgent)]
        [HttpPost("license/add", Name = LicensesRouting.AddSingleItem)]
        public async Task<IActionResult> AddSingleItem(License model)
        {
            var license = await logic.AddDataModelAsync(model);
            await logProvider.AddToLogAsync($"Added new license (Id: {license.Id}).");
            return RedirectToRoute(LicensesRouting.Index);
        }

        /// <summary>
        /// Post action to update license.
        /// </summary>
        /// <param name="model">Updated license information</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.GovermentAgent)]
        [HttpPost("license/edit", Name = LicensesRouting.EditSingleItem)]
        public async Task<IActionResult> EditSingleItem(License model)
        {
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(model.Id);
            if (statusMessage.IsCompleted)
            {
                await logic.UpdateDataModelAsync(model);
                await logProvider.AddToLogAsync($"Updated license information (Id: {model.Id}).");
                return RedirectToRoute(LicensesRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        /// <summary>
        /// Post action to delete license.
        /// </summary>
        /// <param name="key">Id of license to be deleted</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.GovermentAgent)]
        [HttpPost("license/delete", Name = LicensesRouting.DeleteSingleItem)]
        public async Task<IActionResult> DeleteSingleItem(string key)
        {
            int id = Int32.Parse(key);
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync(id);
            if (statusMessage.IsCompleted)
            {
                await logic.RemoveDataModelAsync(id);
                await logProvider.AddToLogAsync($"Removed license (Id: {key}).");
                return RedirectToRoute(LicensesRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        /// <summary>
        /// Convert LicensesDataModel to LicensesViewModel
        /// </summary>
        /// <param name="licenseDataModel">Data model to be converted</param>
        /// <returns>Converted LicensesViewModel instance</returns>
        private LicensesViewModel LicensesDataModelToViewModel(LicensesDataModel licenseDataModel)
        {
            return new LicensesViewModel
            {
                Id = licenseDataModel.Id,
                GovermentNum = licenseDataModel.GovermentNum,
                Date = licenseDataModel.Date.ToShortDateString(),
                ProductId = licenseDataModel.ProductId,
                ProductName = licenseDataModel.ProductName,
                ProductCAS = licenseDataModel.ProductCAS,
                IsValid = licenseValidation.IsValid(new License { Date = licenseDataModel.Date })
            };
        }
    }
}
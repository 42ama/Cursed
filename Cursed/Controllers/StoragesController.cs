using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cursed.Models.DataModel.Storages;
using Cursed.Models.Context;
using Cursed.Models.Entities.Data;
using Cursed.Models.Logic;
using Cursed.Models.StaticReferences.Routing;
using Cursed.Models.Interfaces.ControllerCRUD;
using Cursed.Models.DataModel.Pagenation;
using Cursed.Models.LogicValidation;
using Cursed.Models.Services;
using Cursed.Models.DataModel.Authorization;

namespace Cursed.Controllers
{
    /// <summary>
    /// Storages section controller. Consists of CRUD actions for storages, including read action for
    /// both single storage and collection of all storages.
    /// </summary>
    [Route("storages")]
    public class StoragesController : Controller, ICUD<Storage>, IReadColection, IReadSingle, IReadUpdateForm
    {
        private readonly StoragesLogic logic;
        private readonly StoragesLogicValidation logicValidation;
        private readonly ILogProvider<CursedAuthenticationContext> logProvider;

        public StoragesController(CursedDataContext db, 
            [FromServices] IErrorHandlerFactory errorHandlerFactory,
            [FromServices] ILogProvider<CursedAuthenticationContext> logProvider)
        {
            logic = new StoragesLogic(db);
            logicValidation = new StoragesLogicValidation(db, errorHandlerFactory);
            this.logProvider = logProvider;
        }

        /// <summary>
        /// Main page of section, contains consolidated collection of storages. 
        /// Can be navigated through pagenation.
        /// </summary>
        /// <param name="currentPage">Defines which portion of items from collection, will be shown</param>
        /// <param name="itemsOnPage">Defines how many item there will be in a portion</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpGet("", Name = StoragesRouting.Index)]
        public async Task<IActionResult> Index(int currentPage = 1, int itemsOnPage = 20)
        {
            var model = await logic.GetAllDataModelAsync();

            // form pagenation model
            var pagenationModel = new Pagenation<StoragesModel>(model, itemsOnPage, currentPage);

            return View(pagenationModel);
        }

        /// <summary>
        /// Displays a single storage, which found by <c>key</c>
        /// </summary>
        /// <param name="key">Id of storage to be found</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpGet("storage", Name = StoragesRouting.SingleItem)]
        public async Task<IActionResult> SingleItem(string key)
        {
            int id = Int32.Parse(key);
            var statusMessage = await logicValidation.CheckGetSingleDataModelAsync(id);
            if(statusMessage.IsCompleted)
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
        /// Display a page with form to update/add new storage.
        /// </summary>
        /// <param name="key">Id of storage to be edited, if null - considered that storage added insted of edited</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager)]
        [HttpGet("storage/edit", Name = StoragesRouting.GetEditSingleItem)]
        public async Task<IActionResult> GetEditSingleItem(string key)
        {
            // add distincted from edit, by presence of key parameter
            // further on they distincted by ViewData[SaveRoute]
            if (key != null)
            {
                int id = Int32.Parse(key);
                ViewData["SaveRoute"] = StoragesRouting.EditSingleItem;
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
                ViewData["SaveRoute"] = StoragesRouting.AddSingleItem;
                return View("EditSingleItem");
            }
        }

        /// <summary>
        /// Post action to add new storage.
        /// </summary>
        /// <param name="model">Storage to be added</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager)]
        [HttpPost("storage/add", Name = StoragesRouting.AddSingleItem)]
        public async Task<IActionResult> AddSingleItem(Storage model)
        {
            var storage = await logic.AddDataModelAsync(model);
            await logProvider.AddToLogAsync($"Added new storage (Id: {storage.Id}).");
            return RedirectToRoute(StoragesRouting.Index);
        }

        /// <summary>
        /// Post action to update storage.
        /// </summary>
        /// <param name="model">Updated storage information</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager)]
        [HttpPost("storage/edit", Name = StoragesRouting.EditSingleItem)]
        public async Task<IActionResult> EditSingleItem(Storage model)
        {
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(model.Id);
            if (statusMessage.IsCompleted)
            {
                await logic.UpdateDataModelAsync(model);
                await logProvider.AddToLogAsync($"Updated storage information (Id: {model.Id}).");
                return RedirectToRoute(StoragesRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        /// <summary>
        /// Post action to delete storage.
        /// </summary>
        /// <param name="key">Id of storage to be deleted</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager)]
        [HttpPost("storage/delete", Name = StoragesRouting.DeleteSingleItem)]
        public async Task<IActionResult> DeleteSingleItem(string key)
        {
            int id = Int32.Parse(key);
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync(id);
            if (statusMessage.IsCompleted)
            {
                await logic.RemoveDataModelAsync(id);
                await logProvider.AddToLogAsync($"Removed storage (Id: {key}).");
                return RedirectToRoute(StoragesRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }
    }
}
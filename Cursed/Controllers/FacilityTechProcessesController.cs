using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cursed.Models.DataModel.FacilityTechProcesses;
using Cursed.Models.Context;
using Cursed.Models.Entities.Data;
using Cursed.Models.Logic;
using Cursed.Models.StaticReferences.Routing;
using Cursed.Models.DataModel.Pagenation;
using Cursed.Models.LogicValidation;
using Cursed.Models.Services;
using Cursed.Models.DataModel.Authorization;
using Cursed.Models.Interfaces.ControllerCRUD;

namespace Cursed.Controllers
{
    /// <summary>
    /// Tech processes section controller. Consists of CRUD actions for tech processes.
    /// </summary>
    [Route("facilities/tech-processes")]
    public class FacilityTechProcessesController : Controller, IReadCollectionByParam, ICreate<TechProcess>, IUpdate<TechProcess>, IDeleteByModel<TechProcess>
    {
        private readonly FacilityTechProcessesLogic logic;
        private readonly FacilityTechProcessesLogicValidation logicValidation;
        private readonly ILogProvider<CursedAuthenticationContext> logProvider;
        public FacilityTechProcessesController(CursedDataContext db, 
            [FromServices] IErrorHandlerFactory errorHandlerFactory,
            [FromServices] ILogProvider<CursedAuthenticationContext> logProvider)
        {
            logic = new FacilityTechProcessesLogic(db);
            logicValidation = new FacilityTechProcessesLogicValidation(db, errorHandlerFactory);
            this.logProvider = logProvider;
        }

        /// <summary>
        /// Main page of section, contains consolidated collection of tech process. 
        /// Can be navigated through pagenation.
        /// </summary>
        /// <param name="key">Id of facility to which processes belongs</param>
        /// <param name="currentPage">Defines which portion of items from collection, will be shown</param>
        /// <param name="itemsOnPage">Defines how many item there will be in a portion</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpGet("", Name = FacilityTechProcessesRouting.Index)]
        public async Task<IActionResult> Index(string key, int currentPage = 1, int itemsOnPage = 20)
        {
            int facilityId = Int32.Parse(key);
            ViewData["FacilityId"] = facilityId;
            var model = await logic.GetAllDataModelAsync(facilityId);

            // form pagenation model
            var pagenationModel = new Pagenation<FacilityTechProcessesDataModel>(model, itemsOnPage, currentPage);

            return View(pagenationModel);
        }

        /// <summary>
        /// Post action to add new tech process.
        /// </summary>
        /// <param name="model">Tech process to be added</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpPost("add", Name = FacilityTechProcessesRouting.AddSingleItem)]
        public async Task<IActionResult> AddSingleItem(TechProcess model)
        {
            var statusMessage = await logicValidation.CheckAddDataModelAsync(model);
            if (statusMessage.IsCompleted)
            {
                var techProcess = await logic.AddDataModelAsync(model);
                await logProvider.AddToLogAsync($"Added new technological process (Facility Id: {techProcess.FacilityId}; Recipe Id: {techProcess.RecipeId}).");
                return RedirectToRoute(FacilityTechProcessesRouting.Index, new { key = model.FacilityId });
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        /// <summary>
        /// Post action to update tech process.
        /// </summary>
        /// <param name="model">Updated tech process information</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpPost("edit", Name = FacilityTechProcessesRouting.EditSingleItem)]
        public async Task<IActionResult> EditSingleItem(TechProcess model)
        {
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(model);
            if (statusMessage.IsCompleted)
            {
                await logic.UpdateDataModelAsync(model);
                await logProvider.AddToLogAsync($"Updated technological process information (Facility Id: {model.FacilityId}; Recipe Id: {model.RecipeId}).");
                return RedirectToRoute(FacilityTechProcessesRouting.Index, new { key = model.FacilityId });
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        /// <summary>
        /// Post action to delete tech process.
        /// </summary>
        /// <param name="model">Model of tech process containing key information (FacilityId and RecipeId) to find tech process</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpPost("delete", Name = FacilityTechProcessesRouting.DeleteSingleItem)]
        public async Task<IActionResult> DeleteSingleItem(TechProcess model)
        {
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync((model.FacilityId, model.RecipeId));
            if (statusMessage.IsCompleted)
            {
                await logic.RemoveDataModelAsync(model);
                await logProvider.AddToLogAsync($"Removed technological process (Facility Id: {model.FacilityId}; Recipe Id: {model.RecipeId}).");
                return RedirectToRoute(FacilityTechProcessesRouting.Index, new { key = model.FacilityId });
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }
    }
}
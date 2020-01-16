using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cursed.Models.DataModel.Companies;
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
    [Route("companies")]
    public class CompaniesController : Controller, ICUD<Company>, IReadColection, IReadSingle, IReadUpdateForm
    {
        private readonly CompaniesLogic logic;
        private readonly CompaniesLogicValidation logicValidation;
        private readonly ILogProvider<CursedAuthenticationContext> logProvider;
        public CompaniesController(CursedDataContext db, 
            [FromServices] IErrorHandlerFactory errorHandlerFactory,
            [FromServices] ILogProvider<CursedAuthenticationContext> logProvider)
        {
            logic = new CompaniesLogic(db);
            logicValidation = new CompaniesLogicValidation(db, errorHandlerFactory);
            this.logProvider = logProvider;
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpGet("", Name = CompaniesRouting.Index)]
        public async Task<IActionResult> Index(int currentPage = 1, int itemsOnPage = 20)
        {
            var model = await logic.GetAllDataModelAsync();

            var pagenationModel = new Pagenation<CompaniesModel>(model, itemsOnPage, currentPage);

            return View(pagenationModel);

        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpGet("company", Name = CompaniesRouting.SingleItem)]
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

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager)]
        [HttpGet("company/edit", Name = CompaniesRouting.GetEditSingleItem)]
        public async Task<IActionResult> GetEditSingleItem(string key)
        {
            if (key != null)
            {
                int id = Int32.Parse(key);
                ViewData["SaveRoute"] = CompaniesRouting.EditSingleItem;
                var statusMessage = await logicValidation.CheckGetSingleUpdateModelAsync(id);
                if(statusMessage.IsCompleted)
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
                ViewData["SaveRoute"] = CompaniesRouting.AddSingleItem;
                return View("EditSingleItem");
            }
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager)]
        [HttpPost("company/add", Name = CompaniesRouting.AddSingleItem)]
        public async Task<IActionResult> AddSingleItem(Company model)
        {
            var company = await logic.AddDataModelAsync(model);
            await logProvider.AddToLogAsync($"Added new company (Id: {company.Id}).");
            return RedirectToRoute(CompaniesRouting.Index);
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager)]
        [HttpPost("company/edit", Name = CompaniesRouting.EditSingleItem)]
        public async Task<IActionResult> EditSingleItem(Company model)
        {
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(model.Id);
            if (statusMessage.IsCompleted)
            {
                await logic.UpdateDataModelAsync(model);
                await logProvider.AddToLogAsync($"Updated company information (Id: {model.Id}).");
                return RedirectToRoute(CompaniesRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager)]
        [HttpPost("company/delete", Name = CompaniesRouting.DeleteSingleItem)]
        public async Task<IActionResult> DeleteSingleItem(string key)
        {
            int id = Int32.Parse(key);
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync(id);
            if(statusMessage.IsCompleted)
            {
                await logic.RemoveDataModelAsync(id);
                await logProvider.AddToLogAsync($"Removed company (Id: {key}).");
                return RedirectToRoute(CompaniesRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }
    }
}
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cursed.Models.DataModel.Recipes;
using Cursed.Models.Context;
using Cursed.Models.Entities.Data;
using Cursed.Models.Logic;
using Cursed.Models.Interfaces.ControllerCRUD;
using Cursed.Models.DataModel.Pagenation;
using Cursed.Models.StaticReferences.Routing;
using Cursed.Models.LogicValidation;
using Cursed.Models.Services;
using Cursed.Models.DataModel.Authorization;

namespace Cursed.Controllers
{
    [Route("recipes")]
    public class RecipesController : Controller, ICUD<Recipe>, IReadColection, IReadSingle, IReadUpdateForm
    {
        private readonly RecipesLogic logic;
        private readonly RecipesLogicValidation logicValidation;
        private readonly ILogProvider<CursedAuthenticationContext> logProvider;
        public RecipesController(CursedDataContext db, 
            [FromServices] IErrorHandlerFactory errorHandlerFactory,
            [FromServices] ILogProvider<CursedAuthenticationContext> logProvider)
        {
            logic = new RecipesLogic(db);
            logicValidation = new RecipesLogicValidation(db, errorHandlerFactory);
            this.logProvider = logProvider;
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist, AuthorizeRoles.GovermentAgent)]
        [HttpGet("", Name = RecipesRouting.Index)]
        public async Task<IActionResult> Index(int currentPage = 1, int itemsOnPage = 20)
        {
            var model = await logic.GetAllDataModelAsync();
            var pagenationModel = new Pagenation<RecipesModel>(model, itemsOnPage, currentPage);
            return View(pagenationModel);
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist, AuthorizeRoles.GovermentAgent)]
        [HttpGet("recipe", Name = RecipesRouting.SingleItem)]
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

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpGet("recipe/edit", Name = RecipesRouting.GetEditSingleItem)]
        public async Task<IActionResult> GetEditSingleItem(string key)
        {
            if (key != null)
            {
                int id = Int32.Parse(key);
                ViewData["SaveRoute"] = RecipesRouting.EditSingleItem;
                return await CheckSingleUpdateModelAndGetView(id);
            }
            else
            {
                ViewData["SaveRoute"] = RecipesRouting.AddSingleItem;
                return View("EditSingleItem");
            }
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpPost("recipe/add", Name = RecipesRouting.AddSingleItem)]
        public async Task<IActionResult> AddSingleItem(Recipe model)
        {
            var recipe = await logic.AddDataModelAsync(model);
            await logProvider.AddToLogAsync($"Added new recipe (Id: {recipe.Id}).");
            return RedirectToRoute(RecipesRouting.Index);
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpGet("recipe/add-child", Name = RecipesRouting.AddChildSingleItem)]
        public async Task<IActionResult> GetChildSingleItem(string key)
        {
            int id = Int32.Parse(key);
            ViewData["SaveRoute"] = RecipesRouting.AddChildSingleItem;
            ViewData["parentId"] = id;
            return await CheckSingleUpdateModelAndGetView(id);
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.SeniorTechnologist)]
        [HttpPost("recipe/technologist-approval", Name = RecipesRouting.InverseTechnologistApproval)]
        public async Task<IActionResult> InverseTechnologistApproval(string key)
        {
            int recipeId = Int32.Parse(key);
            await logic.InverseTechnologistApprovalAsync(recipeId);
            await logProvider.AddToLogAsync($"Recipe (Id: {recipeId}) changed technological approval state.");
            return RedirectToRoute(RecipesRouting.SingleItem, new { key = key });
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.GovermentAgent)]
        [HttpPost("recipe/goverment-approval", Name = RecipesRouting.InverseGovermentApproval)]
        public async Task<IActionResult> InverseGovermentApproval(string key)
        {
            int recipeId = Int32.Parse(key);
            await logic.InverseGovermentApprovalAsync(recipeId);
            await logProvider.AddToLogAsync($"Recipe (Id: {recipeId}) changed goverment approval state.");
            return RedirectToRoute(RecipesRouting.SingleItem, new { key = key });
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpPost("recipe/add-child", Name = RecipesRouting.AddChildSingleItem)]
        public async Task<IActionResult> AddChildSingleItem(Recipe model, int parentId)
        {
            var recipe = await logic.AddChildDataModelAsync(model, parentId);
            await logProvider.AddToLogAsync($"Added new recipe (Id: {recipe.Id}), which is child to recipe (Id: {parentId}).");
            return RedirectToRoute(RecipesRouting.Index);
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpPost("recipe/edit", Name = RecipesRouting.EditSingleItem)]
        public async Task<IActionResult> EditSingleItem(Recipe model)
        {
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(model.Id);
            if (statusMessage.IsCompleted)
            {
                await logic.UpdateDataModelAsync(model);
                await logProvider.AddToLogAsync($"Updated recipe information (Id: {model.Id}).");
                return RedirectToRoute(RecipesRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpPost("recipe/delete", Name = RecipesRouting.DeleteSingleItem)]
        public async Task<IActionResult> DeleteSingleItem(string key)
        {
            int id = Int32.Parse(key);
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync(id);
            if (statusMessage.IsCompleted)
            {
                await logic.RemoveDataModelAsync(id);
                await logProvider.AddToLogAsync($"Removed recipe (Id: {key}).");
                return RedirectToRoute(RecipesRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        private async Task<ViewResult> CheckSingleUpdateModelAndGetView(int id)
        {
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
    }
}
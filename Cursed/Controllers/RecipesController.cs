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
    /// <summary>
    /// Recipes section controller. Consists of CRUD actions for recipes, including read action for
    /// both single recipe and collection of all recipes.
    /// </summary>
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

        /// <summary>
        /// Main page of section, contains consolidated collection of recipes. 
        /// Can be navigated through pagenation.
        /// </summary>
        /// <param name="currentPage">Defines which portion of items from collection, will be shown</param>
        /// <param name="itemsOnPage">Defines how many item there will be in a portion</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist, AuthorizeRoles.GovermentAgent)]
        [HttpGet("", Name = RecipesRouting.Index)]
        public async Task<IActionResult> Index(int currentPage = 1, int itemsOnPage = 20)
        {
            var model = await logic.GetAllDataModelAsync();

            // form pagenation model
            var pagenationModel = new Pagenation<RecipesModel>(model, itemsOnPage, currentPage);

            return View(pagenationModel);
        }

        /// <summary>
        /// Displays a single recipe, which found by <c>key</c>
        /// </summary>
        /// <param name="key">Id of recipe to be found</param>
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

        /// <summary>
        /// Display a page with form to update/add new recipe.
        /// </summary>
        /// <param name="key">Id of recipe to be edited, if null - considered that recipe added insted of edited</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpGet("recipe/edit", Name = RecipesRouting.GetEditSingleItem)]
        public async Task<IActionResult> GetEditSingleItem(string key)
        {
            // add distincted from edit, by presence of key parameter
            // further on they distincted by ViewData[SaveRoute]
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

        /// <summary>
        /// Post action to add new recipe.
        /// </summary>
        /// <param name="model">Recipe to be added</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpPost("recipe/add", Name = RecipesRouting.AddSingleItem)]
        public async Task<IActionResult> AddSingleItem(Recipe model)
        {
            var recipe = await logic.AddDataModelAsync(model);
            await logProvider.AddToLogAsync($"Added new recipe (Id: {recipe.Id}).");
            return RedirectToRoute(RecipesRouting.Index);
        }

        /// <summary>
        /// Display a page with form to add new recipe as child to current recipe.
        /// </summary>
        /// <param name="key">Id of recipe to be parent of new recipe</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpGet("recipe/add-child", Name = RecipesRouting.AddChildSingleItem)]
        public async Task<IActionResult> GetChildSingleItem(string key)
        {
            int id = Int32.Parse(key);
            ViewData["SaveRoute"] = RecipesRouting.AddChildSingleItem;
            ViewData["parentId"] = id;
            return await CheckSingleUpdateModelAndGetView(id);
        }

        /// <summary>
        /// Post action to change recipe technologist approval status.
        /// </summary>
        /// <param name="key">Id of recipe to be found</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.SeniorTechnologist)]
        [HttpPost("recipe/technologist-approval", Name = RecipesRouting.InverseTechnologistApproval)]
        public async Task<IActionResult> InverseTechnologistApproval(string key)
        {
            int recipeId = Int32.Parse(key);
            await logic.InverseTechnologistApprovalAsync(recipeId);
            await logProvider.AddToLogAsync($"Recipe (Id: {recipeId}) changed technological approval state.");
            return RedirectToRoute(RecipesRouting.SingleItem, new { key = key });
        }

        /// <summary>
        /// Post action to change recipe goverment approval status.
        /// </summary>
        /// <param name="key">Id of recipe to be found</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.GovermentAgent)]
        [HttpPost("recipe/goverment-approval", Name = RecipesRouting.InverseGovermentApproval)]
        public async Task<IActionResult> InverseGovermentApproval(string key)
        {
            int recipeId = Int32.Parse(key);
            await logic.InverseGovermentApprovalAsync(recipeId);
            await logProvider.AddToLogAsync($"Recipe (Id: {recipeId}) changed goverment approval state.");
            return RedirectToRoute(RecipesRouting.SingleItem, new { key = key });
        }

        /// <summary>
        /// Post action to add new recipe, as child to existing recipe.
        /// </summary>
        /// <param name="model">Recipe to be added</param>
        /// <param name="parentId">Id of recipe, which will be registered as parent</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpPost("recipe/add-child", Name = RecipesRouting.AddChildSingleItem)]
        public async Task<IActionResult> AddChildSingleItem(Recipe model, int parentId)
        {
            var statusMessage = await logicValidation.CheckGetSingleDataModelAsync(parentId);
            if (statusMessage.IsCompleted)
            {
                var recipe = await logic.AddChildDataModelAsync(model, parentId);
                await logProvider.AddToLogAsync($"Added new recipe (Id: {recipe.Id}), which is child to recipe (Id: {parentId}).");
                return RedirectToRoute(RecipesRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        /// <summary>
        /// Post action to update recipe.
        /// </summary>
        /// <param name="model">Updated recipe information</param>
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

        /// <summary>
        /// Post action to delete recipe.
        /// </summary>
        /// <param name="key">Id of recipe to be deleted</param>
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

        /// <summary>
        /// Used to check if recipe with <c>id</c> is valid to get for edit
        /// </summary>
        /// <param name="id">Id of recipe to be found</param>
        /// <returns>EditSingleItem view, if recipe valid and CustomError view otherwise</returns>
        private async Task<IActionResult> CheckSingleUpdateModelAndGetView(int id)
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
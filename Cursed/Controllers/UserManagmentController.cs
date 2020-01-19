using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cursed.Models.Context;
using Cursed.Models.Logic;
using Cursed.Models.DataModel.Pagenation;
using Cursed.Models.Interfaces.ControllerCRUD;
using Cursed.Models.StaticReferences.Routing;
using Cursed.Models.Services;
using Cursed.Models.LogicValidation;
using Cursed.Models.DataModel.Authorization;
using Cursed.Models.DataModel.Authentication;
using Cursed.Models.Entities.Authentication;
using Microsoft.AspNetCore.Http;

namespace Cursed.Controllers
{
    /// <summary>
    /// User managment section controller. Consists of CRUD actions for users, including read action for
    /// both single user and collection of all users.
    /// </summary>
    [Route("user-managment")]
    public class UserManagmentController : Controller, IReadColection, IReadSingle, IReadUpdateForm, ICreate<RegistrationModel>, IUpdate<UserData>, IUpdate<UserAuthUpdateModel>, IDeleteByKey
    {
        private readonly UserManagmentLogic logic;
        private readonly UserManagmentLogicValidation logicValidation;
        private readonly ILogProvider<CursedAuthenticationContext> logProvider;

        public UserManagmentController(CursedAuthenticationContext db, 
            [FromServices] IGenPasswordHash genPasswordHash, 
            [FromServices] IErrorHandlerFactory errorHandlerFactory, 
            [FromServices] IHttpContextAccessor contextAccessor,
            [FromServices] ILogProvider<CursedAuthenticationContext> logProvider)
        {
            logic = new UserManagmentLogic(db, genPasswordHash);
            logicValidation = new UserManagmentLogicValidation(db, errorHandlerFactory, contextAccessor, genPasswordHash);
            this.logProvider = logProvider;
        }

        /// <summary>
        /// Main page of section, contains consolidated collection of users. 
        /// Can be navigated through pagenation.
        /// </summary>
        /// <param name="currentPage">Defines which portion of items from collection, will be shown</param>
        /// <param name="itemsOnPage">Defines how many item there will be in a portion</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator)]
        [HttpGet("", Name = UserManagmentRouting.Index)]
        public async Task<IActionResult> Index(int currentPage = 1, int itemsOnPage = 20)
        {
            var model = await logic.GetAllDataModelAsync();

            // form pagenation model
            var pagenationModel = new Pagenation<UserData>(model, itemsOnPage, currentPage);

            return View(pagenationModel);
        }

        /// <summary>
        /// Displays a single user, which found by <c>key</c>
        /// </summary>
        /// <param name="key">Id of user to be found</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator)]
        [HttpGet("user", Name = UserManagmentRouting.SingleItem)]
        public async Task<IActionResult> SingleItem(string key)
        {
            var statusMessage = await logicValidation.CheckGetSingleDataModelAsync(key);
            if (statusMessage.IsCompleted)
            {
                var model = await logic.GetSingleDataModelAsync(key);
                return View(model);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        /// <summary>
        /// Display a page with form to update/add new user.
        /// </summary>
        /// <param name="key">Id of user to be edited, if null - considered that user added insted of edited</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator)]
        [HttpGet("user/edit", Name = UserManagmentRouting.GetEditSingleItem)]
        public async Task<IActionResult> GetEditSingleItem(string key)
        {
            // add distincted from edit, by presence of key parameter
            // further on they distincted by ViewData[SaveRoute]
            if (key != null)
            {
                ViewData["SaveRoute"] = UserManagmentRouting.GetEditSingleItem;
                var statusMessage = await logicValidation.CheckGetSingleUpdateModelAsync(key);
                if (statusMessage.IsCompleted)
                {
                    var model = await logic.GetSingleUpdateModelAsync(key);
                    return View("EditSingleItem", new UserDataUpdateModel(model));
                }
                else
                {
                    return View("CustomError", statusMessage);
                }
            }
            else
            {
                return View("AddSingleItem");
            }

        }

        /// <summary>
        /// Post action to add new user.
        /// </summary>
        /// <param name="model">User to be added</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator)]
        [HttpPost("user/add", Name = UserManagmentRouting.AddSingleItem)]
        public async Task<IActionResult> AddSingleItem(RegistrationModel model)
        {
            var statusMessage = await logicValidation.CheckAddSingleDataModelAsync(model.Login, model.RoleName);
            if (statusMessage.IsCompleted)
            {
                var user = await logic.AddDataModelAsync(model);
                await logProvider.AddToLogAsync($"Added new user (Login: {user.Login}).");
                return RedirectToRoute(UserManagmentRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        /// <summary>
        /// Post action to update user data.
        /// </summary>
        /// <param name="model">Updated user data</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator)]
        [HttpPost("user/edit-auth", Name = UserManagmentRouting.EditUserAuth)]
        public async Task<IActionResult> EditSingleItem(UserAuthUpdateModel model)
        {
            var statusMessage = await logicValidation.CheckUpdateUserAuthUpdateModelAsync(model.Login, model.PasswordOld);
            if (statusMessage.IsCompleted)
            {
                await logic.UpdateDataModelAsync(model);
                await logProvider.AddToLogAsync($"Updated user information (Id: {model.Login}).");
                return RedirectToRoute(UserManagmentRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        /// <summary>
        /// Post action to update user security data.
        /// </summary>
        /// <param name="model">Updated user security data</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator)]
        [HttpPost("user/edit-data", Name = UserManagmentRouting.EditUserData)]
        public async Task<IActionResult> EditSingleItem(UserData model)
        {
            var statusMessage = await logicValidation.CheckUpdateUserDataUpdateModelAsync(model.Login, model.RoleName);
            if (statusMessage.IsCompleted)
            {
                await logic.UpdateDataModelAsync(model);
                await logProvider.AddToLogAsync($"Updated user security information (Id: {model.Login}).");
                return RedirectToRoute(UserManagmentRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        /// <summary>
        /// Post action to delete user.
        /// </summary>
        /// <param name="key">Id of user to be deleted</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator)]
        [HttpPost("user/delete", Name = UserManagmentRouting.DeleteSingleItem)]
        public async Task<IActionResult> DeleteSingleItem(string key)
        {
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync(key);
            if (statusMessage.IsCompleted)
            {
                await logic.RemoveDataModelAsync(key);
                await logProvider.AddToLogAsync($"Removed user (Login: {key}).");
                return RedirectToRoute(UserManagmentRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }
    }
}
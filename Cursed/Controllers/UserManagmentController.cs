using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cursed.Models.DataModel.ProductsCatalog;
using Cursed.Models.Context;
using Cursed.Models.Entities;
using Cursed.Models.Logic;
using Cursed.Models.DataModel.Utility;
using Cursed.Models.Interfaces.ControllerCRUD;
using Cursed.Models.StaticReferences.Routing;
using Cursed.Models.Services;
using Cursed.Models.LogicValidation;
using Microsoft.AspNetCore.Authorization;
using Cursed.Models.DataModel.Utility.Authorization;
using Cursed.Models.DataModel.Authentication;
using Cursed.Models.Entities.Authentication;
using Microsoft.AspNetCore.Http;

namespace Cursed.Controllers
{
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

        [AuthorizeRoles(AuthorizeRoles.Administrator)]
        [HttpGet("", Name = UserManagmentRouting.Index)]
        public async Task<IActionResult> Index(int currentPage = 1, int itemsOnPage = 20)
        {
            var model = await logic.GetAllDataModelAsync();
            var pagenationModel = new Pagenation<UserData>(model, itemsOnPage, currentPage);

            return View(pagenationModel);
        }

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

        [AuthorizeRoles(AuthorizeRoles.Administrator)]
        [HttpGet("user/edit", Name = UserManagmentRouting.GetEditSingleItem)]
        public async Task<IActionResult> GetEditSingleItem(string key)
        {
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

        [AuthorizeRoles(AuthorizeRoles.Administrator)]
        [HttpPost("user/edit-data", Name = UserManagmentRouting.EditUserAuth)]
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

        [AuthorizeRoles(AuthorizeRoles.Administrator)]
        [HttpPost("user/edit-auth", Name = UserManagmentRouting.EditUserData)]
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
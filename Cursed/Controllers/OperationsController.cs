using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cursed.Models.Context;
using Cursed.Models.Entities.Data;
using Cursed.Models.Logic;
using Cursed.Models.Interfaces.ControllerCRUD;
using Cursed.Models.StaticReferences.Routing;
using Cursed.Models.LogicValidation;
using Cursed.Models.Services;
using Cursed.Models.DataModel.Authorization;

namespace Cursed.Controllers
{
    /// <summary>
    /// Operations section controller. Consists of CUD actions for operations. Read actions
    /// handled through transactions section.
    /// </summary>
    [Route("transactions/transaction")]
    public class OperationsController : Controller, IReadUpdateForm, ICreate<Operation>, IUpdate<Operation>, IDeleteByModel<Operation>
    {
        private readonly OperationsLogic logic;
        private readonly OperationsLogicValidation logicValidation;
        private readonly ILogProvider<CursedAuthenticationContext> logProvider;
        public OperationsController(CursedDataContext db, 
            [FromServices] IErrorHandlerFactory errorHandlerFactory,
            [FromServices] ILogProvider<CursedAuthenticationContext> logProvider)
        {
            logic = new OperationsLogic(db);
            logicValidation = new OperationsLogicValidation(db, errorHandlerFactory);
            this.logProvider = logProvider;
        }

        /// <summary>
        /// Display a page with form to update operation.
        /// </summary>
        /// <param name="key">Id of operation to be edit</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager)]
        [HttpGet("operation/edit", Name = OperationsRouting.GetEditSingleItem)]
        public async Task<IActionResult> GetEditSingleItem(string key)
        {
            int id = Int32.Parse(key);
            var statusMessage = await logicValidation.CheckGetSingleUpdateModelAsync(id);

            if (statusMessage.IsCompleted)
            {
                if (statusMessage.IsCompleted)
                {
                    var model = await logic.GetSingleUpdateModelAsync(id);
                    return View("EditSingleItem", model);
                }
            }
            return View("CustomError", statusMessage);
        }

        /// <summary>
        /// Post action to add new operation.
        /// </summary>
        /// <param name="model">Operation to be added</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager)]
        [HttpPost("operation/add", Name = OperationsRouting.AddSingleItem)]
        public async Task<IActionResult> AddSingleItem(Operation model)
        {
            var operation = await logic.AddDataModelAsync(model);
            await logProvider.AddToLogAsync($"Added new operation (Id: {operation.Id}).");
            return RedirectToRoute(TransactionsRouting.SingleItem, new { key = model.TransactionId });
        }

        /// <summary>
        /// Post action to update operation.
        /// </summary>
        /// <param name="model">Updated operation information</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager)]
        [HttpPost("operation/edit", Name = OperationsRouting.EditSingleItem)]
        public async Task<IActionResult> EditSingleItem(Operation model)
        {
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(model.Id);
            if (statusMessage.IsCompleted)
            {
                await logic.UpdateDataModelAsync(model);
                await logProvider.AddToLogAsync($"Updated operation information (Id: {model.Id}).");
                return RedirectToRoute(TransactionsRouting.SingleItem, new { key = model.TransactionId });
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        /// <summary>
        /// Post action to delete operation.
        /// </summary>
        /// <param name="model">Model of operation containing key information (Id) to find operation and transaction information (TransactionId) for further redirect</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager)]
        [HttpPost("operation/delete", Name = OperationsRouting.DeleteSingleItem)]
        public async Task<IActionResult> DeleteSingleItem(Operation model)
        {
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync(model.Id);
            if (statusMessage.IsCompleted)
            {
                await logic.RemoveDataModelAsync(model);
                await logProvider.AddToLogAsync($"Removed operation (Id: {model.Id}).");
                return RedirectToRoute(TransactionsRouting.SingleItem, new { key = model.TransactionId });
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }
    }
}
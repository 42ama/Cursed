using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cursed.Models.DataModel.Transactions;
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
    /// Transactions section controller. Consists of CRUD actions for transactions, including read action for
    /// both single transaction and collection of all transactions. Read action for single transaction, also
    /// provides view of all related operations.
    /// </summary>
    [Route("transactions")]
    public class TransactionsController : Controller, ICUD<TransactionBatch>, IReadColection, IReadSingle, IReadUpdateForm
    {
        private readonly TransactionsLogic logic;
        private readonly TransactionsLogicValidation logicValidation;
        private readonly ILogProvider<CursedAuthenticationContext> logProvider;
        public TransactionsController(CursedDataContext db, 
            [FromServices] IOperationDataValidation operationValidation, 
            [FromServices] IErrorHandlerFactory errorHandlerFactory,
            [FromServices] ILogProvider<CursedAuthenticationContext> logProvider)
        {
            logic = new TransactionsLogic(db);
            logicValidation = new TransactionsLogicValidation(db, operationValidation, errorHandlerFactory);
            this.logProvider = logProvider;
        }

        /// <summary>
        /// Main page of section, contains consolidated collection of transactions. 
        /// Can be navigated through pagenation.
        /// </summary>
        /// <param name="currentPage">Defines which portion of items from collection, will be shown</param>
        /// <param name="itemsOnPage">Defines how many item there will be in a portion</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpGet("", Name = TransactionsRouting.Index)]
        public async Task<IActionResult> Index(int currentPage = 1, int itemsOnPage = 20)
        {
            var model = await logic.GetAllDataModelAsync();

            // form pagenation model
            var pagenationModel = new Pagenation<TransactionsModel>(model, itemsOnPage, currentPage);

            return View(pagenationModel);
        }

        /// <summary>
        /// Displays a single transaction, which found by <c>key</c>
        /// </summary>
        /// <param name="key">Id of transaction to be found</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpGet("transaction", Name = TransactionsRouting.SingleItem)]
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
        /// Display a page with form to update/add new transaction.
        /// </summary>
        /// <param name="key">Id of transaction to be edited, if null - considered that transaction added insted of edited</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager)]
        [HttpGet("transaction/edit", Name = TransactionsRouting.GetEditSingleItem)]
        public async Task<IActionResult> GetEditSingleItem(string key)
        {
            // add distincted from edit, by presence of key parameter
            // further on they distincted by ViewData[SaveRoute]
            if (key != null)
            {
                int id = Int32.Parse(key);
                ViewData["SaveRoute"] = TransactionsRouting.EditSingleItem;
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
                ViewData["SaveRoute"] = TransactionsRouting.AddSingleItem;
                return View("EditSingleItem");
            }
        }

        /// <summary>
        /// Post action to close transaction. Which mean apply all operations record in it to database.
        /// </summary>
        /// <param name="key">Id of transaction to be found</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager)]
        [HttpPost("transaction/close", Name = TransactionsRouting.CloseTransaction)]
        public async Task<IActionResult> CloseTransaction(string key)
        {
            int id = Int32.Parse(key);
            var statusMessage = await logicValidation.CheckCloseTransactionAsync(id);
            if (statusMessage.IsCompleted)
            {
                await logic.CloseTransactionAsync(id);
                await logProvider.AddToLogAsync($"Closed transaction (Id: {key}).");
                return RedirectToRoute(TransactionsRouting.SingleItem, new { key });
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        /// <summary>
        /// Post action to open transaction. Which mean undo all operations record in it to database.
        /// </summary>
        /// <param name="key">Id of transaction to be found</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager)]
        [HttpPost("transaction/open", Name = TransactionsRouting.OpenTransaction)]
        public async Task<IActionResult> OpenTransaction(string key)
        {
            int id = Int32.Parse(key);
            var statusMessage = await logicValidation.CheckOpenTransactionAsync(id);
            if (statusMessage.IsCompleted)
            {
                await logic.OpenTransactionAsync(id);
                await logProvider.AddToLogAsync($"Opened transaction (Id: {key}).");
                return RedirectToRoute(TransactionsRouting.SingleItem, new { key });
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        /// <summary>
        /// Post action to add new transaction.
        /// </summary>
        /// <param name="model">Transaction to be added</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager)]
        [HttpPost("transaction/add", Name = TransactionsRouting.AddSingleItem)]
        public async Task<IActionResult> AddSingleItem(TransactionBatch model)
        {
            var statusMessage = await logicValidation.CheckAddDataModelAsync(model);
            if (statusMessage.IsCompleted)
            {
                var transaction = await logic.AddDataModelAsync(model);
                await logProvider.AddToLogAsync($"Added new transaction (Id: {transaction.Id}).");
                return RedirectToRoute(TransactionsRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        /// <summary>
        /// Post action to update transaction.
        /// </summary>
        /// <param name="model">Updated transaction information</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager)]
        [HttpPost("transaction/edit", Name = TransactionsRouting.EditSingleItem)]
        public async Task<IActionResult> EditSingleItem(TransactionBatch model)
        {
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(model);
            if (statusMessage.IsCompleted)
            {
                await logic.UpdateDataModelAsync(model);
                await logProvider.AddToLogAsync($"Updated transaction information (Id: {model.Id}).");
                return RedirectToRoute(TransactionsRouting.SingleItem, new { key = model.Id });
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        /// <summary>
        /// Post action to delete transaction.
        /// </summary>
        /// <param name="key">Id of transaction to be deleted</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager)]
        [HttpPost("transaction/delete", Name = TransactionsRouting.DeleteSingleItem)]
        public async Task<IActionResult> DeleteSingleItem(string key)
        {
            int id = Int32.Parse(key);
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync(id);
            if (statusMessage.IsCompleted)
            {
                await logic.RemoveDataModelAsync(id);
                await logProvider.AddToLogAsync($"Removed transaction (Id: {key}).");
                return RedirectToRoute(TransactionsRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }
    }
}
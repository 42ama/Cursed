using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cursed.Models.Data.Transactions;
using Cursed.Models.Data.Shared;
using Cursed.Models.Context;
using Cursed.Models.Entities;
using Cursed.Models.Logic;
using Cursed.Models.Interfaces.ControllerCRUD;
using Cursed.Models.Data.Utility;
using Cursed.Models.Routing;
using Cursed.Models.LogicValidation;
using Cursed.Models.Services;

namespace Cursed.Controllers
{
    [Route("transactions")]
    public class TransactionsController : Controller, ICUD<TransactionBatch>, IReadColection, IReadSingle, IReadUpdateForm
    {
        private readonly TransactionsLogic logic;
        private readonly TransactionsLogicValidation logicValidation;
        public TransactionsController(CursedContext db, [FromServices] IOperationDataValidation operationValidation, [FromServices] IErrorHandlerFactory errorHandlerFactory)
        {
            logic = new TransactionsLogic(db);
            logicValidation = new TransactionsLogicValidation(db, operationValidation, errorHandlerFactory);
        }

        [HttpGet("", Name = TransactionsRouting.Index)]
        public async Task<IActionResult> Index(int currentPage = 1, int itemsOnPage = 20)
        {
            var model = await logic.GetAllDataModelAsync();
            var pagenationModel = new Pagenation<TransactionsModel>(model, itemsOnPage, currentPage);
            return View(pagenationModel);
        }

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

        [HttpGet("transaction/edit", Name = TransactionsRouting.GetEditSingleItem)]
        public async Task<IActionResult> GetEditSingleItem(string key)
        {
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

        [HttpPost("transaction/close", Name = TransactionsRouting.CloseTransaction)]
        public async Task<IActionResult> CloseTransaction(string key)
        {
            int id = Int32.Parse(key);
            var statusMessage = await logicValidation.CheckCloseTransactionAsync(id);
            if (statusMessage.IsCompleted)
            {
                await logic.CloseTransactionAsync(id);
                return RedirectToRoute(TransactionsRouting.SingleItem, new { key });
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        [HttpPost("transaction/open", Name = TransactionsRouting.OpenTransaction)]
        public async Task<IActionResult> OpenTransaction(string key)
        {
            int id = Int32.Parse(key);
            var statusMessage = await logicValidation.CheckOpenTransactionAsync(id);
            if (statusMessage.IsCompleted)
            {
                await logic.OpenTransactionAsync(id);
                return RedirectToRoute(TransactionsRouting.SingleItem, new { key });
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        [HttpPost("transaction/add", Name = TransactionsRouting.AddSingleItem)]
        public async Task<IActionResult> AddSingleItem(TransactionBatch model)
        {
            await logic.AddDataModelAsync(model);
            return RedirectToRoute(TransactionsRouting.Index);
        }

        [HttpPost("transaction/edit", Name = TransactionsRouting.EditSingleItem)]
        public async Task<IActionResult> EditSingleItem(TransactionBatch model)
        {
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(model.Id);
            if (statusMessage.IsCompleted)
            {
                await logic.UpdateDataModelAsync(model);
                return RedirectToRoute(TransactionsRouting.SingleItem, new { key = model.Id });
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        [HttpPost("transaction/delete", Name = TransactionsRouting.DeleteSingleItem)]
        public async Task<IActionResult> DeleteSingleItem(string key)
        {
            int id = Int32.Parse(key);
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync(id);
            if (statusMessage.IsCompleted)
            {
                await logic.RemoveDataModelAsync(id);
                return RedirectToRoute(TransactionsRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }
    }
}
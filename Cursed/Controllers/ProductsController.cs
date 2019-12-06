using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cursed.Models.Data.Products;
using Cursed.Models.Data.Shared;
using Cursed.Models.Context;
using Cursed.Models.Entities;
using Cursed.Models.Logic;
using Cursed.Models.Routing;
using Cursed.Models.Interfaces.ControllerCRUD;
using Cursed.Models.Data.Utility;

namespace Cursed.Controllers
{
    [Route("products")]
    public class ProductsController : Controller, IReadCollectionByParam
    {
        private readonly ProductsLogic logic;
        public ProductsController(CursedContext db)
        {
            logic = new ProductsLogic(db);
        }

        [HttpGet("", Name = ProductsRouting.Index)]
        public async Task<IActionResult> Index(string key, int currentPage = 1, int itemsOnPage = 20)
        {
            int storageId = Int32.Parse(key);
            ViewData["StorageId"] = storageId;
            var statusMessage = await logic.GetAllDataModelAsync(storageId);
            if(statusMessage.IsCompleted)
            {
                var pagenationModel = new Pagenation<ProductsDataModel>(statusMessage.ReturnValue, itemsOnPage, currentPage);
                return View(pagenationModel);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }
    }
}
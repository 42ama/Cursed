using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cursed.Models.Data.ProductsCatalog;
using Cursed.Models.Context;
using Cursed.Models.Entities;
using Cursed.Models.Logic;
using Cursed.Models.Data.Utility;
using Cursed.Models;
using Cursed.Models.Routing;

namespace Cursed.Controllers
{
    [Route("products-catalog")]
    public class ProductsCatalogController : Controller, IControllerRESTAsync<ProductCatalog>
    {
        private readonly ILogger<ProductsCatalogController> _logger;
        private readonly ProductCatalogLogic logic;

        public ProductsCatalogController(CursedContext db, ILogger<ProductsCatalogController> logger)
        {
            logic = new ProductCatalogLogic(db);
            _logger = logger;
        }

        [HttpGet("", Name = ProductsCatalogRouting.Index)]
        public async Task<IActionResult> Index(int currentPage = 1, int itemsOnPage = 20)
        {
            var model = await logic.GetAllDataModelAsync();
            var pagenationModel = new Pagenation<ProductsCatalogModel>(model, itemsOnPage,currentPage);

            return View(pagenationModel);
        }

        [HttpGet("product", Name = ProductsCatalogRouting.SingleItem)]
        public async Task<IActionResult> SingleItem(string key)
        {
            int id = Int32.Parse(key);
            var model = await logic.GetSingleDataModelAsync(id);
            return View(model);
        }

        // get for add/edit form
        [HttpGet("product/edit", Name = ProductsCatalogRouting.GetEditSingleItem)]
        public async Task<IActionResult> GetEditSingleItem(string key)
        {
            if(key != null)
            {
                int id = Int32.Parse(key);
                ViewData["SaveRoute"] = ProductsCatalogRouting.EditSingleItem;
                var model = await logic.GetSingleUpdateModelAsync(id);
                return View("EditSingleItem", model);
            }
            else
            {
                ViewData["SaveRoute"] = ProductsCatalogRouting.AddSingleItem;
                return View("EditSingleItem");
            }
            
        }

        //post item
        [HttpPost("product/add", Name = ProductsCatalogRouting.AddSingleItem)]
        public async Task<IActionResult> AddSingleItem(ProductCatalog model)
        {
            await logic.AddDataModelAsync(model);
            return RedirectToRoute(ProductsCatalogRouting.Index);
        }

        //put item
        [HttpPost("product/edit", Name = ProductsCatalogRouting.EditSingleItem)]
        public async Task<IActionResult> EditSingleItem(ProductCatalog model)
        {
            await logic.UpdateDataModelAsync(model);
            return RedirectToRoute(ProductsCatalogRouting.Index);
        }

        //delete item
        [HttpPost("product/delete", Name = ProductsCatalogRouting.DeleteSingleItem)]
        public async Task<IActionResult> DeleteSingleItem(string key)
        {
            int id = Int32.Parse(key);
            await logic.RemoveDataModelAsync(id);
            return RedirectToRoute(ProductsCatalogRouting.Index);
        }
    }
}

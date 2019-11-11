using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cursed.Models.Data.ProductCatalog;
using Cursed.Models.Context;
using Cursed.Models.Entities;
using Cursed.Models.Logic;
using Cursed.Models.Data.Utility;
using Cursed.Models;

namespace Cursed.Controllers
{
    [Route("product-catalog")]
    public class ProductCatalogController : Controller, IControllerRESTAsync<ProductCatalog>
    {
        private readonly ILogger<ProductCatalogController> _logger;
        private readonly ProductCatalogLogic logic;

        public ProductCatalogController(CursedContext db, ILogger<ProductCatalogController> logger)
        {
            logic = new ProductCatalogLogic(db);
            _logger = logger;
        }

        [HttpGet("", Name = "ProductCatalogAll")]
        public async Task<IActionResult> Index(int currentPage = 1, int itemsOnPage = 20)
        {
            var model = await logic.GetAllDataModelAsync();
            var pagenationModel = new Pagenation<ProductCatalogAllModel>(model, itemsOnPage,currentPage);

            return View(pagenationModel);
        }

        [HttpGet("product", Name = "ProductCatalogSingle")]
        public async Task<IActionResult> SingleItem(string key)
        {
            int id = Int32.Parse(key);
            var model = await logic.GetSingleDataModelAsync(id);
            return View(model);
        }

        // get for add/edit form
        [HttpGet("product/edit", Name = "GetForEditProductCatalogSingle")]
        public async Task<IActionResult> GetEditSingleItem(string key)
        {
            if(key != null)
            {
                int id = Int32.Parse(key);
                ViewData["SaveRoute"] = "EditProductCatalogSingle";
                var model = await logic.GetSingleUpdateModelAsync(id);
                return View("EditSingleItem", model);
            }
            else
            {
                ViewData["SaveRoute"] = "AddProductCatalogSingle";
                return View("EditSingleItem");
            }
            
        }

        //post item
        [HttpPost("product/add", Name = "AddProductCatalogSingle")]
        public async Task<IActionResult> AddSingleItem(ProductCatalog model)
        {
            await logic.AddDataModelAsync(model);
            return RedirectToRoute("ProductCatalogAll");
        }

        //put item
        [HttpPost("product/edit", Name = "EditProductCatalogSingle")]
        public async Task<IActionResult> EditSingleItem(ProductCatalog model)
        {
            await logic.UpdateDataModelAsync(model);
            return RedirectToRoute("ProductCatalogAll");
        }

        //delete item
        [HttpPost("product/delete", Name = "DeleteProductCatalogSingle")]
        public async Task<IActionResult> DeleteSingleItem(string key)
        {
            int id = Int32.Parse(key);
            await logic.RemoveDataModelAsync(id);
            return RedirectToRoute("ProductCatalogAll");
        }
    }
}

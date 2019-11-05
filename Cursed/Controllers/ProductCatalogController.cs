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
            var dataModel = await logic.GetAllDataModelAsync();
            List<ProductCatalogAllVM> viewModel = new List<ProductCatalogAllVM>();
            foreach (var item in dataModel)
            {
                var viewItem = new ProductCatalogAllVM
                {
                    ProductId = item.ProductId,
                    Name = item.Name,
                    CAS = (item.CAS ?? 0).ToString(),
                    Type = item.Type,
                    RecipesCount = item.RecipesCount,
                    StoragesCount = item.StoragesCount,
                    LicenseRequired = item.LicenseRequired,
                    LicenseId = item.License?.Id ?? -1,
                    Date = item.License?.Date.ToShortDateString() ?? "---",
                    GovermentNum = item.License?.GovermentNum ?? -1
                };
                
                if(item.LicenseRequired)
                {
                    if(item.License?.IsValid == true)
                    {
                        viewItem.IsValid = true;
                    }
                    else
                    {
                        viewItem.IsValid = false;
                    }
                }

                viewModel.Add(viewItem);
            }
            var pagenationModel = new Pagenation<ProductCatalogAllVM>(viewModel, itemsOnPage,currentPage);

            return View(pagenationModel);
        }

        [HttpGet("product", Name = "ProductCatalogSingle")]
        public async Task<IActionResult> SingleItem(int id)
        {
            var dataModel = await logic.GetSingleDataModelAsync(id);
            var viewModel = new ProductCatalogSingleVM
            {
                ProductId = dataModel.ProductId,
                Name = dataModel.Name,
                CAS = (dataModel.CAS ?? 0).ToString(),
                Type = dataModel.Type,
                LicenseRequired = dataModel.LicenseRequired,
                Licenses = dataModel.Licenses,
                Recipes = dataModel.Recipes,
                Storages = dataModel.Storages
            };
            return View(viewModel);
        }

        // get for add/edit form
        [HttpGet("product/edit", Name = "GetForEditProductCatalogSingle")]
        public async Task<IActionResult> GetEditSingleItem(int? id)
        {
            if(id.HasValue)
            {
                var model = await logic.GetSingleUpdateModelAsync(id);
                return View("EditSingleItem", model);
            }
            else
            {
                return View("EditSingleItem");
            }
            
        }

        //post item
        [HttpPost("product", Name = "ProductCatalogSingle")]
        public async Task<IActionResult> AddSingleItem(ProductCatalog model)
        {
            await logic.AddDataModelAsync(model);
            return RedirectToRoute("ProductCatalogAll");
        }

        //put item
        [HttpPut("product", Name = "ProductCatalogSingle")]
        public async Task<IActionResult> EditSingleItem(ProductCatalog model)
        {
            await logic.UpdateDataModelAsync(model);
            return RedirectToRoute("ProductCatalogAll");
        }

        //delete item
        [HttpDelete("product", Name = "ProductCatalogSingle")]
        public async Task<IActionResult> DeleteSingleItem(int id)
        {
            await logic.RemoveDataModelAsync(new ProductCatalog { Id = id });
            return RedirectToRoute("ProductCatalogAll");
        }
    }
}

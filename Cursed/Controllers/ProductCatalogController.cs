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

namespace Cursed.Controllers
{
    [Route("product-catalog")]
    public class ProductCatalogController : Controller
    {
        private readonly ILogger<ProductCatalogController> _logger;
        private readonly ProductCatalogLogic logic;

        public ProductCatalogController(CursedContext db, ILogger<ProductCatalogController> logger)
        {
            logic = new ProductCatalogLogic(db);
            _logger = logger;
        }

        [HttpGet("", Name = "ProductCatalogAll")]
        public async Task<IActionResult> Index()
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
                    StoragesCount = item.StoragesCount
                };
                if(item.License != null)
                {
                    viewItem.LicensedUntil = item.License.Date.ToShortDateString();
                    viewItem.GovermentNum = item.License.GovermentNum.ToString();
                }
                else
                {
                    viewItem.LicensedUntil = "---";
                    viewItem.GovermentNum = "---";
                }
                if (item.LicenseRequired == true)
                {
                    if(item.License?.IsValid == true)
                    {
                        viewItem.LicenseSummary = "Valid";
                        viewItem.AttentionColor = "green";
                    }
                    else
                    {
                        viewItem.LicenseSummary = "Not valid";
                        viewItem.AttentionColor = "red";
                    }
                }
                else
                {
                    viewItem.LicenseSummary = "Not required";
                    viewItem.AttentionColor = null;
                }

                viewModel.Add(viewItem);
            }
            
            return View(viewModel);
        }

        [HttpGet("product", Name = "ProductCatalogSingle")]
        public async Task<IActionResult> SingleItem(int id)
        {
            var dataModel = await logic.GetSingleDataModelAsync(id);
            return View(dataModel);
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cursed.Models.Data;
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

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var dataModel = await logic.GetDataModelsAsync();
            List<ProductCatalogViewModel> viewModel = new List<ProductCatalogViewModel>();
            foreach (var item in dataModel)
            {
                var viewItem = new ProductCatalogViewModel
                {
                    ProductId = item.ProductId,
                    Name = item.Name,
                    CAS = (item.CAS ?? 0).ToString(),
                    Type = item.Type,
                    RecipesCount = item.Recipes?.Count ?? 0,
                    StoragesCount = item.Storages?.Count ?? 0
                };
                viewItem.LicensedUntil = item.LicensedUntil != null ? item.LicensedUntil.Value.ToShortDateString() : "---";
                viewItem.GovermentNum = item.GovermentNum != null ? item.GovermentNum.ToString() : "---";
                if (item.LicenseRequired == true)
                {
                    if(item.LicensedUntil != null && item.GovermentNum != null && item.LicensedUntil>DateTime.UtcNow)
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
    }
}

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

        [HttpGet("index")]
        public async Task<IActionResult> Index()
        {
            var model = await logic.GetDataModelsAsync();
            return View(model);
        }
    }
}

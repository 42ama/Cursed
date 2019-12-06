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
using Cursed.Models.Interfaces.ControllerCRUD;
using Cursed.Models.Routing;
using Cursed.Models.Services;

namespace Cursed.Controllers
{
    [Route("products-catalog")]
    public class ProductsCatalogController : Controller, ICUD<ProductCatalog>, IReadColection, IReadSingle, IReadUpdateForm
    {
        private readonly ProductsCatalogLogic logic;

        public ProductsCatalogController(CursedContext db, [FromServices] ILicenseValidation licenseValidation)
        {
            logic = new ProductsCatalogLogic(db, licenseValidation);
        }

        [HttpGet("", Name = ProductsCatalogRouting.Index)]
        public async Task<IActionResult> Index(int currentPage = 1, int itemsOnPage = 20)
        {
            var statusMessage = await logic.GetAllDataModelAsync();
            if (statusMessage.IsCompleted)
            {
                var pagenationModel = new Pagenation<ProductsCatalogModel>(statusMessage.ReturnValue, itemsOnPage, currentPage);

                return View(pagenationModel);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        [HttpGet("product", Name = ProductsCatalogRouting.SingleItem)]
        public async Task<IActionResult> SingleItem(string key)
        {
            int id = Int32.Parse(key);
            var statusMessage = await logic.GetSingleDataModelAsync(id);
            if (statusMessage.IsCompleted)
            {
                return View(statusMessage.ReturnValue);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        // get for add/edit form
        [HttpGet("product/edit", Name = ProductsCatalogRouting.GetEditSingleItem)]
        public async Task<IActionResult> GetEditSingleItem(string key)
        {
            if(key != null)
            {
                int id = Int32.Parse(key);
                ViewData["SaveRoute"] = ProductsCatalogRouting.EditSingleItem;
                var statusMessage = await logic.GetSingleUpdateModelAsync(id);
                if (statusMessage.IsCompleted)
                {
                    return View("EditSingleItem", statusMessage.ReturnValue);
                }
                else
                {
                    return View("CustomError", statusMessage);
                }
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
            var statusMessage = await logic.AddDataModelAsync(model);
            if(statusMessage.IsCompleted)
            {
                return RedirectToRoute(ProductsCatalogRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        //put item
        [HttpPost("product/edit", Name = ProductsCatalogRouting.EditSingleItem)]
        public async Task<IActionResult> EditSingleItem(ProductCatalog model)
        {
            var statusMessage = await logic.UpdateDataModelAsync(model);
            if (statusMessage.IsCompleted)
            {
                return RedirectToRoute(ProductsCatalogRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        //delete item
        [HttpPost("product/delete", Name = ProductsCatalogRouting.DeleteSingleItem)]
        public async Task<IActionResult> DeleteSingleItem(string key)
        {
            int id = Int32.Parse(key);
            var statusMessage = await logic.RemoveDataModelAsync(id);
            if (statusMessage.IsCompleted)
            {
                return RedirectToRoute(ProductsCatalogRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }
    }
}

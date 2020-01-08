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
using Cursed.Models.LogicValidation;
using Microsoft.AspNetCore.Authorization;
using Cursed.Models.Data.Utility.Authorization;

namespace Cursed.Controllers
{
    [Route("products-catalog")]
    public class ProductsCatalogController : Controller, ICUD<ProductCatalog>, IReadColection, IReadSingle, IReadUpdateForm
    {
        private readonly ProductsCatalogLogic logic;
        private readonly ProductsCatalogLogicValidation logicValidation;

        public ProductsCatalogController(CursedDataContext db, [FromServices] ILicenseValidation licenseValidation, [FromServices] IErrorHandlerFactory errorHandlerFactory)
        {
            logic = new ProductsCatalogLogic(db, licenseValidation);
            logicValidation = new ProductsCatalogLogicValidation(db, errorHandlerFactory);
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist, AuthorizeRoles.GovermentAgent)]
        [HttpGet("", Name = ProductsCatalogRouting.Index)]
        public async Task<IActionResult> Index(int currentPage = 1, int itemsOnPage = 20)
        {
            var model = await logic.GetAllDataModelAsync();
            var pagenationModel = new Pagenation<ProductsCatalogModel>(model, itemsOnPage, currentPage);

            return View(pagenationModel);
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist, AuthorizeRoles.GovermentAgent)]
        [HttpGet("product", Name = ProductsCatalogRouting.SingleItem)]
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

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpGet("product/edit", Name = ProductsCatalogRouting.GetEditSingleItem)]
        public async Task<IActionResult> GetEditSingleItem(string key)
        {
            if(key != null)
            {
                int id = Int32.Parse(key);
                ViewData["SaveRoute"] = ProductsCatalogRouting.EditSingleItem;
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
                ViewData["SaveRoute"] = ProductsCatalogRouting.AddSingleItem;
                return View("EditSingleItem");
            }
            
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpPost("product/add", Name = ProductsCatalogRouting.AddSingleItem)]
        public async Task<IActionResult> AddSingleItem(ProductCatalog model)
        {
            await logic.AddDataModelAsync(model);
            return RedirectToRoute(ProductsCatalogRouting.Index);
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpPost("product/edit", Name = ProductsCatalogRouting.EditSingleItem)]
        public async Task<IActionResult> EditSingleItem(ProductCatalog model)
        {
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(model.Id);
            if (statusMessage.IsCompleted)
            {
                await logic.UpdateDataModelAsync(model);
                return RedirectToRoute(ProductsCatalogRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpPost("product/delete", Name = ProductsCatalogRouting.DeleteSingleItem)]
        public async Task<IActionResult> DeleteSingleItem(string key)
        {
            int id = Int32.Parse(key);
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync(id);
            if (statusMessage.IsCompleted)
            {
                await logic.RemoveDataModelAsync(id);
                return RedirectToRoute(ProductsCatalogRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }
    }
}

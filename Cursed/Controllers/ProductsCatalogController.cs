using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cursed.Models.DataModel.ProductsCatalog;
using Cursed.Models.Context;
using Cursed.Models.Entities.Data;
using Cursed.Models.Logic;
using Cursed.Models.DataModel.Pagenation;
using Cursed.Models.Interfaces.ControllerCRUD;
using Cursed.Models.StaticReferences.Routing;
using Cursed.Models.Services;
using Cursed.Models.LogicValidation;
using Cursed.Models.DataModel.Authorization;

namespace Cursed.Controllers
{
    /// <summary>
    /// Products catalog section controller. Consists of CRUD actions for products in catalog, including read action for
    /// both single product and collection of all products.
    /// </summary>
    [Route("products-catalog")]
    public class ProductsCatalogController : Controller, ICUD<ProductCatalog>, IReadColection, IReadSingle, IReadUpdateForm
    {
        private readonly ProductsCatalogLogic logic;
        private readonly ProductsCatalogLogicValidation logicValidation;
        private readonly ILogProvider<CursedAuthenticationContext> logProvider;

        public ProductsCatalogController(CursedDataContext db, 
            [FromServices] ILicenseValidation licenseValidation, 
            [FromServices] IErrorHandlerFactory errorHandlerFactory,
            [FromServices] ILogProvider<CursedAuthenticationContext> logProvider)
        {
            logic = new ProductsCatalogLogic(db, licenseValidation);
            logicValidation = new ProductsCatalogLogicValidation(db, errorHandlerFactory);
            this.logProvider = logProvider;
        }

        /// <summary>
        /// Main page of section, contains consolidated collection of products from catalog. 
        /// Can be navigated through pagenation.
        /// </summary>
        /// <param name="currentPage">Defines which portion of items from collection, will be shown</param>
        /// <param name="itemsOnPage">Defines how many item there will be in a portion</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist, AuthorizeRoles.GovermentAgent)]
        [HttpGet("", Name = ProductsCatalogRouting.Index)]
        public async Task<IActionResult> Index(int currentPage = 1, int itemsOnPage = 20)
        {
            var model = await logic.GetAllDataModelAsync();

            // form pagenation model
            var pagenationModel = new Pagenation<ProductsCatalogModel>(model, itemsOnPage, currentPage);

            return View(pagenationModel);
        }

        /// <summary>
        /// Displays a single product from catalog, which found by <c>key</c>
        /// </summary>
        /// <param name="key">Id of product from catalog to be found</param>
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

        /// <summary>
        /// Display a page with form to update/add new product to catalog.
        /// </summary>
        /// <param name="key">Id of product from catalog to be edited, if null - considered that product added to catalog insted of edited</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpGet("product/edit", Name = ProductsCatalogRouting.GetEditSingleItem)]
        public async Task<IActionResult> GetEditSingleItem(string key)
        {
            // add distincted from edit, by presence of key parameter
            // further on they distincted by ViewData[SaveRoute]
            if (key != null)
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

        /// <summary>
        /// Post action to add new product to catalog.
        /// </summary>
        /// <param name="model">Product to be added to catalog</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpPost("product/add", Name = ProductsCatalogRouting.AddSingleItem)]
        public async Task<IActionResult> AddSingleItem(ProductCatalog model)
        {
            var product = await logic.AddDataModelAsync(model);
            await logProvider.AddToLogAsync($"Added new product into catalog (Id: {product.Id}).");
            return RedirectToRoute(ProductsCatalogRouting.Index);
        }

        /// <summary>
        /// Post action to update product from catalog.
        /// </summary>
        /// <param name="model">Updated product from catalog information</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpPost("product/edit", Name = ProductsCatalogRouting.EditSingleItem)]
        public async Task<IActionResult> EditSingleItem(ProductCatalog model)
        {
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(model.Id);
            if (statusMessage.IsCompleted)
            {
                await logic.UpdateDataModelAsync(model);
                await logProvider.AddToLogAsync($"Updated product in catalog information (Id: {model.Id}).");
                return RedirectToRoute(ProductsCatalogRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }

        /// <summary>
        /// Post action to delete product from catalog.
        /// </summary>
        /// <param name="key">Id of product from catalog to be deleted</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpPost("product/delete", Name = ProductsCatalogRouting.DeleteSingleItem)]
        public async Task<IActionResult> DeleteSingleItem(string key)
        {
            int id = Int32.Parse(key);
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync(id);
            if (statusMessage.IsCompleted)
            {
                await logic.RemoveDataModelAsync(id);
                await logProvider.AddToLogAsync($"Removed product from catalog (Id: {key}).");
                return RedirectToRoute(ProductsCatalogRouting.Index);
            }
            else
            {
                return View("CustomError", statusMessage);
            }
        }
    }
}

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cursed.Models.DataModel.Products;
using Cursed.Models.Context;
using Cursed.Models.Logic;
using Cursed.Models.StaticReferences.Routing;
using Cursed.Models.Interfaces.ControllerCRUD;
using Cursed.Models.DataModel.Pagenation;
using Cursed.Models.DataModel.Authorization;

namespace Cursed.Controllers
{
    /// <summary>
    /// Products section controller. Consists of Index action, which displays products in specific storage.
    /// </summary>
    [Route("products")]
    public class ProductsController : Controller, IReadCollectionByParam
    {
        private readonly ProductsLogic logic;
        public ProductsController(CursedDataContext db)
        {
            logic = new ProductsLogic(db);
        }

        /// <summary>
        /// Main page of section, contains consolidated collection of products at storage. 
        /// Can be navigated through pagenation.
        /// </summary>
        /// <param name="key">Id of storage to which products belongs</param>
        /// <param name="currentPage">Defines which portion of items from collection, will be shown</param>
        /// <param name="itemsOnPage">Defines how many item there will be in a portion</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpGet("", Name = ProductsRouting.Index)]
        public async Task<IActionResult> Index(string key, int currentPage = 1, int itemsOnPage = 20)
        {
            int storageId = Int32.Parse(key);
            ViewData["StorageId"] = storageId;
            var model = await logic.GetAllDataModelAsync(storageId);

            // form pagenation model
            var pagenationModel = new Pagenation<ProductsDataModel>(model, itemsOnPage, currentPage);

            return View(pagenationModel);
        }
    }
}
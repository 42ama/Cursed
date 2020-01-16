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
    [Route("products")]
    public class ProductsController : Controller, IReadCollectionByParam
    {
        private readonly ProductsLogic logic;
        public ProductsController(CursedDataContext db)
        {
            logic = new ProductsLogic(db);
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist)]
        [HttpGet("", Name = ProductsRouting.Index)]
        public async Task<IActionResult> Index(string key, int currentPage = 1, int itemsOnPage = 20)
        {
            int storageId = Int32.Parse(key);
            ViewData["StorageId"] = storageId;
            var model = await logic.GetAllDataModelAsync(storageId);

            var pagenationModel = new Pagenation<ProductsDataModel>(model, itemsOnPage, currentPage);
            return View(pagenationModel);
        }
    }
}
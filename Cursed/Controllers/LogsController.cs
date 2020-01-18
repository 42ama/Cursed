using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cursed.Models.Context;
using Cursed.Models.Logic;
using Cursed.Models.StaticReferences.Routing;
using Cursed.Models.DataModel.Pagenation;
using Cursed.Models.DataModel.Authorization;
using Cursed.Models.Entities.Authentication;

namespace Cursed.Controllers
{
    /// <summary>
    /// Logs section controller. Used for accesing systems logs.
    /// </summary>
    [Route("logs")]
    public class LogsController : Controller
    {
        private readonly LogsLogic logic;
        public LogsController(CursedAuthenticationContext db)
        {
            logic = new LogsLogic(db);
        }

        /// <summary>
        /// Main page of section, contains consolidated collection of logs. 
        /// Can be navigated through pagenation.
        /// </summary>
        /// <param name="currentPage">Defines which portion of items from collection, will be shown</param>
        /// <param name="itemsOnPage">Defines how many item there will be in a portion</param>
        [AuthorizeRoles(AuthorizeRoles.Administrator)]
        [HttpGet("", Name = LogsRouting.Index)]
        public async Task<IActionResult> Index(int currentPage = 1, int itemsOnPage = 20)
        {
            var model = await logic.GetAllDataModelAsync();

            // form pagenation model
            var pagenationModel = new Pagenation<LogRecord>(model, itemsOnPage, currentPage);

            return View(pagenationModel);
        }
    }
}
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
    [Route("logs")]
    public class LogsController : Controller
    {
        private readonly LogsLogic logic;
        public LogsController(CursedAuthenticationContext db)
        {
            logic = new LogsLogic(db);
        }

        [AuthorizeRoles(AuthorizeRoles.Administrator)]
        [HttpGet("", Name = LogsRouting.Index)]
        public async Task<IActionResult> Index(int currentPage = 1, int itemsOnPage = 20)
        {
            var model = await logic.GetAllDataModelAsync();

            var pagenationModel = new Pagenation<LogRecord>(model, itemsOnPage, currentPage);

            return View(pagenationModel);
        }
    }
}
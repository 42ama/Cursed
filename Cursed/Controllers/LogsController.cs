using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Cursed.Models.DataModel.Products;
using Cursed.Models.DataModel.Shared;
using Cursed.Models.Context;
using Cursed.Models.Entities;
using Cursed.Models.Logic;
using Cursed.Models.Routing;
using Cursed.Models.Interfaces.ControllerCRUD;
using Cursed.Models.DataModel.Utility;
using Cursed.Models.DataModel.Utility.Authorization;
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
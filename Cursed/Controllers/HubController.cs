using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cursed.Models.DataModel.Utility.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cursed.Controllers
{
    public class HubController : Controller
    {
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist, AuthorizeRoles.GovermentAgent)]
        [Route("hub/index", Name = Models.StaticReferences.Routing.HubRouting.Index)]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
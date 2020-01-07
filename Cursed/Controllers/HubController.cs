using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Cursed.Controllers
{
    public class HubController : Controller
    {
        [Route("hub/index", Name = Models.Routing.HubRouting.Index)]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
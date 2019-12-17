using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Cursed.Controllers
{
    public class HubController : Controller
    {
        [Route("hub/index")]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
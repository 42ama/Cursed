using System.Text;
using System.Threading.Tasks;
using Cursed.Models.Context;
using Cursed.Models.DataModel.Authorization;
using Cursed.Models.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Cursed.Controllers
{
    public class HubController : Controller
    {
        private readonly ILogProvider<CursedAuthenticationContext> logProvider;
        public HubController([FromServices] ILogProvider<CursedAuthenticationContext> logProvider)
        {
            this.logProvider = logProvider;
        }
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist, AuthorizeRoles.GovermentAgent)]
        [Route("hub/index", Name = Models.StaticReferences.Routing.HubRouting.Index)]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("hub/error")]
        public async Task<IActionResult> Error()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if(feature != null)
            {
                var exception = feature.Error;
                var exceptionInner = exception.InnerException;

                var exceptionMessage = new StringBuilder("Error occured\n:");
                exceptionMessage.Append($"Stack trace: {exception.StackTrace}\n");
                exceptionMessage.Append($"Message: {exception.Message}\n");

                while (exceptionInner != null)
                {
                    exceptionMessage.Append($"Stack trace: {exceptionInner.StackTrace}\n");
                    exceptionMessage.Append($"Message: {exceptionInner.Message}\n");
                    exceptionInner = exceptionInner.InnerException;
                }

                await logProvider.AddToLogAsync(exceptionMessage.ToString());
            }
            return View();
        }
    }
}
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
    /// <summary>
    /// Hub section controller. Consist of Index and Error pages.
    /// </summary>
    public class HubController : Controller
    {
        private readonly ILogProvider<CursedAuthenticationContext> logProvider;
        public HubController([FromServices] ILogProvider<CursedAuthenticationContext> logProvider)
        {
            this.logProvider = logProvider;
        }

        /// <summary>
        /// Used as main page to display to user all section that he can access.
        /// </summary>
        [AuthorizeRoles(AuthorizeRoles.Administrator, AuthorizeRoles.Manager, AuthorizeRoles.Technologist, AuthorizeRoles.SeniorTechnologist, AuthorizeRoles.GovermentAgent)]
        [Route("hub/index", Name = Models.StaticReferences.Routing.HubRouting.Index)]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Used to inform user when unforseen error occures, and log it for later fixing.
        /// </summary>
        [AllowAnonymous]
        [Route("hub/error")]
        public async Task<IActionResult> Error()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            // feature is not null, when action accessed through exception handling redirection
            if(feature != null)
            {
                // gather exception
                var exception = feature.Error;
                var exceptionInner = exception.InnerException;

                // form a message
                var exceptionMessage = new StringBuilder("Error occured\n:");
                exceptionMessage.Append($"Stack trace: {exception.StackTrace}\n");
                exceptionMessage.Append($"Message: {exception.Message}\n");

                while (exceptionInner != null)
                {
                    exceptionMessage.Append($"Stack trace: {exceptionInner.StackTrace}\n");
                    exceptionMessage.Append($"Message: {exceptionInner.Message}\n");
                    exceptionInner = exceptionInner.InnerException;
                }

                // log exception message
                await logProvider.AddToLogAsync(exceptionMessage.ToString());
            }
            return View();
        }
    }
}
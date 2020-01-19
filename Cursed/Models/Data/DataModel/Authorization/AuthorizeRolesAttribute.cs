using Microsoft.AspNetCore.Authorization;

namespace Cursed.Models.DataModel.Authorization
{
    /// <summary>
    /// Custom AuthorizeAttribute used to remove use of magic strings in Authorization Attributes
    /// </summary>
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(params string[] roles) : base()
        {
            Roles = string.Join(",", roles);
        }
    }
}

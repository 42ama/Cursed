

namespace Cursed.Models.DataModel.Authorization
{
    /// <summary>
    /// Static refrences for authoriztion roles, used to reduce coupling
    /// </summary>
    public class AuthorizeRoles
    {
        public const string Administrator = "ADMIN";
        public const string Technologist = "TECHNOLOGIST";
        public const string SeniorTechnologist = "SENIORTECHNOLOGIST";
        public const string GovermentAgent = "GOVERMENTAGENT";
        public const string Manager = "MANAGER";
    }
    
}

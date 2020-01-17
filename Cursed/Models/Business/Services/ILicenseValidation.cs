using Cursed.Models.Entities.Data;

namespace Cursed.Models.Services
{
    public interface ILicenseValidation
    {
        bool IsValid(License license);
    }
}

using System.Threading.Tasks;
using Cursed.Models.Entities.Data;
using Cursed.Models.DataModel.ErrorHandling;

namespace Cursed.Models.Services
{
    public interface IOperationDataValidation
    {
        Task<IErrorHandler> IsValidAsync(Operation operation);
    }
}

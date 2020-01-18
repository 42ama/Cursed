using System.Threading.Tasks;
using Cursed.Models.Entities.Data;
using Cursed.Models.DataModel.ErrorHandling;

namespace Cursed.Models.Services
{
    /// <summary>
    /// Provides means to validatie Operation
    /// </summary>
    public interface IOperationDataValidation
    {
        /// <summary>
        /// Validate <c>operation</c>
        /// </summary>
        /// <param name="operation">Operation which will be validated</param>
        /// <returns>Status message with validaton information</returns>
        Task<IErrorHandler> IsValidAsync(Operation operation);
    }
}

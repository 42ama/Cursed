using System.Threading.Tasks;
using Cursed.Models.Services;
using Cursed.Models.Entities.Data;
using Cursed.Models.DataModel.ErrorHandling;

namespace Cursed.Tests.Stubs
{
    /// <summary>
    /// Stub for using instead of IOperationDataValidation. Returns valid status message
    /// </summary>
    public class OperationValidationStub : IOperationDataValidation
    {
        /// <returns>valid status message</returns>
        public async Task<IErrorHandler> IsValidAsync(Operation operation)
        {
            return new StatusMessage 
            { 
                ProblemStatus = new Problem
                {
                    Entity = "Operation.",
                    EntityKey = operation.Id.ToString()
                }
            };
        }
    }
}

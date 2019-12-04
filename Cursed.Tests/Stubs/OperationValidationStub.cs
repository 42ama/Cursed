using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cursed.Models.Services;
using Cursed.Models.Entities;
using Cursed.Models.Data.Utility.ErrorHandling;

namespace Cursed.Tests.Stubs
{
    public class OperationValidationStub : IOperationValidation
    {
        /// <returns>valid status message</returns>
        public async Task<StatusMessage> IsValidAsync(Operation operation)
        {
            return new StatusMessage { Entity = $"Operation. Id: {operation.Id}." };
        }
    }
}

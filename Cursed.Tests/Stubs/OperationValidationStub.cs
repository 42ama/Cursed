using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cursed.Models.Services;
using Cursed.Models.Entities;

namespace Cursed.Tests.Stubs
{
    public class OperationValidationStub : IOperationValidation
    {
        /// <returns>true</returns>
        public async Task<bool> IsValidAsync(Operation operation)
        {
            return true;
        }
    }
}

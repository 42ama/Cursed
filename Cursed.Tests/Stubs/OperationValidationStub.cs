using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cursed.Models.Services;
using Cursed.Models.Entities;
using Cursed.Models.Data.Utility.ErrorHandling;

namespace Cursed.Tests.Stubs
{
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
                    EntityKey = operation.Id
                }
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cursed.Models.Entities;
using Cursed.Models.Context;

namespace Cursed.Models.Services
{
    public class OperationValidation : IOperationValidation
    {
        private readonly CursedContext context;
        public OperationValidation(CursedContext context)
        {
            this.context = context;
        }

        public async Task<bool> IsValidAsync(Operation operation)
        {
            return false;
        }
    }
}

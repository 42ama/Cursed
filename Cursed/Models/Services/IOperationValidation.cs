using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cursed.Models.Entities;

namespace Cursed.Models.Services
{
    public interface IOperationValidation
    {
        Task<bool> IsValidAsync(Operation operation);
    }
}

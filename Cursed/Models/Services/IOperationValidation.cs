using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cursed.Models.Entities;
using Cursed.Models.Data.Utility.ErrorHandling;

namespace Cursed.Models.Services
{
    public interface IOperationValidation
    {
        Task<StatusMessage> IsValidAsync(Operation operation);
    }
}

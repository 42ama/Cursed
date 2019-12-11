using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cursed.Models.Services;

namespace Cursed.Models.Data.Utility.ErrorHandling
{
    public class StatusMessageFactory : IErrorHandlerFactory
    {
        public IErrorHandler NewErrorHandler(Problem problemStatus)
        {
            return new StatusMessage
            {
                ProblemStatus = problemStatus
            };
        }
    }
}

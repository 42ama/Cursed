using Cursed.Models.DataModel.Utility.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursed.Models.Services
{
    public interface IErrorHandlerFactory
    {
        public IErrorHandler NewErrorHandler(Problem problemStatus);
    }
}

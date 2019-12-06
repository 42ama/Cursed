using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursed.Models.Data.Utility.ErrorHandling
{
    public abstract class AbstractErrorHandlerFactory
    {
        public abstract AbstractErrorHandler NewErrorHandler(string entity, object entityKey = null);

        public abstract AbstractErrorHandler<T> NewErrorHandler<T>(string entity, object entityKey = null);
    }
}

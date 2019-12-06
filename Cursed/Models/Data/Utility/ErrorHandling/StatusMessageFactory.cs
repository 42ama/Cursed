using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursed.Models.Data.Utility.ErrorHandling
{
    public class StatusMessageFactory : AbstractErrorHandlerFactory
    {
        public override AbstractErrorHandler NewErrorHandler(string entity, object entityKey)
        {
            return new StatusMessage
            {
                Entity = entity,
                EntityKey = entityKey
            };
        }

        public override AbstractErrorHandler<T> NewErrorHandler<T>(string entity, object entityKey) 
        { 
            return new StatusMessage<T>
            {
                Entity = entity,
                EntityKey = entityKey
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursed.Models.Data.Utility.ErrorHandling
{
    public abstract class AbstractErrorHandler
    {
        public abstract bool IsCompleted  { get; }
        public string Entity { get; set; }
        public object EntityKey { get; set; }
        public abstract List<Problem> Problems { get; set; }

        public abstract void AddProblem(Problem problem);
    }

    public abstract class AbstractErrorHandler<T> : AbstractErrorHandler
    {
        public T ReturnValue { get; set; }
    }
}

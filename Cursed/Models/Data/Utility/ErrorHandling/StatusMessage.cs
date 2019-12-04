using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursed.Models.Data.Utility.ErrorHandling
{
    public abstract class AbstractStatusMessage
    {
        public bool IsCompleted
        {
            get
            {
                if(Problems.Count > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public string Entity { get; set; }
        public object EntityKey { get; set; }
        public List<Problem> Problems { get; set; } = new List<Problem>();
    }

    public class StatusMessage : AbstractStatusMessage
    {
    }

    public class StatusMessage<T> : AbstractStatusMessage
    {
        public T ReturnValue { get; set; }
    }
}

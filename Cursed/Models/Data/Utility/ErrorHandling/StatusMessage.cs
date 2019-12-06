using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursed.Models.Data.Utility.ErrorHandling
{
    public class StatusMessage : AbstractErrorHandler
    {
        public override bool IsCompleted
        {
            get
            {
                if (Problems.Count() > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public override List<Problem> Problems { get; set; } = new List<Problem>();
    }

    public class StatusMessage<T> : AbstractErrorHandler<T>
    {
        public override bool IsCompleted
        {
            get
            {
                if (Problems.Count() > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public override List<Problem> Problems { get; set; } = new List<Problem>();
    }
}

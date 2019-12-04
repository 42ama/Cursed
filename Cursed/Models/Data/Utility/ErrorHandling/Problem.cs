using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursed.Models.Data.Utility.ErrorHandling
{
    public struct Problem
    {
        public string Entity { get; set; }
        public object EntityKey { get; set; }
        public string Message { get; set; }
    }
}

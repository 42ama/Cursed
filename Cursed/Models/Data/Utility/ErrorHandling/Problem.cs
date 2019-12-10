using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursed.Models.Data.Utility.ErrorHandling
{
    public class Problem
    {
        public string Entity { get; set; }
        public int EntityKey { get; set; }
        public string Message { get; set; }
        public string RedirectRoute { get; set; }
        public bool UseKeyWithRoute { get; set; } = true;
    }
}

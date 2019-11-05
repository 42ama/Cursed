using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursed.Models.Data.Utility
{
    public abstract class PagenationAbstract
    {
        public int ItemsOnPage { get; protected set; }
        public int CurrentPage { get; protected set; }
        public virtual int Total { get; protected set; }
        public bool Next { get { return CurrentPage < Total; } }
        public bool Prev { get { return CurrentPage > 1; } }

    }
}

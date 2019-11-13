using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursed.Models.Data.Utility
{
    public class CollectionPlusPagenation<T, V>
    {
        public IEnumerable<T> Collection { get; set; }
        public Pagenation<V> Pagenation { get; set; }
    }
}

using System.Collections.Generic;

namespace Cursed.Models.DataModel.Pagenation
{
    /// <summary>
    /// Used for presenting Pagenation and Collection in same model
    /// </summary>
    /// <typeparam name="T">Type of entities of which collection consist</typeparam>
    /// <typeparam name="V">Type of entities of which pagenated collection consist</typeparam>
    public class CollectionPlusPagenation<T, V>
    {
        public IEnumerable<T> Collection { get; set; }
        public Pagenation<V> Pagenation { get; set; }
    }
}

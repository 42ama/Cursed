using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursed.Models.DataModel.Utility
{
    public class Pagenation<T> : PagenationAbstract
    {
        override public int Total { get; protected set; }
        public List<T> PagenatedCollection { get; protected set; }
        
        public Pagenation(IEnumerable<T> collection, int itemsOnPage, int currentPage)
        {
            ItemsOnPage = itemsOnPage;
            CurrentPage = currentPage;

            List<T> Collection = collection.ToList();

            Total = (int)Math.Ceiling(Collection.Count / (double)ItemsOnPage);
            

            if (currentPage >= 1 && currentPage <= Total)
            {
                int itemsToCopy = ItemsOnPage;
                if ((currentPage - 1) * ItemsOnPage + ItemsOnPage > Collection.Count)
                {
                    itemsToCopy = Collection.Count - (currentPage - 1) * ItemsOnPage;
                }
                PagenatedCollection = Collection.GetRange((currentPage - 1) * ItemsOnPage, itemsToCopy);
            }
            else
            {
                PagenatedCollection = new List<T>();
            }
        }
    }
}

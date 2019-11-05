using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursed.Models.Data.Utility
{
    public class Pagenation<T> : PagenationAbstract
    {
        override public int Total { get { return (int)Math.Ceiling(Collection.Count / (double)ItemsOnPage); } }
        public List<T> Collection { get; protected set; }
        public List<T> PagenatedCollection { get; protected set; }
        
        public Pagenation(IEnumerable<T> collection, int itemsOnPage, int currentPage)
        {
            Collection = collection.ToList();
            ItemsOnPage = itemsOnPage;
            CurrentPage = currentPage;

            if (currentPage >= 1 && currentPage <= Total)
            {
                // ArgumentException: Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.
                int itemsToCopy = ItemsOnPage;
                if ((currentPage - 1) * ItemsOnPage + ItemsOnPage > Collection.Count)
                {
                    itemsToCopy = Collection.Count - (currentPage - 1) * ItemsOnPage;
                }
                PagenatedCollection = Collection.GetRange((currentPage - 1) * ItemsOnPage, itemsToCopy);
            }
            else
            {
                throw new ArgumentOutOfRangeException("currentPage", "Index of page must be in range between 0 and total count of pages");
            }
        }
    }
    /*public class Pagenation<T>
    {
        public List<T> Collection { get; private set; }
        public List<T> PagenatedCollection { get; private set; }
        public int ItemsOnPage { get; private set; }
        public int CurrentPage { get; private set; }
        public int Total { get { return (int)Math.Ceiling(Collection.Count / (double)ItemsOnPage); } }
        public bool Next { get { return CurrentPage < Total; } }
        public bool Prev { get { return CurrentPage > 1; } }

        public Pagenation(IEnumerable<T> collection, int itemsOnPage, int currentPage)
        {
            Collection = collection.ToList();
            ItemsOnPage = itemsOnPage;
            CurrentPage = currentPage;

            if (currentPage >= 1 && currentPage <= Total)
            {
                // ArgumentException: Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.
                int itemsToCopy = ItemsOnPage;
                if ((currentPage-1)*ItemsOnPage + ItemsOnPage > Collection.Count)
                {
                    itemsToCopy = Collection.Count - (currentPage - 1) * ItemsOnPage;
                }
                PagenatedCollection = Collection.GetRange((currentPage - 1) * ItemsOnPage, itemsToCopy);
            }
            else
            {
                throw new ArgumentOutOfRangeException("currentPage", "Index of page must be in range between 0 and total count of pages");
            }
        }
    }*/
}

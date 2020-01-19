

namespace Cursed.Models.DataModel.Pagenation
{
    /// <summary>
    /// Model used as base forpagenation
    /// </summary>
    public abstract class PagenationAbstract
    {
        /// <summary>
        /// Items to display on single page.
        /// </summary>
        public int ItemsOnPage { get; protected set; }

        /// <summary>
        /// Portion of data to be displayed aka page.
        /// </summary>
        public int CurrentPage { get; protected set; }

        /// <summary>
        /// Total count of pages.
        /// </summary>
        public virtual int Total { get; protected set; }

        /// <summary>
        /// If there is next page equals true and false otherwise.
        /// </summary>
        public bool Next { get { return CurrentPage < Total; } }

        /// <summary>
        /// If there is previous page equals true and false otherwise.
        /// </summary>
        public bool Prev { get { return CurrentPage > 1; } }

    }
}

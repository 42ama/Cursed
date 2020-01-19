using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cursed.Models.Interfaces.ControllerCRUD
{
    /// <summary>
    /// Read action of CRUD model. Used for Pagenated collections, which 
    /// have different sets of data, choosen by unique identificator.
    /// </summary>
    public interface IReadCollectionByParam
    {
        /// <summary>
        /// Reads data and shapes it into View
        /// </summary>
        /// <param name="key">Unique identificator</param>
        /// <param name="currentPage">Current page in Pagenation model</param>
        /// <param name="itemsOnPage">Items located on page in Pagenation model</param>
        /// <returns>View</returns>
        Task<IActionResult> Index(string key, int currentPage, int itemsOnPage);
    }
}

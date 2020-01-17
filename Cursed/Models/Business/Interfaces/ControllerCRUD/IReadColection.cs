using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cursed.Models.Interfaces.ControllerCRUD
{
    /// <summary>
    /// Read action of CRUD model. Used for Pagenated collections.
    /// </summary>
    public interface IReadColection
    {
        /// <summary>
        /// Reads data and shapes it into View
        /// </summary>
        /// <param name="currentPage">Current page in Pagenation model</param>
        /// <param name="itemsOnPage">Items located on page in Pagenation model</param>
        /// <returns>View</returns>
        Task<IActionResult> Index(int currentPage, int itemsOnPage);
    }
}

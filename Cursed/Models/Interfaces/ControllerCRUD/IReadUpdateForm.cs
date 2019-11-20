using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cursed.Models.Interfaces.ControllerCRUD
{
    /// <summary>
    /// Read action of CRUD model. Used for display edit/add contols for data model.
    /// </summary>
    public interface IReadUpdateForm
    {
        /// <summary>
        /// Reads data and shapes it into View
        /// </summary>
        /// <param name="key">Unique identificator. Action displays edit controls if not empty, and add controls if it is</param>
        /// <returns>View</returns>
        Task<IActionResult> GetEditSingleItem(string key);
    }
}

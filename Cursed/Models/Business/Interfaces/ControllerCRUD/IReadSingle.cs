using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cursed.Models.Interfaces.ControllerCRUD
{
    /// <summary>
    /// Read action of CRUD model. Used for single data model.
    /// </summary>
    public interface IReadSingle
    {
        /// <summary>
        /// Reads data and shapes it into View
        /// </summary>
        /// <param name="key">Unique identificator</param>
        /// <returns>View</returns>
        Task<IActionResult> SingleItem(string key);
    }
}

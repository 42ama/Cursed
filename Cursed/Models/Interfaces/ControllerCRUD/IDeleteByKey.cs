using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cursed.Models.Interfaces.ControllerCRUD
{
    // <summary>
    /// Delete action of CRUD model
    /// </summary>
    public interface IDeleteByKey
    {
        /// <summary>
        /// Delete data model from database
        /// </summary>
        /// <param name="key">Unique identificator of data model</param>
        /// <returns>Redirect to view</returns>
        Task<IActionResult> DeleteSingleItem(string key);
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cursed.Models.Interfaces.ControllerCRUD
{
    /// <summary>
    /// Create action of CRUD model
    /// </summary>
    /// <typeparam name="T">Data model type</typeparam>
    public interface ICreate<T>
    {
        /// <summary>
        /// Add data model to database
        /// </summary>
        /// <param name="model">Data model to be added</param>
        /// <returns>Redirect to view</returns>
        Task<IActionResult> AddSingleItem(T model);
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cursed.Models.Interfaces.ControllerCRUD
{
    // <summary>
    /// Delete action of CRUD model
    /// </summary>
    /// <typeparam name="T">Data model type</typeparam>
    public interface IDeleteByModel<T>
    {
        /// <summary>
        /// Delete data model from database
        /// </summary>
        /// <param name="model">Data model to be deleted</param>
        /// <returns>Redirect to view</returns>
        Task<IActionResult> DeleteSingleItem(T model);
    }
}

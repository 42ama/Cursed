using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cursed.Models
{
    /// <typeparam name="T">Add/Update model</typeparam>
    public interface IControllerRESTAsync<T>
    {
        // get all
        Task<IActionResult> Index(int currentPage, int itemsOnPage);
        // get single
        Task<IActionResult> SingleItem(string key);
        // get for add/edit
        Task<IActionResult> GetEditSingleItem(string key);
        // post single
        Task<IActionResult> AddSingleItem(T model);
        // put single
        Task<IActionResult> EditSingleItem(T model);
        // delete single
        Task<IActionResult> DeleteSingleItem(string key);
    }
}

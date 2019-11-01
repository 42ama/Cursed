﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cursed.Models
{
    /// <typeparam name="T">Add/Update model</typeparam>
    public interface IControllerRESTAsync<T>
    {
        // get all
        Task<IActionResult> Index();
        // get single
        Task<IActionResult> SingleItem(int id);
        // get for add/edit
        Task<IActionResult> GetEditSingleItem(int? id);
        // post single
        Task<IActionResult> AddSingleItem(T model);
        // put single
        Task<IActionResult> EditSingleItem(T model);
        // delete single
        Task<IActionResult> DeleteSingleItem(int id);
    }
}
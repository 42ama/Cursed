﻿using System.Threading.Tasks;
using Cursed.Models.DataModel.Utility.ErrorHandling;

namespace Cursed.Models.Interfaces.LogicCRUD
{
    /// <summary>
    /// Delete action of CRUD model
    /// </summary>
    /// <typeparam name="T">Data model type</typeparam>
    public interface IDeleteByModel<T>
    {
        /// <summary>
        /// Delete data model from database
        /// </summary>
        /// <param name="model">Data model to be deleted</param>
        Task RemoveDataModelAsync(T model);
    }
}

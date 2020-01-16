using System.Threading.Tasks;
using Cursed.Models.DataModel.Utility.ErrorHandling;

namespace Cursed.Models.Interfaces.LogicCRUD
{
    /// <summary>
    /// Update action of CRUD model
    /// </summary>
    /// <typeparam name="T">Data model type</typeparam>
    public interface IUpdate<T>
    {
        /// <summary>
        /// Update data model at database
        /// </summary>
        /// <param name="model">Data model to be updated</param>
        Task UpdateDataModelAsync(T model);
    }
}

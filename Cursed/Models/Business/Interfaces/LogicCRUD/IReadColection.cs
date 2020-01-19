using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cursed.Models.Interfaces.LogicCRUD
{
    /// <summary>
    /// Read action of CRUD model. Used for collections.
    /// </summary>
    interface IReadColection<T>
    {
        /// <summary>
        /// Returns collection of all models from database in specific section
        /// </summary>
        /// <returns>Collection of all models from database in specific section</returns>
        Task<IEnumerable<T>> GetAllDataModelAsync();
    }
}

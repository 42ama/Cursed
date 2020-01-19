using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cursed.Models.Interfaces.LogicCRUD
{
    /// <summary>
    /// Read action of CRUD model. Used for collections, which 
    /// have different sets of data, choosen by unique identificator.
    /// </summary>
    public interface IReadCollectionByParam<T>
    {
        /// <summary>
        /// Returns collection of all models from database in specific section, choosen by unique identificator
        /// </summary>
        /// <param name="key">Unique identificator</param>
        /// <returns>Collection of all models from database in specific section, choosen by unique identificator</returns>
        Task<IEnumerable<T>> GetAllDataModelAsync(object key);
    }
}

using System.Threading.Tasks;
using Cursed.Models.Data.Utility.ErrorHandling;

namespace Cursed.Models.Interfaces.LogicCRUD
{
    /// <summary>
    /// Read action of CRUD model. Used for single data model.
    /// </summary>
    public interface IReadSingle<T>
    {
        /// <summary>
        /// Returns single model from database in specific section, choosen by unique identificator
        /// </summary>
        /// <param name="key">Unique identificator</param>
        /// <returns>Single model from database in specific section, choosen by unique identificator</returns>
        Task<AbstractErrorHandler<T>> GetSingleDataModelAsync(object key);
    }
}

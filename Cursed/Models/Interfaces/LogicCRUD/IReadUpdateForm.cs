using System.Threading.Tasks;
using Cursed.Models.DataModel.Utility.ErrorHandling;

namespace Cursed.Models.Interfaces.LogicCRUD
{
    /// <summary>
    /// Read action of CRUD model. Used for single data model to be updated.
    /// </summary>
    public interface IReadUpdateForm<T>
    {
        /// <summary>
        /// Returns single model to be updated from database in specific section, choosen by unique identificator
        /// </summary>
        /// <param name="key">Unique identificator</param>
        /// <returns>Single model to be updated from database in specific section, choosen by unique identificator</returns>
        Task<T> GetSingleUpdateModelAsync(object key);
    }
}

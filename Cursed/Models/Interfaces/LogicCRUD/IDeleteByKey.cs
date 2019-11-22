using System.Threading.Tasks;

namespace Cursed.Models.Interfaces.LogicCRUD
{
    /// <summary>
    /// Delete action of CRUD model
    /// </summary>
    public interface IDeleteByKey
    {
        /// <summary>
        /// Delete data model from database
        /// </summary>
        /// <param name="key">Unique identificator of data model</param>
        Task RemoveDataModelAsync(object key);
    }
}

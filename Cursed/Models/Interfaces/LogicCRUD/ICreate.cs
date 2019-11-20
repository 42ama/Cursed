using System.Threading.Tasks;

namespace Cursed.Models.Interfaces.LogicCRUD
{
    /// <summary>
    /// Create action of CRUD model
    /// </summary>
    /// <typeparam name="T">Data model type</typeparam>
    public interface ICreate<T>
    {
        /// <summary>
        /// Add data model to database
        /// </summary>
        /// <param name="model">Data model to be added</param>
        Task AddDataModelAsync(T model);
    }
}

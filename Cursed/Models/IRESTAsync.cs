using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cursed.Models
{
    /// <summary>
    /// Implements async RESTfull service.
    /// </summary>
    /// <typeparam name="A">All data model, used in get all</typeparam>
    /// <typeparam name="B">Data model, used in get</typeparam>
    /// <typeparam name="C">Update model, used in post, put, delete</typeparam>
    public interface IRESTAsync<A, B, C>
    {
        /// <summary>
        /// Returns collection of all models from service
        /// </summary>
        /// <returns>Collection of all models from service</returns>
        Task<IEnumerable<A>> GetAllDataModelAsync();
        /// <summary>
        /// Return specified model from service
        /// </summary>
        /// <param name="key">Unique parameter</param>
        /// <returns>Specified model from service</returns>
        Task<B> GetSingleDataModelAsync(object key);
        /// <summary>
        /// Return specified update model from service
        /// </summary>
        /// <param name="key">Unique parameter</param>
        /// <returns>Specified update model from service</returns>
        Task<C> GetSingleUpdateModelAsync(object key);
        /// <summary>
        /// Add new model to service
        /// </summary>
        /// <param name="dataModel">New model</param>
        Task AddDataModelAsync(C dataModel);
        /// <summary>
        /// Update existing model in service
        /// </summary>
        /// <param name="updatedDataModel">Updated version of model</param>
        Task UpdateDataModelAsync(C updatedDataModel);
        /// <summary>
        /// Remove existing model from service
        /// </summary>
        /// <param name="key">Unique parameter</param>
        Task RemoveDataModelAsync(object key);
    }
}

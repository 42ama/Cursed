using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cursed.Models
{
    /// <summary>
    /// Implements async RESTfull service.
    /// </summary>
    /// <typeparam name="T">Display model, used in get</typeparam>
    /// <typeparam name="K">Update model, used in post, put, delete</typeparam>
    public interface IRESTAsync<T, K>
    {
        /// <summary>
        /// Returns collection of all models from service
        /// </summary>
        /// <returns>Collection of all models from service</returns>
        Task<IEnumerable<T>> GetDataModelsAsync();
        /// <summary>
        /// Return specified model from service
        /// </summary>
        /// <param name="UId">Unique parameter</param>
        /// <returns>Specified model from service</returns>
        Task<T> GetDataModelAsync(object UId);
        /// <summary>
        /// Add new model to service
        /// </summary>
        /// <param name="dataModel">New model</param>
        Task AddDataModelAsync(K dataModel);
        /// <summary>
        /// Update existing model in service
        /// </summary>
        /// <param name="updatedDataModel">Updated version of model</param>
        Task UpdateDataModelAsync(K updatedDataModel);
        /// <summary>
        /// Remove existing model from service
        /// </summary>
        /// <param name="dataModel">Model to be removed</param>
        Task RemoveDataModelAsync(K dataModel);
    }
}

using Cursed.Models.DataModel.ErrorHandling;

namespace Cursed.Models.Services
{
    /// <summary>
    /// Facotry for producing IErrorHandler instances
    /// </summary>
    public class StatusMessageFactory : IErrorHandlerFactory
    {
        /// <summary>
        /// Creates new IErrorHandler, with <c>problemStatus</c> as basic handler information
        /// </summary>
        /// <param name="problemStatus">Basic handler information</param>
        /// <returns>IErrorHandler instance, with <c>problemStatus</c> as basic handler information</returns>
        public IErrorHandler NewErrorHandler(Problem problemStatus)
        {
            return new StatusMessage
            {
                ProblemStatus = problemStatus
            };
        }
    }
}

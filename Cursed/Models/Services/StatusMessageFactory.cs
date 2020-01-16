using Cursed.Models.DataModel.Utility.ErrorHandling;

namespace Cursed.Models.Services
{
    public class StatusMessageFactory : IErrorHandlerFactory
    {
        public IErrorHandler NewErrorHandler(Problem problemStatus)
        {
            return new StatusMessage
            {
                ProblemStatus = problemStatus
            };
        }
    }
}

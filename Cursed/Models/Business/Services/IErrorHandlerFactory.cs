using Cursed.Models.DataModel.ErrorHandling;

namespace Cursed.Models.Services
{
    public interface IErrorHandlerFactory
    {
        public IErrorHandler NewErrorHandler(Problem problemStatus);
    }
}

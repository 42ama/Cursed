using System.Collections.Generic;

namespace Cursed.Models.DataModel.ErrorHandling
{
    public interface IErrorHandler
    {
        public bool IsCompleted { get; }
        public Problem ProblemStatus { get; set; }
        public List<Problem> Problems { get; set; }

        public void AddProblem(Problem problem);
    }
}

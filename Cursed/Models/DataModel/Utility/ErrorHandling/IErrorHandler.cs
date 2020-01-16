using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursed.Models.DataModel.Utility.ErrorHandling
{
    public interface IErrorHandler
    {
        public bool IsCompleted { get; }
        public Problem ProblemStatus { get; set; }
        public List<Problem> Problems { get; set; }

        public void AddProblem(Problem problem);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursed.Models.Data.Utility.ErrorHandling
{
    public class StatusMessage : IErrorHandler
    {
        public bool IsCompleted
        {
            get
            {
                if (Problems.Count() > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public Problem ProblemStatus { get; set; }

        public List<Problem> Problems { get; set; } = new List<Problem>();

        public void AddProblem(Problem problem)
        {
            Problems.Add(problem);
        }
    }
}

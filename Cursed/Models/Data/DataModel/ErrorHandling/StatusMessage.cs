using System.Collections.Generic;
using System.Linq;

namespace Cursed.Models.DataModel.ErrorHandling
{
    /// <summary>
    /// Error handler to single entity. Can contain multiple errors.
    /// </summary>
    public class StatusMessage : IErrorHandler
    {
        /// <summary>
        /// True if no errors found and false otherwise.
        /// </summary>
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

        /// <summary>
        /// Contains basic handler information.
        /// </summary>
        public Problem ProblemStatus { get; set; }

        /// <summary>
        /// List of found problems
        /// </summary>
        public List<Problem> Problems { get; set; } = new List<Problem>();

        /// <summary>
        /// Add new <c>problem</c> to list of problems
        /// </summary>
        /// <param name="problem">Problem to be added</param>
        public void AddProblem(Problem problem)
        {
            Problems.Add(problem);
        }
    }
}

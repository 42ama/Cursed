using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cursed.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace Cursed.Models.Services
{
    /// <summary>
    /// Logging service, which stores logs in provided database
    /// </summary>
    /// <typeparam name="T">Specific database context class</typeparam>
    public interface ILogProvider<T> where T : DbContext
    {
        /// <summary>
        /// Add record to log
        /// </summary>
        /// <param name="message">Record message</param>
        Task AddToLogAsync(string message);

        /// <summary>
        /// Add record to log, used when user name is not defined in http-context
        /// (due to async sign-in), but already known at controller.
        /// </summary>
        /// <param name="message">Record message</param>
        /// <param name="userLogin">User name</param>
        Task AddToLogAsync(string message, string userLogin);
    }
}

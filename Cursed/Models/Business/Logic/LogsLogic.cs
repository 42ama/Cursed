using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Context;
using Cursed.Models.Interfaces.LogicCRUD;
using Cursed.Models.Entities.Authentication;

namespace Cursed.Models.Logic
{
    /// <summary>
    /// Logs section logic. Consists of read action for logs.
    /// </summary>
    public class LogsLogic : IReadColection<LogRecord>
    {
        private readonly CursedAuthenticationContext db;
        public LogsLogic(CursedAuthenticationContext db)
        {
            this.db = db;
        }

        /// <summary>
        /// Gather all logs from database.
        /// </summary>
        /// <returns>All logs from database</returns>
        public async Task<IEnumerable<LogRecord>> GetAllDataModelAsync()
        {
            return await db.LogRecord.OrderByDescending(i => i.Id).ToListAsync();
        }
    }
}

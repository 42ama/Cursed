using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cursed.Models.Context;
using Cursed.Models.Interfaces.LogicCRUD;
using Cursed.Models.Entities.Authentication;

namespace Cursed.Models.Logic
{
    public class LogsLogic : IReadColection<LogRecord>
    {
        private readonly CursedAuthenticationContext db;
        public LogsLogic(CursedAuthenticationContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<LogRecord>> GetAllDataModelAsync()
        {
            return await db.LogRecord.ToListAsync();
        }
    }
}

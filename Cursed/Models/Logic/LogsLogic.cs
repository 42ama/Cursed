using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Cursed.Models.Context;
using Cursed.Models.DataModel.Companies;
using Cursed.Models.Entities;
using Cursed.Models.Interfaces.LogicCRUD;
using Cursed.Models.DataModel.Utility;
using Cursed.Models.DataModel.Utility.ErrorHandling;
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

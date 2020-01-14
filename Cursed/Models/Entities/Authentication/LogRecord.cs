using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursed.Models.Entities.Authentication
{
    public class LogRecord
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string UserLogin { get; set; }
        public string UserIP { get; set; }
        public string Record { get; set; }


        public virtual UserAuth UserAuth { get; set; }
    }
}

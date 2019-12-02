using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Cursed.Tests.Extensions
{
    public static class DbSetExtensions
    {
        // used to clear tables in dbset
        public static async Task ClearIfAny<T>(this DbSet<T> dbSet) where T : class
        {
            if(await dbSet.AnyAsync())
            {
                dbSet.RemoveRange(dbSet);
            }
        }
    }
}

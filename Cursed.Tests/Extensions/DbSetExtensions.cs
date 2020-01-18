﻿using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Cursed.Tests.Extensions
{
    /// <summary>
    /// Extenstion to Db Set. Clears db set completly.
    /// </summary>
    public static class DbSetExtensions
    {
        /// <summary>
        /// Clears db set completly.
        /// </summary>
        /// <typeparam name="T">Type of entities in db set</typeparam>
        /// <param name="dbSet">Db set to be cleared</param>
        public static async Task ClearIfAny<T>(this DbSet<T> dbSet) where T : class
        {
            if(await dbSet.AnyAsync())
            {
                dbSet.RemoveRange(dbSet);
            }
        }
    }
}

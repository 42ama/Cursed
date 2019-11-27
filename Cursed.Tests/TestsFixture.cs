using System;
using System.Collections.Generic;

using Cursed.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace Cursed.Tests
{
    public class TestsFixture : IDisposable
    {
        public readonly CursedContext db;

        public TestsFixture()
        {
            
            var options = new DbContextOptionsBuilder<CursedContext>()
                .UseInMemoryDatabase(databaseName: "CursedTestingDB")
                .Options;

            db = new CursedContext(options);
        }

        public void Dispose()
        {
            db.Database.EnsureDeleted();
        }
    }
}

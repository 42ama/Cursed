using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using Cursed.Tests.Extensions;
using Cursed.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace Cursed.Tests
{
    public class TestsFixture : IDisposable
    {
        public readonly CursedDataContext db;
        public readonly CursedAuthenticationContext dbAuth;

        public TestsFixture()
        {
            
            var options = new DbContextOptionsBuilder<CursedDataContext>()
                .UseInMemoryDatabase(databaseName: "CursedTestingDB")
                .Options;

            db = new CursedDataContext(options);
            dbAuth = new CursedAuthenticationContext(new DbContextOptionsBuilder<CursedAuthenticationContext>()
                .UseInMemoryDatabase(databaseName: "CursedAuthTestingDB")
                .Options);
        }

        public async void Dispose()
        {
            await db.Database.EnsureDeletedAsync();
            await dbAuth.Database.EnsureDeletedAsync();
        }

        public static async Task ClearDatabase(CursedDataContext context)
        {
            await context.RecipeInheritance.ClearIfAny();
            await context.RecipeProductChanges.ClearIfAny();
            await context.Operation.ClearIfAny();
            await context.License.ClearIfAny();
            await context.TransactionBatch.ClearIfAny();
            await context.Company.ClearIfAny();
            await context.TechProcess.ClearIfAny();
            await context.Facility.ClearIfAny();
            await context.Product.ClearIfAny();
            await context.Recipe.ClearIfAny();
            await context.ProductCatalog.ClearIfAny();
            await context.Storage.ClearIfAny();
            await context.SaveChangesAsync();
        }

        public static async Task ClearDatabase(CursedAuthenticationContext context)
        {
            await context.Role.ClearIfAny();
            await context.UserData.ClearIfAny();
            await context.UserAuth.ClearIfAny();
            await context.SaveChangesAsync();
        }
    }

    [CollectionDefinition("Tests collection")]
    public class TestsCollection : ICollectionFixture<TestsFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Cursed.Models.Logic;
using Cursed.Models.Entities;

namespace Cursed.Tests.Tests.Logic
{
    public class ProductsCatalog : IClassFixture<TestsFixture>
    {
        private readonly TestsFixture fixture;
        private readonly ProductsCatalogLogic logic;

        public ProductsCatalog(TestsFixture fixture)
        {
            this.fixture = fixture;
            logic = new ProductsCatalogLogic(fixture.db);
        }

        [Fact]
        public async void AddDataModelAsync()
        {
            // arrange
            var entityModel = new ProductCatalog
            {
                Id = 44440,
                Cas = 4040404,
                LicenseRequired = true,
                Name = "Testotin",
                Type = Models.Data.Shared.ProductCatalogTypes.Product
            };

            // act
            await logic.AddDataModelAsync(entityModel);

            // assert
            var testModel = await fixture.db.ProductCatalog.FirstOrDefaultAsync(i => i.Id == entityModel.Id);
            Assert.Equal(entityModel, testModel);

            // dispose
            fixture.db.ProductCatalog.Remove(testModel);
            await fixture.db.SaveChangesAsync();
        }

        [Fact]
        public async void RemoveDataModelAsync()
        {
            // arrange
            var entityModel = new ProductCatalog
            {
                Id = 44440,
                Cas = 4040404,
                LicenseRequired = true,
                Name = "Testotin",
                Type = Models.Data.Shared.ProductCatalogTypes.Product
            };
            fixture.db.Add(entityModel);
            await fixture.db.SaveChangesAsync();

            // act
            await logic.RemoveDataModelAsync(entityModel.Id);

            // assert
            var testModel = await fixture.db.ProductCatalog.FirstOrDefaultAsync(i => i.Id == entityModel.Id);
            Assert.Null(testModel);
        }

        [Fact]
        public async void UpdateDataModelAsync()
        {
            // arrange
            var entityModel = new ProductCatalog
            {
                Id = 44440,
                Cas = 4040404,
                LicenseRequired = true,
                Name = "Testotin",
                Type = Models.Data.Shared.ProductCatalogTypes.Product
            };
            fixture.db.Add(entityModel);
            await fixture.db.SaveChangesAsync();

            var updatedModel = new ProductCatalog
            {
                Id = 44440,
                Cas = 4040404,
                LicenseRequired = true,
                Name = "Testedtin",
                Type = Models.Data.Shared.ProductCatalogTypes.Product
            };

            // act
            await logic.UpdateDataModelAsync(updatedModel);

            // assert
            var testModel = await fixture.db.ProductCatalog.FirstOrDefaultAsync(i => i.Id == updatedModel.Id);
            Assert.Equal(updatedModel.Name, testModel.Name);

            // dispose
            fixture.db.ProductCatalog.Remove(testModel);
            await fixture.db.SaveChangesAsync();
        }
    }
}

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Cursed.Models.Logic;
using Cursed.Models.Entities;

namespace Cursed.Tests.Tests.Logic
{
    public class Licenses : IClassFixture<TestsFixture>
    {
        private readonly TestsFixture fixture;
        private readonly LicensesLogic logic;

        public Licenses(TestsFixture fixture)
        {
            this.fixture = fixture;
            logic = new LicensesLogic(fixture.db);
        }

        [Fact]
        public async void AddDataModelAsync()
        {
            // arrange
            var entityModel = new License
            {
                Id = 44440,
                ProductId = 44440,
                Date = DateTime.Now,
                GovermentNum = 4040404
            };

            // act
            await logic.AddDataModelAsync(entityModel);

            // assert
            var testModel = await fixture.db.License.FirstOrDefaultAsync(i => i.Id == entityModel.Id);
            Assert.Equal(entityModel, testModel);

            // dispose
            fixture.db.License.Remove(testModel);
            await fixture.db.SaveChangesAsync();
        }

        [Fact]
        public async void RemoveDataModelAsync()
        {
            // arrange
            var entityModel = new License
            {
                Id = 44440,
                ProductId = 44440,
                Date = DateTime.Now,
                GovermentNum = 4040404
            }; ;
            fixture.db.Add(entityModel);
            await fixture.db.SaveChangesAsync();

            // act
            await logic.RemoveDataModelAsync(entityModel.Id);

            // assert
            var testModel = await fixture.db.License.FirstOrDefaultAsync(i => i.Id == entityModel.Id);
            Assert.Null(testModel);
        }

        [Fact]
        public async void UpdateDataModelAsync()
        {
            // arrange
            var entityModel = new License
            {
                Id = 44440,
                ProductId = 44440,
                Date = DateTime.Now,
                GovermentNum = 4040404
            };
            fixture.db.Add(entityModel);
            await fixture.db.SaveChangesAsync();

            var updatedModel = new License
            {
                Id = 44440,
                ProductId = 44440,
                Date = DateTime.Now,
                GovermentNum = 5050505
            };

            // act
            await logic.UpdateDataModelAsync(updatedModel);

            // assert
            var testModel = await fixture.db.License.FirstOrDefaultAsync(i => i.Id == updatedModel.Id);
            Assert.Equal(updatedModel.GovermentNum, testModel.GovermentNum);

            // dispose
            fixture.db.License.Remove(testModel);
            await fixture.db.SaveChangesAsync();
        }
    }
}

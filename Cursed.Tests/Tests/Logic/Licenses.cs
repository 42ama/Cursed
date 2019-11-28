using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Cursed.Models.Logic;
using Cursed.Models.Entities;
using Cursed.Models.Data.Licenses;
using Cursed.Models.Data.Utility;
using Cursed.Tests.Helpers;

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
            var expected = new License
            {
                Id = 44440,
                ProductId = 44440,
                Date = DateTime.Now.Trim(TimeSpan.TicksPerDay),
                GovermentNum = 4040404
            };

            // act
            await logic.AddDataModelAsync(expected);

            // assert
            var actual = await fixture.db.License.FirstOrDefaultAsync(i => i.Id == expected.Id);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.ProductId, actual.ProductId);
            Assert.Equal(expected.Date, actual.Date);
            Assert.Equal(expected.GovermentNum, actual.GovermentNum);

            // dispose
            fixture.db.License.Remove(actual);
            await fixture.db.SaveChangesAsync();
        }

        [Fact]
        public async void RemoveDataModelAsync()
        {
            // arrange
            var license = new License
            {
                Id = 44440,
                ProductId = 44440,
                Date = DateTime.Now.Trim(TimeSpan.TicksPerDay),
                GovermentNum = 4040404
            }; ;
            fixture.db.Add(license);
            await fixture.db.SaveChangesAsync();

            // act
            await logic.RemoveDataModelAsync(license.Id);

            // assert
            var actual = await fixture.db.License.FirstOrDefaultAsync(i => i.Id == license.Id);
            Assert.Null(actual);
        }

        [Fact]
        public async void UpdateDataModelAsync()
        {
            // arrange
            var license = new License
            {
                Id = 44440,
                ProductId = 44440,
                Date = DateTime.Now.Trim(TimeSpan.TicksPerDay),
                GovermentNum = 4040404
            };
            fixture.db.Add(license);
            await fixture.db.SaveChangesAsync();

            var expected = new License
            {
                Id = 44440,
                ProductId = 44440,
                Date = DateTime.Now.Trim(TimeSpan.TicksPerDay),
                GovermentNum = 5050505
            };

            // act
            await logic.UpdateDataModelAsync(expected);

            // assert
            var actual = await fixture.db.License.FirstOrDefaultAsync(i => i.Id == expected.Id);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.ProductId, actual.ProductId);
            Assert.Equal(expected.Date, actual.Date);
            Assert.Equal(expected.GovermentNum, actual.GovermentNum);

            // dispose
            fixture.db.License.Remove(actual);
            await fixture.db.SaveChangesAsync();
        }

        [Fact]
        public async void GetAllDataModelAsync()
        {
            // arrange
            var products = new ProductCatalog[]
            {
                new ProductCatalog
                {
                    Id = 44440,
                    Cas = 4040404,
                    Name = "Testin",
                },
                new ProductCatalog
                {
                    Id = 44441,
                    Cas = 4040414,
                    Name = "Testotin",
                }
            };
            var licenses = new License[]
            {
                new License
                {
                    Id = 44440,
                    Date = DateTime.Now.Trim(TimeSpan.TicksPerDay).AddDays(-1),
                    GovermentNum = 4040404,
                    ProductId = 44440
                },
                new License
                {
                    Id = 44441,
                    Date = DateTime.Now.Trim(TimeSpan.TicksPerDay).AddDays(1),
                    GovermentNum = 4040414,
                    ProductId = 44440
                },
                new License
                {
                    Id = 44442,
                    Date = DateTime.Now.Trim(TimeSpan.TicksPerDay).AddDays(1),
                    GovermentNum = 4041404,
                    ProductId = 44441
                },
            };

            fixture.db.ProductCatalog.AddRange(products);
            fixture.db.License.AddRange(licenses);
            await fixture.db.SaveChangesAsync();
            var expected = new List<LicensesDataModel>
            {
                new LicensesDataModel
                {
                    Id = 44440,
                    Date = DateTime.Now.Trim(TimeSpan.TicksPerDay).AddDays(-1),
                    GovermentNum = 4040404,
                    ProductId = 44440,
                    ProductCAS = 4040404,
                    ProductName = "Testin"
                },
                new LicensesDataModel
                {
                    Id = 44441,
                    Date = DateTime.Now.Trim(TimeSpan.TicksPerDay).AddDays(1),
                    GovermentNum = 4040414,
                    ProductId = 44440,
                    ProductCAS = 4040404,
                    ProductName = "Testin"
                },
                new LicensesDataModel
                {
                    Id = 44442,
                    Date = DateTime.Now.Trim(TimeSpan.TicksPerDay).AddDays(1),
                    GovermentNum = 4041404,
                    ProductId = 44441,
                    ProductCAS = 4040414,
                    ProductName = "Testotin"
                }
            };

            // act
            var actual = (await logic.GetAllDataModelAsync()).ToList();

            // assert
            foreach (var expectedItem in expected)
            {
                Assert.Contains(actual, actualItem =>
                    expectedItem.Id == actualItem.Id &&
                    expectedItem.Date == actualItem.Date &&
                    expectedItem.GovermentNum == actualItem.GovermentNum &&
                    expectedItem.ProductId == actualItem.ProductId &&
                    expectedItem.ProductCAS == actualItem.ProductCAS &&
                    expectedItem.ProductName == actualItem.ProductName);
            }

            // dispose
            fixture.db.License.RemoveRange(licenses);
            fixture.db.ProductCatalog.RemoveRange(products);
            await fixture.db.SaveChangesAsync();
        }

        [Fact]
        public async void GetSingleDataModelAsync()
        {
            // arrange
            var products = new ProductCatalog[]
            {
                new ProductCatalog
                {
                    Id = 44440,
                    Cas = 4040404,
                    Name = "Testin",
                }
            };
            var licenses = new License[]
            {
                new License
                {
                    Id = 44441,
                    Date = DateTime.Now.Trim(TimeSpan.TicksPerDay).AddDays(1),
                    GovermentNum = 4040414,
                    ProductId = 44440
                }
            };

            fixture.db.ProductCatalog.AddRange(products);
            fixture.db.License.AddRange(licenses);
            await fixture.db.SaveChangesAsync();
            var expected =  new LicensesDataModel
            {
                Id = 44441,
                Date = DateTime.Now.Trim(TimeSpan.TicksPerDay).AddDays(1),
                GovermentNum = 4040414,
                ProductId = 44440,
                ProductCAS = 4040404,
                ProductName = "Testin"
            };

            // act
            var actual = await logic.GetSingleDataModelAsync(expected.Id);

            // assert
            Assert.Equal(actual.Id, expected.Id);
            Assert.Equal(actual.Date, expected.Date);
            Assert.Equal(actual.GovermentNum, expected.GovermentNum);
            Assert.Equal(actual.ProductId, expected.ProductId);
            Assert.Equal(actual.ProductCAS, expected.ProductCAS);
            Assert.Equal(actual.ProductName, expected.ProductName);

            // dispose1
            fixture.db.License.RemoveRange(licenses);
            fixture.db.ProductCatalog.RemoveRange(products);
            await fixture.db.SaveChangesAsync();
        }

        [Fact]
        public async void GetSingleUpdateModelAsync()
        {
            // arrange
            var expected = new License
            {
                Id = 44441,
                Date = DateTime.Now.Trim(TimeSpan.TicksPerDay).AddDays(1),
                GovermentNum = 4040414,
                ProductId = 44440
            };

            fixture.db.License.Add(expected);
            await fixture.db.SaveChangesAsync();

            // act
            var actual = await logic.GetSingleUpdateModelAsync(expected.Id);

            // assert
            Assert.Equal(actual.Id, expected.Id);
            Assert.Equal(actual.Date, expected.Date);
            Assert.Equal(actual.GovermentNum, expected.GovermentNum);
            Assert.Equal(actual.ProductId, expected.ProductId);

            // dispose
            fixture.db.License.Remove(actual);
            await fixture.db.SaveChangesAsync();
        }
    }
}

using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Cursed.Models.Logic;
using Cursed.Models.Entities;
using Cursed.Models.Data.Storages;
using Cursed.Models.Data.Shared;
using Cursed.Models.Data.Utility;

namespace Cursed.Tests.Tests.Logic
{
    [Collection("Tests collection")]
    public class StoragesTests : IDisposable
    {
        private readonly TestsFixture fixture;
        private readonly StoragesLogic logic;

        public StoragesTests(TestsFixture fixture)
        {
            this.fixture = fixture;
            logic = new StoragesLogic(fixture.db);
        }

        public async void Dispose()
        {
            await TestsFixture.ClearDatabase(fixture.db);
        }

        private Storage GetStorage()
        {
            return new Storage
            {
                Id = 44440,
                Name = "Test storage",
                Latitude = (decimal)12.3456,
                Longitude = (decimal)78.9012,
                CompanyId = 44440
            };
        }

        private IEnumerable<Storage> GetStorages()
        {
            return new Storage[]
            {
                new Storage
                {
                    Id = 44440,
                    Name = "Test storage #1",
                    Latitude = (decimal)12.3456,
                    Longitude = (decimal)78.9012,
                    CompanyId = 44440
                },
                new Storage
                {
                    Id = 44441,
                    Name = "Test storage #2",
                    Latitude = (decimal)12.3456,
                    Longitude = (decimal)78.9012,
                    CompanyId = 44440
                },
                new Storage
                {
                    Id = 44442,
                    Name = "Test storage #3",
                    Latitude = (decimal)12.3456,
                    Longitude = (decimal)78.9012,
                    CompanyId = 44441
                }
            };
        }

        private IEnumerable<Product> GetProducts()
        {
            return new Product[]
            {
                new Product
                {
                    Id = 44440,
                    Uid = 44440,
                    StorageId = 44440,
                    Price = 15,
                    Quantity = 12,
                    QuantityUnit = "mg."
                },
                new Product
                {
                    Id = 44441,
                    Uid = 44441,
                    StorageId = 44440,
                    Price = 17,
                    Quantity = 2,
                    QuantityUnit = "mg."
                },
                new Product
                {
                    Id = 44442,
                    StorageId = 44441
                },
                new Product
                {
                    Id = 44443,
                    StorageId = 44442
                }
            };
        }

        private IEnumerable<Company> GetCompanies()
        {
            return new Company[]
            {
                new Company
                {
                    Id = 44440,
                    Name = "Company #1"
                },
                new Company
                {
                    Id = 44441,
                    Name = "Company #2"
                }
            };
        }

        private IEnumerable<ProductCatalog> GetProductCatalogs()
        {
            return new ProductCatalog[]
            {
                new ProductCatalog
                {
                    Id = 44440,
                    Name = "Testin"
                },
                new ProductCatalog
                {
                    Id = 44441,
                    Name = "Testesteron"
                }
            };
        }

        [Fact]
        public async void AddStorage_ToEmptyDbTable_AddedStorageEqualExpectedStorage()
        {
            // arrange
            var expected = GetStorage();

            // act
            await logic.AddDataModelAsync(expected);

            // assert
            var actual = await fixture.db.Storage.FirstOrDefaultAsync(i => i.Id == expected.Id);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Latitude, actual.Latitude);
            Assert.Equal(expected.Longitude, actual.Longitude);
            Assert.Equal(expected.CompanyId, actual.CompanyId);
        }

        [Fact]
        public async void RemoveStorag_FromInitializedDbTable_RemovedStoragNotFoundInDb()
        {
            // arrange
            var storage = GetStorage();
            fixture.db.Add(storage);
            await fixture.db.SaveChangesAsync();

            // act
            await logic.RemoveDataModelAsync(storage.Id);

            // assert
            var actual = await fixture.db.Storage.FirstOrDefaultAsync(i => i.Id == storage.Id);
            Assert.Null(actual);
        }

        [Fact]
        public async void UpdateStorage_AtInitializedDbTable_UpdatedStorageEqualExpectedStorage()
        {
            // arrange
            var storage = GetStorage();
            fixture.db.Add(storage);
            await fixture.db.SaveChangesAsync();

            var expected = new Storage
            {
                Id = storage.Id,
                Name = "Tested storage",
                Latitude = (decimal)12.3456,
                Longitude = (decimal)78.9012,
                CompanyId = 44440
            };

            // act
            await logic.UpdateDataModelAsync(expected);

            // assert
            var actual = await fixture.db.Storage.FirstOrDefaultAsync(i => i.Id == expected.Id);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Latitude, actual.Latitude);
            Assert.Equal(expected.Longitude, actual.Longitude);
            Assert.Equal(expected.CompanyId, actual.CompanyId);
        }

        [Fact]
        public async void GetListStoragesModel_FromInitializedDbTables_LogicStoragesModelsEqualExpectedStoragesModels()
        {
            var storages = GetStorages();
            var products = GetProducts();
            var companies = GetCompanies();

            fixture.db.Storage.AddRange(storages);
            fixture.db.Product.AddRange(products);
            fixture.db.Company.AddRange(companies);
            await fixture.db.SaveChangesAsync();
            var expected = new List<StoragesModel>
            {
                new StoragesModel
                {
                    Id = 44440,
                    Name = "Test storage #1",
                    Latitude = (decimal)12.3456,
                    Longitude = (decimal)78.9012,
                    Company = new TitleIdContainer{Id = 44440, Title = "Company #1"},
                    ProductsCount = 2
                },
                new StoragesModel
                {
                    Id = 44441,
                    Name = "Test storage #2",
                    Latitude = (decimal)12.3456,
                    Longitude = (decimal)78.9012,
                    Company = new TitleIdContainer{Id = 44440, Title = "Company #1"},
                    ProductsCount = 1
                },
                new StoragesModel
                {
                    Id = 44442,
                    Name = "Test storage #3",
                    Latitude = (decimal)12.3456,
                    Longitude = (decimal)78.9012,
                    Company = new TitleIdContainer{Id = 44441, Title = "Company #2"},
                    ProductsCount = 1
                }
            };

            // act
            var actual = (await logic.GetAllDataModelAsync()).ReturnValue.ToList();

            // assert
            foreach (var expectedItem in expected)
            {
                Assert.Contains(actual, actualItem =>
                    expectedItem.Id == actualItem.Id &&
                    expectedItem.Name == actualItem.Name &&
                    expectedItem.Latitude == actualItem.Latitude &&
                    expectedItem.Longitude == actualItem.Longitude &&
                    expectedItem.Company.Title == actualItem.Company.Title &&
                    expectedItem.Company.Id == actualItem.Company.Id &&
                    expectedItem.ProductsCount == actualItem.ProductsCount);
            }
        }

        [Fact]
        public async void GetStorageModel_FromInitializedDbTables_LogicStorageModelEqualExpectedStorageModel()
        {
            // arrange
            var productsCatalog = GetProductCatalogs();
            var storages = GetStorages();
            var products = GetProducts();
            var companies = GetCompanies();


            fixture.db.ProductCatalog.AddRange(productsCatalog);
            fixture.db.Storage.AddRange(storages);
            fixture.db.Product.AddRange(products);
            fixture.db.Company.AddRange(companies);
            await fixture.db.SaveChangesAsync();
            var expected = new StorageModel
            {
                Id = 44440,
                Name = "Test storage #1",
                Latitude = (decimal)12.3456,
                Longitude = (decimal)78.9012,
                Company = new TitleIdContainer { Title = "Company #1", Id = 44440 },
                Products = new List<ProductContainer>()
            };
            foreach (var product in products.Where(i => i.StorageId == expected.Id))
	        {
                var productCatalog = productsCatalog.Single(i => product.Uid == i.Id);
                expected.Products.Add(new ProductContainer
                {
                    Id = product.Id,
                    Uid = productCatalog.Id,
                    Name = productCatalog.Name,
                    Price = product.Price,
                    Quantity = product.Quantity,
                    QuantityUnit = product.QuantityUnit
                });
	        }

            // act
            var actual = (await logic.GetSingleDataModelAsync(expected.Id)).ReturnValue;

            // assert
            Assert.Equal(actual.Id, expected.Id);
            Assert.Equal(actual.Name, expected.Name);
            Assert.Equal(actual.Latitude, expected.Latitude);
            Assert.Equal(actual.Longitude, expected.Longitude);
            Assert.Equal(actual.Company.Title, expected.Company.Title);
            Assert.Equal(actual.Company.Id, expected.Company.Id);
            foreach (var expectedProduct in expected.Products)
            {
                Assert.Contains(actual.Products, actualProduct =>
                    actualProduct.Id == expectedProduct.Id &&
                    actualProduct.Uid == expectedProduct.Uid &&
                    actualProduct.Name == expectedProduct.Name &&
                    actualProduct.Price == expectedProduct.Price &&
                    actualProduct.Quantity == expectedProduct.Quantity &&
                    actualProduct.QuantityUnit == expectedProduct.QuantityUnit);
            }
        }

        [Fact]
        public async void GetStorage_FromInitializedDbTable_LogicStorageEqualExpectedStorage()
        {
            // arrange
            var expected = GetStorage();

            fixture.db.Add(expected);
            await fixture.db.SaveChangesAsync();

            // act
            var actual = (await logic.GetSingleUpdateModelAsync(expected.Id)).ReturnValue;

            // assert
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Latitude, actual.Latitude);
            Assert.Equal(expected.Longitude, actual.Longitude);
            Assert.Equal(expected.CompanyId, actual.CompanyId);
        }
    }
}

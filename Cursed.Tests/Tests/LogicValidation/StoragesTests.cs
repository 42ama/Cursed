﻿using System;
using System.Collections.Generic;
using Xunit;
using Cursed.Models.LogicValidation;
using Cursed.Models.Entities.Data;
using Cursed.Models.Services;

namespace Cursed.Tests.Tests.LogicValidation
{
    [Collection("Tests collection")]
    public class StoragesTests : IDisposable
    {
        private readonly TestsFixture fixture;
        private readonly StoragesLogicValidation logicValidation;

        public StoragesTests(TestsFixture fixture)
        {
            this.fixture = fixture;
            logicValidation = new StoragesLogicValidation(fixture.db, new StatusMessageFactory());
        }

        public async void Dispose()
        {
            await TestsFixture.ClearDatabase(fixture.db);
        }

        private Company GetCompany()
        {
            return new Company
            {
                Id = 44440,
                Name = "Test company"
            };
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

        private Product GetProduct()
        {
            return new Product
            {
                Id = 44440,
                Uid = 44440,
                StorageId = 44440,
                Price = 17,
                Quantity = 12,
                QuantityUnit = "mg."
            };
        }

        private IEnumerable<Operation> GetOperations()
        {
            return new Operation[]
            {
                new Operation
                {
                    TransactionId = 44440,
                    Id = 44440,
                    ProductId = 44440,
                    StorageToId = 44440,
                    StorageFromId = 44441,
                    Price = 15,
                    Quantity = 22
                },
                new Operation
                {
                    TransactionId = 44440,
                    Id = 44441,
                    ProductId = 44440,
                    StorageToId = 44441,
                    StorageFromId = 44440,
                    Price = 17,
                    Quantity = 8
                }
            };
        }

        [Fact]
        public async void CheckRemoveStorage_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var storage = GetStorage();
            fixture.db.Add(storage);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync(storage.Id);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckRemoveStorage_FromInitializedDbTableWithRelatedEntities_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var storage = GetStorage();
            var product = GetProduct();
            var operations = GetOperations();
            fixture.db.Add(storage);
            fixture.db.Add(product);
            fixture.db.AddRange(operations);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync(storage.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);

        }

        [Fact]
        public async void CheckGetStorage_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var storage = GetStorage();

            // act
            var statusMessage = await logicValidation.CheckGetSingleDataModelAsync(storage.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckGetStorage_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var storage = GetStorage();
            fixture.db.Add(storage);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckGetSingleDataModelAsync(storage.Id);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckGetStorageForUpdate_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var storage = GetStorage();

            // act
            var statusMessage = await logicValidation.CheckGetSingleUpdateModelAsync(storage.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckGetStorageForUpdate_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var storage = GetStorage();
            fixture.db.Add(storage);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckGetSingleUpdateModelAsync(storage.Id);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckUpdateStorage_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var storage = GetStorage();

            // act
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(storage);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckUpdateStorage_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var storage = GetStorage();
            var company = GetCompany();
            fixture.db.Add(storage);
            fixture.db.Add(company);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(storage);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckAddStorage_ToEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var storage = GetStorage();

            // act
            var statusMessage = await logicValidation.CheckAddDataModelAsync(storage);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckAddStorage_WithInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var storage = GetStorage();
            var company = GetCompany();
            fixture.db.Add(storage);
            fixture.db.Add(company);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckAddDataModelAsync(storage);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }
    }
}

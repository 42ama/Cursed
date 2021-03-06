﻿using System;
using Xunit;
using Cursed.Models.LogicValidation;
using Cursed.Models.Entities.Data;
using Cursed.Models.StaticReferences;
using Cursed.Models.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Cursed.Models.DataModel.ErrorHandling;

namespace Cursed.Tests.Tests.LogicValidation
{
    [Collection("Tests collection")]
    public class OperationsTests : IDisposable
    {
        private readonly TestsFixture fixture;
        private readonly OperationsLogicValidation logicValidation;
        private readonly IErrorHandlerFactory errorHandlerFactory;

        public OperationsTests(TestsFixture fixture)
        {
            this.fixture = fixture;
            errorHandlerFactory = new StatusMessageFactory();
            logicValidation = new OperationsLogicValidation(fixture.db, errorHandlerFactory);
        }

        public async void Dispose()
        {
            await TestsFixture.ClearDatabase(fixture.db);
        }

        private Operation GetOperation()
        {
            return new Operation
            {
                Id = 44440,
                ProductId = 44440,
                Price = 12,
                Quantity = 14,
                StorageFromId = 44440,
                StorageToId = 44441,
                TransactionId = 44440
            };
        }

        private ProductCatalog GetProductCatalog()
        {
            return new ProductCatalog
            {
                Id = 44440,
                Cas = 4040404,
                LicenseRequired = true,
                Name = "Testotin"
            };
        }
        private Company GetCompany()
        {
            return new Company
            {
                Id = 44440,
                Name = "Test company"
            };
        }

        private Storage[] GetStorages()
        {
            return new Storage[]
            {
                new Storage
                {
                    Id = 44440,
                    Name = "Test storage",
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
            };
        }

        private TransactionBatch GetOpenTransaction()
        {
            return new TransactionBatch
            {
                Id = 44440,
                CompanyId = 44440,
                Date = DateTime.UtcNow,
                IsOpen = true,
                Type = TransactionTypes.Income,
                Comment = "This is open transaction."
            };
        }
        private TransactionBatch GetClosedTransaction()
        {
            return new TransactionBatch
            {
                Id = 44440,
                CompanyId = 44440,
                Date = DateTime.UtcNow,
                IsOpen = false,
                Type = TransactionTypes.Income,
                Comment = "This is closed transaction."
            };
        }

        [Fact]
        public async void CheckRemoveOperation_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var operation = GetOperation();

            // act
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync(operation.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckRemoveOperation_FromInitializedDbTableOpenTransaction_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var operation = GetOperation();
            var transaction = GetOpenTransaction();
            fixture.db.Add(operation);
            fixture.db.Add(transaction);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync(operation.Id);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckRemoveOperation_FromInitializedDbTableClosedTransaction_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var operation = GetOperation();
            var transaction = GetClosedTransaction();
            fixture.db.Add(operation);
            fixture.db.Add(transaction);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync(operation.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckGetOperationForUpdate_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var operation = GetOperation();

            // act
            var statusMessage = await logicValidation.CheckGetSingleUpdateModelAsync(operation.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckGetOperationForUpdate_FromInitializedDbTableClosedTransaction_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var operation = GetOperation();
            var transaction = GetClosedTransaction();
            fixture.db.Add(operation);
            fixture.db.Add(transaction);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckGetSingleUpdateModelAsync(operation.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckGetOperationForUpdate_FromBadInitializedDbTableOpenTransaction_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var operation = GetOperation();
            var transaction = GetOpenTransaction();
            fixture.db.Add(operation);
            fixture.db.Add(transaction);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckGetSingleUpdateModelAsync(operation.Id);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckUpdateOperation_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var operation = GetOperation();

            // act
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(operation);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckUpdateOperation_FromInitializedDbTableClosedTransaction_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var operation = GetOperation();
            var transaction = GetClosedTransaction();
            fixture.db.Add(operation);
            fixture.db.Add(transaction);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(operation);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckUpdateOperation_FromBadInitializedDbTableOpenTransaction_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var operation = GetOperation();
            var transaction = GetOpenTransaction();
            fixture.db.Add(operation);
            fixture.db.Add(transaction);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(operation);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckUpdateOperation_FromInitializedDbTableOpenTransaction_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var company = GetCompany();
            var productCatalog = GetProductCatalog();
            var storages = GetStorages();
            var operation = GetOperation();
            var transaction = GetOpenTransaction();
            fixture.db.Add(company);
            fixture.db.Add(productCatalog);
            fixture.db.Storage.AddRange(storages);
            fixture.db.Add(operation);
            fixture.db.Add(transaction);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(operation);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckAddOperation_WithBadInitializedDbTableOpenTransaction_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var operation = GetOperation();
            var transaction = GetOpenTransaction();
            fixture.db.Add(operation);
            fixture.db.Add(transaction);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckAddDataModelAsync(operation);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckAddOperation_WithInitializedDbTableOpenTransaction_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var company = GetCompany();
            var productCatalog = GetProductCatalog();
            var storages = GetStorages();
            var operation = GetOperation();
            var transaction = GetOpenTransaction();
            fixture.db.Add(company);
            fixture.db.Add(productCatalog);
            fixture.db.Storage.AddRange(storages);
            fixture.db.Add(operation);
            fixture.db.Add(transaction);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckAddDataModelAsync(operation);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public void CheckValidateModel_WithoutErrors_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = "Technological procces."
            });

            // act
            var actual = logicValidation.ValidateModel(statusMessage, new ModelStateDictionary());

            // assert
            Assert.True(actual.IsCompleted);
        }

        [Fact]
        public void CheckValidateModel_WithErrors_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var statusMessage = errorHandlerFactory.NewErrorHandler(new Problem
            {
                Entity = "Technological procces."
            });
            var modelState = new ModelStateDictionary();
            modelState.AddModelError("key", "error");

            // act
            var actual = logicValidation.ValidateModel(statusMessage, modelState);

            // assert
            Assert.False(actual.IsCompleted);
        }
    }
}

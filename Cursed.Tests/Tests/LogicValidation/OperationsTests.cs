using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Cursed.Models.LogicValidation;
using Cursed.Models.Entities;
using Cursed.Models.Data.Companies;
using Cursed.Models.Data.Shared;
using Cursed.Tests.Extensions;
using Cursed.Models.Data.Utility.ErrorHandling;

namespace Cursed.Tests.Tests.LogicValidation
{
    [Collection("Tests collection")]
    public class OperationsTests : IDisposable
    {
        private readonly TestsFixture fixture;
        private readonly OperationsLogicValidation logicValidation;

        public OperationsTests(TestsFixture fixture)
        {
            this.fixture = fixture;
            logicValidation = new OperationsLogicValidation(fixture.db, new StatusMessageFactory());
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
        public async void CheckGetOperationForUpdate_FromInitializedDbTableOpenTransaction_ErrorHandlerIsCompletedTrue()
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
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(operation.Id);

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
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(operation.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckUpdateOperation_FromInitializedDbTableOpenTransaction_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var operation = GetOperation();
            var transaction = GetOpenTransaction();
            fixture.db.Add(operation);
            fixture.db.Add(transaction);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(operation.Id);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Cursed.Models.LogicValidation;
using Cursed.Models.Entities;
using Cursed.Models.Data.Companies;
using Cursed.Models.Data.Shared;
using Cursed.Tests.Stubs;
using Cursed.Models.Data.Utility.ErrorHandling;

namespace Cursed.Tests.Tests.LogicValidation
{
    [Collection("Tests collection")]
    public class TransactionsTests : IDisposable
    {
        private readonly TestsFixture fixture;
        private readonly TransactionsLogicValidation logicValidation;

        public TransactionsTests(TestsFixture fixture)
        {
            this.fixture = fixture;
            logicValidation = new TransactionsLogicValidation(fixture.db, new OperationValidationStub(), new StatusMessageFactory());
        }

        public async void Dispose()
        {
            await TestsFixture.ClearDatabase(fixture.db);
        }

        private TransactionBatch GetTransaction()
        {
            return new TransactionBatch
            {
                Id = 44440,
                Date = DateTime.UtcNow,
                CompanyId = 44440,
                Type = TransactionTypes.Income,
                IsOpen = true,
                Comment = "Why chicken cross the road?"
            };
        }

        [Fact]
        public async void CheckRemoveTransaction_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var transaction = GetTransaction();
            fixture.db.Add(transaction);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync(transaction.Id);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckRemoveTransaction_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var transaction = GetTransaction();
            

            // act
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync(transaction.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckRemoveTransaction_FromInitializedDbTableButClosed_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var transaction = GetTransaction();
            transaction.IsOpen = false;
            fixture.db.Add(transaction);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync(transaction.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckUpdateTransaction_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var transaction = GetTransaction();

            // act
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(transaction.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckUpdateTransaction_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var transaction = GetTransaction();
            fixture.db.Add(transaction);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(transaction.Id);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckUpdateTransaction_FromInitializedDbTableButClosed_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var transaction = GetTransaction();
            transaction.IsOpen = false;
            fixture.db.Add(transaction);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(transaction.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckGetTransaction_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var transaction = GetTransaction();

            // act
            var statusMessage = await logicValidation.CheckGetSingleDataModelAsync(transaction.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckGetTransaction_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var transaction = GetTransaction();
            fixture.db.Add(transaction);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckGetSingleDataModelAsync(transaction.Id);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckGetTransactionForUpdate_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var transaction = GetTransaction();

            // act
            var statusMessage = await logicValidation.CheckGetSingleUpdateModelAsync(transaction.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckGetTransactionForUpdate_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var transaction = GetTransaction();
            fixture.db.Add(transaction);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckGetSingleUpdateModelAsync(transaction.Id);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }
    }
}

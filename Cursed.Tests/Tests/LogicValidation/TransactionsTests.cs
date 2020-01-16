using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Cursed.Models.LogicValidation;
using Cursed.Models.Entities.Data;
using Cursed.Models.DataModel.Companies;
using Cursed.Models.DataModel.Shared;
using Cursed.Tests.Stubs;
using Cursed.Models.DataModel.Utility.ErrorHandling;

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

        private TransactionBatch[] GetTransactions()
        {
            return new TransactionBatch[]
            {
                new TransactionBatch
                {
                    Id = 44440,
                    Date = DateTime.UtcNow,
                    CompanyId = 44440,
                    Type = TransactionTypes.Income,
                    IsOpen = false,
                    Comment = "Why chicken cross the road?"
                },
                new TransactionBatch
                {
                    Id = 44441,
                    Date = DateTime.UtcNow.AddMonths(-1),
                    CompanyId = 44440,
                    Type = TransactionTypes.Income,
                    IsOpen = false,
                    Comment = "To get to the OTHER SIDE!"
                }
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

        [Fact]
        public async void CheckOpenTransaction_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var transaction = GetTransaction();

            // act
            var statusMessage = await logicValidation.CheckOpenTransactionAsync(transaction.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckOpenTransaction_FromInitializedDbTableTrasnactionIsNotLastClosed_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var transactions = GetTransactions();
            fixture.db.AddRange(transactions);
            await fixture.db.SaveChangesAsync();

            var transactionToCheckId = 44441;

            // act
            var statusMessage = await logicValidation.CheckOpenTransactionAsync(transactionToCheckId);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckOpenTransaction_FromInitializedDbTableTrasnactionIsLastClosed_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var transactions = GetTransactions();
            fixture.db.AddRange(transactions);
            await fixture.db.SaveChangesAsync();

            var transactionToCheckId = 44440;

            // act
            var statusMessage = await logicValidation.CheckOpenTransactionAsync(transactionToCheckId);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }
    }
}

﻿using System;
using Xunit;
using Cursed.Models.LogicValidation;
using Cursed.Models.Entities.Data;
using Cursed.Models.StaticReferences;
using Cursed.Models.Services;

namespace Cursed.Tests.Tests.LogicValidation
{
    [Collection("Tests collection")]
    public class ProductsCatalogTests : IDisposable
    {
        private readonly TestsFixture fixture;
        private readonly ProductsCatalogLogicValidation logicValidation;

        public ProductsCatalogTests(TestsFixture fixture)
        {
            this.fixture = fixture;
            logicValidation = new ProductsCatalogLogicValidation(fixture.db, new StatusMessageFactory());
        }

        public async void Dispose()
        {
            await TestsFixture.ClearDatabase(fixture.db);
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

        private License GetLicense()
        {
            return new License
            {
                Id = 44440,
                ProductId = 44440,
                Date = DateTime.UtcNow,
                GovermentNum = 4040404
            };
        }

        private RecipeProductChanges GetRecipeProductChanges()
        {
            return new RecipeProductChanges
            {
                ProductId = 44440,
                RecipeId = 44440,
                Quantity = (decimal)12.3456,
                Type = ProductCatalogTypes.Product
            };
        }

        [Fact]
        public async void CheckRemoveProductCatalog_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var productCatalog = GetProductCatalog();
            fixture.db.Add(productCatalog);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync(productCatalog.Id);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckRemoveProductCatalog_FromInitializedDbTableWithRelatedEntities_ErrorHandlerIsCompletedFalseHaveSpecificProblems()
        {
            // arrange
            var productCatalog = GetProductCatalog();
            var product = GetProduct();
            var recipeProductChanges = GetRecipeProductChanges();
            var license = GetLicense();
            fixture.db.Add(productCatalog);
            fixture.db.Add(product);
            fixture.db.Add(recipeProductChanges);
            fixture.db.Add(license);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckRemoveDataModelAsync(productCatalog.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
            Assert.Contains(statusMessage.Problems, problem =>
            problem.Entity == "Product." && Int32.Parse(problem.EntityKey) == product.Id);
            Assert.Contains(statusMessage.Problems, problem =>
            problem.Entity.Contains("Product changes.") &&
            Int32.Parse(problem.EntityKey) == recipeProductChanges.RecipeId);
            Assert.Contains(statusMessage.Problems, problem =>
            problem.Entity == "License." && Int32.Parse(problem.EntityKey) == license.Id);
        }

        [Fact]
        public async void CheckGetProductCatalog_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var productCatalog = GetProductCatalog();

            // act
            var statusMessage = await logicValidation.CheckGetSingleDataModelAsync(productCatalog.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckGetProductCatalog_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var productCatalog = GetProductCatalog();
            fixture.db.Add(productCatalog);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckGetSingleDataModelAsync(productCatalog.Id);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckGetProductCatalogForUpdate_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var productCatalog = GetProductCatalog();

            // act
            var statusMessage = await logicValidation.CheckGetSingleUpdateModelAsync(productCatalog.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckGetProductCatalogForUpdate_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var productCatalog = GetProductCatalog();
            fixture.db.Add(productCatalog);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckGetSingleUpdateModelAsync(productCatalog.Id);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckUpdateProductCatalog_FromEmptyDbTable_ErrorHandlerIsCompletedFalse()
        {
            // arrange
            var productCatalog = GetProductCatalog();

            // act
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(productCatalog.Id);

            // assert
            Assert.False(statusMessage.IsCompleted);
        }

        [Fact]
        public async void CheckUpdateProductCatalog_FromInitializedDbTable_ErrorHandlerIsCompletedTrue()
        {
            // arrange
            var productCatalog = GetProductCatalog();
            fixture.db.Add(productCatalog);
            await fixture.db.SaveChangesAsync();

            // act
            var statusMessage = await logicValidation.CheckUpdateDataModelAsync(productCatalog.Id);

            // assert
            Assert.True(statusMessage.IsCompleted);
        }

    }
}

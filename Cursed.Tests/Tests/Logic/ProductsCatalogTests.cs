using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Cursed.Models.Logic;
using Cursed.Models.Entities.Data;
using Cursed.Models.DataModel.ProductsCatalog;
using Cursed.Models.DataModel.Shared;
using Cursed.Models.StaticReferences;
using Cursed.Models.DataModel.Utility;
using Cursed.Tests.Extensions;
using Cursed.Tests.Stubs;

namespace Cursed.Tests.Tests.Logic
{
    [Collection("Tests collection")]
    public class ProductsCatalogTests : IDisposable
    {
        private readonly TestsFixture fixture;
        private readonly ProductsCatalogLogic logic;

        public ProductsCatalogTests(TestsFixture fixture)
        {
            this.fixture = fixture;
            logic = new ProductsCatalogLogic(fixture.db, new LicenseValidationStub());
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
                Name = "Testotin",
                Type = ProductCatalogTypes.Product
            };
        }

        private IEnumerable<ProductCatalog> GetProductsCatalog()
        {
            return new ProductCatalog[]
            {
                new ProductCatalog
                {
                    Id = 44440,
                    Cas = 4040404,
                    Name = "Testin",
                    Type = ProductCatalogTypes.Material,
                    LicenseRequired = true
                },
                new ProductCatalog
                {
                    Id = 44441,
                    Cas = 4040414,
                    Name = "Testotin",
                    Type = ProductCatalogTypes.Product,
                    LicenseRequired = false
                }
            };
        }

        private IEnumerable<License> GetLicenses()
        {
            return new License[]
            {
                new License
                {
                    Id = 44440,
                    Date = DateTime.UtcNow,
                    GovermentNum = 4040404,
                    ProductId = 44440
                },
                new License
                {
                    Id = 44441,
                    Date = DateTime.UtcNow,
                    GovermentNum = 4040414,
                    ProductId = 44440
                },
                new License
                {
                    Id = 44442,
                    Date = DateTime.UtcNow,
                    GovermentNum = 4041404,
                    ProductId = 44441
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
                    StorageId = 44440
                },
                new Product
                {
                    Id = 44441,
                    Uid = 44440,
                    StorageId = 44441
                },
                new Product
                {
                    Id = 44442,
                    Uid = 44440,
                    StorageId = 44442
                },
                new Product
                {
                    Id = 44443,
                    Uid = 44441,
                    StorageId = 44440
                },
            };
        }

        private IEnumerable<RecipeProductChanges> GetRecipeProductsChanges()
        {
            return new RecipeProductChanges[]
            {
                new RecipeProductChanges
                {
                    ProductId = 44440,
                    RecipeId = 44440,
                    Type = ProductCatalogTypes.Material
                },
                new RecipeProductChanges
                {
                    ProductId = 44441,
                    RecipeId = 44440,
                    Type = ProductCatalogTypes.Material
                },
                new RecipeProductChanges
                {
                    ProductId = 44441,
                    RecipeId = 44441,
                    Type = ProductCatalogTypes.Product
                }
            };

        }

        private IEnumerable<Storage> GetStorages()
        {
            return new Storage[]
            {
                    new Storage
                    {
                        Id = 44440,
                        Name = "Storage #1"
                    },
                    new Storage
                    {
                        Id = 44441,
                        Name = "Storage #2"
                    },
                    new Storage
                    {
                        Id = 44442,
                        Name = "Storage #3"
                    }
            };
        }

        private IEnumerable<Recipe> GetRecipes()
        {
            return new Recipe[]
            {
                new Recipe
                {
                    Id = 44440,
                    Content = "The music is too bad, I can't hear anything. *showes some dancing moves*"
                }
            };
        }

        [Fact]
        public async void AddProductCatalog_ToEmptyDbTable_AddedProductCatalogEqualExpectedProductCatalog()
        {
            // arrange
            var expected = GetProductCatalog();

            // act
            await logic.AddDataModelAsync(expected);

            // assert
            var actual = await fixture.db.ProductCatalog.FirstOrDefaultAsync(i => i.Id == expected.Id);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Cas, actual.Cas);
            Assert.Equal(expected.LicenseRequired, actual.LicenseRequired);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Type, actual.Type);
        }

        [Fact]
        public async void RemoveProductCatalog_FromInitializedDbTable_RemovedProductCatalogNotFoundInDb()
        {
            // arrange
            var product = GetProductCatalog();
            fixture.db.Add(product);
            await fixture.db.SaveChangesAsync();

            // act
            await logic.RemoveDataModelAsync(product.Id);

            // assert
            var actual = await fixture.db.ProductCatalog.FirstOrDefaultAsync(i => i.Id == product.Id);
            Assert.Null(actual);
        }

        [Fact]
        public async void UpdateProductCatalog_AtInitializedDbTable_UpdatedProductCatalogEqualExpectedProductCatalog()
        {
            // arrange
            var product = GetProductCatalog();
            fixture.db.Add(product);
            await fixture.db.SaveChangesAsync();

            var expected = new ProductCatalog
            {
                Id = product.Id,
                Cas = 4040404,
                LicenseRequired = true,
                Name = "Testedtin",
                Type = ProductCatalogTypes.Product
            };

            // act
            await logic.UpdateDataModelAsync(expected);

            // assert
            var actual = await fixture.db.ProductCatalog.FirstOrDefaultAsync(i => i.Id == expected.Id);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Cas, actual.Cas);
            Assert.Equal(expected.LicenseRequired, actual.LicenseRequired);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Type, actual.Type);
        }

        [Fact]
        public async void GetListProductsCatalogModel_FromInitializedDbTables_LogicProductsCatalogModelsEqualExpectedProductsCatalogModels()
        {
            // arrange
            var productsCatalog = GetProductsCatalog();
            var licenses = GetLicenses();
            var products = GetProducts();
            var recipeProductsChanges = GetRecipeProductsChanges();

            fixture.db.ProductCatalog.AddRange(productsCatalog);
            fixture.db.License.AddRange(licenses);
            fixture.db.Product.AddRange(products);
            fixture.db.RecipeProductChanges.AddRange(recipeProductsChanges);
            await fixture.db.SaveChangesAsync();
            var expected = new List<ProductsCatalogModel>
            {
                new ProductsCatalogModel
                {
                    ProductId = 44440,
                    CAS = 4040404,
                    Name = "Testin",
                    Type = ProductCatalogTypes.Material,
                    LicenseRequired = true,
                    IsValid = true,
                    StoragesCount = 3,
                    RecipesCount = 1
                },
                new ProductsCatalogModel
                {
                    ProductId = 44441,
                    CAS = 4040414,
                    Name = "Testotin",
                    Type = ProductCatalogTypes.Product,
                    LicenseRequired = false,
                    IsValid = true,
                    StoragesCount = 1,
                    RecipesCount = 2
                }
            };

            // act
            var actual = (await logic.GetAllDataModelAsync()).ToList();

            // assert
            foreach (var expectedItem in expected)
            {
                Assert.Contains(actual, actualItem =>
                    expectedItem.ProductId == actualItem.ProductId &&
                    expectedItem.CAS == actualItem.CAS &&
                    expectedItem.Name == actualItem.Name &&
                    expectedItem.Type == actualItem.Type &&
                    expectedItem.LicenseRequired == actualItem.LicenseRequired &&
                    expectedItem.IsValid == actualItem.IsValid &&
                    expectedItem.StoragesCount == actualItem.StoragesCount &&
                    expectedItem.RecipesCount == actualItem.RecipesCount);
            }
        }

        [Fact]
        public async void GetProductCatalogModel_FromInitializedDbTables_LogicProductCatalogModelEqualExpectedProductCatalogModel()
        {
            // arrange
            var productsCatalog = GetProductsCatalog();
            var licenses = GetLicenses();
            var storages = GetStorages();
            var recipes = GetRecipes();
            var products = GetProducts();
            var recipeProductsChanges = GetRecipeProductsChanges();

            fixture.db.ProductCatalog.AddRange(productsCatalog);
            fixture.db.License.AddRange(licenses);
            fixture.db.Storage.AddRange(storages);
            fixture.db.Product.AddRange(products);
            fixture.db.Recipe.AddRange(recipes);
            fixture.db.RecipeProductChanges.AddRange(recipeProductsChanges);
            await fixture.db.SaveChangesAsync();
            var expected = new ProductCatalogModel
            {
                ProductId = 44440,
                CAS = 4040404,
                Name = "Testin",
                Type = ProductCatalogTypes.Material,
                LicenseRequired = true,
                Licenses = new List<(License license, bool isValid)>(),
                Storages = new List<TitleIdContainer>(),
                Recipes = new List<TitleIdContainer>()
        };
            
            foreach (var license in licenses)
            {
                expected.Licenses.Add((license,true));
            }
            foreach (var storage in storages)
            {
                expected.Storages.Add(new TitleIdContainer { Title = storage.Name, Id = storage.Id });
            }
            foreach (var recipe in recipes)
            {
                if(recipe.Content.Length >= 45)
                {
                    expected.Recipes.Add(new TitleIdContainer { Id = recipe.Id, Title = recipe.Content.Substring(0, 45) + "..." });
                }
            }
            

            // act
            var actual = await logic.GetSingleDataModelAsync(expected.ProductId);

            // assert
            Assert.Equal(actual.ProductId, expected.ProductId);
            Assert.Equal(actual.CAS, expected.CAS);
            Assert.Equal(actual.Name, expected.Name);
            Assert.Equal(actual.Type, expected.Type);
            Assert.Equal(actual.LicenseRequired, expected.LicenseRequired);
            foreach (var expectedStorage in expected.Storages)
            {
                Assert.Contains(actual.Storages, actualStorage =>
                    expectedStorage.Id == actualStorage.Id &&
                    expectedStorage.Title == actualStorage.Title);
            }
            foreach (var expectedRecipe in expected.Recipes)
            {
                Assert.Contains(actual.Recipes, actualRecipe =>
                    expectedRecipe.Id == actualRecipe.Id &&
                    expectedRecipe.Title == actualRecipe.Title);
            }
        }

        [Fact]
        public async void GetProductCatalog_FromInitializedDbTable_LogicProductCatalogEqualExpectedProductCatalog()
        {
            // arrange
            var expected = GetProductCatalog();

            fixture.db.ProductCatalog.Add(expected);
            await fixture.db.SaveChangesAsync();

            // act
            var actual = await logic.GetSingleUpdateModelAsync(expected.Id);

            // assert
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Cas, actual.Cas);
            Assert.Equal(expected.LicenseRequired, actual.LicenseRequired);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Type, actual.Type);
        }
    }
}

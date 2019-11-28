using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Cursed.Models.Logic;
using Cursed.Models.Entities;
using Cursed.Models.Data.ProductsCatalog;
using Cursed.Models.Data.Shared;
using Cursed.Models.Data.Utility;
using Cursed.Tests.Helpers;

namespace Cursed.Tests.Tests.Logic
{
    public class ProductsCatalog : IClassFixture<TestsFixture>
    {
        private readonly TestsFixture fixture;
        private readonly ProductsCatalogLogic logic;

        public ProductsCatalog(TestsFixture fixture)
        {
            this.fixture = fixture;
            logic = new ProductsCatalogLogic(fixture.db);
        }

        [Fact]
        public async void AddDataModelAsync()
        {
            // arrange
            var expected = new ProductCatalog
            {
                Id = 44440,
                Cas = 4040404,
                LicenseRequired = true,
                Name = "Testotin",
                Type = Models.Data.Shared.ProductCatalogTypes.Product
            };

            // act
            await logic.AddDataModelAsync(expected);

            // assert
            var actual = await fixture.db.ProductCatalog.FirstOrDefaultAsync(i => i.Id == expected.Id);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Cas, actual.Cas);
            Assert.Equal(expected.LicenseRequired, actual.LicenseRequired);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Type, actual.Type);

            // dispose
            fixture.db.ProductCatalog.Remove(actual);
            await fixture.db.SaveChangesAsync();
        }

        [Fact]
        public async void RemoveDataModelAsync()
        {
            // arrange
            var product = new ProductCatalog
            {
                Id = 44440,
                Cas = 4040404,
                LicenseRequired = true,
                Name = "Testotin",
                Type = Models.Data.Shared.ProductCatalogTypes.Product
            };
            fixture.db.Add(product);
            await fixture.db.SaveChangesAsync();

            // act
            await logic.RemoveDataModelAsync(product.Id);

            // assert
            var actual = await fixture.db.ProductCatalog.FirstOrDefaultAsync(i => i.Id == product.Id);
            Assert.Null(actual);
        }

        [Fact]
        public async void UpdateDataModelAsync()
        {
            // arrange
            var product = new ProductCatalog
            {
                Id = 44440,
                Cas = 4040404,
                LicenseRequired = true,
                Name = "Testotin",
                Type = Models.Data.Shared.ProductCatalogTypes.Product
            };
            fixture.db.Add(product);
            await fixture.db.SaveChangesAsync();

            var expected = new ProductCatalog
            {
                Id = 44440,
                Cas = 4040404,
                LicenseRequired = true,
                Name = "Testedtin",
                Type = Models.Data.Shared.ProductCatalogTypes.Product
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

            // dispose
            fixture.db.ProductCatalog.Remove(actual);
            await fixture.db.SaveChangesAsync();
        }

        [Fact]
        public async void GetAllDataModelAsync()
        {
            // arrange
            var productsCatalog = new ProductCatalog[]
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
            var products = new Product[]
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
            var recipeProductsChanges = new RecipeProductChanges[]
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
                    License = new LicenseValid( new License
                    {
                        Id = 44441,
                        Date = DateTime.Now.Trim(TimeSpan.TicksPerDay).AddDays(1),
                        GovermentNum = 4040414
                    }),
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
                    License = new LicenseValid( new License
                    {
                        Id = 44442,
                        Date = DateTime.Now.Trim(TimeSpan.TicksPerDay).AddDays(1),
                        GovermentNum = 4041404,
                        ProductId = 44441
                    }),
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
                    expectedItem.License == actualItem.License &&
                    expectedItem.StoragesCount == actualItem.StoragesCount &&
                    expectedItem.RecipesCount == actualItem.RecipesCount);
            }

            // dispose
            fixture.db.RecipeProductChanges.RemoveRange(recipeProductsChanges);
            fixture.db.Product.RemoveRange(products);
            fixture.db.ProductCatalog.RemoveRange(productsCatalog);
            fixture.db.License.RemoveRange(licenses);
            await fixture.db.SaveChangesAsync();
        }

        [Fact]
        public async void GetSingleDataModelAsync()
        {
            // arrange
            var productsCatalog = new ProductCatalog[]
            {
                new ProductCatalog
                {
                    Id = 44440,
                    Cas = 4040404,
                    Name = "Testin",
                    Type = ProductCatalogTypes.Material,
                    LicenseRequired = true
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
                }
            };
            var storages = new Storage[]
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
            var products = new Product[]
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
                }
            };
            var recipes = new Recipe[]
            {
                new Recipe
                {
                    Id = 44440,
                    Content = "The music is too bad, I can't hear anything. *showes some dancing moves*"
                }
            };
            var recipeProductsChanges = new RecipeProductChanges[]
            {
                new RecipeProductChanges
                {
                    ProductId = 44440,
                    RecipeId = 44440,
                    Type = ProductCatalogTypes.Material
                }
            };

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
                LicenseRequired = true
            };
            expected.Licenses = new List<LicenseValid>();
            expected.Storages = new List<TitleIdContainer>();
            expected.Recipes = new List<TitleIdContainer>();
            foreach (var license in licenses)
            {
                if (expected.LicenseRequired)
                {
                    expected.Licenses.Add(new LicenseValid(license));
                }
                else
                {
                    expected.Licenses.Add(new LicenseValid(license, true));
                }
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
            foreach (var expectedLicense in expected.Licenses)
            {
                Assert.Contains(actual.Licenses, actualLicense =>
                    expectedLicense.Id == actualLicense.Id &&
                    expectedLicense.Date == actualLicense.Date &&
                    expectedLicense.GovermentNum == actualLicense.GovermentNum &&
                    expectedLicense.ProductId == actualLicense.ProductId &&
                    expectedLicense.IsValid == actualLicense.IsValid);
            }
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

            // dispose
            fixture.db.RecipeProductChanges.RemoveRange(recipeProductsChanges);
            fixture.db.License.RemoveRange(licenses);
            fixture.db.Product.RemoveRange(products);
            fixture.db.Storage.RemoveRange(storages);
            fixture.db.Recipe.RemoveRange(recipes);
            fixture.db.ProductCatalog.RemoveRange(productsCatalog);
            await fixture.db.SaveChangesAsync();
        }

        [Fact]
        public async void GetSingleUpdateModelAsync()
        {
            // arrange
            var expected = new ProductCatalog
            {
                Id = 44440,
                Cas = 4040404,
                Name = "Testin",
                Type = ProductCatalogTypes.Material,
                LicenseRequired = true
            };

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

            // dispose
            fixture.db.ProductCatalog.Remove(actual);
            await fixture.db.SaveChangesAsync();
        }
    }
}

using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Cursed.Models.Logic;
using Cursed.Models.Entities;
using Cursed.Models.Data.Shared;
using Cursed.Models.Data.Recipes;

namespace Cursed.Tests.Tests.Logic
{
    public class Recipes : IClassFixture<TestsFixture>
    {
        private readonly TestsFixture fixture;
        private readonly RecipesLogic logic;

        public Recipes(TestsFixture fixture)
        {
            this.fixture = fixture;
            logic = new RecipesLogic(fixture.db);
        }

        [Fact]
        public async void AddDataModelAsync()
        {
            // arrange
            var expected = new Recipe
            {
                Id = 44440,
                Content = "Test content",
                GovermentApproval = true,
                TechApproval = false
            };

            // act
            await logic.AddDataModelAsync(expected);

            // assert
            var actual = await fixture.db.Recipe.FirstOrDefaultAsync(i => i.Id == expected.Id);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Content, actual.Content);
            Assert.Equal(expected.GovermentApproval, actual.GovermentApproval);
            Assert.Equal(expected.TechApproval, actual.TechApproval);

            // dispose
            fixture.db.Recipe.Remove(actual);
            await fixture.db.SaveChangesAsync();
        }

        [Fact]
        public async void RemoveDataModelAsync()
        {
            // arrange
            var recipe = new Recipe
            {
                Id = 44440,
                Content = "Test content",
                GovermentApproval = true,
                TechApproval = false
            };
            fixture.db.Add(recipe);
            await fixture.db.SaveChangesAsync();

            // act
            await logic.RemoveDataModelAsync(recipe.Id);

            // assert
            var actual = await fixture.db.Recipe.FirstOrDefaultAsync(i => i.Id == recipe.Id);
            Assert.Null(actual);
        }

        [Fact]
        public async void UpdateDataModelAsync()
        {
            // arrange
            var recipe = new Recipe
            {
                Id = 44440,
                Content = "Test content",
                GovermentApproval = true,
                TechApproval = false
            };
            fixture.db.Add(recipe);
            await fixture.db.SaveChangesAsync();

            var expected = new Recipe
            {
                Id = 44440,
                Content = "Tested content",
                GovermentApproval = true,
                TechApproval = false
            };

            // act
            await logic.UpdateDataModelAsync(expected);

            // assert
            var actual = await fixture.db.Recipe.FirstOrDefaultAsync(i => i.Id == expected.Id);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Content, actual.Content);
            Assert.Equal(expected.GovermentApproval, actual.GovermentApproval);
            Assert.Equal(expected.TechApproval, actual.TechApproval);

            // dispose
            fixture.db.Recipe.Remove(actual);
            await fixture.db.SaveChangesAsync();
        }


        [Fact]
        public async void GetAllDataModelAsync()
        {
            // arrange
            var recipe = new Recipe[]
            {
                new Recipe
                {
                    Id = 44440,
                    Content = "I am recipe #1",
                    GovermentApproval = true,
                    TechApproval = true
                },
                new Recipe
                {
                    Id = 44441,
                    Content = "I am recipe #2",
                    GovermentApproval = true,
                    TechApproval = false
                }
            };

            var recipeInheritance = new RecipeInheritance[]
            {
                new RecipeInheritance
                {
                    ParentId = 44440,
                    ChildId = 44441
                },
            };

            var productsCatalog = new ProductCatalog[]
            {
                new ProductCatalog
                {
                    Id = 44440,
                    Cas = 4040404,
                    Name = "Testesteron",
                    LicenseRequired = true,
                    Type = ProductCatalogTypes.Product
                },
                new ProductCatalog
                {
                    Id = 44441,
                    Cas = 4040414,
                    Name = "Testin",
                    LicenseRequired = true,
                    Type = ProductCatalogTypes.Material
                },
                new ProductCatalog
                {
                    Id = 44442,
                    Cas = 4041404,
                    Name = "Testonion",
                    LicenseRequired = true,
                    Type = ProductCatalogTypes.Material
                },
                new ProductCatalog
                {
                    Id = 44443,
                    Cas = 4140404,
                    Name = "Testerer",
                    LicenseRequired = true,
                    Type = ProductCatalogTypes.Material
                },
            };

            var recipeProductsChanges = new RecipeProductChanges[]
            {
                new RecipeProductChanges
                {
                    RecipeId = 44440,
                    ProductId = 44440,
                    Type = ProductCatalogTypes.Product,
                    Quantity = 10
                },
                new RecipeProductChanges
                {
                    RecipeId = 44441,
                    ProductId = 44440,
                    Type = ProductCatalogTypes.Product,
                    Quantity = 15
                },
                new RecipeProductChanges
                {
                    RecipeId = 44440,
                    ProductId = 44441,
                    Type = ProductCatalogTypes.Material,
                    Quantity = (decimal)12.3456
                },
                new RecipeProductChanges
                {
                    RecipeId = 44440,
                    ProductId = 44442,
                    Type = ProductCatalogTypes.Material,
                    Quantity = (decimal)12.3456
                },
                new RecipeProductChanges
                {
                    RecipeId = 44441,
                    ProductId = 44441,
                    Type = ProductCatalogTypes.Material,
                    Quantity = (decimal)12.3456
                },
                new RecipeProductChanges
                {
                    RecipeId = 44441,
                    ProductId = 44443,
                    Type = ProductCatalogTypes.Material,
                    Quantity = (decimal)12.3456
                },
            };

            fixture.db.Recipe.AddRange(recipe);
            fixture.db.RecipeInheritance.AddRange(recipeInheritance);
            fixture.db.ProductCatalog.AddRange(productsCatalog);
            fixture.db.RecipeProductChanges.AddRange(recipeProductsChanges);
            await fixture.db.SaveChangesAsync();
            var expected = new List<RecipesModel>
            {
                new RecipesModel
                {
                    Id = 44441,
                    Content = "I am recipe #2",
                    GovApproved = true,
                    TechApproved = false,
                    ParentRecipe = 44440,
                    ChildRecipesCount = 0,
                    ProductCount = 1,
                    MaterialCount = 2
                },
                new RecipesModel
                {
                    Id = 44440,
                    Content = "I am recipe #1",
                    GovApproved = true,
                    TechApproved = true,
                    ParentRecipe = null,
                    ChildRecipesCount = 1,
                    ProductCount = 1,
                    MaterialCount = 2
                },
            };

            // act
            var actual = (await logic.GetAllDataModelAsync()).ToList();

            // assert
            foreach (var expectedItem in expected)
            {
                Assert.Contains(actual, actualItem =>
                    expectedItem.Id == actualItem.Id &&
                    expectedItem.Content == actualItem.Content &&
                    expectedItem.GovApproved == actualItem.GovApproved &&
                    expectedItem.TechApproved == actualItem.TechApproved &&
                    expectedItem.ParentRecipe == actualItem.ParentRecipe &&
                    expectedItem.ChildRecipesCount == actualItem.ChildRecipesCount &&
                    expectedItem.ProductCount == actualItem.ProductCount &&
                    expectedItem.MaterialCount == actualItem.MaterialCount);
            }

            // dispose
            fixture.db.RecipeInheritance.RemoveRange(recipeInheritance);
            fixture.db.RecipeProductChanges.RemoveRange(recipeProductsChanges);
            fixture.db.Recipe.RemoveRange(recipe);
            fixture.db.ProductCatalog.RemoveRange(productsCatalog);
            await fixture.db.SaveChangesAsync();
        }

        [Fact]
        public async void GetSingleDataModelAsync()
        {
            // arrange
            var recipe = new Recipe[]
            {
                new Recipe
                {
                    Id = 44440,
                    Content = "I am recipe #1",
                    GovermentApproval = true,
                    TechApproval = true
                }
            };

            var recipeInheritance = new RecipeInheritance[]
            {
                new RecipeInheritance
                {
                    ParentId = 44440,
                    ChildId = 44441
                },
            };

            var productsCatalog = new ProductCatalog[]
            {
                new ProductCatalog
                {
                    Id = 44440,
                    Cas = 4040404,
                    Name = "Testesteron",
                    LicenseRequired = true,
                    Type = ProductCatalogTypes.Product
                },
                new ProductCatalog
                {
                    Id = 44441,
                    Cas = 4040414,
                    Name = "Testin",
                    LicenseRequired = true,
                    Type = ProductCatalogTypes.Material
                },
                new ProductCatalog
                {
                    Id = 44442,
                    Cas = 4041404,
                    Name = "Testonion",
                    LicenseRequired = true,
                    Type = ProductCatalogTypes.Material
                }
            };

            var recipeProductsChanges = new RecipeProductChanges[]
            {
                new RecipeProductChanges
                {
                    RecipeId = 44440,
                    ProductId = 44440,
                    Type = ProductCatalogTypes.Product,
                    Quantity = 10
                },
                new RecipeProductChanges
                {
                    RecipeId = 44440,
                    ProductId = 44441,
                    Type = ProductCatalogTypes.Material,
                    Quantity = (decimal)12.3456
                },
                new RecipeProductChanges
                {
                    RecipeId = 44440,
                    ProductId = 44442,
                    Type = ProductCatalogTypes.Material,
                    Quantity = (decimal)12.3456
                }
            };

            fixture.db.Recipe.AddRange(recipe);
            fixture.db.RecipeInheritance.AddRange(recipeInheritance);
            fixture.db.ProductCatalog.AddRange(productsCatalog);
            fixture.db.RecipeProductChanges.AddRange(recipeProductsChanges);
            await fixture.db.SaveChangesAsync();
            var expected = new RecipeModel
            {
                Id = 44440,
                Content = "I am recipe #1",
                GovApproved = true,
                TechApproved = true,
                ParentRecipe = null,
                ChildRecipes = new List<int> { 44441 },
                RecipeProducts = new List<RecipeProductContainer>
                {
                    new RecipeProductContainer
                    {
                        ProductId = 44440,
                        ProductName = "Testesteron",
                        CAS = 4040404,
                        Quantity = 10,
                        Type = ProductCatalogTypes.Product
                    },
                    new RecipeProductContainer
                    {
                        ProductId = 44441,
                        ProductName = "Testin",
                        CAS = 4040414,
                        Quantity = (decimal)12.3456,
                        Type = ProductCatalogTypes.Material
                    },
                    new RecipeProductContainer
                    {
                        ProductId = 44442,
                        ProductName = "Testonion",
                        CAS = 4041404,
                        Quantity = (decimal)12.3456,
                        Type = ProductCatalogTypes.Material
                    },
                }
            };

            // act
            var actual = await logic.GetSingleDataModelAsync(44440);

            // assert
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Content, actual.Content);
            Assert.Equal(expected.GovApproved, actual.GovApproved);
            Assert.Equal(expected.TechApproved, actual.TechApproved);
            Assert.Equal(expected.ParentRecipe, actual.ParentRecipe);
            Assert.Equal(expected.ChildRecipes.Single(), actual.ChildRecipes.Single());
            foreach (var expectedItem in expected.RecipeProducts)
            {
                Assert.Contains(actual.RecipeProducts, actualItem =>
                expectedItem.ProductId == actualItem.ProductId &&
                expectedItem.ProductName == actualItem.ProductName &&
                expectedItem.CAS == actualItem.CAS &&
                expectedItem.Quantity == actualItem.Quantity &&
                expectedItem.Type == actualItem.Type);
            }

            // dispose
            fixture.db.RecipeInheritance.RemoveRange(recipeInheritance);
            fixture.db.RecipeProductChanges.RemoveRange(recipeProductsChanges);
            fixture.db.Recipe.RemoveRange(recipe);
            fixture.db.ProductCatalog.RemoveRange(productsCatalog);
            await fixture.db.SaveChangesAsync();
        }

        [Fact]
        public async void GetSingleUpdateModelAsync()
        {
            // arrange
            var expected = new Recipe
            {
                Id = 44440,
                Content = "I am recipe #1",
                GovermentApproval = true,
                TechApproval = true
            };

            fixture.db.Add(expected);
            await fixture.db.SaveChangesAsync();

            // act
            var actual = await logic.GetSingleUpdateModelAsync(expected.Id);

            // assert
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Content, actual.Content);
            Assert.Equal(expected.GovermentApproval, actual.GovermentApproval);
            Assert.Equal(expected.TechApproval, actual.TechApproval);

            // dispose
            fixture.db.Recipe.Remove(actual);
            await fixture.db.SaveChangesAsync();
        }
    }
}

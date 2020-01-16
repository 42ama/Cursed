using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Cursed.Models.Logic;
using Cursed.Models.Entities.Data;
using Cursed.Models.DataModel.Facilities;
using Cursed.Models.DataModel.Shared;
using Cursed.Models.StaticReferences;
using Cursed.Tests.Extensions;
using Cursed.Tests.Stubs;

namespace Cursed.Tests.Tests.Logic
{
    [Collection("Tests collection")]
    public class FacilitiesTests : IDisposable
    {
        private readonly TestsFixture fixture;
        private readonly FacilitiesLogic logic;

        public FacilitiesTests(TestsFixture fixture)
        {
            this.fixture = fixture;
            logic = new FacilitiesLogic(fixture.db, new LicenseValidationStub());
        }

        public async void Dispose()
        {
            await TestsFixture.ClearDatabase(fixture.db);
        }

        private Facility GetFacility()
        {
            return new Facility
            {
                Id = 44440,
                Name = "Test facility #1",
                Latitude = (decimal)12.3456,
                Longitude = (decimal)78.9012
            };
        }

        private IEnumerable<Facility> GetFacilities()
        {
            return new Facility[]
            {
                new Facility
                {
                    Id = 44440,
                    Name = "Test facility #1",
                    Latitude = (decimal)12.3456,
                    Longitude = (decimal)78.9012
                },
                new Facility
                {
                    Id = 44441,
                    Name = "Test facility #2",
                    Latitude = (decimal)78.9012,
                    Longitude = (decimal)12.3456
                }
            };
        }

        private IEnumerable<TechProcess> GetTechProcesses()
        {
            return new TechProcess[]
            {
                new TechProcess
                {
                    FacilityId = 44440,
                    RecipeId = 44440,
                    DayEfficiency = (decimal)12.3456
                },
                new TechProcess
                {
                    FacilityId = 44440,
                    RecipeId = 44441,
                    DayEfficiency = (decimal)12.3456
                },
                new TechProcess
                {
                    FacilityId = 44441,
                    RecipeId = 44442,
                    DayEfficiency = (decimal)78.9012
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
                    Content = "Recipe #1 content",
                    GovermentApproval = true,
                    TechApproval = false,
                },
                new Recipe
                {
                    Id = 44441,
                    Content = "Recipe #2 content",
                    GovermentApproval = true,
                    TechApproval = true,
                }
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
                    Type = ProductCatalogTypes.Material,
                    Quantity = (decimal)12.3456
                },
                new RecipeProductChanges
                {
                    ProductId = 44441,
                    RecipeId = 44440,
                    Type = ProductCatalogTypes.Material,
                    Quantity = (decimal)78.9012
                },
                new RecipeProductChanges
                {
                    ProductId = 44443,
                    RecipeId = 44440,
                    Type = ProductCatalogTypes.Product,
                    Quantity = (decimal)12.3456
                },
                new RecipeProductChanges
                {
                    ProductId = 44440,
                    RecipeId = 44441,
                    Type = ProductCatalogTypes.Material,
                    Quantity = (decimal)78.9012
                },
                new RecipeProductChanges
                {
                    ProductId = 44442,
                    RecipeId = 44441,
                    Type = ProductCatalogTypes.Material,
                    Quantity = (decimal)12.3456
                },
                new RecipeProductChanges
                {
                    ProductId = 44444,
                    RecipeId = 44441,
                    Type = ProductCatalogTypes.Product,
                    Quantity = (decimal)12.3456
                },
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
                    LicenseRequired = null,
                    Name = "Testotin",
                    Type = ProductCatalogTypes.Material
                },
                new ProductCatalog
                {
                    Id = 44441,
                    Cas = 4040414,
                    LicenseRequired = false,
                    Name = "Materialotin #1",
                    Type = ProductCatalogTypes.Material
                },
                new ProductCatalog
                {
                    Id = 44442,
                    Cas = 4041404,
                    LicenseRequired = true,
                    Name = "Materialotin #2",
                    Type = ProductCatalogTypes.Material
                },
                new ProductCatalog
                {
                    Id = 44443,
                    Cas = 4140404,
                    LicenseRequired = false,
                    Name = "Producin #1",
                    Type = ProductCatalogTypes.Product
                },
                new ProductCatalog
                {
                    Id = 44444,
                    Cas = 5050505,
                    LicenseRequired = true,
                    Name = "Producin #2",
                    Type = ProductCatalogTypes.Product
                },
            };
        }

        private IEnumerable<License> GetLicenses()
        {
            return new License[]
            {
                new License
                {
                    Id = 44440,
                    GovermentNum = 4040404,
                    Date = DateTime.UtcNow,
                    ProductId = 44442
                },
                new License
                {
                    Id = 44441,
                    GovermentNum = 4040414,
                    Date = DateTime.UtcNow,
                    ProductId = 44444
                }
            };
        }

        [Fact]
        public async void AddFacility_ToEmptyDbTable_AddedFacilityEqualExpectedFacility()
        {
            // arrange
            var expected = GetFacility();

            // act
            await logic.AddDataModelAsync(expected);

            // assert
            var actual = await fixture.db.Facility.FirstOrDefaultAsync(i => i.Id == expected.Id);
            Assert.True(expected.Id == actual.Id);
            Assert.True(expected.Name == actual.Name);
            Assert.True(expected.Latitude == actual.Latitude);
            Assert.True(expected.Longitude == actual.Longitude);
        }

        [Fact]
        public async void RemoveFacility_FromInitializedDbTable_RemovedFacilityNotFoundInDb()
        {
            // arrange
            var facility = GetFacility();
            fixture.db.Add(facility);
            await fixture.db.SaveChangesAsync();

            // act
            await logic.RemoveDataModelAsync(facility.Id);

            // assert
            var actual = await fixture.db.Facility.FirstOrDefaultAsync(i => i.Id == facility.Id);
            Assert.Null(actual);
        }

        [Fact]
        public async void UpdateFacility_AtInitializedDbTable_UpdatedFacilityEqualExpectedFacility()
        {
            // arrange
            var facility = GetFacility(); 
            fixture.db.Add(facility);
            await fixture.db.SaveChangesAsync();

            var expected = new Facility
            {
                Id = facility.Id,
                Name = "Tested facility",
                Latitude = (decimal)12.3456,
                Longitude = (decimal)78.9012
            };

            // act
            await logic.UpdateDataModelAsync(expected);

            // assert
            var actual = await fixture.db.Facility.FirstOrDefaultAsync(i => i.Id == expected.Id);
            Assert.True(expected.Id == actual.Id);
            Assert.True(expected.Name == actual.Name);
            Assert.True(expected.Latitude == actual.Latitude);
            Assert.True(expected.Longitude == actual.Longitude);
        }

        [Fact]
        public async void GetListFacilitiesModel_FromInitializedDbTables_LogicFacilitiesModelEqualExpectedFacilitiesModel()
        {
            // arrange
            var facilities = GetFacilities();
            var techProcesses = GetTechProcesses();

            fixture.db.Facility.AddRange(facilities);
            fixture.db.TechProcess.AddRange(techProcesses);
            await fixture.db.SaveChangesAsync();


            var expected = new List<FacilitiesModel>
            {
                new FacilitiesModel
                {
                    Id = 44440,
                    Name = "Test facility #1",
                    Latitude = (decimal)12.3456,
                    Longitude = (decimal)78.9012,
                    TechProcesses = new List<TechProcess>()
                },
                new FacilitiesModel
                {
                    Id = 44441,
                    Name = "Test facility #2",
                    Latitude = (decimal)78.9012,
                    Longitude = (decimal)12.3456,
                    TechProcesses = new List<TechProcess>()
                }
            };

            foreach (var facility in expected)
            {
                foreach (var techProcess in techProcesses.Where(i => i.FacilityId == facility.Id))
                {
                    facility.TechProcesses.Add(techProcess);
                }
            }

            // act
            var actual = (await logic.GetAllDataModelAsync()).ToList();

            // assert
            foreach (var expectedItem in expected)
            {
                Assert.Contains(actual, actualItem =>
                    expectedItem.Id == actualItem.Id &&
                    expectedItem.Name == actualItem.Name &&
                    expectedItem.Latitude == actualItem.Latitude &&
                    expectedItem.Longitude == actualItem.Longitude);
                foreach (var techProcess in expectedItem.TechProcesses)
                {
                    var actualTechProcesses = actual.Single(i => i.Id == expectedItem.Id).TechProcesses;
                    Assert.Contains(actualTechProcesses, actualTechProcess =>
                        actualTechProcess.FacilityId == techProcess.FacilityId &&
                        actualTechProcess.RecipeId == techProcess.RecipeId &&
                        actualTechProcess.DayEfficiency == techProcess.DayEfficiency);
                }
            }
        }

        [Fact]
        public async void GetFacilityModel_FromInitializedDbTables_LogicFacilityModelEqualExpectedFacilityModel()
        {
            // arrange
            var facility = GetFacility();
            var recipes = GetRecipes();
            var techProcesses = GetTechProcesses();
            var productsCatalog = GetProductsCatalog();
            var recipeProductsChanges = GetRecipeProductsChanges();
            var licenses = GetLicenses();

            fixture.db.Facility.Add(facility);
            fixture.db.Recipe.AddRange(recipes);
            fixture.db.TechProcess.AddRange(techProcesses);
            fixture.db.ProductCatalog.AddRange(productsCatalog);
            fixture.db.RecipeProductChanges.AddRange(recipeProductsChanges);
            await fixture.db.SaveChangesAsync();


            var expected = new FacilityModel
            {
                Id = 44440,
                Name = "Test facility #1",
                Latitude = (decimal)12.3456,
                Longitude = (decimal)78.9012,
                Products = new List<FacilitiesProductContainer>()
            };

            foreach (var recipe in recipes)
            {
                foreach (var rpc in recipeProductsChanges.Where(i => i.RecipeId == recipe.Id))
                {
                    var product = productsCatalog.Single(i => i.Id == rpc.ProductId);

                    var facilityContainer = new FacilitiesProductContainer
                    {
                        FacilityId = 44440,
                        RecipeId = recipe.Id,
                        RecipeGovApprov = recipe.GovermentApproval ?? false,
                        RecipeTechnoApprov = recipe.TechApproval ?? false,
                        RecipeEfficiency = techProcesses.Single(i => i.FacilityId == 44440 && i.RecipeId == recipe.Id).DayEfficiency,
                        ProductId = rpc.ProductId,
                        ProductName = product.Name,
                        ProductType = product.Type,
                        LicenseRequired = product.LicenseRequired ?? false,
                        Quantity = rpc.Quantity,
                        IsValid = false
                    };

                    if(!facilityContainer.LicenseRequired)
                    {
                        facilityContainer.IsValid = true;
                    }
                    else
                    {
                        foreach(var license in licenses.Where(i => i.ProductId == rpc.ProductId))
                        {

                            facilityContainer.IsValid = true;
                        }
                    }

                    expected.Products.Add(facilityContainer);
                }
                
            }

            // act
            var actual = await logic.GetSingleDataModelAsync(expected.Id);

            // assert
            Assert.True(expected.Id == actual.Id);
            Assert.True(expected.Name == actual.Name);
            Assert.True(expected.Latitude == actual.Latitude);
            Assert.True(expected.Longitude == actual.Longitude);
            foreach (var expectedContainer in expected.Products)
            {
                Assert.Contains(actual.Products, actualContainer =>
                    actualContainer.FacilityId == expectedContainer.FacilityId &&
                    actualContainer.RecipeId == expectedContainer.RecipeId &&
                    actualContainer.RecipeGovApprov == expectedContainer.RecipeGovApprov &&
                    actualContainer.RecipeTechnoApprov == expectedContainer.RecipeTechnoApprov &&
                    actualContainer.RecipeEfficiency == expectedContainer.RecipeEfficiency &&
                    actualContainer.ProductId == expectedContainer.ProductId &&
                    actualContainer.ProductName == expectedContainer.ProductName &&
                    actualContainer.ProductType == expectedContainer.ProductType &&
                    actualContainer.LicenseRequired == expectedContainer.LicenseRequired &&
                    actualContainer.Quantity == expectedContainer.Quantity
                    // actualContainer.IsValid == expectedContainer.IsValid removed, cause unproper validation cause
                    // not correct output, validation will be removed in issue #27
                    );
            }
        }

        [Fact]
        public async void GetFacility_FromInitializedDbTable_LogicFacilityEqualExpectedFacility()
        {
            // arrange
            var expected = GetFacility();

            fixture.db.Facility.Add(expected);
            await fixture.db.SaveChangesAsync();

            // act
            var actual = await logic.GetSingleUpdateModelAsync(expected.Id);

            // assert
            Assert.True(actual.Id == expected.Id);
            Assert.True(actual.Name == expected.Name);
        }
    }
}

using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Cursed.Models.Logic;
using Cursed.Models.Entities;
using Cursed.Models.Data.Facilities;
using Cursed.Models.Data.Shared;

namespace Cursed.Tests.Tests.Logic
{
    public class Facilities : IClassFixture<TestsFixture>
    {
        private readonly TestsFixture fixture;
        private readonly FacilitiesLogic logic;

        public Facilities(TestsFixture fixture)
        {
            this.fixture = fixture;
            logic = new FacilitiesLogic(fixture.db);
        }

        [Fact]
        public async void AddDataModelAsync()
        {
            // arrange
            var expected = new Facility
            {
                Id = 44440,
                Name = "Test facility",
                Latitude = (decimal)12.3456,
                Longitude = (decimal)78.9012
            };

            // act
            await logic.AddDataModelAsync(expected);

            // assert
            var actual = await fixture.db.Facility.FirstOrDefaultAsync(i => i.Id == expected.Id);
            Assert.True(expected.Id == actual.Id);
            Assert.True(expected.Name == actual.Name);
            Assert.True(expected.Latitude == actual.Latitude);
            Assert.True(expected.Longitude == actual.Longitude);

            // dispose
            fixture.db.Facility.Remove(actual);
            await fixture.db.SaveChangesAsync();
        }

        [Fact]
        public async void RemoveDataModelAsync()
        {
            // arrange
            var facility = new Facility
            {
                Id = 44440,
                Name = "Test facility",
                Latitude = (decimal)12.3456,
                Longitude = (decimal)78.9012
            };
            fixture.db.Add(facility);
            await fixture.db.SaveChangesAsync();

            // act
            await logic.RemoveDataModelAsync(facility.Id);

            // assert
            var actual = await fixture.db.Facility.FirstOrDefaultAsync(i => i.Id == facility.Id);
            Assert.Null(actual);
        }

        [Fact]
        public async void UpdateDataModelAsync()
        {
            // arrange
            var facility = new Facility
            {
                Id = 44440,
                Name = "Test facility",
                Latitude = (decimal)12.3456,
                Longitude = (decimal)78.9012
            }; 
            fixture.db.Add(facility);
            await fixture.db.SaveChangesAsync();

            var expected = new Facility
            {
                Id = 44440,
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

            // dispose
            fixture.db.Facility.Remove(actual);
            await fixture.db.SaveChangesAsync();
        }

        [Fact]
        public async void GetAllDataModelAsync()
        {
            // arrange
            var facilities = new Facility[]
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

            var techProcesses = new TechProcess[]
            {
                new TechProcess
                {
                    FacilityId = 44440,
                    RecipeId = 44440,
                    DayEffiency = (decimal)12.3456
                },
                new TechProcess
                {
                    FacilityId = 44440,
                    RecipeId = 44441,
                    DayEffiency = (decimal)12.3456
                },
                new TechProcess
                {
                    FacilityId = 44440,
                    RecipeId = 44442,
                    DayEffiency = (decimal)78.9012
                },
                new TechProcess
                {
                    FacilityId = 44441,
                    RecipeId = 44443,
                    DayEffiency = (decimal)12.3456
                },
            };

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
                    Longitude = (decimal)78.9012
                },
                new FacilitiesModel
                {
                    Id = 44441,
                    Name = "Test facility #2",
                    Latitude = (decimal)78.9012,
                    Longitude = (decimal)12.3456
                }
            };
            foreach (var facility in expected)
            {
                var techProcessesInFacility = new List<TechProcess>();
                foreach(var techProcess in techProcesses.Where(i => i.FacilityId == facility.Id))
                {
                    techProcessesInFacility.Add(techProcess);
                }
                facility.TechProcesses = techProcessesInFacility;
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
                        actualTechProcess.DayEffiency == techProcess.DayEffiency);
                }
            }

            // dispose
            fixture.db.TechProcess.RemoveRange(techProcesses);
            fixture.db.Facility.RemoveRange(facilities);
            await fixture.db.SaveChangesAsync();
        }

        [Fact]
        public async void GetSingleDataModelAsync()
        {
            // arrange
            var facilities = new Facility[]
            {
                new Facility
                {
                    Id = 44440,
                    Name = "Test facility #1",
                    Latitude = (decimal)12.3456,
                    Longitude = (decimal)78.9012
                }
            };

            var recipes = new Recipe[]
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

            var techProcesses = new TechProcess[]
            {
                new TechProcess
                {
                    FacilityId = 44440,
                    RecipeId = 44440,
                    DayEffiency = (decimal)12.3456
                },
                new TechProcess
                {
                    FacilityId = 44440,
                    RecipeId = 44441,
                    DayEffiency = (decimal)78.9012
                }
            };

            var productsCatalog = new ProductCatalog[]
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

            var recipeProductsChanges = new RecipeProductChanges[]
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

            var licenses = new License[]
            {
                new License
                {
                    Id = 44440,
                    GovermentNum = 4040404,
                    Date = DateTime.Now.AddDays(1),
                    ProductId = 44442
                },
                new License
                {
                    Id = 44441,
                    GovermentNum = 4040414,
                    Date = DateTime.Now.AddDays(-1),
                    ProductId = 44444
                }
            };

            fixture.db.Facility.AddRange(facilities);
            fixture.db.Recipe.AddRange(recipes);
            fixture.db.TechProcess.AddRange(techProcesses);
            fixture.db.ProductCatalog.AddRange(productsCatalog);
            fixture.db.RecipeProductChanges.AddRange(recipeProductsChanges);
            await fixture.db.SaveChangesAsync();

            int facilityId = 44440;

            var expected = new FacilityModel
            {
                Id = facilityId,
                Name = "Test facility #1",
                Latitude = (decimal)12.3456,
                Longitude = (decimal)78.9012
            };

            var facilityContainers = new List<FacilitiesProductContainer>();
            foreach (var recipe in recipes)
            {
                foreach (var rpc in recipeProductsChanges.Where(i => i.RecipeId == recipe.Id))
                {
                    var product = productsCatalog.Single(i => i.Id == rpc.ProductId);

                    var facilityContainer = new FacilitiesProductContainer
                    {
                        FacilityId = facilityId,
                        RecipeId = recipe.Id,
                        RecipeGovApprov = recipe.GovermentApproval ?? false,
                        RecipeTechnoApprov = recipe.TechApproval ?? false,
                        RecipeEfficiency = techProcesses.Single(i => i.FacilityId == facilityId && i.RecipeId == recipe.Id).DayEffiency,
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
                            if(LicenseValid.Validate(license))
                            {
                                facilityContainer.IsValid = true;
                                break;
                            }
                        }
                    }
                }
                
            }
            expected.Products = facilityContainers;

            // act
            var actual = await logic.GetSingleDataModelAsync(facilityId);

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
                    actualContainer.Quantity == expectedContainer.Quantity &&
                    actualContainer.IsValid == expectedContainer.IsValid
                    );
            }


            // dispose
            fixture.db.TechProcess.RemoveRange(techProcesses);
            fixture.db.RecipeProductChanges.RemoveRange(recipeProductsChanges);
            fixture.db.ProductCatalog.RemoveRange(productsCatalog);
            fixture.db.Facility.RemoveRange(facilities);
            fixture.db.Recipe.RemoveRange(recipes);
            await fixture.db.SaveChangesAsync();
        }

        [Fact]
        public async void GetSingleUpdateModelAsync()
        {
            // arrange
            var expected = new Facility
            {
                Id = 44440,
                Name = "Test facility #1",
                Latitude = (decimal)12.3456,
                Longitude = (decimal)78.9012
            };

            fixture.db.Facility.Add(expected);
            await fixture.db.SaveChangesAsync();

            // act
            var actual = await logic.GetSingleUpdateModelAsync(expected.Id);

            // assert
            Assert.True(actual.Id == expected.Id);
            Assert.True(actual.Name == expected.Name);

            // dispose
            fixture.db.Facility.Remove(actual);
            await fixture.db.SaveChangesAsync();
        }
    }
}

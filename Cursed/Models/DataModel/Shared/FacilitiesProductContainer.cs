using Cursed.Models.Entities;
using System.Collections.Generic;

namespace Cursed.Models.DataModel.Shared
{
    public class FacilitiesProductContainer
    {
        public int FacilityId { get; set; }
        public int RecipeId { get; set; }
        public decimal RecipeEfficiency { get; set; }
        public bool RecipeTechnoApprov { get; set; }
        public bool RecipeGovApprov { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public bool LicenseRequired { get; set; }
        public bool IsValid { get; set; }
        public decimal Quantity { get; set; }
    }
}



namespace Cursed.Models.DataModel.Recipes
{
    /// <summary>
    /// Model used as base for recipe data gathering 
    /// </summary>
    public abstract class RecipesAbstractModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public bool? TechApproved { get; set; }
        public bool? GovApproved { get; set; }
        public int? ParentRecipe { get; set; }
    }
}

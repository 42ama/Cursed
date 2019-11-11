using Cursed.Models.Data.Shared;

namespace Cursed.Models.Data.Recipe
{
    public class RecipeAllModel : RecipeAbstractModel
    {
        public int ProductCount { get; set; }
        public int MaterialCount { get; set; }
        public int ChildRecipesCount { get; set; }
    }
}

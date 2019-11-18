using Cursed.Models.Data.Shared;

namespace Cursed.Models.Data.Recipes
{
    public class RecipesModel : RecipesAbstractModel
    {
        public int ProductCount { get; set; }
        public int MaterialCount { get; set; }
        public int ChildRecipesCount { get; set; }
    }
}



namespace Cursed.Models.DataModel.Recipes
{
    public class RecipesModel : RecipesAbstractModel
    {
        public int ProductCount { get; set; }
        public int MaterialCount { get; set; }
        public int ChildRecipesCount { get; set; }
    }
}

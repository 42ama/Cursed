

namespace Cursed.Models.DataModel.Recipes
{
    /// <summary>
    /// Model used for multiple recipes data gathering
    /// </summary>
    public class RecipesModel : RecipesAbstractModel
    {
        public int ProductCount { get; set; }
        public int MaterialCount { get; set; }
        public int ChildRecipesCount { get; set; }
    }
}

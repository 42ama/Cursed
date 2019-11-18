using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cursed.Models.Data.Shared;

namespace Cursed.Models.Data.Recipes
{
    public class RecipeModel : RecipesAbstractModel
    {
        public List<RecipeProductContainer> RecipeProducts { get; set; }
        public List<int> ChildRecipes { get; set; }
    }
}

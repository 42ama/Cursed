using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursed.Models.Routing
{
    public class RecipesRouting
    {
        public const string Index = "GetRecipes";
        public const string SingleItem = "GetRecipe";
        public const string GetEditSingleItem = "GetRecipeForEdit";
        public const string AddSingleItem = "AddRecipe";
        public const string EditSingleItem = "EditRecipe";
        public const string DeleteSingleItem = "DeleteRecipe";
    }
}

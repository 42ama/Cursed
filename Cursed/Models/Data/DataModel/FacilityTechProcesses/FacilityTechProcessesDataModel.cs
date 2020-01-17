

namespace Cursed.Models.DataModel.FacilityTechProcesses
{
    public class FacilityTechProcessesDataModel
    {
        public int FacilityId { get; set; }
        public string FacilityName { get; set; }
        public int RecipeId { get; set; }
        public string RecipeContent { get; set; }
        public bool RecipeTechApprov { get; set; }
        public bool RecipeGovApprov { get; set; }
        public decimal DayEffiency { get; set; }
    }
}

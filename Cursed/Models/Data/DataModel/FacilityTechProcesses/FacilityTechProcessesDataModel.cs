

namespace Cursed.Models.DataModel.FacilityTechProcesses
{
    /// <summary>
    /// Model used for tech process gathering
    /// </summary>
    public class FacilityTechProcessesDataModel
    {
        public int FacilityId { get; set; }
        public string FacilityName { get; set; }
        public int RecipeId { get; set; }
        public string RecipeContent { get; set; }
        public bool RecipeTechApprov { get; set; }
        public bool RecipeGovApprov { get; set; }
        public decimal DayEfficiency { get; set; }
    }
}

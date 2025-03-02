using System.Text.Json.Serialization;

namespace SageFinancialAPI.Entities
{
    public class Budget : BaseEntity
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public Guid ProfileId { get; set; }
        public Profile Profile { get; set; } = null!;
        [JsonIgnore]
        public ICollection<BudgetGoal> BudgetGoals { get; set; } = [];

    }
}
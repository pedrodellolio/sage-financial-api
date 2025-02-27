using System.Text.Json.Serialization;

namespace SageFinancialAPI.Entities
{
    public class Label : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string ColorHex { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; } = false;
        public Guid ProfileId { get; set; }
        public Profile Profile { get; set; } = null!;
        [JsonIgnore]
        public ICollection<BudgetGoal> BudgetGoals { get; set; } = [];
        [JsonIgnore]
        public ICollection<Transaction> Transactions { get; set; } = [];
    }
}
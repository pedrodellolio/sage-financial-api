using System.Text.Json.Serialization;

namespace SageFinancialAPI.Entities
{
    public class Label : BaseEntity
    {
        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set => _title = value.Trim().ToUpper();
        }
        public string ColorHex { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public bool IsDefault { get; set; } = false;
        public Guid ProfileId { get; set; }
        public Profile Profile { get; set; } = null!;
        [JsonIgnore]
        public ICollection<BudgetGoal> BudgetGoals { get; set; } = [];
        [JsonIgnore]
        public ICollection<Transaction> Transactions { get; set; } = [];
    }
}
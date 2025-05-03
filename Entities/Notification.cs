using System.Text.Json.Serialization;

namespace SageFinancialAPI.Entities
{
    public class Notification : BaseEntity
    {
        public bool IsEnabled { get; set; }
        public DateTimeOffset TriggerDate { get; set; }
        public Guid? TransactionId { get; set; }
        public Transaction? Transaction { get; set; }
        public Guid? BudgetGoalId { get; set; }
        public BudgetGoal? BudgetGoal { get; set; }
        public Guid ProfileId { get; set; }
        public Profile Profile { get; set; } = null!;
    }
}
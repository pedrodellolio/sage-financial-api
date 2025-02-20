namespace SageFinancialAPI.Entities
{
    public class BudgetGoal : BaseEntity
    {
        public int Value { get; set; }
        public BudgetGoalType Type { get; set; }
        public Guid LabelId { get; set; }
        public Label Label { get; set; } = null!;
        public Guid BudgetId { get; set; }
        public Budget Budget { get; set; } = null!;
    }

    public enum BudgetGoalType
    {
        PERCENTAGE,
        CURRENCY
    }
}
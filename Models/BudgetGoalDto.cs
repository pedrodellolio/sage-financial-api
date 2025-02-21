using SageFinancialAPI.Entities;

namespace SageFinancialAPI.Models
{
    public class BudgetGoalDto
    {
        public int Value { get; set; }
        public BudgetGoalType Type { get; set; }
        public Guid LabelId { get; set; }
    }
}
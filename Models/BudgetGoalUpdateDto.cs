using SageFinancialAPI.Entities;

namespace SageFinancialAPI.Models
{
    public class BudgetGoalUpdateDto
    {
        public Guid Id { get; set; }
        public int Value { get; set; }
        public BudgetGoalType Type { get; set; }
        public Guid LabelId { get; set; }
    }
}
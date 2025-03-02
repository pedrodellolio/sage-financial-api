using SageFinancialAPI.Entities;

namespace SageFinancialAPI.Models
{
    public class BudgetGoalResponseDto
    {
        public int Value { get; set; }
        public BudgetGoalType Type { get; set; }
        public Guid LabelId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal AmountBrl { get; set; }
    }
}
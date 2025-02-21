namespace SageFinancialAPI.Models
{
    public class BudgetUpdateDto
    {
        public Guid Id { get; set;}
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
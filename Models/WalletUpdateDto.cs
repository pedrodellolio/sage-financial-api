namespace SageFinancialAPI.Models
{
    public class WalletUpdateDto
    {
        public Guid Id { get; set; }
        public int ExpensesBrl { get; set; }
        public int IncomesBrl { get; set; }
    }
}
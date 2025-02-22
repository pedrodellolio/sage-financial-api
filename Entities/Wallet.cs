using System.Text.Json.Serialization;

namespace SageFinancialAPI.Entities
{
    public class Wallet : BaseEntity
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal ExpensesBrl { get; set; }
        public decimal IncomesBrl { get; set; }
        public Guid ProfileId { get; set; }
        public Profile Profile { get; set; } = null!;

        [JsonIgnore]
        public ICollection<Transaction> Transactions { get; set; } = [];
    }
}
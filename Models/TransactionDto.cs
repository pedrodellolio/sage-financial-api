using SageFinancialAPI.Entities;

namespace SageFinancialAPI.Models
{
    public class TransactionDto
    {
        public string Title { get; set; } = string.Empty;
        public decimal ValueBrl { get; set; }
        public TransactionType Type { get; set; }
        public DateTime OccurredAt { get; set; }
        public ICollection<Guid> Labels { get; set; } = [];
    }
}
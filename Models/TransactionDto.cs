using SageFinancialAPI.Entities;

namespace SageFinancialAPI.Models
{
    public class TransactionDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal ValueBrl { get; set; }
        public TransactionType Type { get; set; }
        public DateTimeOffset OccurredAt { get; set; }
        public LabelDto? Label { get; set; }
        public RecurrenceType? Frequency { get; set; }
        public decimal InterestPercentage { get; set; }
        public int TotalInstallments { get; set; }
        public Guid? ParentTransactionId { get; set; }
    }
}
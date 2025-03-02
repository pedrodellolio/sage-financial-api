using System.Text.Json.Serialization;

namespace SageFinancialAPI.Entities
{
    public class Transaction : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public decimal ValueBrl { get; set; }
        public TransactionType Type { get; set; }
        public DateTime OccurredAt { get; set; }
        public Guid WalletId { get; set; }
        public Wallet Wallet { get; set; } = null!;
        public Guid? FileId { get; set; }
        public File? File { get; set; } = null;
        public Guid? LabelId { get; set; }
        public Label? Label { get; set; } = null;
    }

    public enum TransactionType
    {
        EXPENSE,
        INCOME
    }
}
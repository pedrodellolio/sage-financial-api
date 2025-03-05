using System.Text.Json.Serialization;

namespace SageFinancialAPI.Entities
{
    public class Transaction : BaseEntity
    {
        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set => _title = value.Trim().ToUpper();
        }

        public decimal ValueBrl { get; set; }
        public TransactionType Type { get; set; }
        public DateTimeOffset OccurredAt { get; set; }
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
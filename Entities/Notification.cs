using System.Text.Json.Serialization;

namespace SageFinancialAPI.Entities
{
    public class Notification : BaseEntity
    {
        public DateTimeOffset TriggerDate { get; set; }
        public Guid TransactionId { get; set; }
        public Transaction Transaction { get; set; } = default!;
        public Guid ProfileId { get; set; }
        public Profile Profile { get; set; } = null!;
    }
}
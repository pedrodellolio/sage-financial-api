namespace SageFinancialAPI.Entities
{
    public class File : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public Guid ProfileId { get; set; }
        public Profile Profile { get; set; } = null!;
        public ICollection<Transaction> Transactions { get; set; } = [];
    }
}
using System.Text.Json.Serialization;

namespace SageFinancialAPI.Entities
{
    public class Profile : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        [JsonIgnore]
        public ICollection<Wallet> Wallets { get; set; } = [];
        [JsonIgnore]
        public ICollection<Label> Labels { get; set; } = [];
        [JsonIgnore]
        public ICollection<Budget> Budgets { get; set; } = [];
        [JsonIgnore]
        public ICollection<File> Files { get; set; } = [];
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
using System.Text.Json.Serialization;

namespace SageFinancialAPI.Entities
{
    public class Profile : BaseEntity
    {
        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set => _title = value.Trim().ToUpper();
        }
        public bool IsActive { get; set; } = true;
        public bool IsDefault { get; set; } = false;
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
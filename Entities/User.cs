using System.Text.Json.Serialization;

namespace SageFinancialAPI.Entities
{
    public class User : BaseEntity
    {
        // public string Username { get; set; } = string.Empty;
        // public string PasswordHash { get; set; } = string.Empty;
        // public string Role { get; set; } = string.Empty;
        // public string? RefreshToken { get; set; }
        // public DateTime? RefreshTokenExpiryTime { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PushNotificationsToken { get; set; } = string.Empty;
        [JsonIgnore]
        public ICollection<Profile> Profiles { get; set; } = [];
        [JsonIgnore]
        public ICollection<Wallet> Wallets { get; set; } = [];
    }
}
namespace SageFinancialAPI.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public ICollection<Profile> Profiles { get; set; } = [];
        public ICollection<Wallet> Wallets { get; set; } = [];

    }
}
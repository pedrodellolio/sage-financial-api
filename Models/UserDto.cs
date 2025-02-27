namespace SageFinancialAPI.Models
{
    public class UserDto
    {
        public string Email { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }
}
using SageFinancialAPI.Entities;

namespace SageFinancialAPI.Models
{
    public class ProfileBalanceDto
    {
        public required Profile Profile { get; set; }
        public decimal Balance { get; set; }
    }
}
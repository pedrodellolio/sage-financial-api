using SageFinancialAPI.Entities;

namespace SageFinancialAPI.Models
{
    public class TransactionUpdateDto : TransactionDto
    {
        public Guid Id { get; set; }
    }
}
using SageFinancialAPI.Entities;
using SageFinancialAPI.Models;

namespace SageFinancialAPI.Extensions
{

    public static class TransactionExtensions
    {
        public static TransactionResponseDto ToResponseDto(this Transaction transaction)
        {
            return new TransactionResponseDto
            {
                Id = transaction.Id,
                Title = transaction.Title,
                Type = transaction.Type,
                ValueBrl = transaction.ValueBrl,
                OccurredAt = transaction.OccurredAt.ToLocalTime(),
                Label = transaction.Label?.ToDto()
            };
        }
    }
}
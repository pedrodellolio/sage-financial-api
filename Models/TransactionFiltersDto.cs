using Microsoft.Identity.Client;
using SageFinancialAPI.Entities;

namespace SageFinancialAPI.Models
{
    public class TransactionFiltersDto
    {
        public bool OnlyInstallment { get; set; }
        public bool OnlyRecurrent { get; set; }
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        //public List<Guid> LabelIds { get; set; } = new();
    }
}
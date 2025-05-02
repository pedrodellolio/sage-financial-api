using SageFinancialAPI.Entities;
using SageFinancialAPI.Models;

namespace SageFinancialAPI.Extensions
{
    public static class LabelExtensions
    {
        public static LabelDto ToDto(this Label label)
        {
            return new LabelDto
            {
                Id = label.Id,
                Title = label.Title,
                ColorHex = label.ColorHex
            };
        }
    }
}
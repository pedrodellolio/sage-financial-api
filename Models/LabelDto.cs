namespace SageFinancialAPI.Models
{
    public class LabelDto
    {
        public string Title { get; set; } = string.Empty;
        public string ColorHex { get; set; } = string.Empty;
        public Guid ProfileId { get; set; }
        public bool IsDefault { get; set; }
    }
}
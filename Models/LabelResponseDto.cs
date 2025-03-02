namespace SageFinancialAPI.Models
{
    public class LabelResponseDto
    {
        public string Title { get; set; } = string.Empty;
        public string ColorHex { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }
}
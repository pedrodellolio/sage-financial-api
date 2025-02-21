namespace SageFinancialAPI.Models
{
    public class LabelUpdateDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ColorHex { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
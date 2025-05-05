namespace SageFinancialAPI.Models
{
    public class FileDto
    {
        public string Name { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public required List<Mapping> Mapping { get; set; }
    }

    public class Mapping
    {
        public int Index { get; set; }
        public string Prop { get; set; } = string.Empty;
    }
}
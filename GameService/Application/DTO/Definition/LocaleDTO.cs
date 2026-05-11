namespace Application.DTO.Definition
{
    public class LocaleDTO
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public bool IsEnabled { get; set; }
    }

    public class LocalizedTextDTO
    {
        public string NameKey { get; set; } = string.Empty;
        public string DescriptionKey { get; set; } = string.Empty;
    }
}
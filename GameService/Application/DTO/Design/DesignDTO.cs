namespace Application.DTO.Design
{
    public class DefinitionVersionLogDTO
    {
        public string ID { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public long Version { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
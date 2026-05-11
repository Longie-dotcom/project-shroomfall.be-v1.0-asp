namespace Application.DTO.Design
{
    public class UpdateDefinitionDTO
    {
        public string? Key { get; set; }
        public long Version { get; set; }
        public string? Description { get; set; }
    }

    public class UserRefreshDTO
    {
        public string DefinitionVersion { get; set; } = string.Empty;
    }
}
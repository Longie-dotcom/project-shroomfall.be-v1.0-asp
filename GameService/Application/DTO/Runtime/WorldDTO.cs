namespace Application.DTO.Runtime
{
    public class RoomRuntimeDTO
    {
        public string ID { get; set; } = string.Empty;
        public string DefinitionID { get; set; } = string.Empty;
        public string? OwnerID { get; set; }
        public List<EntityRuntimeDTO> Entities { get; set; } = new List<EntityRuntimeDTO>();
    }
}
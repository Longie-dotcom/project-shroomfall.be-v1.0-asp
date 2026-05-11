namespace Application.DTO.Connection
{
    public class CreateSessionDTO
    {
        public string PlayerDefinitionID { get; set; } = string.Empty;
        public string RoomDefinitionID { get; set; } = string.Empty;
    }

    public class LoadSessionDTO
    {
        public string PlayerInstanceID { get; set; } = string.Empty;
    }
}
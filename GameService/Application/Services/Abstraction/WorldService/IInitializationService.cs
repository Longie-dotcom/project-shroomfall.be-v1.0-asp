using Domain.Runtime.EntityDomain;
using Domain.Runtime.WorldDomain.World;

namespace Application.Services.Abstraction.WorldService
{
    public class WorldContext
    {
        public List<EntityInstance> Entities { get; set; } = new();
        public List<RoomSpatial> Rooms { get; set; } = new();
        public List<PendingRoomInitialization> PendingRooms { get; set; } = new();
    }

    public class PendingRoomInitialization
    {
        public string RoomSpatialID { get; set; } = string.Empty;
        public string RoomDefinitionID { get; set; } = string.Empty;
    }

    public class WorldTransaction
    {
        public WorldContext Context { get; set; } = new(); 
        public bool IsExpanded { get; set; }
        public bool IsValid => IsExpanded && !Context.PendingRooms.Any();
    }

    public interface IInitializationService
    {
        WorldContext InitializeRoomEnvironment(
            string roomSpatialId,
            string roomDefinitionId);
    }
}

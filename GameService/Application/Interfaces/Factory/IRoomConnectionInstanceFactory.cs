using Domain.Document.WorldDomain;
using Domain.Runtime.WorldDomain;

namespace Application.Interfaces.Factory
{
    public interface IRoomConnectionInstanceFactory
    {
        RoomConnectionInstance Create(
            string definitionId,
            string sourceRoomSpatialId,
            string sourceEntityInstanceId,
            string? destinationRoomSpatialId,
            string? destinationEntityInstanceId);
        RoomConnectionInstance CreateFromDocument(
            RoomConnectionDocument doc);
    }
}

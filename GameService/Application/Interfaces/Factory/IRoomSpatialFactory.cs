using Domain.Document.WorldDomain;
using Domain.Runtime.WorldDomain;

namespace Application.Interfaces.Factory
{
    public interface IRoomSpatialFactory
    {
        RoomSpatial Create(
            string definitionId,
            string instanceId,
            string? ownerId);
        RoomSpatial CreateFromDocument(
            RoomDocument doc);
    }
}

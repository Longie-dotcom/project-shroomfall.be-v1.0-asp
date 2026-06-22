using Domain.Definition.WorldDomain;

namespace Application.Interfaces.Cache.WorldDomain
{
    public interface IRoomConnectionCache
    {
        void Load(
            IEnumerable<RoomConnection> data);
        IReadOnlyCollection<RoomConnection> GetAll();
        RoomConnection? Get(
            string id);
        RoomConnection? GetBySource(
            string roomDefinitionId,
            string entityDefinitionId);
        RoomConnection? GetByDestination(
            string roomDefinitionId,
            string entityDefinitionId);
    }
}

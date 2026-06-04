using Application.Interfaces.Cache;
using Domain.Definition.WorldDomain;

namespace Infrastructure.Cache
{
    public class RoomConnectionCache : IRoomConnectionCache
    {
        #region Attributes
        private Dictionary<string, RoomConnection> idMap = new();
        private Dictionary<(string roomDefinitionId, string entityDefinitionId), RoomConnection> sourceMap = new();
        private Dictionary<(string roomDefinitionId, string entityDefinitionId), RoomConnection> destinationMap = new();
        #endregion

        #region Properties
        #endregion

        public RoomConnectionCache()
        {

        }

        #region Methods
        public void Load(
            IEnumerable<RoomConnection> data)
        {
            idMap = data.ToDictionary(x => x.ID);

            sourceMap = data.ToDictionary(
                x => (
                    x.SourceRoomID,
                    x.SourceEntityID));

            destinationMap = data.ToDictionary(
                x => (
                    x.DestinationRoomID,
                    x.DestinationEntityID));
        }

        public IReadOnlyCollection<RoomConnection> GetAll()
        {
            return idMap.Values.ToList();
        }

        public RoomConnection? Get(
            string id)
        {
            return idMap.TryGetValue(id, out var item)
                ? item
                : null;
        }

        public RoomConnection? GetBySource(
            string roomDefinitionId,
            string entityDefinitionId)
        {
            return sourceMap.TryGetValue(
                (roomDefinitionId, entityDefinitionId),
                out var item)
                    ? item
                    : null;
        }

        public RoomConnection? GetByDestination(
            string roomDefinitionId,
            string entityDefinitionId)
        {
            return destinationMap.TryGetValue(
                (roomDefinitionId, entityDefinitionId),
                out var item)
                    ? item
                    : null;
        }
        #endregion
    }
}
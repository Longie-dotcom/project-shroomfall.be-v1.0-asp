namespace Domain.Runtime.WorldDomain
{
    public class RoomSpatial
    {
        #region Attributes
        private readonly Dictionary<int, LayerSpatial> layers = new();
        #endregion

        #region Properties
        public string ID { get; private set; }
        public string DefinitionID { get; private set; }
        public string? OwnerID { get; private set; }
        #endregion

        public RoomSpatial(
            string id,
            string definitionId,
            string? ownerId)
        {
            ID = id;
            DefinitionID = definitionId;
            OwnerID = ownerId;
        }

        #region Command
        public void AddEntity(
            string entityId, 
            int chunkX, int chunkY, int cellX, int cellY, int z)
        {
            var layer = GetOrCreateLayer(z);

            layer.AddEntity(entityId, chunkX, chunkY, cellX, cellY);
        }

        public void RemoveEntity(
            string entityId, 
            int chunkX, int chunkY, int cellX, int cellY, int z)
        {
            if (!layers.TryGetValue(z, out var layer)) return;

            layer.RemoveEntity(entityId, chunkX, chunkY, cellX, cellY);
        }

        public void EntityMove(
            string entityId, 
            (int cx, int cy, int x, int y, int z) oldPos,
            (int cx, int cy, int x, int y, int z) newPos)
        {
            RemoveEntity(entityId, oldPos.cx, oldPos.cy, oldPos.x, oldPos.y, oldPos.z);

            AddEntity(entityId, newPos.cx, newPos.cy, newPos.x, newPos.y, newPos.z);
        }

        private LayerSpatial GetOrCreateLayer(int z)
        {
            if (!layers.TryGetValue(z, out var layer))
            {
                layer = new LayerSpatial();
                layers[z] = layer;
            }
            return layer;
        }
        #endregion

        #region Query
        public IEnumerable<string> Query(int x, int y, int z)
        {
            if (!layers.TryGetValue(z, out var layer))
                return Enumerable.Empty<string>();

            return layer.Query(x, y);
        }
        #endregion
    }
}
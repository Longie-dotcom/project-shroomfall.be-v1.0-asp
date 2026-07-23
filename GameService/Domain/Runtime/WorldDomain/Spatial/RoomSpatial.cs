using Domain.Runtime.EntityDomain;

namespace Domain.Runtime.WorldDomain.Spatial
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
            EntityInstance entity, 
            int chunkX, int chunkY, int cellX, int cellY, int z)
        {
            var layer = GetOrCreateLayer(z);

            layer.AddEntity(entity, chunkX, chunkY, cellX, cellY);
        }

        public void RemoveEntity(
            EntityInstance entity, 
            int chunkX, int chunkY, int cellX, int cellY, int z)
        {
            if (!layers.TryGetValue(z, out var layer)) return;

            layer.RemoveEntity(entity, chunkX, chunkY, cellX, cellY);
        }

        public void EntityMove(
            EntityInstance entity, 
            (int cx, int cy, int x, int y, int z) oldPos,
            (int cx, int cy, int x, int y, int z) newPos)
        {
            RemoveEntity(entity, oldPos.cx, oldPos.cy, oldPos.x, oldPos.y, oldPos.z);

            AddEntity(entity, newPos.cx, newPos.cy, newPos.x, newPos.y, newPos.z);
        }

        private LayerSpatial GetOrCreateLayer(
            int z)
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
        public IEnumerable<EntityInstance> Query(
            int x, int y, int z)
        {
            if (!layers.TryGetValue(z, out var layer))
                return Enumerable.Empty<EntityInstance>();

            return layer.Query(x, y);
        }
        #endregion
    }
}
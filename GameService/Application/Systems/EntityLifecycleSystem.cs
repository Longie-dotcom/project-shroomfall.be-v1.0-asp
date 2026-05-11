using Application.Events.Abstraction;
using Application.Events.Event;
using Domain.Abstraction.World;
using Domain.Runtime.EntityDomain;

namespace Application.Systems
{
    public class EntityLifecycleSystem
    {
        #region Attributes
        private readonly IWorldQuery world;
        private readonly IEventBus bus;
        #endregion

        #region Properties
        private readonly HashSet<string> known = new();
        private readonly Dictionary<string, string> lastRoom = new();
        #endregion

        public EntityLifecycleSystem(
            IWorldQuery world,
            IEventBus bus)
        {
            this.world = world;
            this.bus = bus;
        }

        #region Methods
        public void Update()
        {
            var currentEntities = world.GetAll<EntityInstance>().ToList();
            var currentIds = currentEntities.Select(e => e.ID).ToHashSet();

            // ───────── SPAWN ─────────
            foreach (var entity in currentEntities)
            {
                if (known.Contains(entity.ID))
                    continue;

                bus.Publish(new EntityLifecycleEvent(
                    entity.ID,
                    entity.RoomSpatialID,
                    EntityLifecycleType.Spawn));

                known.Add(entity.ID);
                lastRoom[entity.ID] = entity.RoomSpatialID;
            }

            // ───────── DESPAWN ─────────
            foreach (var id in known.ToList())
            {
                if (currentIds.Contains(id))
                    continue;

                var roomId = lastRoom.TryGetValue(id, out var r)
                    ? r
                    : null;

                bus.Publish(new EntityLifecycleEvent(
                    id,
                    roomId!,
                    EntityLifecycleType.Despawn));

                known.Remove(id);
                lastRoom.Remove(id);
            }
        }
        #endregion
    }
}
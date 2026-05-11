using Application.Services.Abstraction.WorldService;
using Domain.Abstraction.World;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.WorldDomain.World;

namespace Application.Services.WorldService
{
    public class RoomHandle
    {
        public string RoomID { get; set; } = string.Empty;
        public RoomSpatial? Instance { get; set; }
        public bool IsLoaded => Instance != null;
    }

    public class RoomSnapshot
    {
        public RoomSpatial Room { get; set; }
        public List<EntityInstance> Entities { get; set; } = new();
    }

    public class ContextService : IContextService
    {
        #region Attributes
        private readonly IWorldQuery worldQuery;
        private readonly IEntityCommand entityCommand;
        private readonly IRoomCommand roomCommand;

        private readonly Dictionary<string, RoomHandle> rooms = new();
        private readonly Dictionary<string, HashSet<string>> roomUsers = new();
        #endregion

        #region Properties
        #endregion

        public ContextService(
            IWorldQuery worldQuery,
            IEntityCommand entityCommand,
            IRoomCommand roomCommand)
        {
            this.worldQuery = worldQuery;
            this.entityCommand = entityCommand;
            this.roomCommand = roomCommand;
        }

        #region Methods
        public void LoadRoom(
            RoomSnapshot snapshot, 
            string playerInstanceId)
        {
            var room = snapshot.Room;

            if (!rooms.TryGetValue(room.ID, out var handle))
            {
                roomCommand.AddRoom(room);

                foreach (var entity in snapshot.Entities)
                {
                    entityCommand.AddEntity(entity);
                }

                rooms[room.ID] = new RoomHandle
                {
                    RoomID = room.ID,
                    Instance = room
                };
            }

            AddPlayerToRoom(room.ID, playerInstanceId);
        }

        public RoomSnapshot? UnloadRoom(
            string roomId)
        {
            if (!rooms.TryGetValue(roomId, out var handle))
                return null;

            var snapshot = new RoomSnapshot
            {
                Room = handle.Instance!,
                Entities = worldQuery.GetAll<EntityInstance>()
                    .Where(e => e.RoomSpatialID == roomId)
                    .ToList()
            };

            foreach (var entity in snapshot.Entities)
                entityCommand.RemoveEntity(entity.ID);

            roomCommand.RemoveRoom(roomId);

            handle.Instance = null;
            roomUsers.Remove(roomId);

            return snapshot;
        }

        public void ChangeRoom(
            string entityId,
            string fromRoomId,
            RoomSnapshot toRoom)
        {
            // Update membership graph
            RemovePlayerFromRoom(fromRoomId, entityId);
            AddPlayerToRoom(toRoom.Room.ID, entityId);

            // Ensure target room is loaded
            if (!rooms.ContainsKey(toRoom.Room.ID))
            {
                LoadRoom(toRoom, entityId);
            }

            // Cleanup old room only if empty
            if (roomUsers.TryGetValue(fromRoomId, out var users) &&
                users.Count == 0)
            {
                var snapshot = UnloadRoom(fromRoomId);

                if (snapshot != null)
                {
                    rooms.Remove(fromRoomId);
                }
            }
        }

        public void AddEntity(
            EntityInstance entity)
        {
            entityCommand.AddEntity(entity);
        }

        public void RemoveEntity(
            string entityId)
        {
            var entity = worldQuery.Get<EntityInstance>(entityId);
            if (entity == null)
                return;

            entityCommand.RemoveEntity(entityId);
        }

        private void AddPlayerToRoom(string roomId, string playerId)
        {
            if (!roomUsers.TryGetValue(roomId, out var set))
            {
                set = new HashSet<string>();
                roomUsers[roomId] = set;
            }

            set.Add(playerId);
        }

        private void RemovePlayerFromRoom(string roomId, string playerId)
        {
            if (!roomUsers.TryGetValue(roomId, out var set))
                return;

            set.Remove(playerId);
        }
        #endregion
    }
}
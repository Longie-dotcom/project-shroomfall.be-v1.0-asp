using Application.Services.WorldService;
using Domain.Runtime.EntityDomain;

namespace Application.Services.Abstraction.WorldService
{
    public interface IContextService
    {
        void LoadRoom(
            RoomSnapshot snapshot,
            string playerInstanceId);
        RoomSnapshot? UnloadRoom(
            string roomId);
        void ChangeRoom(
            string entityId,
            string fromRoomId,
            RoomSnapshot toRoom);
        void AddEntity(
            EntityInstance entity);
        void RemoveEntity(
            string entityId);
    }
}

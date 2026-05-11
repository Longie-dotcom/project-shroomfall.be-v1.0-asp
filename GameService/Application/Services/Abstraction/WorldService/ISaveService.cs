using Application.Services.WorldService;
using Domain.Runtime.EntityDomain;

namespace Application.Services.Abstraction.WorldService
{
    public interface ISaveService
    {
        Task<RoomSnapshot?> LoadRoomSnapshotAsync(
            string roomId);
        Task<PlayerInstance?> LoadPlayerAsync(
            string playerInstanceId);
        Task SaveRoomAsync(
            RoomSnapshot snapshot);
        Task SaveEntityAsync(
            EntityInstance entity);
        Task SaveWorldAsync(
            WorldContext context);
    }
}

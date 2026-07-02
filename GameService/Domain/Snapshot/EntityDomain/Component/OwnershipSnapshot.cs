using Domain.Abstraction;

namespace Domain.Snapshot.EntityDomain.Component
{
    public class OwnershipSnapshot : ComponentSnapshot
    {
        public string UserID { get; set; } = string.Empty;
        public string PersonalRoomID { get; set; } = string.Empty;
    }
}
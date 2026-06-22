using Domain.Abstraction;
using Domain.Common;

namespace Domain.Snapshot.EntityDomain.Component
{
    public class TransformSnapshot : ComponentSnapshot
    {
        public string RoomSpatialID { get; set; } = string.Empty;
        public int LayerZ { get; set; }
        public Vector2 Position { get; set; } = Vector2.Zero;
    }
}
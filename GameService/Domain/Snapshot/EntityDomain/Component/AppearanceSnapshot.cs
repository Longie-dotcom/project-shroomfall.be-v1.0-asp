using Domain.Abstraction;
using Domain.Common;

namespace Domain.Snapshot.EntityDomain.Component
{
    public class AppearanceSnapshot : ComponentSnapshot
    {
        public string SkinID { get; set; } = string.Empty;
        public HSV SkinColor { get; set; } = new HSV();
    }
}
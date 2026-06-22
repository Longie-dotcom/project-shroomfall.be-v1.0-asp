using Domain.Abstraction;
using Domain.Common;

namespace Domain.Snapshot.EntityDomain.Component
{
    public class AppearanceSnapshot : ComponentSnapshot
    {
        public string SkinID { get; set; } = string.Empty;
        public HSV SkinColor { get; set; } = new HSV();
        public string? HairID { get; set; } = string.Empty;
        public string? EyesID { get; set; } = string.Empty;
        public string? ShirtID { get; set; } = string.Empty;
        public string? PantID { get; set; } = string.Empty;
        public HSV? HairColor { get; set; } = new HSV();
        public HSV? PantColor { get; set; } = new HSV();
    }
}
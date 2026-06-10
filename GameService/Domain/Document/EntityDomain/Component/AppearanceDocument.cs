using Domain.Common;

namespace Domain.Document.EntityDomain.Component
{
    public class AppearanceDocument
    {
        public string SkinID { get; set; } = string.Empty;
        public HSVDocument SkinColor { get; set; } = new();
        public string? HairID { get; set; }
        public string? EyesID { get; set; }
        public string? ShirtID { get; set; }
        public string? PantID { get; set; }
        public HSVDocument? HairColor { get; set; }
        public HSVDocument? PantColor { get; set; }
    }
}
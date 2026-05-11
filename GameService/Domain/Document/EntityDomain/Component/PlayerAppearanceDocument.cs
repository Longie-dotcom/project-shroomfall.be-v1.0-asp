using Domain.Common;

namespace Domain.Document.EntityDomain.Component
{
    public class PlayerAppearanceDocument : AppearanceDocument
    {
        public string HairID { get; set; } = string.Empty;
        public string GlassesID { get; set; } = string.Empty;
        public string ShirtID { get; set; } = string.Empty;
        public string PantID { get; set; } = string.Empty;
        public string ShoeID { get; set; } = string.Empty;
        public string EyesID { get; set; } = string.Empty;
        public HSVDocument HairColor { get; set; } = new();
        public HSVDocument PantColor { get; set; } = new();
        public HSVDocument EyeColor { get; set; } = new();
    }
}
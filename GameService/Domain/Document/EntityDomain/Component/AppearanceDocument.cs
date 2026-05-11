using Domain.Common;

namespace Domain.Document.EntityDomain.Component
{
    public class AppearanceDocument
    {
        public string SkinID { get; set; } = string.Empty;
        public HSVDocument SkinColor { get; set; } = new();
    }
}
using Domain.Document.EntityDomain.Component;

namespace Domain.Document.EntityDomain
{
    public class PlayerDocument : CreatureDocument
    {
        public string UserID { get; set; } = string.Empty;
        public PlayerAppearanceDocument PlayerAppearance { get; set; } = new();
    }
}
using Contract.Enum.ItemDomain;

namespace Domain.Document.ItemDomain
{
    public class ItemDocument
    {
        public string ID { get; set; } = string.Empty;
        public string DefinitionID { get; set; } = string.Empty;
        public int Count { get; set; }
        public int? CurrentDurability { get; set; }
        public ItemQuality Quality { get; set; }
    }
}
namespace Domain.Document.ItemDomain
{
    public class InventoryDocument
    {
        public string ID { get; set; } = string.Empty;
        public string DefinitionID { get; set; } = string.Empty;
        public List<ItemDocument> Items { get; set; } = new();
    }
}
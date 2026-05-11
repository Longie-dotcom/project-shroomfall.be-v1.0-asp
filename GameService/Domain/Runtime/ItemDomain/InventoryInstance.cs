namespace Domain.Runtime.ItemDomain
{
    public class InventoryInstance
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; }
        public string DefinitionID { get; }
        public List<ItemInstance> Items { get; }
        #endregion

        public InventoryInstance(
            string id, 
            string definitionID,
            List<ItemInstance> items)
        {
            ID = id;
            DefinitionID = definitionID;
            Items = items;
        }

        #region Methods
        #endregion
    }
}
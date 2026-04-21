using Domain.AttributeDomain.Attribute;
using Domain.ItemDomain.Enum;

namespace Domain.ItemDomain
{
    public class Item
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; private set; }
        public ItemDefinition Definition { get; private set; }
        public int Count { get; private set; }
        #endregion

        public Item(
            string id,
            ItemDefinition definition,
            int count)
        {
            ID = id;
            Definition = definition;
            Count = count;
        }

        #region Methods
        #endregion
    }
}

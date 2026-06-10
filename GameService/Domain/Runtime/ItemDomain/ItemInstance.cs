using Contract.Enum.ItemDomain;
using Domain.DomainException;
using Domain.Shared;

namespace Domain.Runtime.ItemDomain
{
    public class ItemInstance
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get;}
        public string DefinitionID { get; }
        public int Count { get; private set; }
        public int? CurrentDurability { get; private set; }
        public ItemQuality Quality { get; private set; }
        #endregion

        public ItemInstance(
            string id,
            string definitionId,
            int count,
            int? durability,
            ItemQuality quality)
        {
            ID = id;
            DefinitionID = definitionId;
            Count = count;
            CurrentDurability = durability;
            Quality = quality;
        }

        #region Methods
        public void Add(int amount)
        {
            if (amount <= 0)
                throw new BadRequest(ResponseCode.Item_InvalidAmount);

            Count += amount;
        }

        public void Remove(int amount)
        {
            if (amount < 0)
                throw new BadRequest(ResponseCode.Item_InvalidAmount);

            if (Count < amount)
                throw new BadRequest(ResponseCode.Item_NotEnoughAmount);

            Count -= amount;
        }

        public bool DegradeDurability(int amount)
        {
            if (!CurrentDurability.HasValue)
                return false;

            CurrentDurability = Math.Max(0, CurrentDurability.Value - amount);

            return CurrentDurability.Value <= 0;
        }
        #endregion
    }
}

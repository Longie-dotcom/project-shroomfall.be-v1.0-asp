using Contract.Enum.MetaDomain.Item;

namespace Domain.Abstraction
{
    public interface IItemStateContract
    {
        string DefinitionID { get; }
        int Amount { get; }
        ItemQuality Quality { get; }
        int? Durability { get; }
    }
}

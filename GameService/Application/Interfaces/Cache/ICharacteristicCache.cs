using Domain.Definition.AttributeDomain;

namespace Application.Interfaces.Cache
{
    public interface ICharacteristicCache
    {
        void Load(
            IEnumerable<Characteristic> data);
        IReadOnlyCollection<Characteristic> GetAll();
        Characteristic? Get(
            string id);
    }
}

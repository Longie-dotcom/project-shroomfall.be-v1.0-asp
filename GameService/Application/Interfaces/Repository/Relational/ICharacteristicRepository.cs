using Domain.Definition.AttributeDomain;

namespace Application.Interfaces.Repository.Relational
{
    public interface ICharacteristicRepository : ISQLGenericRepository<Characteristic>, IRelationalRepository
    {
        Task<IEnumerable<Characteristic>> GetAllWithAttributeValuesAsync();
    }
}

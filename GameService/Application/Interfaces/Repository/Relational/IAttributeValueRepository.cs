using Domain.Definition.AttributeDomain;

namespace Application.Interfaces.Repository.Relational
{
    public interface IAttributeValueRepository : ISQLGenericRepository<AttributeValue>, IRelationalRepository
    {

    }
}

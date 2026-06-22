using Application.Interfaces.Repository.Base;
using Domain.Definition.EntityDomain;

namespace Application.Interfaces.Repository.Relational
{
    public interface IEntityDefinitionRepository : ISQLGenericRepository<EntityDefinition>, IRelationalRepository
    {

    }
}

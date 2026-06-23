using Application.Interfaces.Repository.Base;
using Domain.Definition.EntityDomain.Component;

namespace Application.Interfaces.Repository.Relational
{
    public interface ILifetimeDefinitionRepository : ISQLDefinitionRepository<LifetimeDefinition>, IRelationalRepository
    {

    }
}

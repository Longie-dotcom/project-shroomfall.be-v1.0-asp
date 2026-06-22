using Application.Interfaces.Repository.Base;
using Domain.Definition.EntityDomain.Component;

namespace Application.Interfaces.Repository.Relational
{
    public interface IAppearanceDefinitionRepository : ISQLGenericRepository<AppearanceDefinition>, IRelationalRepository
    {

    }
}

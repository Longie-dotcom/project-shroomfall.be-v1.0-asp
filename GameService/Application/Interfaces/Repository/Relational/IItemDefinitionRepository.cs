using Application.Interfaces.Repository.Base;
using Domain.Definition.MetaDomain;

namespace Application.Interfaces.Repository.Relational
{
    public interface IItemDefinitionRepository : ISQLGenericRepository<ItemDefinition>, IRelationalRepository
    {

    }
}

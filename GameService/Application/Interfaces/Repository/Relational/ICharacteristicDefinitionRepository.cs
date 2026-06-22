using Application.Interfaces.Repository.Base;
using Domain.Definition.EntityDomain.Component;

namespace Application.Interfaces.Repository.Relational
{
    public interface ICharacteristicDefinitionRepository : ISQLGenericRepository<CharacteristicDefinition>, IRelationalRepository
    {

    }
}

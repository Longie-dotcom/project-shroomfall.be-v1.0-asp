using Domain.Document.AttributeDomain;
using Domain.Runtime.AttributeDomain;

namespace Application.Interfaces.Factory
{
    public interface ICharacteristicInstanceFactory
    {
        CharacteristicInstance Create(
            string definitionId);
        CharacteristicInstance CreateFromDocument(
            CharacteristicDocument doc);
    }
}

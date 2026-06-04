using Application.Interfaces.Cache;
using Application.Interfaces.Factory;
using Domain.Document.AttributeDomain;
using Domain.DomainException;
using Domain.Runtime.AttributeDomain;
using Domain.Shared;

namespace Infrastructure.Factory
{
    public class CharacteristicInstanceFactory : ICharacteristicInstanceFactory
    {
        #region Attributes
        private readonly ICharacteristicCache characteristicCache;
        #endregion

        #region Properties
        #endregion

        public CharacteristicInstanceFactory(
            ICharacteristicCache characteristicCache)
        {
            this.characteristicCache = characteristicCache;
        }

        #region Methods
        public CharacteristicInstance Create(
            string definitionId)
        {
            var characteristicDef = characteristicCache.Get(definitionId);
            if (characteristicDef == null ||
                characteristicDef.AttributeValues == null ||
                characteristicDef.AttributeValues.Count == 0)
                throw new InternalException(
                    ResponseCode.CharacteristicInstanceFactory_DefinitionNotFound,
                    $"Characteristic definition with ID: {definitionId} is not found in cache");

            return new CharacteristicInstance(
                id: Guid.NewGuid().ToString(),
                definitionId: characteristicDef.ID
            );
        }

        public CharacteristicInstance CreateFromDocument(
            CharacteristicDocument doc)
        {
            if (doc == null)
                throw new InternalException(
                    ResponseCode.CharacteristicInstanceFactory_DocumentNotFound,
                    $"Characteristic document is not found");

            var characteristicDef = characteristicCache.Get(doc.DefinitionID);
            if (characteristicDef == null ||
                characteristicDef.AttributeValues == null ||
                characteristicDef.AttributeValues.Count == 0)
                throw new InternalException(
                    ResponseCode.CharacteristicInstanceFactory_DefinitionFromDocumentNotFound,
                    $"Characteristic definition with ID: {doc.DefinitionID} is not found in cache");

            var instance = new CharacteristicInstance(
                id: doc.ID,
                definitionId: doc.DefinitionID
            );

            return instance;
        }
        #endregion
    }
}
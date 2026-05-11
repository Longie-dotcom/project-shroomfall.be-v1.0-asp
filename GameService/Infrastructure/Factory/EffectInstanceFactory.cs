using Application.Interfaces.Cache;
using Application.Interfaces.Factory;
using Domain.Document.AttributeDomain;
using Domain.DomainException;
using Domain.Runtime.AttributeDomain;
using Domain.Shared;

namespace Infrastructure.Factory
{
    public class EffectInstanceFactory : IEffectInstanceFactory
    {
        #region Attributes
        private readonly IEffectCache effectCache;
        #endregion

        #region Properties
        #endregion

        public EffectInstanceFactory(
            IEffectCache effectCache)
        {
            this.effectCache = effectCache;
        }

        #region Methods
        public EffectInstance Create(
            string definitionId,
            string? sourceItemInstanceId = null)
        {
            var effectDef = effectCache.Get(definitionId);

            if (effectDef == null)
            {
                throw new InternalException(
                    ResponseCode.EffectInstanceFactory_DefinitionNotFound,
                    $"Effect definition with ID: {definitionId} is not found in cache");
            }

            return new EffectInstance(
                id: Guid.NewGuid().ToString(),
                definitionId: effectDef.ID,
                remainingTime: effectDef.Duration,
                sourceItemInstanceId: sourceItemInstanceId
            );
        }

        public EffectInstance CreateFromDocument(
            EffectDocument doc)
        {
            if (doc == null)
                throw new InternalException(
                    ResponseCode.EffectInstanceFactory_DefinitionFromDocumentNotFound,
                    $"Effect document is not found");

            var effectDef = effectCache.Get(doc.DefinitionID);

            if (effectDef == null)
            {
                throw new InternalException(
                    ResponseCode.EffectInstanceFactory_DefinitionFromDocumentNotFound,
                    $"Effect definition with ID: {doc.DefinitionID} is not found in cache");
            }

            var instance = new EffectInstance(
                id: doc.ID,
                definitionId: doc.DefinitionID,
                remainingTime: doc.RemainingTime,
                sourceItemInstanceId: doc.SourceItemInstanceID
            );

            return instance;
        }
        #endregion
    }
}
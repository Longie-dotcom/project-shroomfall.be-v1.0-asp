using Domain.Document.AttributeDomain;
using Domain.Runtime.AttributeDomain;

namespace Application.Interfaces.Factory
{
    public interface IEffectInstanceFactory
    {
        EffectInstance Create(
            string definitionId,
            string? sourceItemInstanceId = null);
        EffectInstance CreateFromDocument(
            EffectDocument doc);
    }
}

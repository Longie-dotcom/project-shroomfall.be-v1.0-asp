using Application.Interfaces.Factory;
using AutoMapper;
using Domain.Document.EntityDomain;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Shared;

namespace Application.Helper
{
    public static class EntityMapper
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        #region Methods
        public static EntityInstance ToRuntime(EntityDocument doc, IEntityInstanceFactory factory)
        {
            return doc switch
            {
                PlayerDocument p => factory.CreateFromDocument(p),
                CreatureDocument c => factory.CreateFromDocument(c),
                WorldObjectDocument w => factory.CreateFromDocument(w),
                _ => throw new InternalException(
                    ResponseCode.EntityMapper_InvalidDocumentType,
                    $"Unknown document entity type: {doc.GetType().Name}")
            };
        }

        public static EntityDocument ToDocument(EntityInstance entity, IMapper mapper)
        {
            return entity switch
            {
                PlayerInstance p => mapper.Map<PlayerDocument>(p),
                CreatureInstance c => mapper.Map<CreatureDocument>(c),
                WorldObjectInstance w => mapper.Map<WorldObjectDocument>(w),
                _ => throw new InternalException(
                    ResponseCode.EntityMapper_InvalidRuntimeType,
                    $"Unknown runtime entity type: {entity.GetType().Name}")
            };
        }
        #endregion
    }
}
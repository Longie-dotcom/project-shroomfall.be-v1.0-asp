using Contract.DTO.Definition.EntityDomain.Component;
using Contract.Enum.EntityDomain;
using Domain.Abstraction;
using Domain.DomainException;
using Domain.Runtime.EntityDomain.Component;
using ResponseCode;

namespace Domain.Shared
{
    public static class CollisionShapeMapper
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        #region Methods
        public static ICollisionShape FromDefinition(
            CollisionDefinitionDTO collision)
        {
            switch (collision.ShapeType)
            {
                case CollisionShapeType.Point:
                    return new PointShape(
                        collision.IsBlocking);

                case CollisionShapeType.Box:
                    return new BoxShape(
                        collision.Width,
                        collision.Height,
                        collision.IsBlocking
                    );

                case CollisionShapeType.Circle:
                    return new CircleShape(
                        collision.Radius,
                        collision.IsBlocking
                    );

                default:
                    throw new InternalException(
                        DomainCode.CollisionShapeMapperCode.InvalidShapeType,
                        $"Collision type is not found: {collision.ShapeType}");
            }
        }
        #endregion
    }
}
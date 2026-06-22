using Contract.Enum.EntityDomain;
using Domain.Abstraction;
using Domain.Definition.EntityDomain.Component;
using Domain.Runtime.EntityDomain.Component;
using Domain.Shared.DomainException;
using Domain.Shared.ResponseCode;

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
            CollisionDefinition collision)
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
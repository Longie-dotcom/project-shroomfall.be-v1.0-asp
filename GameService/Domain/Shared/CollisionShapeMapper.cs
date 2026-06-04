using Contract.Enum.EntityDomain;
using Domain.Abstraction;
using Domain.Definition.EntityDomain.Component;
using Domain.DomainException;
using Domain.Runtime.EntityDomain.Component;

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
            Collision collision)
        {
            switch (collision.ShapeType)
            {
                case CollisionShapeType.Point:
                    return new PointShape(
                        collision.IsBlocking,
                        collision.IsTrigger);

                case CollisionShapeType.Box:
                    return new BoxShape(
                        collision.Width,
                        collision.Height,
                        collision.IsBlocking,
                        collision.IsTrigger
                    );

                case CollisionShapeType.Circle:
                    return new CircleShape(
                        collision.Radius,
                        collision.IsBlocking,
                        collision.IsTrigger
                    );

                default:
                    throw new InternalException(
                        ResponseCode.CollisionShapeMapper_InvalidShapeType,
                        $"Collision type is not found: {collision.ShapeType}");
            }
        }
        #endregion
    }
}
using Contract.Enum.EntityDomain;
using Domain.DomainException;
using Domain.Shared;

namespace Domain.Definition.EntityDomain.Component
{
    public class Collision
    {
        #region Attributes
        #endregion

        #region Properties
        public CollisionShapeType ShapeType { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }
        public float Radius { get; private set; }
        public bool IsBlocking { get; private set; }
        public bool IsTrigger { get; private set; }
        public float OffsetX { get; private set; }
        public float OffsetY { get; private set; }
        #endregion

        protected Collision() 
        { 
        
        }

        public Collision(
            CollisionShapeType shapeType,
            float width,
            float height,
            float radius,
            bool isBlocking,
            bool isTrigger,
            float offsetX = 0f,
            float offsetY = 0f)
        {
            if (width < 0)
                throw new BadRequest(ResponseCode.Collision_InvalidWidth);

            if (height < 0)
                throw new BadRequest(ResponseCode.Collision_InvalidHeight);

            if (radius < 0)
                throw new BadRequest(ResponseCode.Collision_InvalidRadius);

            switch (shapeType)
            {
                case CollisionShapeType.Point:
                    break;

                case CollisionShapeType.Box:
                    if (width <= 0)
                        throw new BadRequest(ResponseCode.Collision_InvalidWidth);

                    if (height <= 0)
                        throw new BadRequest(ResponseCode.Collision_InvalidHeight);
                    break;

                case CollisionShapeType.Circle:
                    if (radius <= 0)
                        throw new BadRequest(ResponseCode.Collision_InvalidRadius);
                    break;

                default:
                    throw new BadRequest(ResponseCode.Collision_InvalidShapeType);
            }

            ShapeType = shapeType;
            Width = width;
            Height = height;
            Radius = radius;

            OffsetX = offsetX;
            OffsetY = offsetY;

            IsBlocking = isBlocking;
            IsTrigger = isTrigger;
        }

        #region Methods
        #endregion
    }
}
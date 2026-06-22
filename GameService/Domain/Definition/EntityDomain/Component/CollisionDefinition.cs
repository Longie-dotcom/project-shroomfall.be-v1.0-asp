using Contract.Enum.EntityDomain;
using Domain.Abstraction;
using Domain.Shared.DomainException;
using Domain.Shared.ResponseCode;

namespace Domain.Definition.EntityDomain.Component
{
    public class CollisionDefinition : ComponentDefinition
    {
        #region Attributes
        #endregion

        #region Properties
        public CollisionShapeType ShapeType { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }
        public float Radius { get; private set; }
        public bool IsBlocking { get; private set; }
        public CollisionLayer Layer { get; private set; }
        public CollisionLayer Mask { get; private set; }
        public float OffsetX { get; private set; }
        public float OffsetY { get; private set; }
        #endregion

        protected CollisionDefinition() : base() { }

        public CollisionDefinition(
            Guid id,
            string entityDefinitionId,
            CollisionShapeType shapeType,
            float width,
            float height,
            float radius,
            bool isBlocking,
            CollisionLayer layer,
            CollisionLayer mask,
            float offsetX = 0f,
            float offsetY = 0f) : base(id, entityDefinitionId)
        {
            if (width < 0)
                throw new BadRequest(
                    DomainCode.CollisionDefinitionCode.WidthNegative,
                    $"Collision definition creation failed for entity '{entityDefinitionId}'. Width cannot be negative. Value: {width}");

            if (height < 0)
                throw new BadRequest(
                    DomainCode.CollisionDefinitionCode.HeightNegative,
                    $"Collision definition creation failed for entity '{entityDefinitionId}'. Height cannot be negative. Value: {height}");

            if (radius < 0)
                throw new BadRequest(
                    DomainCode.CollisionDefinitionCode.RadiusNegative,
                    $"Collision definition creation failed for entity '{entityDefinitionId}'. Radius cannot be negative. Value: {radius}");

            switch (shapeType)
            {
                case CollisionShapeType.Point:
                    break;

                case CollisionShapeType.Box:
                    if (width <= 0)
                        throw new BadRequest(
                            DomainCode.CollisionDefinitionCode.BoxWidthMissing,
                            $"Collision definition creation failed for box shape on entity '{entityDefinitionId}'. Box requires a width greater than 0.");

                    if (height <= 0)
                        throw new BadRequest(
                            DomainCode.CollisionDefinitionCode.BoxHeightMissing,
                            $"Collision definition creation failed for box shape on entity '{entityDefinitionId}'. Box requires a height greater than 0.");
                    break;

                case CollisionShapeType.Circle:
                    if (radius <= 0)
                        throw new BadRequest(
                            DomainCode.CollisionDefinitionCode.CircleRadiusMissing,
                            $"Collision definition creation failed for circle shape on entity '{entityDefinitionId}'. Circle requires a radius greater than 0.");
                    break;

                default:
                    throw new BadRequest(
                        DomainCode.CollisionDefinitionCode.UnsupportedShapeType,
                        $"Collision definition creation failed for entity '{entityDefinitionId}'. The shape type value '{(int)shapeType}' is not supported.");
            }

            ShapeType = shapeType;
            Width = width;
            Height = height;
            Radius = radius;
            IsBlocking = isBlocking;
            Layer = layer;
            Mask = mask;
            OffsetX = offsetX;
            OffsetY = offsetY;
        }

        #region Methods
        #endregion
    }
}
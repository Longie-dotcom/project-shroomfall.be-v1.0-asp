using Contract.Enum.EntityDomain;
using Domain.Abstraction;
using Domain.DomainException;
using Domain.Shared;
using ResponseCode;

namespace Domain.Definition.EntityDomain.Component
{
    public class CollisionDefinition : ComponentDefinition
    {
        #region Attributes
        #endregion

        #region Properties
        public CollisionRole CollisionRole { get; set; }
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
            CollisionRole collisionRole,
            CollisionShapeType shapeType,
            float width,
            float height,
            float radius,
            bool isBlocking,
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

            CollisionLayer finalLayer;
            CollisionLayer finalMask;

            switch (collisionRole)
            {
                case CollisionRole.Player:
                    finalLayer = CollisionLayer.Player;
                    finalMask = CollisionPresets.PlayerMask;
                    break;

                case CollisionRole.Enemy:
                    finalLayer = CollisionLayer.Enemy;
                    finalMask = CollisionPresets.EnemyMask;
                    break;

                case CollisionRole.PlayerProjectile:
                    finalLayer = CollisionLayer.PlayerProjectile;
                    finalMask = CollisionPresets.PlayerProjectileMask;
                    break;

                case CollisionRole.EnemyProjectile:
                    finalLayer = CollisionLayer.EnemyProjectile;
                    finalMask = CollisionPresets.EnemyProjectileMask;
                    break;

                case CollisionRole.Collectible:
                    finalLayer = CollisionLayer.Collectible;
                    finalMask = CollisionPresets.CollectibleMask;
                    break;

                case CollisionRole.Wall:
                    finalLayer = CollisionLayer.Wall;
                    finalMask = CollisionPresets.WallMask;
                    break;

                default:
                    finalLayer = CollisionLayer.None;
                    finalMask = CollisionLayer.None;
                    break;
            }

            ShapeType = shapeType;
            Width = width;
            Height = height;
            Radius = radius;
            IsBlocking = isBlocking;
            Layer = finalLayer;
            Mask = finalMask;
            OffsetX = offsetX;
            OffsetY = offsetY;
        }

        #region Methods
        #endregion
    }
}
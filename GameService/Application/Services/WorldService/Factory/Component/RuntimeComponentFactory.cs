using Domain.Common;
using Domain.Runtime.EntityDomain.Component;

namespace Application.Services.WorldService.Factory.Component
{
    public class RuntimeComponentFactory
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public RuntimeComponentFactory() { }

        #region Methods
        public ActionInstance CreateAction()
        {
            return new ActionInstance();
        }

        public EffectContainerInstance CreateEffectContainer()
        {
            return new EffectContainerInstance();
        }

        public EquipmentInstance CreateEquipment()
        {
            return new EquipmentInstance();
        }

        public OwnershipInstance CreateOwnership(
            string userId)
        {
            return new OwnershipInstance(userId);
        }

        public TransformInstance CreateTransform(
            string roomSpatialId,
            int layerZ,
            Vector2 position)
        {
            return new TransformInstance(roomSpatialId, layerZ, position);
        }

        public WorldItemPayloadInstance CreateWorldItemPayload(
            ItemInstance itemInstance)
        {
            return new WorldItemPayloadInstance(itemInstance);
        }
        #endregion
    }
}
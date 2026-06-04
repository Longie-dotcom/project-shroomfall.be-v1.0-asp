using Contract.Enum.EntityDomain;
using Domain.Common;
using Domain.Definition.EntityDomain.Component;
using Domain.Definition.LocalizationDomain;
using Domain.DomainException;
using Domain.Shared;

namespace Domain.Definition.EntityDomain
{
    public class Portal : Entity
    {
        #region Attributes
        #endregion

        #region Properties
        public Collision Entrance { get; private set; }
        public Vector2 EntrancePosition { get; private set; }
        #endregion

        protected Portal()
        {

        }

        public Portal(
            string id,
            EntityType type,
            LocalizedText localizedText,
            Appearance appearance,
            Collision collision,
            Collision entrance,
            Vector2 entrancePosition) : base(
                id,
                type,
                localizedText,
                appearance,
                collision)
        {
            if (entrance == null)
                throw new BadRequest(ResponseCode.Portal_InvalidEntrance);

            if (entrance.IsBlocking)
                throw new BadRequest(ResponseCode.Portal_EntranceMustBeNonBlocking);

            if (!entrance.IsTrigger)
                throw new BadRequest(ResponseCode.Portal_EntranceMustBeTrigger);

            Entrance = entrance;
            EntrancePosition = entrancePosition;
        }

        #region Methods
        #endregion
    }
}
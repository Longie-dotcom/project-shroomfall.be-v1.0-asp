using Application.Services.WorldService;
using Contract.Enum.MetaDomain.Effect;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;
using Domain.Runtime.WorldDomain.Run;

namespace Application.Services.MetaService
{
    public enum DeathOutcome
    {
        None,
        Entity,
        Player // Keep this simple first
    }

    public class DeathService
    {
        #region Attributes
        private readonly PartyService<CombatRunInstance, CombatRunParticipant> partyService;
        #endregion

        #region Properties
        #endregion

        public DeathService(
            PartyService<CombatRunInstance, CombatRunParticipant> partyService)
        {
            this.partyService = partyService;
        }

        #region Methods
        public DeathOutcome CheckDeath(
            EntityInstance entity,
            AttributeType vital,
            float previousValue,
            float currentValue)
        {
            if (vital != AttributeType.Health)
                return DeathOutcome.None;

            if (previousValue <= 0f || currentValue > 0f)
                return DeathOutcome.None;

            if (entity.GetComponent<OwnershipInstance>() == null)
                return DeathOutcome.Entity;

            return partyService.IsPlayerInRun(entity.ID)
                ? DeathOutcome.Player
                : DeathOutcome.None;
        }
        #endregion
    }
}
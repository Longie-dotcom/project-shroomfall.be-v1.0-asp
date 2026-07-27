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

            if (currentValue > 0f)
                return DeathOutcome.None;

            var ownership = entity.GetComponent<OwnershipInstance>();

            // Case A: Unowned entity (e.g., standard wild monsters / creeps)
            if (ownership == null)
            {
                Console.WriteLine($"[CheckDeath] Outcome: Entity (Unowned entity died)");
                return DeathOutcome.Entity;
            }

            // Case B: Owned entity - Check if it's a player
            if (partyService.IsPlayerInRun(entity.ID))
            {
                Console.WriteLine($"[CheckDeath] Outcome: Player (Active run player died)");
                return DeathOutcome.Player;
            }

            // Case C: Non-player owned entity (e.g., player summons, pet, or owned minion)
            Console.WriteLine($"[CheckDeath] Outcome: Entity (Owned non-player entity died)");
            return DeathOutcome.Entity;
        }
        #endregion
    }
}
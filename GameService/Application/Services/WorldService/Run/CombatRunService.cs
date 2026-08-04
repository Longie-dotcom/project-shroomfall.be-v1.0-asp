using Contract.Enum.WorldDomain;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;
using Domain.Runtime.WorldDomain.Run;

namespace Application.Services.WorldService.Run
{
    public class CombatRunService
    {
        #region Attributes
        private readonly PartyService<CombatRunInstance, CombatRunParticipant> partyService;
        #endregion

        #region Properties
        #endregion

        public CombatRunService(
            PartyService<CombatRunInstance, CombatRunParticipant> partyService)
        {
            this.partyService = partyService;
        }

        #region Commands
        public void StartRun(CombatRunInstance run)
        {
            // PartyService handles validating if run or players are already registered
            partyService.RegisterRun(run);
        }

        public bool EndRun(string runId)
        {
            return partyService.RemoveRun(runId);
        }

        public bool HandlePlayerDeath(
            EntityInstance player)
        {
            var run = partyService.GetRunByPlayer(player.ID);
            if (run == null)
                return false;

            var participant = run.Participants.FirstOrDefault(p => p.EntityInstanceID == player.ID);
            participant?.SetMode(CombatRunParticipantMode.Spectator);

            var action = player?.GetComponent<ActionInstance>();
            if (action != null)
            {
                action.CanUseItems = false;
                action.ClearItemUseIntent();
            }

            run.CheckFail();

            return true;
        }
        #endregion

        #region Queries
        public CombatRunInstance? GetRunByPlayer(string playerEntityInstanceId)
        {
            return partyService.GetRunByPlayer(playerEntityInstanceId);
        }
        #endregion
    }
}

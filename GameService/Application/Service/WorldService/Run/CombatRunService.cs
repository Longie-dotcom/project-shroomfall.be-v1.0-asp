using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;
using Domain.Runtime.WorldDomain.Run;

namespace Application.Service.WorldService.Run
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
        public void RegisterRun(
            CombatRunInstance run)
        {
            partyService.RegisterRun(run);
        }

        public bool RemoveRun(
            string runId)
        {
            return partyService.RemoveRun(runId);
        }

        public void AddPlayerToRun(
            string runId,
            string playerInstanceId)
        {
            partyService.AddPlayerToRun(runId, playerInstanceId);
        }

        public bool HandlePlayerDisconnect(
            string playerInstanceId)
        {
            return partyService.HandleDisconnect(playerInstanceId);
        }

        public bool HandlePlayerReconnect(
            string playerInstanceId)
        {
            return partyService.HandleReconnect(playerInstanceId);
        }

        public bool HandlePlayerQuit(
            string playerInstanceId)
        {
            return partyService.HandleQuit(playerInstanceId);
        }

        public bool HandlePlayerDeath(
            EntityInstance player)
        {
            var run = partyService.GetRunByPlayer(player.ID);
            if (run == null)
                return false;

            var participant = run.Participants.FirstOrDefault(p => p.EntityInstanceID == player.ID);
            participant?.SetSpectator();

            var action = player.GetComponent<ActionInstance>();
            if (action != null)
            {
                action.CanUseItems = false;
                action.ClearItemUseIntent();
            }

            run.CheckFail();
            return true;
        }

        public bool AdvanceFloor(
            string runId,
            string nextRoomSpatialId)
        {
            var run = partyService.GetRun(runId);
            if (run == null) 
                return false;

            run.AdvanceFloor(nextRoomSpatialId);
            return true;
        }

        public void Tick()
        {
            var expiredParticipants = partyService.GetExpiredInactiveParticipants(TimeSpan.FromSeconds(10)).ToList();
            foreach (var (run, participant) in expiredParticipants)
            {
                partyService.HandleQuit(participant.EntityInstanceID);
            }
        }
        #endregion

        #region Queries
        public CombatRunInstance? GetRunByPlayer(
            string playerEntityInstanceId)
        {
            return partyService.GetRunByPlayer(playerEntityInstanceId);
        }

        public CombatRunInstance? GetRun(
            string runId)
        {
            return partyService.GetRun(runId);
        }
        #endregion
    }
}

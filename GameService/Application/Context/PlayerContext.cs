namespace Application.Context
{
    public class PlayerContext
    {
        #region Attributes
        private readonly Dictionary<string, HashSet<string>> roomPlayerInstances = new();
        #endregion

        #region Properties
        #endregion

        public PlayerContext()
        {

        }

        #region Methods
        public void JoinRoom(
            string roomSpatialId,
            string playerInstanceId)
        {
            if (!roomPlayerInstances.TryGetValue(roomSpatialId, out var set))
            {
                set = new HashSet<string>();
                roomPlayerInstances[roomSpatialId] = set;
            }

            set.Add(playerInstanceId);
        }

        public void LeaveRoom(
            string roomSpatialId,
            string playerInstanceId)
        {
            if (!roomPlayerInstances.TryGetValue(roomSpatialId, out var set))
                return;

            set.Remove(playerInstanceId);
        }

        public bool IsRoomEmpty(
            string roomSpatialId)
        {
            return !roomPlayerInstances.TryGetValue(roomSpatialId, out var set)
                || set.Count == 0;
        }
        #endregion
    }
}
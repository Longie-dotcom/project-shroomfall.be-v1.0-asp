namespace Domain.Abstraction
{
    public interface IRunParticipant
    {
        string EntityInstanceID { get; }
        DateTime? InactiveSinceUtc { get; }

        void SetInactive();
        void SetActive();
    }
}

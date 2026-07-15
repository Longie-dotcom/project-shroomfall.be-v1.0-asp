namespace Domain.Abstraction
{
    public interface IRunInstance<TParticipant> 
        where TParticipant : IRunParticipant
    {
        string ID { get; }
        string LeaderEntityInstanceID { get; }
        IEnumerable<TParticipant> Participants { get; }
    }
}

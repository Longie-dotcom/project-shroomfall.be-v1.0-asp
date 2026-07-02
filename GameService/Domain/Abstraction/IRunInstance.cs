namespace Domain.Abstraction
{
    public interface IRunInstance
    {
        string ID { get; }
        string LeaderEntityInstanceID { get; }
        IReadOnlyCollection<string> PlayerEntityInstanceIDs { get; }
    }
}

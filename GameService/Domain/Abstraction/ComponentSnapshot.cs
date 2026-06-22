namespace Domain.Abstraction
{
    public abstract class ComponentSnapshot
    {
        public string Type => GetType().Name;
        public Guid DefinitionID { get; set; }
    }
}
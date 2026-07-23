namespace Domain.Abstraction
{
    public abstract class ComponentSnapshot
    {
        public string Type => GetType().Name;
        public string DefinitionID { get; set; } = string.Empty;
        public string EntityDefinitionID { get; set; } = string.Empty;
    }
}
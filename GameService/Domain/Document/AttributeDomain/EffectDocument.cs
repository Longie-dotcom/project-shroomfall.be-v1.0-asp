namespace Domain.Document.AttributeDomain
{
    public class EffectDocument
    {
        public string ID { get; set; } = string.Empty;
        public string DefinitionID { get; set; } = string.Empty;
        public float? RemainingTime { get; set; }
        public string? SourceItemInstanceID { get; set; }
    }
}
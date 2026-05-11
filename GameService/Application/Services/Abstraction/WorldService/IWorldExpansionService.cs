namespace Application.Services.Abstraction.WorldService
{
    public interface IWorldExpansionService
    {
        WorldContext Expand(
            WorldContext seed);
    }
}

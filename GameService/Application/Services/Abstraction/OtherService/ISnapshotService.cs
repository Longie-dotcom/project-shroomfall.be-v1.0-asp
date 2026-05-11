using Application.DTO.Connection;

namespace Application.Services.Abstraction.OtherService
{
    public interface ISnapshotService
    {
        DefinitionSnapshotDTO BuildDefinitionSnapshot(
            long version);
    }
}

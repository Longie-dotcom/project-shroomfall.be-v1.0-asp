using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Repository.Relational;
using Application.Services.DesignService;
using Contract;
using Contract.DTO.Design;

namespace Application.Features.Design.Handlers
{
    public class UserRefreshHandler : IHandler<UserRefreshCommand, DefinitionSnapshotDTO?>
    {
        #region Attributes
        private readonly IRelationalUoW relational;
        private readonly BuilderService builderService;
        #endregion

        #region Properties
        #endregion

        public UserRefreshHandler(
            IRelationalUoW relational,
            BuilderService builderService)
        {
            this.relational = relational;
            this.builderService = builderService;
        }

        #region Methods
        public async Task<DefinitionSnapshotDTO?> Handle(
            UserRefreshCommand command)
        {
            var dto = command.DTO;

            // Resolve repository
            var definitionVersionLogRepo = relational.GetRepository<IDefinitionVersionLogRepository>();

            // Get latest global definition version
            var latest = await definitionVersionLogRepo.GetLatest(Constraint.GLOBAL_DEFINITION_VERSION);

            // No definition yet
            if (latest == null)
                return null;

            // Client already latest
            if (dto.DefinitionVersion == latest.Version.ToString())
                return null;

            // Return full snapshot
            return builderService.BuildDefinitionSnapshot(latest.Version);
        }
        #endregion
    }
}
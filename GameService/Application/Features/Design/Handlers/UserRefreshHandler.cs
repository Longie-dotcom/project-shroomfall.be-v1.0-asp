using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Cache;
using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using AutoMapper;
using Contract;
using Contract.DTO.Definition.EntityDomain.Component;
using Contract.DTO.Definition.LocalizationDomain;
using Contract.DTO.Definition.MetaDomain;
using Contract.DTO.Definition.WorldDomain;
using Contract.DTO.Feature.Design.Response;

namespace Application.Features.Design.Handlers
{
    public class UserRefreshHandler : IHandler<UserRefreshCommand, DefinitionSnapshotDTO?>
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        private readonly IMapper mapper;
        private readonly ICacheProvider cacheProvider;
        #endregion

        #region Properties
        #endregion

        public UserRefreshHandler(
            IRelationalUoW relationalUoW,
            IMapper mapper,
            ICacheProvider cacheProvider)
        {
            this.relationalUoW = relationalUoW;
            this.mapper = mapper;
            this.cacheProvider = cacheProvider;
        }

        #region Methods
        public async Task<DefinitionSnapshotDTO?> Handle(
            UserRefreshCommand command)
        {
            var dto = command.DTO;

            // Resolve repository
            var definitionVersionLogRepo = relationalUoW.GetRepository<IDefinitionVersionLogRepository>();

            // Get latest global definition version
            var latest = await definitionVersionLogRepo.GetLatest(Constraint.GLOBAL_DEFINITION_VERSION);

            // No definition yet
            if (latest == null)
                return null;

            // Client already latest
            if (dto.DefinitionVersion == latest.Version.ToString())
                return null;

            // Return full snapshot
            return BuildDefinitionSnapshot(latest.Version);
        }

        public DefinitionSnapshotDTO BuildDefinitionSnapshot(
            long version)
        {
            var allRooms = cacheProvider.Room.GetAll();
            var effects = mapper.Map<List<EffectDefinitionDTO>>(cacheProvider.Effect.GetAll());
            var items = mapper.Map<List<ItemDefinitionDTO>>(cacheProvider.Item.GetAll());
            var entities = mapper.Map<List<EntityDefinitionDTO>>(cacheProvider.Entity.GetAll());
            var cells = mapper.Map<List<CellDTO>>(allRooms.SelectMany(r => r.Cells).ToList());
            var entitySpawnRules = mapper.Map<List<EntitySpawnRuleDTO>>(allRooms.SelectMany(r => r.EntitySpawnRules).ToList());
            var rooms = mapper.Map<List<RoomDefinitionDTO>>(allRooms);
            var combatRuns = mapper.Map<List<CombatRunDefinitionDTO>>(cacheProvider.CombatRun.GetAll());
            var locales = mapper.Map<List<LocaleDTO>>(cacheProvider.Locale.GetAll());

            return new DefinitionSnapshotDTO
            {
                Version = version,
                Effects = effects,
                Items = items,
                Entities = entities,
                CombatRuns = combatRuns,
                Rooms = rooms,
                EntitySpawnRules = entitySpawnRules,
                Cells = cells,
                Locales = locales
            };
        }
        #endregion
    }
}
using Application.Interfaces.Cache;
using AutoMapper;
using Contract.DTO.Definition.EntityDomain.Component;
using Contract.DTO.Definition.LocalizationDomain;
using Contract.DTO.Definition.MetaDomain;
using Contract.DTO.Definition.WorldDomain;
using Contract.DTO.Feature.Design.Response;

namespace Application.Services.DesignService
{
    public class DefinitionService
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly ICacheProvider cacheProvider;
        #endregion

        #region Properties
        #endregion

        public DefinitionService(
            IMapper mapper,
            ICacheProvider cacheProvider)
        {
            this.mapper = mapper;
            this.cacheProvider = cacheProvider;
        }

        #region Methods
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
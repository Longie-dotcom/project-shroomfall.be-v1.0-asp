using Application.Interfaces.Cache;
using AutoMapper;
using Contract.DTO.Design;
using Contract.DTO.Domain.Definition;
using Domain.Shared;

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
            var allConnections = cacheProvider.RoomConnection.GetAll();
            var attributes = AttributeDefinitions.AllList()
                .Select(x => mapper.Map<AttributeDefinitionDTO>(x))
                .ToList();
            var effects = mapper.Map<List<EffectDefinitionDTO>>(cacheProvider.Effect.GetAll());
            var items = mapper.Map<List<ItemDefinitionDTO>>(cacheProvider.Item.GetAll());
            var entities = mapper.Map<List<EntityDefinitionDTO>>(cacheProvider.Entity.GetAll());
            var cells = mapper.Map<List<CellDefinitionDTO>>(allRooms.SelectMany(r => r.Cells).ToList());
            var spawnRules = mapper.Map<List<EntitySpawnRuleDefinitionDTO>>(allRooms.SelectMany(r => r.EntitySpawnRules).ToList());
            var rooms = mapper.Map<List<RoomDefinitionDTO>>(allRooms);
            var roomConnections = mapper.Map<List<RoomConnectionDefinitionDTO>>(allConnections);
            var locales = mapper.Map<List<LocaleDTO>>(cacheProvider.Locale.GetAll());

            return new DefinitionSnapshotDTO
            {
                Version = version,
                Attributes = attributes,
                Effects = effects,
                Items = items,
                Entities = entities,
                Cells = cells,
                EntitySpawnRules = spawnRules,
                Rooms = rooms,
                RoomConnections = roomConnections,
                Locales = locales
            };
        }
        #endregion
    }
}
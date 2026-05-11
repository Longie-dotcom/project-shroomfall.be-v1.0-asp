using Application.Interfaces.Cache;
using Application.Services.Abstraction.WorldService;
using Domain.Common;
using Domain.Definition.WorldDomain;
using Domain.Definition.WorldDomain.Enum;
using Domain.DomainException;
using Domain.Shared;

namespace Application.Services.WorldService
{
    public class SpawnService : ISpawnService
    {
        #region Attributes
        private readonly Random random;
        private readonly IRoomCache roomCache;
        #endregion

        #region Properties
        #endregion

        public SpawnService(
            IRoomCache roomCache)
        {
            random = new Random();
            this.roomCache = roomCache;
        }

        #region Methods
        public (Vector2 position, int layerZ) ResolveSpawnPosition(
            string roomDefinitionId,
            SpawnArea area)
        {
            var roomDef = roomCache.Get(roomDefinitionId);

            if (roomDef == null)
                throw new BadRequest(
                    ResponseCode.SpawnService_RoomDefinitionNotFound);

            int x = random.Next(area.MinX, area.MaxX + 1);
            int y = random.Next(area.MinY, area.MaxY + 1);

            var cell = roomCache.GetTopCell(
                roomDef.ID,
                x,
                y);

            if (cell == null)
                throw new BadRequest(
                    ResponseCode.SpawnService_NoSpawnArea);

            return (
                new Vector2(x, y),
                cell.Z
            );
        }

        public (Vector2 position, int layerZ) ResolveSpawnPosition(
            string roomDefinitionId,
            string entityDefinitionId,
            SpawnRuleType type)
        {
            var rule = ResolveSpawnRule(
                roomDefinitionId,
                entityDefinitionId,
                type);

            var area = PickWeightedArea(rule.SpawnAreas);

            return ResolveSpawnPosition(roomDefinitionId, area);
        }

        public SpawnArea PickWeightedArea(
            ICollection<SpawnArea> areas)
        {
            if (areas == null || areas.Count == 0)
                throw new BadRequest(
                    ResponseCode.SpawnService_NoSpawnArea);

            float totalWeight = 0f;

            foreach (var area in areas)
                totalWeight += area.Weight;

            float roll =
                (float)(random.NextDouble() * totalWeight);

            float current = 0f;

            foreach (var area in areas)
            {
                current += area.Weight;

                if (roll <= current)
                    return area;
            }

            // fallback
            return areas.Last();
        }

        private EntitySpawnRule ResolveSpawnRule(
            string roomDefinitionId,
            string entityDefinitionId,
            SpawnRuleType type)
        {
            var roomDef = roomCache.Get(roomDefinitionId);

            if (roomDef == null)
                throw new BadRequest(
                    ResponseCode.SpawnService_RoomDefinitionNotFound);

            var rules = roomDef.EntitySpawnRules
                .Where(r =>
                    r.EntityID == entityDefinitionId &&
                    r.Type == type)
                .ToList();

            if (rules.Count == 0)
                throw new BadRequest(
                    ResponseCode.SpawnService_SpawnNotAllowed);

            return rules[random.Next(rules.Count)];
        }
        #endregion
    }
}
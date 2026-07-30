using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using Application.Services.DesignService;
using Contract.DTO.Definition.WorldDomain;
using Domain.Definition.LocalizationDomain;
using Domain.Definition.WorldDomain;
using Domain.DomainException;
using ResponseCode;
using System.Text.Json;

namespace Application.Features.Design.Handlers
{
    public class RoomDefinitionPayload
    {
        public RoomDefinitionDTO Room { get; set; } = new RoomDefinitionDTO();
        public List<CellDTO> Cells { get; set; } = new List<CellDTO>();
        public List<EntitySpawnRuleDTO> EntitySpawnRules { get; set; } = new List<EntitySpawnRuleDTO>();
    }

    public class ImportRoomDefinitionHandler : IHandler<ImportRoomDefinitionCommand>
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        private readonly LocalizationEntryFactory localizationEntryFactory;
        #endregion

        #region Properties
        #endregion

        public ImportRoomDefinitionHandler(
            IRelationalUoW relationalUoW,
            LocalizationEntryFactory localizationEntryFactory)
        {
            this.relationalUoW = relationalUoW;
            this.localizationEntryFactory = localizationEntryFactory;
        }

        #region Methods
        public async Task Handle(
            ImportRoomDefinitionCommand command)
        {
            if (command.File == null || command.File.Length == 0)
                throw new BadRequest(
                    ApplicationCode.DesignHandlerCode.RoomFilePayloadEmpty,
                    "The uploaded room definition file is null or empty.");

            RoomDefinitionPayload? payload;

            // Stream Reading & Parsing
            try
            {
                using var stream = command.File.OpenReadStream();

                payload = await JsonSerializer.DeserializeAsync<RoomDefinitionPayload>(
                    stream, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException ex)
            {
                throw new BadRequest(
                    ApplicationCode.DesignHandlerCode.RoomFileInvalidJson,
                    $"Failed to deserialize JSON stream due to formatting errors: {ex.Message}");
            }

            // Data Invariant Verification
            if (payload == null || payload.Room == null)
                throw new BadRequest(
                    ApplicationCode.DesignHandlerCode.RoomFileSchemaParseFailed,
                    "The file was parsed but contains an invalid root payload structure or missing 'Room' schema definition.");

            // Extract our clean, validated objects
            var dto = payload.Room;

            // Resolve repository
            var repo = relationalUoW.GetRepository<IRoomDefinitionRepository>();
            await relationalUoW.BeginTransactionAsync();

            // Generate core localization & presentation setup safely via Definition ID
            var localizedText = ForRoom(dto.Id);

            // Process Parent Entity (Create or Track Update)
            var existingRoom = await repo.GetByIdAsync(dto.Id);
            if (existingRoom == null)
            {
                var newRoom = new RoomDefinition(
                    dto.Id,
                    dto.Type,
                    new RoomPresentationDefinition(localizedText, dto.Id));

                await repo.AddAsync(newRoom);
                await localizationEntryFactory.PreSavePlaceholderKeysAsync(localizedText);
            }
            else
            {
                existingRoom.UpdateFields(dto.Type);
            }

            // Map Child Collections directly using your shared project DTOs
            var domainCells = payload.Cells.Select(c => new Cell(
                roomDefinitionId: dto.Id,
                type: c.Type,
                x: c.X,
                y: c.Y,
                z: c.Z
            )).ToList();

            var domainRules = payload.EntitySpawnRules.Select(r => new EntitySpawnRule(
                id: r.ID == Guid.Empty ? Guid.NewGuid() : r.ID,
                type: r.Type,
                minX: r.MinX,
                minY: r.MinY,
                maxX: r.MaxX,
                maxY: r.MaxY,
                minCount: r.MinCount,
                maxCount: r.MaxCount,
                roomDefinitionId: dto.Id,
                entityDefinitionId: r.EntityDefinitionID
            )).ToList();

            // Wipe old children and save current configuration state atomically
            await repo.UpsertChildrenAsync(dto.Id, domainCells, domainRules);
            await relationalUoW.CommitAsync();
        }

        public LocalizedText ForRoom(
            string roomId)
        {
            roomId = string.IsNullOrWhiteSpace(roomId) ? "unknown" : roomId.Trim().ToLowerInvariant();

            return new LocalizedText
            {
                NameKey = $"room.{roomId}.name",
                DescriptionKey = $"room.{roomId}.description"
            };
        }
        #endregion
    }
}
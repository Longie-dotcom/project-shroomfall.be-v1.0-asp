using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using Contract.DTO.Domain.Definition;
using Domain.Definition.WorldDomain;
using Domain.DomainException;
using Domain.Shared;
using ResponseCode;
using System.Text.Json;

namespace Application.Features.Design.Handlers
{
    public class RoomDefinitionPayload
    {
        public RoomDefinitionDTO Room { get; set; } = new RoomDefinitionDTO();
        public List<CellDefinitionDTO> Cells { get; set; } = new List<CellDefinitionDTO>();
        public List<EntitySpawnRuleDefinitionDTO> EntitySpawnRules { get; set; } = new List<EntitySpawnRuleDefinitionDTO>();
    }

    public class UpsertRoomDefinitionHandler : IHandler<UpsertRoomDefinitionCommand>
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        #endregion

        #region Properties
        #endregion

        public UpsertRoomDefinitionHandler(
            IRelationalUoW relationalUoW)
        {
            this.relationalUoW = relationalUoW;
        }

        #region Methods
        public async Task Handle(
            UpsertRoomDefinitionCommand command)
        {
            if (command.File == null || command.File.Length == 0)
            {
                throw new BadRequest(
                    ApplicationCode.DesignHandlerCode.RoomFilePayloadEmpty,
                    "The uploaded room definition file is null or empty.");
            }

            RoomDefinitionPayload? payload;

            // Stream Reading & Parsing
            try
            {
                using var stream = command.File.OpenReadStream();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                payload = await JsonSerializer.DeserializeAsync<RoomDefinitionPayload>(stream, options);
            }
            catch (JsonException ex)
            {
                throw new BadRequest(
                    ApplicationCode.DesignHandlerCode.RoomFileInvalidJson,
                    $"Failed to deserialize JSON stream due to formatting errors: {ex.Message}");
            }

            // Data Invariant Verification
            if (payload == null || payload.Room == null)
            {
                throw new BadRequest(
                    ApplicationCode.DesignHandlerCode.RoomFileSchemaParseFailed,
                    "The file was parsed but contains an invalid root payload structure or missing 'Room' schema definition.");
            }

            // Extract our clean, validated objects
            var dto = payload.Room;

            // Resolve repository
            var repo = relationalUoW.GetRepository<IRoomDefinitionRepository>();

            // Generate core localization & presentation setup safely via Definition ID
            var localizedText = LocalizationFactory.ForRoom(dto.ID);

            // Process Parent Entity (Create or Track Update)
            var existingRoom = await repo.GetByIdAsync(dto.ID);
            if (existingRoom == null)
            {
                var newRoom = new RoomDefinition(
                    dto.ID, 
                    dto.Type, 
                    new RoomPresentationDefinition(localizedText, dto.ID));
                await repo.AddAsync(newRoom);
            }

            // Map Child Collections directly using your shared project DTOs
            var domainCells = payload.Cells.Select(c => new Cell(
                roomDefinitionId: dto.ID,
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
                roomDefinitionId: dto.ID,
                entityDefinitionId: r.EntityDefinitionID
            )).ToList();

            // Wipe old children and save current configuration state atomically
            await repo.UpsertChildrenAsync(dto.ID, domainCells, domainRules);
            await relationalUoW.SaveChangesAsync();
        }
        #endregion
    }
}
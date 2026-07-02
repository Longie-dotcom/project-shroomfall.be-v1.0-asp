using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using Domain.Definition.WorldDomain;
using Domain.Shared;

namespace Application.Features.Design.Handlers
{
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
        public async Task Handle(UpsertRoomDefinitionCommand command)
        {
            var dto = command.Room;

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
            var domainCells = command.Cells.Select(c => new Cell(
                roomDefinitionId: dto.ID,
                type: c.Type,
                x: c.X,
                y: c.Y,
                z: c.Z
            )).ToList();

            var domainRules = command.EntitySpawnRules.Select(r => new EntitySpawnRule(
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
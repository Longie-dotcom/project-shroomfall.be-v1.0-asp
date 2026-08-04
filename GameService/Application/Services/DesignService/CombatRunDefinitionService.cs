using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using Contract.DTO.Definition.WorldDomain;
using Domain.Definition.WorldDomain;

namespace Application.Services.DesignService
{
    public class CombatRunDefinitionService
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        #endregion

        #region Properties
        #endregion

        public CombatRunDefinitionService(
            IRelationalUoW relationalUoW)
        {
            this.relationalUoW = relationalUoW;
        }

        #region Methods
        public async Task UpsertWithoutSave(
            CombatRunDefinitionDTO dto)
        {
            // Upsert flow
            var combatRunRepo = relationalUoW.GetRepository<ICombatRunDefinitionRepository>();
            var existingCombatRun = await combatRunRepo.GetByIdAsync(dto.Id);
            if (existingCombatRun == null)
            {
                // CREATE FLOW 
                var combatRun = new CombatRunDefinition(dto.Id);
                await combatRunRepo.AddAsync(combatRun);
            }

            // ALL FLOWS
            var floors = dto.Floors.Select(f => new Floor(f.Level, f.RoomDefinitionID, dto.Id));
            await combatRunRepo.UpsertFloorsAsync(dto.Id, floors);
        }
        #endregion
    }
}

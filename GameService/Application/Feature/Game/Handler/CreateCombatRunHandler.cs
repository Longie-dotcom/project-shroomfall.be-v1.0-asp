using Application.Feature.Abstraction;
using Application.Feature.Game.Command;
using Application.Interface.Cache;
using Application.Interface.Realtime.Managers;
using Application.Service.WorldService;
using Application.Service.WorldService.Creation;
using Application.Service.WorldService.Run;
using AutoMapper;
using Contract;
using Contract.DTO.Feature.Connection.Response;
using Contract.DTO.Feature.Game.Response;
using Contract.DTO.Runtime.EntityDomain;
using Contract.DTO.Runtime.WorldDomain;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.WorldDomain.Run;
using ResponseCode;

namespace Application.Feature.Game.Handler
{
    public class CreateCombatRunHandler : IHandler<CreateCombatRunCommand, CombatRunDTO>
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly ISessionManager sessionManager;
        private readonly WorldContext worldContext;
        private readonly InitializationService initializationService;
        private readonly CombatRunService combatRunService;
        private readonly RoomMigrationService roomMigrationService;
        private readonly ICacheProvider cacheProvider;
        #endregion

        #region Properties
        #endregion

        public CreateCombatRunHandler(
            IMapper mapper,
            ISessionManager sessionManager,
            WorldContext worldContext,
            InitializationService initializationService,
            CombatRunService combatRunService,
            RoomMigrationService roomMigrationService,
            ICacheProvider cacheProvider)
        {
            this.mapper = mapper;
            this.sessionManager = sessionManager;
            this.worldContext = worldContext;
            this.initializationService = initializationService;
            this.combatRunService = combatRunService;
            this.roomMigrationService = roomMigrationService;
            this.cacheProvider = cacheProvider;
        }

        #region Properties
        public async Task<CombatRunDTO> Handle(
            CreateCombatRunCommand command)
        {
            // Validate player session
            var playerInstanceId = sessionManager.Get(command.UserID);
            if (playerInstanceId == null)
                throw new InternalException(
                    ApplicationCode.GameHandlerCode.CreateCombatRunSessionNotFound,
                    $"Session missing for user '{command.UserID}'.");

            // Validate runtime player
            var player = worldContext.GetEntity(playerInstanceId);
            if (player == null)
                throw new InternalException(
                    ApplicationCode.GameHandlerCode.CreateCombatRunPlayerInstanceNotFound,
                    $"Player runtime instance '{playerInstanceId}' was not found.");

            // Prevent duplicate participation
            var existed = combatRunService.GetRunByPlayer(player.ID);
            if (existed != null)
                throw new BadRequest(
                    ApplicationCode.GameHandlerCode.CreateCombatRunPlayerAlreadyInCombatRun,
                    $"Player '{player.ID}' is already participating in a combat run.");

            // Validate combat run definition
            var combatDefinition = cacheProvider.CombatRun.Get(command.CombatRunDefinitionID);
            if (combatDefinition == null)
                throw new BadRequest(
                    ApplicationCode.GameHandlerCode.CreateCombatRunDefinitionNotFound,
                    $"Combat '{command.CombatRunDefinitionID}' is not found.");

            // Retrieve and initialize first floor
            var roomSpatialId = $"combat-{Guid.NewGuid()}";
            var firstFloor = combatDefinition.Floors.OrderBy(f => f.Level).First();
            initializationService.InitializeRoom(
                firstFloor.RoomDefinitionID,
                roomSpatialId,
                RoomLifecyclePolicy.Ephemeral);

            // Create & register run
            var runInstanceId = GenerateRunCode();
            var run = new CombatRunInstance(
                runInstanceId,
                command.CombatRunDefinitionID,
                playerInstanceId,
                new[] { playerInstanceId },
                roomSpatialId);

            combatRunService.RegisterRun(run);

            // Move leader into combat room
            var room = await roomMigrationService.EnterRoomAsync(
                player,
                roomSpatialId);

            return new CombatRunDTO
            {
                CombatRunInstanceID = runInstanceId,
                SaveGame = BuildSaveGame(player, room)
            };
        }

        private static string GenerateRunCode(
            int length = 6)
        {
            return string
                .Concat(Enumerable.Range(0, length)
                .Select(_ => Constraint.RUN_CODE_CHARS[Random.Shared.Next(Constraint.RUN_CODE_CHARS.Length)]));
        }

        private SaveGameDTO BuildSaveGame(
            EntityInstance player,
            RoomInstanceDTO room)
        {
            var saveGame = new SaveGameDTO()
            {
                PlayerData = mapper.Map<EntityInstanceDTO>(player),
                RoomData = room
            };

            return saveGame;
        }
        #endregion
    }
}

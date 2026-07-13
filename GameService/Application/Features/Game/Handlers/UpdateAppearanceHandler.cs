using Application.Features.Abstraction;
using Application.Features.Game.Commands;
using Application.Interfaces.Realtime.Events;
using Application.Interfaces.Realtime.Events.Game;
using Application.Interfaces.Realtime.Managers;
using Application.Services.WorldService;
using AutoMapper;
using Contract.DTO.Runtime.EntityDomain.Component;
using Domain.Common;
using Domain.DomainException;
using Domain.Runtime.EntityDomain.Component;
using ResponseCode;

namespace Application.Features.Game.Handlers
{
    public class UpdateAppearanceHandler : IHandler<UpdateAppearanceCommand>
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly WorldContext worldContext;
        private readonly ISessionManager sessionManager;
        private readonly IEventBus eventBus;
        #endregion

        #region Properties
        #endregion

        public UpdateAppearanceHandler(
            IMapper mapper,
            WorldContext worldContext,
            ISessionManager sessionManager,
            IEventBus eventBus)
        {
            this.mapper = mapper;
            this.worldContext = worldContext;
            this.sessionManager = sessionManager;  
            this.eventBus = eventBus;
        }

        #region Methods
        public async Task Handle(
            UpdateAppearanceCommand command)
        {
            var dto = command.DTO;

            // Validate session existence
            var playerInstanceId = sessionManager.Get(command.UserID);
            if (string.IsNullOrWhiteSpace(playerInstanceId))
                throw new Unauthorized(
                    ApplicationCode.GameHandlerCode.UpdateAppearanceSessionNotFound,
                    $"User with user ID: {command.UserID} has no session");

            // Validate player instance existence
            var player = worldContext.GetEntity(playerInstanceId);
            if (player == null)
                throw new InternalException(
                    ApplicationCode.GameHandlerCode.UpdateAppearancePlayerInstanceNotFound,
                    $"User with user ID: {command.UserID} has no player instance");

            // Get appearance
            var appearance = player.GetComponent<AppearanceInstance>();
            if (appearance == null)
                throw new InternalException(
                    ApplicationCode.GameHandlerCode.UpdateAppearanceComponentMissing,
                    $"Player instance {playerInstanceId} is missing AppearanceInstance component");

            // Get transform
            var transform = player.GetComponent<TransformInstance>();
            if (transform == null)
                throw new InternalException(
                    ApplicationCode.GameHandlerCode.UpdateAppearanceTransformMissing,
                    $"Player instance {playerInstanceId} is missing TransformInstance component");

            // Update appearance
            appearance.UpdateAppearance(
                dto.SkinID,
                new HSV(dto.SkinColor.H, dto.SkinColor.S, dto.SkinColor.V)
            );

            // Publish changes
            eventBus.Publish(new EntityAppearanceChangedEvent(
                playerInstanceId,
                transform.RoomSpatialID,
                mapper.Map<AppearanceInstanceDTO>(appearance)));

            await Task.CompletedTask;
        }
        #endregion
    }
}
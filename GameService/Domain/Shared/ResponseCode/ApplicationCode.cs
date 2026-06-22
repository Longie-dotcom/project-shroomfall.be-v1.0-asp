namespace Domain.Shared.ResponseCode
{
    public class ApplicationCode
    {
        private static string Code(string className, string problem) => CodeFormatter.Create("application", className, problem);

        #region Contexts
        public static class WorldContextCode
        {
            private static string Format(string problem) => Code("world-context", problem);

            public static string RoomLoaded = Format("room-loaded");
            public static string UnloadTargetMissing = Format("unload-target-missing");
            public static string RoomUnloading = Format("room-unloading");
            public static string EntityRoomChanged = Format("entity-room-changed");
            public static string ConnectionAddedSuccess = Format("connection-added-success");
            public static string ConnectionRemovedSuccess = Format("connection-removed-success");
        }
        #endregion

        #region Features
        public static class ConnectionHandlerCode
        {
            private static string Format(string problem) => Code("connection-handler", problem);

            public static string UnloadSessionPlayerInstanceNotFound = Format("unload-session-player-instance-not-found");
            public static string UnloadSessionTransformMissing = Format("unload-session-transform-missing");
            public static string LoadSessionAlreadyExisted = Format("load-session-already-existed");
            public static string LoadSessionPlayerNotFoundInPersistence = Format("load-session-player-not-found-in-persistence");
            public static string LoadSessionUnauthorizedPlayer = Format("load-session-unauthorized-player");
            public static string LoadSessionOwnershipMissing = Format("load-session-ownership-missing");
            public static string LoadSessionTransformMissing = Format("load-session-transform-missing");
            public static string LoadSessionRoomNotFound = Format("load-session-room-not-found");
        }

        public static class GameHandlerCode
        {
            private static string Format(string problem) => Code("game-handler", problem);

            public static string MoveSessionNotFound = Format("move-session-not-found");
            public static string MovePlayerInstanceNotFound = Format("move-player-instance-not-found");
            public static string MoveTransformMissing = Format("move-transform-missing");
            public static string UseItemSessionNotFound = Format("use-item-session-not-found");
            public static string UseItemPlayerInstanceNotFound = Format("use-item-player-instance-not-found");
            public static string UseItemActionComponentMissing = Format("use-item-action-component-missing");
            public static string UpdateAppearanceSessionNotFound = Format("update-appearance-session-not-found");
            public static string UpdateAppearancePlayerInstanceNotFound = Format("update-appearance-player-instance-not-found");
            public static string UpdateAppearanceComponentMissing = Format("update-appearance-component-missing");
            public static string UpdateAppearanceTransformMissing = Format("update-appearance-transform-missing");
            public static string UnequipItemSessionNotFound = Format("unequip-item-session-not-found");
            public static string UnequipItemPlayerInstanceNotFound = Format("unequip-item-player-instance-not-found");
            public static string TouchEntitySessionNotFound = Format("touch-entity-session-not-found");
            public static string TouchEntityPlayerInstanceNotFound = Format("touch-entity-player-instance-not-found");
            public static string TouchEntityTransformMissing = Format("touch-entity-transform-missing");
            public static string EnterHubSessionNotFound = Format("enter-hub-session-not-found");
            public static string EnterHubPlayerInstanceNotFound = Format("enter-hub-player-instance-not-found");
            public static string EnterHubTransformMissing = Format("enter-hub-transform-missing");
            public static string EnterHubRoomNotFound = Format("enter-hub-room-not-found");
            public static string EnterHubPlayerSpawnRuleMissing = Format("enter-hub-player-spawn-rule-missing");
            public static string EnterHubRoomDefinitionNotFound = Format("enter-hub-room-definition-not-found");
            public static string EnterHubRoomNoSpawnCellFound = Format("enter-hub-room-no-spawn-cell-found");
        }

        public static class IdentityHandlerCode
        {
            private static string Format(string problem) => Code("identity-handler", problem);

            public static string UpdateProfileUserNotFound = Format("update-profile-user-not-found");
            public static string UpdatePreferredLocaleLocaleNotFound = Format("update-preferred-locale-locale-not-found");
            public static string UpdatePreferredLocaleUserNotFound = Format("update-preferred-locale-user-not-found");
            public static string SteamAuthInvalidSteamTicket = Format("steam-auth-invalid-steam-ticket");
            public static string SteamAuthValidationFailed = Format("steam-auth-validation-failed");
            public static string RegisterEmailRequired = Format("register-email-required");
            public static string RegisterEmailAlreadyExists = Format("register-email-already-exists");
            public static string RefreshTokenUserNotFound = Format("refresh-token-user-not-found");
            public static string LoginEmailRequired = Format("login-email-required");
            public static string LoginPasswordRequired = Format("login-password-required");
            public static string LoginInvalidCredentials = Format("login-invalid-credentials");
        }
        #endregion

        #region Services
        // Item Service
        public static class InventoryServiceCode
        {
            private static string Format(string problem) => Code("inventory-service", problem);

            public static string TransferSourceInventoryMissing = Format("transfer-source-inventory-missing");
            public static string AddTargetInventoryMissing = Format("add-target-inventory-missing");
            public static string AddInventoryDefinitionNotFound = Format("add-inventory-definition-not-found");
            public static string AddItemDefinitionNotFound = Format("add-item-definition-not-found");
            public static string RemoveTargetInventoryMissing = Format("remove-target-inventory-missing");
            public static string RemoveItemNotFound = Format("remove-item-not-found");
            public static string DeductTargetInventoryMissing = Format("deduct-target-inventory-missing");
            public static string CanAddTargetInventoryMissing = Format("can-add-target-inventory-missing");
            public static string CanAddInventoryDefinitionNotFound = Format("can-add-inventory-definition-not-found");
            public static string CanAddItemDefinitionNotFound = Format("can-add-item-definition-not-found");
        }

        public static class ItemUsageServiceCode
        {
            private static string Format(string problem) => Code("item-usage-service", problem);

            public static string EquipEquipmentMissing = Format("equip-equipment-missing");
            public static string ExecuteEntityMissingTransform = Format("execute-entity-missing-transform");
            public static string EquipInvalidItem = Format("equip-invalid-item");
            public static string EquipSlotOccupied = Format("equip-slot-occupied");
            public static string UnequipInventoryFull = Format("unequip-inventory-full");
            public static string UnequipItemDefinitionNotFound = Format("unequip-item-definition-not-found");
            public static string UnequipTransactionFailed = Format("unequip-transaction-failed");
        }

        // World Service
        public static class RoomConnectionInstanceFactoryCode
        {
            private static string Format(string problem) => Code("room-connection-instance-factory", problem);

            public static string RehydrateDefinitionNotFound = Format("rehydrate-definition-not-found");
            public static string RehydrateInvalidInstanceData = Format("rehydrate-invalid-instance-data");
            public static string CreateDefinitionNotFound = Format("create-definition-not-found");
            public static string CreateInvalidInstanceData = Format("create-invalid-instance-data");
        }

        public static class RoomSpatialFactoryCode
        {
            private static string Format(string problem) => Code("room-spatial-factory", problem);

            public static string RehydrateDefinitionNotFound = Format("rehydrate-definition-not-found");
            public static string RehydrateRoomWithoutCells = Format("rehydrate-room-without-cells");
            public static string CreateDefinitionNotFound = Format("create-definition-not-found");
            public static string CreateRoomWithoutCells = Format("create-room-without-cells");
        }

        public static class SnapshotComponentFactoryCode
        {
            private static string Format(string problem) => Code("snapshot-component-factory", problem);

            public static string SnapshotTypeNotSupported = Format("snapshot-type-not-supported");
            public static string AIDefinitionNotFound = Format("ai-definition-not-found");
            public static string CollisionDefinitionNotFound = Format("collision-definition-not-found");
            public static string ProjectileDefinitionNotFound = Format("projectile-definition-not-found");
        }

        public static class DefinitionComponentFactoryCode
        {
            private static string Format(string problem) => Code("definition-component-factory", problem);

            public static string ComponentDefinitionNotSupported = Format("component-definition-not-supported");
        }

        public static class EntityInstanceFactoryCode
        {
            private static string Format(string problem) => Code("entity-instance-factory", problem);

            public static string InvalidContextType = Format("invalid-context-type");
            public static string EntityTypeNotSupported = Format("entity-type-not-supported");
            public static string EntityDefinitionNotFound = Format("entity-definition-not-found");
            public static string LifetimeDefinitionNotFound = Format("lifetime-definition-not-found");
            public static string ProjectileDefinitionNotFound = Format("projectile-definition-not-found");
            public static string TriggeredEffectDefinitionNotFound = Format("triggered-effect-definition-not-found");
            public static string CharacteristicDefinitionNotFound = Format("characteristic-definition-not-found");
            public static string InventoryDefinitionNotFound = Format("inventory-definition-not-found");
            public static string AppearanceDefinitionNotFound = Format("appearance-definition-not-found");
            public static string AIDefinitionNotFound = Format("ai-definition-not-found");
            public static string CollisionDefinitionNotFound = Format("collision-definition-not-found");
        }

        public static class EntitySpawnServiceCode
        {
            private static string Format(string problem) => Code("entity-spawn-service", problem);

            public static string ActivateTransformMissing = Format("activate-transform-missing");
            public static string TransitionTransformMissing = Format("transition-transform-missing");
            public static string SpawnEntityCreationFailed = Format("spawn-entity-creation-failed");
            public static string DespawnEntityMissing = Format("despawn-entity-missing");
        }

        public static class InitializationServiceCode
        {
            private static string Format(string problem) => Code("initialization-service", problem);

            public static string RoomDefinitionNotFound = Format("room-definition-not-found");
            public static string NoSpawnCellFound = Format("no-spawn-cell-found");
            public static string SpawnRuleMissing = Format("spawn-rule-missing");
        }

        public static class TopologyServiceCode
        {
            private static string Format(string problem) => Code("topology-service", problem);

            public static string EntityNotFound = Format("entity-not-found");
            public static string TransformComponentMissing = Format("transform-component-missing");
            public static string CurrentRoomNotFound = Format("current-room-not-found");
            public static string ConnectionDefinitionMissing = Format("connection-definition-missing");
            public static string DestinationEntityMissing = Format("destination-entity-missing");
        }
        #endregion

        #region Systems
        public static class ResidencyTickCode
        {
            private static string Format(string problem) => Code("residency-tick", problem);

            public static string RoomSnapshotPersistenceFailed = Format("room-snapshot-persistence-failed");
            public static string RoomSpatialNotFoundInRuntime = Format("room-spatial-not-found-in-runtime");
            public static string RoomSnapshotNotFoundInPersistence = Format("room-snapshot-not-found-in-persistence");
            public static string StateHeartbeatReport = Format("state-heartbeat-report");
        }
        #endregion
    }
}
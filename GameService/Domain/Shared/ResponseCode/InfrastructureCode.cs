namespace Domain.Shared.ResponseCode
{
    public class InfrastructureCode
    {
        private static string Code(string className, string problem) => CodeFormatter.Create("infrastructure", className, problem);

        #region Configuration
        public static class PersistenceConfigurationCode
        {
            private static string Format(string problem) => Code("persistence-configuration", problem);

            public static string SqlConnectionStringMissing = Format("sql-connection-string-missing");
            public static string MongoConnectionStringMissing = Format("mongo-connection-string-missing");
            public static string MongoDatabaseNameMissing = Format("mongo-database-name-missing");
        }

        public static class UtilityConfigurationCode
        {
            private static string Format(string problem) => Code("utility-configuration", problem);

            public static string JwtKeyMissing = Format("jwt-key-missing");
            public static string JwtIssuerMissing = Format("jwt-issuer-missing");
            public static string JwtAudienceMissing = Format("jwt-audience-missing");
            public static string SteamApiKeyMissing = Format("steam-api-key-missing");
            public static string SteamAppIdMissing = Format("steam-app-id-missing");
        }
        #endregion

        #region Cache
        public static class AICacheCode
        {
            private static string Format(string problem) => Code("ai-cache", problem);

            public static string DuplicateAIComponent = Format("duplicate-ai-component");
        }

        public static class AppearanceCacheCode
        {
            private static string Format(string problem) => Code("appearance-cache", problem);

            public static string DuplicateAppearanceComponent = Format("duplicate-appearance-component");
        }

        public static class CollisionCacheCode
        {
            private static string Format(string problem) => Code("collision-cache", problem);

            public static string DuplicateCollisionComponent = Format("duplicate-collision-component");
        }

        public static class CharacteristicCacheCode
        {
            private static string Format(string problem) => Code("characteristic-cache", problem);

            public static string DuplicateCharacteristicComponent = Format("duplicate-characteristic-component");
        }

        public static class EntityRelationshipCacheCode
        {
            private static string Format(string problem) => Code("entity-relationship-cache", problem);

            public static string DuplicateEntityRelationshipComponent = Format("duplicate-entity-relationship-component");
        }

        public static class InteractableCacheCode
        {
            private static string Format(string problem) => Code("interactable-cache", problem);

            public static string DuplicateInteractableComponent = Format("duplicate-interactable-component");
        }

        public static class InventoryCacheCode
        {
            private static string Format(string problem) => Code("inventory-cache", problem);

            public static string DuplicateInventoryComponent = Format("duplicate-inventory-component");
        }

        public static class LifetimeCacheCode
        {
            private static string Format(string problem) => Code("lifetime-cache", problem);

            public static string DuplicateLifetimeComponent = Format("duplicate-lifetime-component");
        }

        public static class ProjectileCacheCode
        {
            private static string Format(string problem) => Code("projectile-cache", problem);

            public static string DuplicateProjectileComponent = Format("duplicate-projectile-component");
        }

        public static class SpawnCacheCode
        {
            private static string Format(string problem) => Code("spawn-cache", problem);

            public static string DuplicateSpawnComponent = Format("duplicate-spawn-component");
        }

        public static class TriggeredEffectCacheCode
        {
            private static string Format(string problem) => Code("triggered-effect-cache", problem);

            public static string DuplicateTriggeredEffectComponent = Format("duplicate-triggered-effect-component");
        }

        public static class LocaleCacheCode
        {
            private static string Format(string problem) => Code("locale-cache", problem);

            public static string NoDefaultLocale = Format("no-default-locale");
            public static string FallbackToDefault = Format("fallback-locale-used");
            public static string FallbackToKey = Format("fallback-key-used");
        }

        public static class CacheProviderCode
        {
            private static string Format(string problem) => Code("cache-provider", problem);

            public static string LoadFailed = Format("load-all-failed");
            public static string LoadSuccess = Format("load-all-success");
        }
        #endregion

        #region Persistence
        #endregion

        #region Realtime
        public static class ConnectionManagerCode
        {
            private static string Format(string problem) => Code("connection-manager", problem);

            public static string ConnectionAdded = Format("connection-added");
            public static string ConnectionRemoved = Format("connection-removed");
            public static string WatchStatusOnline = Format("watch-status-online");
            public static string WatchStatusOffline = Format("watch-status-offline");
        }

        public static class SessionManagerCode
        {
            private static string Format(string problem) => Code("session-manager", problem);

            public static string SessionCreated = Format("session-created");
            public static string SessionOverwritten = Format("session-overwritten");
            public static string SessionRemoved = Format("session-removed");
        }

        public static class HubContextValidatorCode
        {
            private static string Format(string problem) => Code("hub-context-validator", problem);

            public static string UserIdNotFound = Format("user-id-not-found");
            public static string ConnectionIdNotFound = Format("connection-id-not-found");
        }
        #endregion

        #region Repository
        public static class RelationalUoWCode
        {
            private static string Format(string problem) => Code("relational-uow", problem);

            public static string NoTransaction = Format("no-transaction-created");
            public static string CommitFailed = Format("commit-failed");
            public static string SaveChangesFailed = Format("save-changes-failed");
        }
        #endregion

        #region Utility
        public static class SteamValidatorCode
        {
            private static string Format(string problem) => Code("steam-validator", problem);

            public static string ConnectionFailed = Format("connection-failed");
            public static string ConnectionError = Format("connection-error");
            public static string InvalidResponse = Format("invalid-response");
            public static string SteamRejected = Format("steam-rejected");
        }
        #endregion
    }
}
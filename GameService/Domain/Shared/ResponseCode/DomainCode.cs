namespace Domain.Shared.ResponseCode
{
    public class DomainCode
    {
        private static string Code(string className, string problem) => CodeFormatter.Create("domain", className, problem);

        #region Definitions
        public static class UserCode
        {
            private static string Format(string problem) => Code("user", problem);

            public static string InvalidId = Format("invalid-id");
            public static string InvalidName = Format("invalid-name");
            public static string MissingAuth = Format("missing-auth");
            public static string PasswordNotSet = Format("password-not-set");
            public static string InvalidCredentials = Format("invalid-credentials");
            public static string InvalidRefreshToken = Format("invalid-refresh-token");
            public static string ExpiredRefreshToken = Format("expired-refresh-token");
            public static string InvalidDob = Format("invalid-dob");
            public static string InvalidGender = Format("invalid-gender");
        }

        public static class PasswordCode
        {
            private static string Format(string problem) => Code("password", problem);

            public static string PasswordRequired = Format("password-required");
        }

        public static class CollisionDefinitionCode
        {
            private static string Format(string problem) => Code("collision-definition", problem);

            public static string WidthNegative = Format("width-negative");
            public static string HeightNegative = Format("height-negative");
            public static string RadiusNegative = Format("radius-negative");
            public static string BoxWidthMissing = Format("box-width-missing");
            public static string BoxHeightMissing = Format("box-height-missing");
            public static string CircleRadiusMissing = Format("circle-radius-missing");
            public static string UnsupportedShapeType = Format("unsupported-shape-type");
        }

        public static class LocaleCode
        {
            private static string Format(string problem) => Code("locale", problem);

            public static string CanNotDisableDefault = Format("can-not-disable-default");
        }
        #endregion

        #region Runtime
        public static class CharacteristicInstanceCode
        {
            private static string Format(string problem) => Code("characteristic-instance", problem);

            public static string NotAVitalAttribute = Format("not-a-vital-attribute");
            public static string NotACoreAttribute = Format("not-a-core-attribute");
        }

        public static class InventoryInstanceCode
        {
            private static string Format(string problem) => Code("inventory-instance", problem);

            public static string AddAmountInvalid = Format("add-amount-invalid");
            public static string RemoveAmountInvalid = Format("remove-amount-invalid");
            public static string InsufficientItemAmount = Format("insufficient-item-amount");
        }

        public static class SpatialIndexCode
        {
            private static string Format(string problem) => Code("spatial-index", problem);

            public static string RoomSpatialNotFoundOnEntityAdded = Format("room-spatial-not-found-on-entity-added");
            public static string RoomSpatialNotFoundOnEntityRemoved = Format("room-spatial-not-found-on-entity-removed");
            public static string RoomSpatialNotFoundOnEntityMoved = Format("room-spatial-not-found-on-entity-moved");
        }

        public static class ConnectionTopologyCode
        {
            private static string Format(string problem) => Code("connection-topology", problem);

            public static string DuplicateConnectionId = Format("duplicate-connection-id");
            public static string SourceEntityAlreadyBound = Format("source-entity-already-bound");
            public static string ConnectionNotFoundOnRemoved = Format("connection-not-found-on-removed");
        }

        public static class WorldCode
        {
            private static string Format(string problem) => Code("world", problem);

            public static string EntityInstanceNotFoundOnRemoved = Format("entity-instance-not-found-on-removed");
            public static string EntityInstanceNotFoundOnMoved = Format("entity-instance-not-found-on-moved");
            public static string EntityInstanceNotFoundOnRoomChanged = Format("entity-instance-not-found-on-room-changed");
        }
        #endregion

        #region Shared
        public static class CollisionShapeMapperCode
        {
            private static string Format(string problem) => Code("collision-shape-mapper", problem);

            public static string InvalidShapeType = Format("invalid-shape-type");
        }
        #endregion
    }
}
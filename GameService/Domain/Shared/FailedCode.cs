namespace Domain.Shared
{
    public static class FailedCode
    {
        // ─────────────────────────────
        // Common
        // ─────────────────────────────
        public const string Invalid_Id = "common.invalid_id";
        public const string NotFound = "common.not_found";
        public const string Unexpected = "common.unexpected";

        // ─────────────────────────────
        // User - Core
        // ─────────────────────────────
        public const string User_NotFound = "user.not_found";
        public const string User_InvalidPlayerId = "user.invalid_player_id";
        public const string User_InvalidName = "user.invalid_name";
        public const string User_MissingAuth = "user.missing_auth";

        // ─────────────────────────────
        // User - Profile
        // ─────────────────────────────
        public const string User_InvalidDob = "user.invalid_dob";
        public const string User_InvalidGender = "user.invalid_gender";

        // ─────────────────────────────
        // User - Auth
        // ─────────────────────────────
        public const string User_EmailRequired = "user.email_required";
        public const string User_PasswordRequired = "user.password_required";
        public const string User_EmailAlreadyExists = "user.email_already_exists";

        public const string User_InvalidEmail = "user.invalid_email";
        public const string User_InvalidPassword = "user.invalid_password";
        public const string User_PasswordNotSet = "user.password_not_set";
        public const string User_InvalidCredentials = "user.invalid_credentials";

        // ─────────────────────────────
        // User - Token
        // ─────────────────────────────
        public const string User_InvalidRefreshToken = "user.invalid_refresh_token";
        public const string User_ExpiredRefreshToken = "user.expired_refresh_token";

        // ─────────────────────────────
        // Steam
        // ─────────────────────────────
        public const string Steam_InvalidTicket = "steam.invalid_ticket";
        public const string Steam_ValidationFailed = "steam.validation_failed";
    }
}
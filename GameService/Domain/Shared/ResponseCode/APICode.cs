namespace Domain.Shared.ResponseCode
{
    public class APICode
    {
        private static string Code(string className, string problem) => CodeFormatter.Create("api", className, problem);

        #region Helper
        public static class ClaimReaderCode
        {
            private static string Format(string problem) => Code("claim-reader", problem);

            public static string ClaimMissingUserId = Format("claim-missing-user-id");
        }
        #endregion
    }
}
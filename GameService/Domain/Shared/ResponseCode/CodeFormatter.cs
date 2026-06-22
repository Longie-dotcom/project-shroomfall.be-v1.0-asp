namespace Domain.Shared.ResponseCode
{
    public static class CodeFormatter
    {
        public static string Create(string layer, string className, string problem) => $"{layer}.{className.ToLower()}.{problem.ToLower()}";
    }
}
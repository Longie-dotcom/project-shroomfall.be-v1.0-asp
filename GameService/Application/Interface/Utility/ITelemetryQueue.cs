namespace Application.Interface.Utility
{
    public enum TelemetrySeverity
    {
        Debug,      // Internal diagnostic information
        Info,       // Healthy events (e.g., "cache_reloaded_success")
        Warning,    // Recoverable bugs handled optimistically
        Error,      // Client/Domain errors (e.g., BadRequest, NotFound)
        Fatal       // Server crashes that halt execution (InternalException)
    }

    public record TelemetryEvent(
        string Code, 
        string Message,
        DateTime Timestamp,
        TelemetrySeverity Severity);

    public interface ITelemetryQueue
    {
        void EnqueueAlert(
            string code,
            string message, 
            TelemetrySeverity severity);
        bool TryDequeue(
            out TelemetryEvent? alertEvent);
    }
}

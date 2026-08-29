using Application.Interface.Utility;

namespace Infrastructure.Utility
{
    public class TelemetryQueue : ITelemetryQueue
    {
        #region Attributes
        private readonly Queue<TelemetryEvent> queue = new();
        #endregion

        #region Properties
        #endregion

        public TelemetryQueue() { }

        #region Methods
        public void EnqueueAlert(
            string code, 
            string message,
            TelemetrySeverity severity)
        {
            var alertEvent = new TelemetryEvent(code, message, DateTime.UtcNow, severity);
            queue.Enqueue(alertEvent);

            // Note: Or push to RabbitMQ/Redis here
        }

        public bool TryDequeue(
            out TelemetryEvent? alertEvent)
        {
            return queue.TryDequeue(out alertEvent);
        }
        #endregion
    }
}
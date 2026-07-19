using Application.Interfaces.Realtime;
using Application.Interfaces.Utility;
using Contract.DTO.Feature.Admin.Response;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Background
{
    public class TelemetryPublishService : BackgroundService
    {
        #region Attributes
        private readonly ITelemetryQueue telemetryQueue;
        private readonly IRealtimePublisher realtimePublisher;
        private readonly ILogger<TelemetryPublishService> logger;
        #endregion

        #region Properties
        #endregion

        public TelemetryPublishService(
            ITelemetryQueue telemetryQueue,
            IRealtimePublisher realtimePublisher,
            ILogger<TelemetryPublishService> logger)
        {
            this.telemetryQueue = telemetryQueue;
            this.realtimePublisher = realtimePublisher;
            this.logger = logger;
        }

        #region Methods
        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    while (telemetryQueue.TryDequeue(out var alertEvent) && alertEvent != null)
                    {
                        await realtimePublisher.SendTelemetryAlert(
                            new TelemetryEventDTO()
                            {
                                Code = alertEvent.Code,
                                Message = alertEvent.Message,
                                Timestamp = alertEvent.Timestamp,
                                Severity = alertEvent.Severity.ToString(),
                            });
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred while publishing telemetry messages to the dashboard.");
                }
            }
        }
        #endregion
    }
}
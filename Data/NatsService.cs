using NLog;

namespace nats_ui.Data
{
    public class NatsService
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
     
        public void Create(NatsSessionModel natsSession)
        {
            Logger.Info($"Create Session: {natsSession.Url}");
        }
    }
}
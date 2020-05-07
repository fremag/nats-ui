using Microsoft.AspNetCore.Components;
using nats_ui.Data;
using NLog;

namespace nats_ui.Pages
{
    public class NatsSessionComponent : ComponentBase
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        [Inject]
        private NatsService NatsService { get; set; }

        protected NatsSessionModel NatsSession { get; } = new NatsSessionModel();
        
        protected void CreateSession()
        {
            Logger.Info($"CreateSession: {NatsSession}");
            NatsService.Create(NatsSession);
        }
    }
}
using System;
using System.Collections.Generic;
using NLog;

namespace nats_ui.Data
{
    public class NatsService
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
     
        public NatsConfiguration Configuration { get; } = new NatsConfiguration();
        
        public event Action SessionCreated;
        
        public void Create(NatsSessionModel natsSession)
        {
            Logger.Info($"Create Session: {natsSession.Url}");
            Configuration.Sessions.Add(natsSession);
            SessionCreated?.Invoke();
        }
    }

    public class NatsConfiguration
    {
        public List<NatsSessionModel> Sessions { get; } = new List<NatsSessionModel>();
    }
}
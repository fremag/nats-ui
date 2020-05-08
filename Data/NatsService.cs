using System;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace nats_ui.Data
{
    public class NatsService
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
     
        public NatsConfiguration Configuration { get; } = new NatsConfiguration();
        
        public event Action<NatsSessionModel> SessionCreated;
        public event Action<NatsSessionModel> SessionDeleted;
        
        public void Create(NatsSessionModel natsSession)
        {
            Logger.Info($"Create Session: {natsSession.Url}");
            Configuration.Add(natsSession);
            SessionCreated?.Invoke(natsSession);
        }
        
        public void Remove(NatsSessionModel natsSession)
        {
            Logger.Info($"Create Session: {natsSession.Url}");
            Configuration.Remove(natsSession.Name);
            SessionDeleted?.Invoke(natsSession);
        }
    }

    public class NatsConfiguration
    {
        public List<NatsSessionModel> Sessions { get; set; } = new List<NatsSessionModel>();

        public NatsSessionModel GetByName(string name)
        {
            return Sessions.FirstOrDefault(model => model.Name == name);
        }
        
        public void Add(NatsSessionModel natsSession)
        {
            Remove(natsSession.Name);
            Sessions.Add(natsSession);
        }

        public void Remove(string name)
        {
            var session = GetByName(name);
            if (session != null)
            {
                Sessions.Remove(session);
            }
        }
    }
}
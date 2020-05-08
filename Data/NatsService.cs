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
        
        public event Action<NatsConnectionModel> ConnectionCreated;
        public event Action<NatsConnectionModel> ConnectionDeleted;
        
        public void Create(NatsConnectionModel natsConnection)
        {
            Logger.Info($"Create Connection: {natsConnection.Url}");
            Configuration.Add(natsConnection);
            ConnectionCreated?.Invoke(natsConnection);
        }
        
        public void Remove(NatsConnectionModel natsConnection)
        {
            Logger.Info($"Remove Connection: {natsConnection.Url}");
            Configuration.Remove(natsConnection.Name);
            ConnectionDeleted?.Invoke(natsConnection);
        }
    }

    public class NatsConfiguration
    {
        public List<NatsConnectionModel> Connections { get; set; } = new List<NatsConnectionModel>();

        public NatsConnectionModel GetByName(string name)
        {
            return Connections.FirstOrDefault(model => model.Name == name);
        }
        
        public void Add(NatsConnectionModel natsConnection)
        {
            Remove(natsConnection.Name);
            Connections.Add(natsConnection);
        }

        public void Remove(string name)
        {
            var connectionModel = GetByName(name);
            if (connectionModel != null)
            {
                Connections.Remove(connectionModel);
            }
        }
    }
}
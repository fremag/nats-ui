using System;
using System.Collections.Generic;
using System.Linq;
using NATS.Client;
using NLog;

namespace nats_ui.Data
{
    public class NatsService
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
     
        public NatsConfiguration Configuration { get; } = new NatsConfiguration();
        public Dictionary<string, IConnection> Connections { get; } = new Dictionary<string, IConnection>();
        
        public event Action<Connection> ConnectionCreated;
        public event Action<Connection> ConnectionRemoved;

        private ConnectionFactory Factory { get; } = new ConnectionFactory();

        public string Create(Connection connection)
        {
            Logger.Info($"Create Connection: {connection.Url}");
            if (Configuration.Connections.Any(model => model.Name == connection.Name))
            {
                var msg = $"A connection already exists with name: {connection.Name}";
                Logger.Warn(msg);
                return msg;
            }
            Configuration.Add(connection);
            ConnectionCreated?.Invoke(connection);
            return $"Connection created: {connection}";
        }
        
        public void Remove(Connection connection)
        {
            Logger.Info($"Remove Connection: {connection.Url}");
            Configuration.Remove(connection.Name);
            ConnectionRemoved?.Invoke(connection);
        }

        public void Connect(Connection connection)
        {
            if (Connections.ContainsKey(connection.Url))
            {
                return;
            }

            Logger.Info($"Connect: {connection.Url}");
            var conn = Factory.CreateConnection(connection.Url);
            Connections[connection.Url] = conn;
        }

        public void Disconnect(Connection connection)
        {
            Logger.Info($"Diconnect: {connection.Url}");
            if (Connections.TryGetValue(connection.Url, out var conn))
            {
                conn.Close();
            }
        }
    }

    public class NatsConfiguration
    {
        public List<Connection> Connections { get; set; } = new List<Connection>();

        public Connection GetByName(string name)
        {
            return Connections.FirstOrDefault(model => model.Name == name);
        }
        
        public void Add(Connection connection)
        {
            Remove(connection.Name);
            Connections.Add(connection);
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
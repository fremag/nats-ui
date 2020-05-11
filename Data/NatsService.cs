using System;
using System.Collections.Generic;
using NATS.Client;
using NLog;

namespace nats_ui.Data
{
    public class NatsService
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
     
        public NatsConfiguration Configuration { get; } = new NatsConfiguration();
        private Dictionary<string, IConnection> ConnectionsByName { get; } = new Dictionary<string, IConnection>();
        private Dictionary<NatsSubject, List<IAsyncSubscription>> SubscriptionsBySubject { get; } = new Dictionary<NatsSubject, List<IAsyncSubscription>>();
        
        public event Action<Connection> ConnectionCreated;
        public event Action<Connection> ConnectionRemoved;
        public event Action<NatsSubject> SubjectCreated;
        public event Action<NatsSubject> SubjectRemoved;

        private ConnectionFactory Factory { get; } = new ConnectionFactory();

        public string Create(Connection connection)
        {
            Logger.Info($"Create Connection: {connection.Url}");
            if (Configuration.GetConnection(connection.Name) != null)
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
            Configuration.RemoveConnection(connection.Name);
            ConnectionRemoved?.Invoke(connection);
        }

        public string Create(NatsSubject natsSubject)
        {
            Logger.Info($"Create NatsSubject: {natsSubject.Subject}");
            if (Configuration.GetSubject(natsSubject.Subject) != null)
            {
                var msg = $"A subject already exists with subject: {natsSubject.Subject}";
                Logger.Warn(msg);
                return msg;
            }
            Configuration.Add(natsSubject);
            SubjectCreated?.Invoke(natsSubject);
            return $"NatsSubject created: {natsSubject}";
        }
        
        public void Remove(NatsSubject natsSubject)
        {
            Logger.Info($"Remove NatsSubject: {natsSubject.Subject}");
            Configuration.RemoveSubject(natsSubject.Subject);
            SubjectRemoved?.Invoke(natsSubject);
        }

        public void Connect(Connection connection)
        {
            if (ConnectionsByName.ContainsKey(connection.Url))
            {
                return;
            }

            Logger.Info($"Connect: {connection.Url}");
            var conn = Factory.CreateConnection(connection.Url);
            ConnectionsByName[connection.Url] = conn;
        }

        public void Disconnect(Connection connection)
        {
            Logger.Info($"Diconnect: {connection.Url}");
            if (ConnectionsByName.TryGetValue(connection.Url, out var conn))
            {
                conn.Close();
            }
        }

        public void Subscribe(NatsSubject natsSubject)
        {
            List<IAsyncSubscription> subs = new List<IAsyncSubscription>();
            foreach (var conn in ConnectionsByName.Values)
            {
                IAsyncSubscription sub = conn.SubscribeAsync(natsSubject.Subject, OnMessage);
                subs.Add(sub);
            }

            SubscriptionsBySubject[natsSubject] = subs;
            natsSubject.Subscribed = true;
        }

        private void OnMessage(object sender, MsgHandlerEventArgs e)
        {
            
        }

        public void Unsubscribe(NatsSubject natsSubject)
        {
            if (SubscriptionsBySubject.TryGetValue(natsSubject, out var subs))
            {
                foreach (var subscription in subs)
                {
                    subscription.Unsubscribe();
                }

                SubscriptionsBySubject.Remove(natsSubject);
                natsSubject.Subscribed = false;
            }
        }
    }
}
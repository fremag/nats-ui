using System;
using System.Collections.Generic;
using System.Text;
using NATS.Client;
using NLog;

namespace nats_ui.Data
{
    public class NatsService
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
     
        public NatsConfiguration Configuration { get; } = new NatsConfiguration();
        private Dictionary<string, IConnection> ConnectionsByName { get; } = new Dictionary<string, IConnection>();
        private Dictionary<NatsSubscription, List<IAsyncSubscription>> Subscriptions { get; } = new Dictionary<NatsSubscription, List<IAsyncSubscription>>();
        
        public event Action<NatsMessage> MessageReceived;

        private ConnectionFactory Factory { get; } = new ConnectionFactory();
        public List<NatsMessage> Messages { get; } = new List<NatsMessage>();

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
            return $"Connection created: {connection}";
        }
        
        public void Remove(Connection connection)
        {
            Logger.Info($"Remove Connection: {connection.Url}");
            Configuration.RemoveConnection(connection.Name);
        }

        public bool Create(NatsSubscription natsSubscription, out string msg)
        {
            Logger.Info($"Create NatsSubject: {natsSubscription.Subject}");
            if (Configuration.GetSubject(natsSubscription.Subject) != null)
            {
                msg = $"A subject already exists with subject: {natsSubscription.Subject}";
                Logger.Warn(msg);
                return false;
            }
            Configuration.Add(natsSubscription);
            msg = $"NatsSubject created: {natsSubscription}";
            return true;
        }
        
        public void Remove(NatsSubscription natsSubscription)
        {
            Logger.Info($"Remove NatsSubject: {natsSubscription.Subject}");
            Configuration.RemoveSubject(natsSubscription.Subject);
        }

        public void Connect(Connection connection)
        {
            if (ConnectionsByName.ContainsKey(connection.Url))
            {
                return;
            }

            Logger.Info($"Connect: {connection.Url}");
            var conn = Factory.CreateConnection(connection.Url);
            connection.Status = ConnectionStatus.Connected;
            ConnectionsByName[connection.Url] = conn;
        }

        public void Disconnect(Connection connection)
        {
            Logger.Info($"Diconnect: {connection.Url}");
            if (ConnectionsByName.TryGetValue(connection.Url, out var conn))
            {
                conn.Close();
                connection.Status = ConnectionStatus.Disconnected;
            }
        }

        public void Subscribe(NatsSubscription natsSubscription)
        {
            List<IAsyncSubscription> subs = new List<IAsyncSubscription>();
            foreach (var conn in ConnectionsByName.Values)
            {
                IAsyncSubscription sub = conn.SubscribeAsync(natsSubscription.Subject, OnMessage);
                subs.Add(sub);
            }

            Subscriptions[natsSubscription] = subs;
            natsSubscription.Subscribed = true;
        }

        private void OnMessage(object sender, MsgHandlerEventArgs e)
        {
            var url = e.Message.ArrivalSubscription.Connection.ConnectedUrl;
            NatsMessage msg = new NatsMessage
            {
                TimeStamp = DateTime.Now,
                Subject = e.Message.Subject,
                Data = Encoding.Default.GetString(e.Message.Data),
                Url = url
            };
            Messages.Add(msg);
            MessageReceived?.Invoke(msg);
        }

        public void Unsubscribe(NatsSubscription natsSubscription)
        {
            if (Subscriptions.TryGetValue(natsSubscription, out var subs))
            {
                foreach (var subscription in subs)
                {
                    subscription.Unsubscribe();
                }

                Subscriptions.Remove(natsSubscription);
                natsSubscription.Subscribed = false;
            }
        }
    }
}
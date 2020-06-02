using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NATS.Client;
using NLog;

namespace nats_ui.Data
{
    public class NatsService
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        private const string NatsXml = "nats.xml";
     
        public NatsConfiguration Configuration { get; } = new NatsConfiguration();
        private Dictionary<Connection, IConnection> ConnectionsByName { get; } = new Dictionary<Connection, IConnection>();
        private Dictionary<NatsSubscription, List<IAsyncSubscription>> Subscriptions { get; } = new Dictionary<NatsSubscription, List<IAsyncSubscription>>();
        
        public event Action<NatsMessage> MessageReceived;
        public event Action<NatsMessage> MessageSaved;
        public event Action<NatsMessage> MessageSent;
        public event Action<Connection> Connected;
        public event Action<Connection> Disconnected;
        public event Action<NatsSubscription> Subscribed;
        public event Action<NatsSubscription> Unsubscribed;

        private ConnectionFactory Factory { get; } = new ConnectionFactory();
        public List<NatsMessage> ReceivedMessages { get; } = new List<NatsMessage>();
        public List<Connection> Connections => ConnectionsByName.Keys.Select(c => c.Clone()).ToList();

        public NatsService()
        {
            Load();
        }

        public bool Create(Connection connection, out string msg)
        {
            Logger.Info($"Create Connection: {connection.Url}");
            if (Configuration.GetConnection(connection.Name) != null)
            {
                msg = $"A connection already exists with name: {connection.Name}";
                Logger.Warn(msg);
                return false;
            }
            Configuration.Add(connection);
            msg =  $"Connection created: {connection}";
            return true;
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

        public IConnection Connect(Connection connection)
        {
            if (ConnectionsByName.TryGetValue(connection, out var conn))
            {
                return conn;
            }

            Logger.Info($"Connect: {connection.Url}");
            conn = Factory.CreateConnection(connection.Url);
            connection.Status = ConnectionStatus.Connected;
            ConnectionsByName[connection] = conn;
            Connected?.Invoke(connection);
            return conn;
        }

        public void Disconnect(Connection connection)
        {
            Logger.Info($"Diconnect: {connection.Url}");
            if (ConnectionsByName.TryGetValue(connection, out var conn))
            {
                conn.Close();
                connection.Status = ConnectionStatus.Disconnected;
                Disconnected?.Invoke(connection);
            }
        }

        public void Subscribe(NatsSubscription natsSubscription)
        {
            if (Subscriptions.ContainsKey(natsSubscription))
            {
                return;
            }
            
            Logger.Info($"{nameof(Subscribe)}: {natsSubscription.Subject}");
            List<IAsyncSubscription> subs = new List<IAsyncSubscription>();
            foreach (var conn in ConnectionsByName.Values)
            {
                IAsyncSubscription sub = conn.SubscribeAsync(natsSubscription.Subject, OnMessage);
                subs.Add(sub);
            }

            Subscriptions[natsSubscription] = subs;
            natsSubscription.Subscribed = true;
            Subscribed?.Invoke(natsSubscription);
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
            ReceivedMessages.Add(msg);
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
                Unsubscribed?.Invoke(natsSubscription);
            }
        }
        
        public bool Create(Session session, out string msg)
        {
            Configuration.RemoveSession(session.Name);

            session.Subscriptions.AddRange(Subscriptions.Keys);
            session.Connections.AddRange(ConnectionsByName.Keys);

            Configuration.Add(session);
            
            msg = $"Session created: {session.Name}";
            return true;
        }

        public void Remove(Session session)
        {
            Logger.Info($"Remove Session: {session.Name}");
            Configuration.RemoveSession(session.Name);
        }

        public void Init(Session session)
        {
            Logger.Info($"{nameof(Init)}: {session.Name}");
            Close();

            foreach (var connection in session.Connections)
            {
                Configuration.Add(connection);
                Connect(connection);
            }
            foreach (var subscription in session.Subscriptions)
            {
                Configuration.Add(subscription);
                Subscribe(subscription);
            }
        }

        public void Close()
        {
            foreach (List<IAsyncSubscription> subscriptions in Subscriptions.Values)
            {
                foreach (var subscription in subscriptions)
                {
                    subscription.Unsubscribe();
                }
            }

            foreach (var connection in ConnectionsByName.Values)
            {
                connection.Close();
            }

            Subscriptions.Clear();
            ConnectionsByName.Clear();
        }

        public void Save()
        {
            Configuration.Save(NatsXml);
        }
        
        public void Load()
        {
            Close();
            Configuration.Load(NatsXml);
        }

        public void Save(NatsMessage message)
        {
            Configuration.SavedMessages.Add(message);
            MessageSaved?.Invoke(message);
        }

        public void Send(NatsMessage message)
        {
            Logger.Info($"{nameof(Send)}: {message.Subject}, {message.Url}");
            var conn = ConnectionsByName.Values.FirstOrDefault(c => c.ConnectedUrl == message.Url);
            if (conn != null)
            {
                conn.Publish(message.Subject, Encoding.Default.GetBytes(message.Data));
                MessageSent?.Invoke(message);
            }
            else
            {
                Logger.Error($"Can't find connection with Url: {message.Url} !");
            }
        }
    }
}
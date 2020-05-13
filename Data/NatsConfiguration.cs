using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace nats_ui.Data
{
    public class NatsConfiguration
    {
        private static readonly XmlSerializer Xml = new XmlSerializer(typeof(NatsConfiguration)); 

        public List<Connection> Connections { get; } = new List<Connection>();
        public List<NatsSubscription> Subscriptions { get; } = new List<NatsSubscription>();
        public List<Session> Sessions { get; } = new List<Session>();

        public Connection GetConnection(string name) => Connections.FirstOrDefault(connection => connection.Name == name);
        public NatsSubscription GetSubject(string subject) => Subscriptions.FirstOrDefault(subscription => subscription.Subject == subject);
        public Session GetSession(string name) => Sessions.FirstOrDefault(session => session.Name == name);

        public void Add(Connection connection)
        {
            RemoveConnection(connection.Name);
            Connections.Add(connection);
        }

        public void Add(NatsSubscription subscription)
        {
            RemoveSubject(subscription.Subject);
            Subscriptions.Add(subscription);
        }

        public void Add(Session session)
        {
            RemoveSession(session.Name);
            Sessions.Add(session);
        }

        public void RemoveConnection(string name)
        {
            var connectionModel = GetConnection(name);
            if (connectionModel != null)
            {
                Connections.Remove(connectionModel);
            }
        }
        
        public void RemoveSubject(string subject)
        {
            var sub = GetSubject(subject);
            if (sub != null)
            {
                Subscriptions.Remove(sub);
            }
        }

        public void RemoveSession(string name)
        {
            var session = GetSession(name);
            if (session != null)
            {
                Sessions.Remove(session);
            }
        }

        public void Save(string xmlFile)
        {
            Xml.Serialize(File.Open(xmlFile, FileMode.Create), this);
        }
        
        public void Load(string xmlFile)
        {
            if (!File.Exists(xmlFile))
            {
                return;
            }
            var config = (NatsConfiguration)Xml.Deserialize(File.Open(xmlFile, FileMode.Create));
            Sessions.Clear();
            Connections.Clear();
            Subscriptions.Clear();           
            Sessions.AddRange(config.Sessions);
            Connections.AddRange(config.Connections);
            Subscriptions.AddRange(config.Subscriptions);
        }
    }
}
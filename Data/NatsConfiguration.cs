using System.Collections.Generic;
using System.Linq;

namespace nats_ui.Data
{
    public class NatsConfiguration
    {
        public List<Connection> Connections { get; } = new List<Connection>();
        public List<NatsSubscription> Subjects { get; } = new List<NatsSubscription>();
        public List<Session> Sessions { get; } = new List<Session>();

        public Connection GetConnection(string name) => Connections.FirstOrDefault(connection => connection.Name == name);
        public NatsSubscription GetSubject(string subject) => Subjects.FirstOrDefault(subscription => subscription.Subject == subject);
        public Session GetSession(string name) => Sessions.FirstOrDefault(session => session.Name == name);

        public void Add(Connection connection)
        {
            RemoveConnection(connection.Name);
            Connections.Add(connection);
        }

        public void Add(NatsSubscription subscription)
        {
            RemoveSubject(subscription.Subject);
            Subjects.Add(subscription);
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
                Subjects.Remove(sub);
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
    }
}
using System.Collections.Generic;
using System.Linq;

namespace nats_ui.Data
{
    public class NatsConfiguration
    {
        public List<Connection> Connections { get; } = new List<Connection>();
        public List<NatsSubscription> Subjects { get; } = new List<NatsSubscription>();

        public Connection GetConnection(string name) => Connections.FirstOrDefault(model => model.Name == name);
        public NatsSubscription GetSubject(string subject) => Subjects.FirstOrDefault(model => model.Subject == subject);

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
    }
}
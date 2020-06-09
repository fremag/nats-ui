using System.Collections.Generic;

namespace nats_ui.Data
{
    public class Session : ICheckable
    {
        public bool Checked { get; set; }
        public string Name { get; set; }
        public int ConnectionCount => Connections.Count;
        public int SubscriptionCount => Subscriptions.Count;
        public string Trash => "oi oi-trash";
        public string Run => "oi oi-media-play";
        
        public List<Connection> Connections { get; set; } = new List<Connection>();
        public List<NatsSubscription> Subscriptions { get; set; } = new List<NatsSubscription>();
        
        
        public Session(string name)
        {
            Name = name;
        }
    }
}
using System;
using System.Xml.Serialization;

namespace nats_ui.Data
{
    public enum ConnectionStatus
    {
        Connected,
        Disconnected
    }

    public class Connection : ICheckable
    {
        public string Url => $"nats://{Host}:{Port}";

        [XmlIgnore]
        public bool Checked { get; set; }
        [XmlIgnore]
        public ConnectionStatus Status { get; set; } = ConnectionStatus.Disconnected;
        public string Name { get; }
        public string Host { get; }
        public int Port { get; }

        public Connection(string name, string host, int port)
        {
            Name = name;
            Host = host;
            Port = port;
        }

        public override string ToString()
        {
            return $"{Name}, {Host}, {Port}, {Checked}, {Status}";
        }

        protected bool Equals(Connection other)
        {
            return Name == other.Name && Host == other.Host && Port == other.Port;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Connection) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Host, Port);
        }
    }
}
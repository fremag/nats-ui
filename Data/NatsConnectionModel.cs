using System;
using System.ComponentModel.DataAnnotations;

namespace nats_ui.Data
{
    public class NatsConnectionModel
    {
        public string Url => $"nats://{Host}:{Port}";

        public bool Selected { get; set; } = false;
        
        [Required]
        public string Name { get; set; } = "localhost";
        
        [Required]
        public string Host { get; set; } = "127.0.0.1";

        [Required]
        public int Port { get; set; } = 4222;

        public override string ToString()
        {
            return $"{Name}, {Host}, {Port}, {Selected}";
        }

        public NatsConnectionModel Clone()
        {
            return (NatsConnectionModel)MemberwiseClone();
        }

        protected bool Equals(NatsConnectionModel other)
        {
            return Name == other.Name && Host == other.Host && Port == other.Port;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NatsConnectionModel) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Host, Port);
        }
    }
}
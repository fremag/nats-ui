using System.Xml.Serialization;

namespace nats_ui.Data
{
    public class NatsSubscription : ICheckable
    {
        [XmlIgnore]
        public bool Checked { get; set; }
        [XmlIgnore]
        public bool Subscribed { get; set; }
        public string Subject { get; }

        public string StatusImg => Subscribed  ? "oi oi-check" : string.Empty;
        public string Trash => "oi oi-trash";
        public string Subscribe => "oi oi-media-play";
        public string Unsubscribe => "oi oi-media-stop";
        
        public NatsSubscription(string subject)
        {
            Subject = subject;
        }

        protected bool Equals(NatsSubscription other)
        {
            return Subject == other.Subject;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NatsSubscription) obj);
        }

        public override int GetHashCode()
        {
            return (Subject != null ? Subject.GetHashCode() : 0);
        }
    }
}
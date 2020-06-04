using System;
using System.Xml.Serialization;

namespace nats_ui.Data
{
    public enum MessageType {Received, Sent, Request, Reply}
    public class NatsMessage : ICheckable
    {
        [XmlIgnore]
        public bool Checked { get; set; }
        [XmlIgnore]
        public DateTime TimeStamp { get; set; }
        [XmlIgnore]
        public MessageType MessageType { get; set; } 
            
        public string Subject { get; set; }
        public string Data { get; set; }
        public string Url { get; set; }

        public NatsMessage Clone()
        {
            return (NatsMessage)MemberwiseClone(); 
        }
    }
}
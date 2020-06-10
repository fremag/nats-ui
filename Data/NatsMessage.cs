using System;
using System.Xml.Serialization;

namespace nats_ui.Data
{
    public enum MessageType {Received, Publish, Request, Reply}
    public class NatsMessage : ICheckable
    {
        [XmlIgnore]
        public bool Checked { get; set; }
        [XmlIgnore]
        public DateTime TimeStamp { get; set; }
        [XmlIgnore]
        public MessageType MessageType { get; set; } 

        [XmlIgnore]
        public string Type  
        {
            get
            {
                switch (MessageType)
                {
                    case MessageType.Received:
                        return "oi oi-data-transfer-download";
                    case MessageType.Publish:
                        return "oi oi-data-transfer-upload";
                    case MessageType.Request:
                        return "oi oi-question-mark";
                    case MessageType.Reply:
                        return "oi oi-target";
                    default:
                        throw new ArgumentOutOfRangeException(MessageType.ToString());
                }
            }
        }    
        
        public string Subject { get; set; }
        public string Data { get; set; }
        public string Url { get; set; }

        [XmlIgnore]
        public string Inspect => "oi oi-zoom-in";
        
        [XmlIgnore]
        public string Pin => "oi oi-pin";

        public NatsMessage Clone()
        {
            return (NatsMessage)MemberwiseClone(); 
        }
    }
}
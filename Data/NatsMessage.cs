using System;

namespace nats_ui.Data
{
    public class NatsMessage : ISelectable
    {
        public bool Selected { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Subject { get; set; }
        public string Data { get; set; }
        public string Url { get; set; }
    }
}
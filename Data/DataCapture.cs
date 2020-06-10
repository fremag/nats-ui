using System.Xml.Serialization;

namespace nats_ui.Data
{
    public enum CaptureType
    {
        JsonPath,
        Regex
    }
    
    public class DataCapture : ICheckable
    {
        public CaptureType Type { get; set; }
        public string Name { get; set; }
        public string Expression { get; set; }
        [XmlIgnore]
        public bool Checked { get; set; }
        [XmlIgnore]
        public string Trash => "oi oi-trash";
        [XmlIgnore]
        public string Run => "oi oi-media-play";
        
    }
}
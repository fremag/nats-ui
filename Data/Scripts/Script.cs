using System.Collections.Generic;
using System.Xml.Serialization;

namespace nats_ui.Data.Scripts
{
    public class Script : ICheckable
    {
        public string Name { get; set; }
        public List<IScriptCommand> Commands { get; set; } = new List<IScriptCommand>();

        [XmlIgnore]
        public bool Checked { get; set; }

        [XmlIgnore]
        public string File { get; set; }

        [XmlIgnore]
        public int Count => Commands.Count;

        public void Run(NatsService natsService)
        {
        }
    }
}
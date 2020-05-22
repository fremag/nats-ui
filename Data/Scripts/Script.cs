using System;
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

        public void Swap(int indexA, int indexB)
        {
            if (indexA < 0 || indexB < 0 || indexA >= Count || indexB >= Count)
            {
                return;
            }
            
            var itemA = Commands[indexA];
            var itemB = Commands[indexB];
            Commands[indexA]= itemB;
            Commands[indexB] = itemA;
        }

        public void Remove(int index)
        {
            Commands.RemoveAt(index);
        }
    }
}
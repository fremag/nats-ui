using System.Collections.Generic;
using System.Xml.Serialization;

namespace nats_ui.Data.Scripts
{
    public class Script : ICheckable
    {
        public string Name { get; set; }
        public List<ScriptStatement> Commands { get; set; } = new List<ScriptStatement>();

        [XmlIgnore]
        public bool Checked { get; set; }

        [XmlIgnore]
        public string File { get; set; }

        [XmlIgnore]
        public int Count => Commands.Count;

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

        public void Insert(int index, ScriptStatement statement)
        {
            if (index >= Commands.Count)
            {
                Commands.Add(statement);
            }
            else
            {
                Commands.Insert(index, statement);
            }
        }
    }
}
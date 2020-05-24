using System.Collections.Generic;
using System.Xml.Serialization;

namespace nats_ui.Data.Scripts
{
    public class Script : ICheckable
    {
        public string Name { get; set; }
        public List<ScriptStatement> Statements { get; set; } = new List<ScriptStatement>();

        [XmlIgnore]
        public bool Checked { get; set; }

        [XmlIgnore]
        public string File { get; set; }

        [XmlIgnore]
        public int Count => Statements.Count;

        public void Swap(int indexA, int indexB)
        {
            if (indexA < 0 || indexB < 0 || indexA >= Count || indexB >= Count)
            {
                return;
            }
            
            var itemA = Statements[indexA];
            var itemB = Statements[indexB];
            Statements[indexA]= itemB;
            Statements[indexB] = itemA;
        }

        public void Remove(int index)
        {
            Statements.RemoveAt(index);
        }

        public void Insert(int index, ScriptStatement statement)
        {
            if (index >= Statements.Count)
            {
                Statements.Add(statement);
            }
            else
            {
                Statements.Insert(index, statement);
            }
        }
    }
}
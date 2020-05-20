using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using XSerializer;

namespace nats_ui.Data.Scripts
{
    public class Script : ICheckable
    {
        public string Name { get; set; }
        public List<AbstractScriptCommand> Commands { get; set; } = new List<AbstractScriptCommand>();

        [XmlIgnore]
        public bool Checked { get; set; }

        [XmlIgnore]
        public int Count => Commands.Count;

        public void Run(NatsService natsService)
        {
        }
    }

    public class ScriptService
    {
        private static XmlSerializer<Script> Serializer { get; } = new XmlSerializer<Script>();

        public const string DefaultScriptDirectory = "Scripts";
        public string ScriptDirectory { get; }
        public List<Script> Scripts { get; } = new List<Script>();

        public ScriptService(string scriptDirectory = DefaultScriptDirectory)
        {
            ScriptDirectory = scriptDirectory;
            Load(ScriptDirectory);
        }

        public void Load(string directory)
        {
            var scriptFiles = Directory.EnumerateFiles(directory);
            foreach (var scriptFile in scriptFiles)
            {
                var script = LoadScript(scriptFile);
                Scripts.Add(script);
            }
        }

        public Script LoadScript(string xmlFile)
        {
            if (!File.Exists(xmlFile))
            {
                return null;
            }

            var xml = File.ReadAllText(xmlFile);
            if (string.IsNullOrEmpty(xml))
            {
                return null;
            }

            var script = Serializer.Deserialize(xml);
            return script;
        }

        public void Save(string path, Script script)
        {
        }
    }
}
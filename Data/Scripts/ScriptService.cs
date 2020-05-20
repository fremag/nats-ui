using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using XSerializer;

namespace nats_ui.Data.Scripts
{
    public class ScriptService
    {
        private static XmlSerializer<Script> Serializer { get; } = new XmlSerializer<Script>();

        public const string DefaultScriptDirectory = "Scripts";
        public string ScriptDirectory { get; }
        public List<Script> Scripts { get; } = new List<Script>();
        public Dictionary<string, Type> CommandsByName;
        
        public ScriptService(string scriptDirectory = DefaultScriptDirectory)
        {
            ScriptDirectory = scriptDirectory;
            Load(ScriptDirectory);

            CommandsByName = Assembly.GetEntryAssembly().GetTypes().Where(type => !type.IsAbstract && type.GetInterfaces().Any(interf => interf == typeof(IScriptCommand))).ToDictionary(type => type.Name, type => type);
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
            script.File = xmlFile;
            return script;
        }

        public void Save(string path, Script script)
        {
        }
    }
}
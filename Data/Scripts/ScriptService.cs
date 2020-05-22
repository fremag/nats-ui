using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NLog;
using XSerializer;

namespace nats_ui.Data.Scripts
{
    public class ScriptService
    {
        static ScriptService()
        {
            CommandsByName = Assembly.GetEntryAssembly().GetTypes().Where(type => !type.IsAbstract && type.GetInterfaces().Any(interf => interf == typeof(IScriptCommand))).ToDictionary(type => type.Name, type => type);
            XmlSerializationOptions options = new XmlSerializationOptions(shouldIndent: true);
            Serializer = new XmlSerializer<Script>(options, CommandsByName.Values.ToArray());
        }

        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        public static Dictionary<string, Type> CommandsByName { get; }
        private static XmlSerializer<Script> Serializer { get; }
    
        public const string DefaultScriptDirectory = "Scripts";
        public string ScriptDirectory { get; }
        public List<Script> Scripts { get; } = new List<Script>();
        public Script Current { get; private set; } = new Script();
        
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
            script.File = Path.GetFileName(xmlFile);
            return script;
        }

        public void SetCurrent(Script script)
        {
            Current = script;
        }
        
        public void SaveScript(string path, Script script)
        {
            Logger.Info($"Save script: {path}");
            using var fileStream = File.OpenWrite(path);
            Serializer.Serialize(fileStream, script);
        }
        
        public void Save(string name, string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                Logger.Error($"Can't save: file is null or empty: {file} , name: {name}!");
                return;
            }

            Current.Name = name;
            var path = Path.Combine(ScriptDirectory, file);
            SaveScript(path, Current);
        }
        
        public IScriptCommand Create(string name)
        {
            if (name == null || !CommandsByName.TryGetValue(name, out var type))
            {
                throw new ArgumentException($"Unknown command name: {name} !");
            }

            var obj = (IScriptCommand) Activator.CreateInstance(type);
            return obj;
        }

        public void Add(IScriptCommand scriptCommand)
        {
            if (Current == null)
            {
                Current = new Script();
            }   
            
            Current.Commands.Add(scriptCommand);
        }
    }
}
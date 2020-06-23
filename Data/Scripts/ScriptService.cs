using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NLog;
using XSerializer;

namespace nats_ui.Data.Scripts
{
    public class CommandInfo
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public string ParamName1 { get; set; }
        public string ParamName2 { get; set; }
    }
    
    public class ScriptService
    {
        static ScriptService()
        {
            CommandsByName = Assembly.GetEntryAssembly().GetTypes().Where(type => !type.IsAbstract && type.GetInterfaces().Any(interf => interf == typeof(IScriptCommand))).Select(type =>
            {
                var command = (AbstractScriptCommand)Activator.CreateInstance(type);
                return new CommandInfo
                {
                    Name = type.Name,
                    Type = type,
                    ParamName1 = command.ParamName1,
                    ParamName2 = command.ParamName2
                };
            }
                ).ToDictionary(info => info.Name, info => info);
            XmlSerializationOptions options = new XmlSerializationOptions(shouldIndent: true);
            Serializer = new XmlSerializer<Script>(options);
        }

        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        public static Dictionary<string, CommandInfo> CommandsByName { get; }
        private static XmlSerializer<Script> Serializer { get; }
    
        public const string DefaultScriptDirectory = "Scripts";
        public string ScriptDirectory { get; }
        public List<Script> Scripts { get; } = new List<Script>();
        public Script Current { get; private set; } = new Script();
        
        public ScriptService(string scriptDirectory = DefaultScriptDirectory)
        {
            ScriptDirectory = scriptDirectory;
            if (!Directory.Exists(ScriptDirectory))
            {
                Logger.Info($"Create script directory: {ScriptDirectory}");
                Directory.CreateDirectory(ScriptDirectory);
            }
            Load();
        }
        
        public void Load()
        {
            Logger.Info($"Load scripts: {ScriptDirectory}");
            var scriptFiles = Directory.EnumerateFiles(ScriptDirectory);
            Scripts.Clear();
            foreach (var scriptFile in scriptFiles)
            {
                var script = LoadScript(scriptFile);
                Scripts.Add(script);
            }
        }

        public Script LoadScript(string xmlFile)
        {
            Logger.Info($"Load script: {xmlFile}");
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
            Logger.Info($"Set current script: {script.Name}");
            Current = script;
        }
        
        public void SaveScript(string path, Script script)
        {
            Logger.Info($"Save script: {path}");
            using var fileStream = File.Create(path);
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
            if (name == null || !CommandsByName.TryGetValue(name, out var command))
            {
                throw new ArgumentException($"Unknown command name: {name} !");
            }

            var obj = (IScriptCommand) Activator.CreateInstance(command.Type);
            return obj;
        }

        public void Add(ScriptStatement statement)
        {
            Current ??= new Script();
            Current.Statements.Add(statement);
        }

        public void Reload(Script script)
        {
            var path = Path.Combine(ScriptDirectory, script.File);

            var reloadedScript = LoadScript(path);
            script.Name = reloadedScript.Name;
            script.Statements = reloadedScript.Statements;
        }

        public IScriptCommand BuildCommand(ScriptStatement statement)
        {
            var command = Create(statement.Name);
            command.Param1 = statement.Param1;
            command.Param2 = statement.Param2;
            command.Status = ExecutionStatus.Unknown;
            return command;
        }
    }
}
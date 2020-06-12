using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using nats_ui.Data.Scripts;
using NLog;

namespace nats_ui.Data
{
    public delegate void CommandUpdated(IScriptCommand command);

    public class ExecutorService
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        public event CommandUpdated CommandUpdated;
        public IEnumerable<IScriptCommand> Commands { get; private set; }

        private NatsService NatsService { get; set; }
        private Script Script { get; set; }
        private ScriptService ScriptService { get; set; }
        public NatsMessage Message { get; set; }

        public void Setup(Script script, ScriptService scriptService)
        {
            Logger.Info($"{nameof(Setup)}: {script.Name}, {script.File}");
            NatsService = new NatsService();
            Script = script;
            ScriptService = scriptService;

            Commands = BuildCommands(script);
        }

        private IEnumerable<IScriptCommand> BuildCommands(Script script)
        {
            var commands = new List<IScriptCommand>();
            foreach (var statement in script.Statements)
            {
                var scriptCommand = ScriptService.BuildCommand(statement);
                commands.Add(scriptCommand);
            }

            return commands;
        }

        public void Run()
        {
            Logger.Info($"{nameof(Run)}: {Commands.Count()}");

            foreach (var scriptCommand in Commands)
            {
                scriptCommand.Status = CommandStatus.Waiting;
                CommandUpdated?.Invoke(scriptCommand);
            }

            Task.Run(Execute);
        }

        public void Execute()
        {
            Logger.Info($"Execute: Begin");
            Stopwatch sw = new Stopwatch();
            var scriptCommands = Commands.ToArray();
            for (var i = 0; i < scriptCommands.Length; i++)
            {
                var scriptCommand = scriptCommands[i];
                Logger.Info($"Execute[{i+1}/{scriptCommands.Length}]: {scriptCommand}");
                try
                {
                    scriptCommand.TimeStamp = DateTime.Now;
                    sw.Restart();
                    var result = scriptCommand.Execute(NatsService, this);
                    sw.Stop();
                    scriptCommand.Status = CommandStatus.Executed;
                    scriptCommand.Result = result;
                    Logger.Info($"Executed: {result}");
                }
                catch (Exception e)
                {
                    sw.Stop();
                    Logger.Error($"Command failed ! {scriptCommand}");
                    scriptCommand.Status = CommandStatus.Failed;
                    scriptCommand.Result = e.Message;
                }

                scriptCommand.Duration = sw.Elapsed;
                CommandUpdated?.Invoke(scriptCommand);
            }
            Logger.Info($"Execute: End");
        }
    }
}
using System.Collections.Generic;
using nats_ui.Data.Scripts;

namespace nats_ui.Data
{
    internal class ExecutorService
    {
        public IEnumerable<IScriptCommand> Commands { get; private set; }

        private NatsService NatsService { get; set; }
        private Script Script { get; set; }
        private ScriptService ScriptService { get; set; }
       
        public void Setup(NatsService natsService, Script script, ScriptService scriptService)
        {
            NatsService = natsService;
            Script = script;
            ScriptService = scriptService;

            Commands = BuildCommands(script);
        }

        private IEnumerable<IScriptCommand> BuildCommands(Script script)
        {
            var commands = new List<IScriptCommand>(); 
            foreach (var statement in script.Statements)
            {
                var scriptCommand = BuildCommand(statement);
                commands.Add(scriptCommand);
            }

            return commands;
        }

        private IScriptCommand BuildCommand(ScriptStatement statement)
        {
            var command = ScriptService.Create(statement.Name);
            command.Param1 = statement.Param1;
            command.Param2 = statement.Param2;
            return command;
        }
    }
}
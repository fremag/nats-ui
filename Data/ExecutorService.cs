using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using nats_ui.Data.Scripts;
using NLog;

namespace nats_ui.Data
{
    public delegate void CommandUpdated(IScriptCommand command);

    public class Job : ICheckable
    {
        public bool Checked { get; set; }
        public int Id { get; set; }
        public string Name { get; set; } 
        public string File { get; set; } 
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<IScriptCommand> Commands { get; set; } = new List<IScriptCommand>();
        public TimeSpan Duration => EndTime - StartTime;
        
        [XmlIgnore]
        public string Run => "oi oi-media-play";
        [XmlIgnore]
        public string Display => "oi oi-media-play";
    }
    
    public class ExecutorService
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        public event CommandUpdated CommandUpdated;
        private NatsService NatsService { get; set; }
        private ScriptService ScriptService { get; set; }
        public NatsMessage Message { get; set; }
        public Job LastJob { get; set; }
        public List<Job> Jobs { get; } = new List<Job>();
        
        public Job Setup(Script script, ScriptService scriptService)
        {
            Logger.Info($"{nameof(Setup)}: {script.Name}, {script.File}");
            NatsService = new NatsService();
            ScriptService = scriptService;

            var report = new Job {Id = Jobs.Count, File = script.File, Name = script.Name};
            var commands = BuildCommands(script);
            report.Commands.AddRange(commands);
            Jobs.Add(report);
            return report;
        }
        
        public Job Setup(Job oldReport, ScriptService scriptService)
        {
            Logger.Info($"{nameof(Setup)}: {oldReport.Name}, {oldReport.File}");
            NatsService = new NatsService();
            ScriptService = scriptService;

            var report = new Job {Id = Jobs.Count, File = oldReport.File, Name = oldReport.Name};
            var commands = BuildCommands(oldReport.Commands);
            report.Commands.AddRange(commands);
            Jobs.Add(report);
            return report;
        }

        private IEnumerable<IScriptCommand> BuildCommands(List<IScriptCommand> commands)
        {
            foreach (var command in commands)
            {
                yield return ScriptService.BuildCommand(new ScriptStatement {Name = command.Name, Param1 = command.Param1, Param2 = command.Param2});
            }
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

        public void Run(Job job)
        {
            Logger.Info($"{nameof(Run)}: {job.Commands.Count}");
            LastJob = job;
            foreach (var scriptCommand in job.Commands)
            {
                scriptCommand.Status = CommandStatus.Waiting;
                CommandUpdated?.Invoke(scriptCommand);
            }

            Task.Run( () => Execute(job));
        }

        public void Execute(Job job)
        {
            job.StartTime = DateTime.Now;
            Stopwatch sw = new Stopwatch();
            var scriptCommands = job.Commands.ToArray();
            Logger.Info($"Execute: Begin, Nb Commands: {scriptCommands}");
            for (var i = 0; i < scriptCommands.Length; i++)
            {
                var scriptCommand = scriptCommands[i];
                Logger.Info($"Execute[{i+1}/{scriptCommands.Length}]: {scriptCommand}");
                try
                {
                    scriptCommand.TimeStamp = DateTime.Now;
                    sw.Restart();
                    scriptCommand.Status = CommandStatus.Running;
                    CommandUpdated?.Invoke(scriptCommand);
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
            job.EndTime = DateTime.Now;
            Logger.Info($"Execute: End, TotalTime: {job.Duration}");
        }
    }
}
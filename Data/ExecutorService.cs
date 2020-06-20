using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using nats_ui.Data.Scripts;
using NLog;

namespace nats_ui.Data
{
    public delegate void CommandUpdated(IScriptCommand command);
    public delegate void JobUpdated(Job job);

    public class ExecutorService
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        
        public event JobUpdated JobUpdated;
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

            var job = new Job {Id = Jobs.Count, File = oldReport.File, Name = oldReport.Name, Status = ExecutionStatus.Waiting};
            var commands = BuildCommands(oldReport.Commands);
            job.Commands.AddRange(commands);
            Jobs.Add(job);
            return job;
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
            job.Status = ExecutionStatus.Waiting; 
            foreach (var scriptCommand in job.Commands)
            {
                scriptCommand.Status = ExecutionStatus.Waiting;
                CommandUpdated?.Invoke(scriptCommand);
            }

            Task.Run( () => Execute(job));
        }

        public void Execute(Job job)
        {
            job.StartTime = DateTime.Now;
            job.Status = ExecutionStatus.Running;
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
                    scriptCommand.Status = ExecutionStatus.Running;
                    CommandUpdated?.Invoke(scriptCommand);
                    var result = scriptCommand.Execute(NatsService, this);
                    sw.Stop();
                    scriptCommand.Status = ExecutionStatus.Executed;
                    scriptCommand.Result = result;
                    Logger.Info($"Executed: {result}");
                }
                catch (Exception e)
                {
                    sw.Stop();
                    Logger.Error($"Command failed ! {scriptCommand}");
                    scriptCommand.Status = ExecutionStatus.Failed;
                    scriptCommand.Result = e.Message;
                }

                scriptCommand.Duration = sw.Elapsed;
                CommandUpdated?.Invoke(scriptCommand);
            }
            job.EndTime = DateTime.Now;
            job.Status = ExecutionStatus.Executed;
            Logger.Info($"Execute: End, TotalTime: {job.Duration}");
            JobUpdated?.Invoke(job);
        }
    }
}
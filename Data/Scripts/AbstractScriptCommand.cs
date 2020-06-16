using System;

namespace nats_ui.Data.Scripts
{
    public enum CommandStatus
    {
        Unknown, Waiting, Running, Executed, Failed
    }

    public abstract class AbstractScriptCommand : IScriptCommand
    {
        public virtual string ParamName1 { get; } = null;
        public virtual string ParamName2 { get; } = null;

        public string Name => GetType().Name; 
        public string Param1 { get; set; }
        public string Param2 { get; set; }

        public bool Checked { get; set; }
        public string Result { get; set; }
        public CommandStatus Status { get; set; } = CommandStatus.Unknown;
        public DateTime TimeStamp { get; set; }
        public TimeSpan Duration { get; set; }

        public virtual string Execute(NatsService natsService, ExecutorService executorService)
        {
            return "Ok";
        }

        public override string ToString()
        {
            return $"{Name}, {ParamName1}: {Param1?.Substring(0, Math.Min(100, Param1.Length))}, {ParamName2}: {Param2?.Substring(0, Math.Min(100, Param2.Length))}";
        }
    }
}
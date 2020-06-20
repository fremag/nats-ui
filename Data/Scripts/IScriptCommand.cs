using System;

namespace nats_ui.Data.Scripts
{
    public interface IScriptCommand : ICheckable
    {
        string Name { get; }

        string ParamName1 { get; }
        string ParamName2 { get; }

        string Param1 { get; set; }
        string Param2 { get; set; }

        ExecutionStatus Status { get; set; }
        public string Result { get; set; }
        public DateTime TimeStamp { get; set; }
        public TimeSpan Duration { get; set; }

        string Execute(NatsService natsService, ExecutorService executorService);
    }
}
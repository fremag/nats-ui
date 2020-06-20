using System;
using System.Collections.Generic;
using System.Linq;
using nats_ui.Data.Scripts;

namespace nats_ui.Data
{
    public class Job : ICheckable
    {
        public ExecutionStatus Status { get; set; }
        public bool Checked { get; set; }
        public int Id { get; set; }
        public string Name { get; set; } 
        public string File { get; set; } 
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<IScriptCommand> Commands { get; set; } = new List<IScriptCommand>();
        public TimeSpan Duration => EndTime - StartTime;
        public int Errors => Commands.Count(command => command.Status == ExecutionStatus.Failed);
        public int Count => Commands.Count;
        
        public string Run => "oi oi-media-play";
        public string Display => $"[{Id}]";
        public string DisplayLink => $"/executor/{Id}";
    }
}
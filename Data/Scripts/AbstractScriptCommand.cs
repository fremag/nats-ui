using System;
using System.Xml.Serialization;

namespace nats_ui.Data.Scripts
{
    public enum CommandStatus
    {
        Unknown, Ready, Error, Waiting, Executed, Failed
    }

    public abstract class AbstractScriptCommand : IScriptCommand
    {
        public virtual string ParamName1 { get; } = null;
        public virtual string ParamName2 { get; } = null;

        [XmlIgnore]
        public string Name => GetType().Name; 
        public string Param1 { get; set; }
        public string Param2 { get; set; }

        [XmlIgnore]
        public bool Checked { get; set; }
        [XmlIgnore]
        public string Result { get; set; }
        [XmlIgnore]
        public CommandStatus Status { get; set; }
        [XmlIgnore]
        public DateTime TimeStamp { get; set; }
        [XmlIgnore]
        public long ExecTime { get; set; }

        public virtual bool CheckParams(out string msg)
        {
            msg = "Ok";
            return true;
        }

        public virtual CommandStatus Execute()
        {
            return CommandStatus.Executed;
        }
    }
}
using System;
using System.Xml.Serialization;

namespace nats_ui.Data.Scripts
{
    public enum CommandStatus
    {
        Unknown, Ready, Error, Waiting, Executed, Failed
    }

    public interface IScriptCommand : ICheckable
    {
        [XmlIgnore]
        string Name { get; }

        string ParamName1 { get; }
        string ParamName2 { get; }

        string Param1 { get; set; }
        string Param2 { get; set; }

        string Up => "https://img.icons8.com/flat_round/64/000000/collapse-arrow--v1.png";
        string Down => "https://img.icons8.com/flat_round/64/000000/expand-arrow--v1.png";
        string Trash => "img/trash.svg";
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
using System.Xml.Serialization;

namespace nats_ui.Data.Scripts
{
    public interface IScriptCommand : ICheckable
    {
        [XmlIgnore]
        string Name { get; }

        string ParamName1 { get; }
        string ParamName2 { get; }

        string Param1 { get; set; }
        string Param2 { get; set; }
    }
}
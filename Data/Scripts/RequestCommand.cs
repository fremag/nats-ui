using NLog;

namespace nats_ui.Data.Scripts
{
    public class RequestCommand : AbstractScriptCommand
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        public override string ParamName1 => "Subject";
        public override string ParamName2 => "Data";
    }
}
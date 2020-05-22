using NLog;

namespace nats_ui.Data.Scripts
{
    public class ConnectCommand : AbstractScriptCommand
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        public override string ParamName1 => "Host";
        public override string ParamName2 => "Port";
    }
}
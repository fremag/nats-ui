using NLog;

namespace nats_ui.Data.Scripts
{
    public class WaitCommand : AbstractScriptCommand
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        public override string ParamName1 => "Seconds";
    }
}
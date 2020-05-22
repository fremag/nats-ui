using NLog;

namespace nats_ui.Data.Scripts
{
    public class UnsubscribeCommand : AbstractScriptCommand
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        public override string ParamName1 => "Subject";
    }
}
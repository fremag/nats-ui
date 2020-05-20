using NLog;

namespace nats_ui.Data.Scripts
{
    public class PublishCommand : AbstractScriptCommand
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        public override string ParamName1 => "Url";
        public override string ParamName2 => "Data";
    }
}
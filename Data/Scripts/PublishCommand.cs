using NLog;

namespace nats_ui.Data.Scripts
{
    public class PublishCommand : AbstractScriptCommand
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
    }
}
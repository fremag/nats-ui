using NLog;

namespace nats_ui.Data.Scripts
{
    public class SubscribeCommand : AbstractScriptCommand
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
    }
}
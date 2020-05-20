using NLog;

namespace nats_ui.Data.Scripts
{
    public class RequestCommand : AbstractScriptCommand
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
    }
}
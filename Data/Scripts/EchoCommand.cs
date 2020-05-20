using NLog;

namespace nats_ui.Data.Scripts
{
    public class EchoCommand : AbstractScriptCommand
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
    }
}
using NLog;

namespace nats_ui.Data.Scripts
{
    public class ConnectCommand : AbstractScriptCommand
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
    }
}
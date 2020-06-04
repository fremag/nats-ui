using NLog;

namespace nats_ui.Data.Scripts
{
    public class ConnectCommand : AbstractScriptCommand
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        public override string ParamName1 => "Host";
        public override string ParamName2 => "Port";

        public override string Execute(NatsService natsService, ExecutorService executorService)
        {
            var connection = new Connection($"{Param1}:{Param2}", Param1, int.Parse(Param2));
            var conn = natsService.Connect(connection);
            return $"{conn.ConnectedUrl}: {conn.State}";
        }
    }
}
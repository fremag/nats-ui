using NLog;

namespace nats_ui.Data.Scripts
{
    public class RequestCommand : AbstractScriptCommand
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        public override string ParamName1 => "Subject";
        public override string ParamName2 => "Data";

        public override string Execute(NatsService natsService, ExecutorService executorService)
        {
            var result = "";
            foreach (var connection in natsService.Connections)
            {
                NatsMessage msg = new NatsMessage
                {
                    Subject = Param1,
                    Data = Param2,
                    Url = connection.Url
                };
                var reply = natsService.Request(msg, 1000);
                result += $"Url: {connection.Url} ";
                result += $"Data: {reply?.Data ?? "No data !"}";
                executorService.Message = reply;
            }
            
            return result;
        }
    }
}
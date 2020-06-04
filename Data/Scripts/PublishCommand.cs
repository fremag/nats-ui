using NLog;

namespace nats_ui.Data.Scripts
{
    public class PublishCommand : AbstractScriptCommand
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        public override string ParamName1 => "Subject";
        public override string ParamName2 => "Data";

        public override string Execute(NatsService natsService, ExecutorService executorService)
        {
            foreach (var connection in natsService.Connections)
            {
                NatsMessage msg = new NatsMessage
                {
                    Subject = Param1,
                    Data = Param2,
                    Url = connection.Url
                };
                natsService.Publish(msg);
            }

            return "Message sent.";
        }
    }
}
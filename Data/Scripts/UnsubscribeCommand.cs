using NLog;

namespace nats_ui.Data.Scripts
{
    public class UnsubscribeCommand : AbstractScriptCommand
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        public override string ParamName1 => "Subject";
        
        public override string Execute(NatsService natsService, ExecutorService executorService)
        {
            natsService.Unsubscribe(new NatsSubscription(Param1));
            return "Subscribed.";
        }
    }
}
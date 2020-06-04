using System;
using System.Threading;
using NLog;

namespace nats_ui.Data.Scripts
{
    public class WaitCommand : AbstractScriptCommand
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        public override string ParamName1 => "Seconds";
        private NatsMessage Message { get; set; }
        
        public override string Execute(NatsService natsService, ExecutorService executorService)
        {
            natsService.MessageReceived += OnMessageReceived; 
            var nbMs = (int) (1000 * double.Parse(Param1));
            DateTime now = DateTime.Now;
            while (Message == null && (DateTime.Now - now).TotalMilliseconds < nbMs)
            {
                Thread.Sleep(10);
            }

            natsService.MessageReceived -= OnMessageReceived;
            if (Message == null)
            {
                throw new TimeoutException("No message received !");
            }

            executorService.Message = Message;
            return "Message received !";
        }

        private void OnMessageReceived(NatsMessage message)
        {
            Message = message;
        }
    }
}
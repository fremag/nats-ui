using nats_ui.Data.Scripts;
using NLog;

namespace nats_ui.Data
{
    public class RecordService
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
    
        private Script Script { get; set; }
        public void StartRecord(NatsService natsService, ScriptService scriptService)
        {
            Script = scriptService.Current;
            natsService.Connected += OnConnected;
            natsService.Disconnected += OnDisconnected;
            natsService.MessageSent += OnMessageSent;
            natsService.Subscribed += OnSubscribed;
            natsService.Unsubscribed += OnUnsubscribed;
        }

        public void StopRecord(NatsService natsService)
        {
            natsService.Connected -= OnConnected;
            natsService.Disconnected -= OnDisconnected;
            natsService.MessageSent -= OnMessageSent;
            natsService.Subscribed -= OnSubscribed;
            natsService.Unsubscribed -= OnUnsubscribed;
        }

        private void OnUnsubscribed(NatsSubscription subscription)
        {
            Logger.Info($"{nameof(OnUnsubscribed)}");
            Script.Statements.Add(new ScriptStatement {Name = nameof(UnsubscribeCommand), Param1 = subscription.Subject, Param2=""});
        }

        private void OnSubscribed(NatsSubscription subscription)
        {
            Logger.Info($"{nameof(OnSubscribed)}");
            Script.Statements.Add(new ScriptStatement {Name = nameof(SubscribeCommand), Param1 = subscription.Subject, Param2=""});
        }

        private void OnMessageSent(NatsMessage message)
        {
            Logger.Info($"{nameof(OnMessageSent)}");
            Script.Statements.Add(new ScriptStatement {Name = nameof(PublishCommand), Param1 = message.Subject, Param2=message.Data});
        }

        private void OnDisconnected(Connection connection)
        {
            Logger.Info($"{nameof(OnDisconnected)}");
            Script.Statements.Add(new ScriptStatement {Name = nameof(DisconnectCommand), Param1 = connection.Host, Param2=connection.Port.ToString()});
        }

        private void OnConnected(Connection connection)
        {
            Logger.Info($"{nameof(OnConnected)}");
            Script.Statements.Add(new ScriptStatement {Name = nameof(ConnectCommand), Param1 = connection.Host, Param2=connection.Port.ToString()});
        }
    }
}
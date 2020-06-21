using C1.Blazor.Core;
using nats_ui.Data;

namespace nats_ui.Pages.Subscriptions
{
    public class SubscriptionCellFactory : StandardCellFactory<NatsSubscription>
    {
        protected override void PrepareCellStyle(string colName, NatsSubscription job, C1Style cellStyle)
        {
        }
   }
}
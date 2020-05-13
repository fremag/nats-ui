using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using nats_ui.Data;
using NLog;

namespace nats_ui.Pages.Subscriptions
{
    public class SubscriptionsComponent : ComponentBase
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        protected class SubjectModel
        {
            public string Subject { get; set; }
            public override string ToString() => $"{Subject}";
        }

        [Inject]
        private NatsService NatsService { get; set; }

        protected SubjectModel Model { get; } = new SubjectModel();
        protected StandardGridModel<NatsSubscription> Subscriptions { get; } = new StandardGridModel<NatsSubscription>();
        protected SubscriptionCellFactory GridCellFactory { get; } = new SubscriptionCellFactory();

        protected override Task OnInitializedAsync()
        {
            Subscriptions.SetData(NatsService.Configuration.Subjects);
            Subscriptions.SelectedItemChanged += OnSelectedItemChanged;
            Subscriptions.ItemDoubleClicked += OnItemDoubleClicked;
            return Task.CompletedTask;
        }

        protected void CreateSubscription()
        {
            Logger.Info($"{nameof(CreateSubscription)}: {Model}");
            var natsSubscription = new NatsSubscription(Model.Subject)
            {
                Checked = false,
                Subscribed = false
            };
            
            if (NatsService.Create(natsSubscription, out var msg))
            {
                Subscriptions.Insert(0, natsSubscription);
            }
        }

        protected void RemoveSubscriptions()
        {
            Logger.Info(nameof(RemoveSubscriptions));
            foreach(var (i, subscription) in Subscriptions.GetCheckedItems())
            {
                Logger.Info($"{nameof(RemoveSubscriptions)}: {subscription}");
                NatsService.Remove(subscription);
                Subscriptions.Remove(i);
            }
        }
        
        private void OnSelectedItemChanged(NatsSubscription subscription)
        {
            Model.Subject = subscription.Subject;
            InvokeAsync(StateHasChanged);
        }

        private void OnItemDoubleClicked(int index, NatsSubscription subscription)
        {
            if (subscription.Subscribed)
            {
                NatsService.Unsubscribe(subscription);
            }
            else
            {
                NatsService.Subscribe(subscription);
            }
        }
    }
}
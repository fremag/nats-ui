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
            Subscriptions.SetData(NatsService.Configuration.Subscriptions);
            Subscriptions.SelectedItemChanged += OnSelectedItemChanged;
            Subscriptions.ItemDoubleClicked += OnItemDoubleClicked;
            Subscriptions.ItemClicked += OnItemClicked;
            return Task.CompletedTask;
        }

        private void OnItemClicked(string colName, NatsSubscription subscription)
        {
            switch (colName)
            {
                case nameof(NatsSubscription.Unsubscribe):
                    Unsubscribe(subscription);
                    break;
                case nameof(NatsSubscription.Subscribe):
                    Subscribe(subscription);
                    break;
                case nameof(NatsSubscription.Trash):
                    Logger.Info($"{nameof(NatsSubscription.Trash)}: {subscription}");
                    NatsService.Remove(subscription);
                    Subscriptions.Remove(subscription);
                    break;
            }
        }

        private void Unsubscribe(NatsSubscription subscription)
        {
            Logger.Info($"{nameof(NatsSubscription.Unsubscribe)}: {subscription}");
            NatsService.Unsubscribe(subscription);
            Subscriptions.Update(subscription);
        }

        private void Subscribe(NatsSubscription subscription)
        {
            Logger.Info($"{nameof(NatsSubscription.Subscribe)}: {subscription}");
            NatsService.Subscribe(subscription);
            Subscriptions.Update(subscription);
        }

        protected void CreateSubscription()
        {
            Logger.Info($"{nameof(CreateSubscription)}: {Model}");
            var natsSubscription = new NatsSubscription(Model.Subject)
            {
                Checked = false,
                Subscribed = false
            };
            
            if (NatsService.Create(natsSubscription, out _))
            {
                Subscriptions.Insert(0, natsSubscription);
            }
            Subscribe(natsSubscription);
        }
        
        private void OnSelectedItemChanged(NatsSubscription subscription)
        {
            Model.Subject = subscription.Subject;
            InvokeAsync(StateHasChanged);
        }

        private void OnItemDoubleClicked(string colName, NatsSubscription subscription)
        {
            if (subscription.Subscribed)
            {
                Unsubscribe(subscription); 
            }
            else
            {
                Subscribe(subscription);
            }
        }
    }
}
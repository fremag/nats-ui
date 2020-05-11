using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using C1.Blazor.Grid;
using C1.DataCollection;
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
        protected C1DataCollection<NatsSubscription> Subscriptions { get; private set; }
        protected SubscriptionCellFactory GridCellFactory { get; } = new SubscriptionCellFactory();

        protected override Task OnInitializedAsync()
        {
            NatsService.SubscriptionCreated += OnSubscriptionCreated;
            NatsService.SubscriptionRemoved += OnSubscriptionRemoved;
            Subscriptions = new C1DataCollection<NatsSubscription>(new List<NatsSubscription>(NatsService.Configuration.Subjects));
            return Task.CompletedTask;
        }

        private void OnSubscriptionRemoved(NatsSubscription subscription)
        {
            for (int i = Subscriptions.Count - 1; i >= 0; i--)
            {
                if (Subscriptions[i].Equals(subscription))
                {
                    Subscriptions.RemoveAsync(i);
                    return;
                }
            }
        }

        private void OnSubscriptionCreated(NatsSubscription subscription)
        {
            Subscriptions.InsertAsync(0, subscription);
        }

        protected void CreateSubscription()
        {
            Logger.Info($"{nameof(CreateSubscription)}: {Model}");
            if (Subscriptions.Contains(Model.Subject))
            {
                return;
            }

            NatsService.Create(new NatsSubscription(Model.Subject)
            {
                Selected = false,
                Subscribed = false
            });
        }

        protected void RemoveSubscriptions()
        {
            Logger.Info(nameof(RemoveSubscriptions));
            foreach (var subject in Subscriptions.OfType<NatsSubscription>().Where(natsSubject => natsSubject.Selected).ToArray())
            {
                Logger.Info($"RemoveSubject: {subject}");
                NatsService.Remove(subject);
            }

            InvokeAsync(StateHasChanged);
        }
        
        protected void SelectedSubjectChanged(object sender, GridCellRangeEventArgs e)
        {
            var selected = GetSelected(e.CellRange);
            if (selected == null)
            {
                return;
            }

            Model.Subject = selected.Subject;
            InvokeAsync(StateHasChanged);
        }

        private NatsSubscription GetSelected(GridCellRange cellRange)
        {
            if (cellRange == null)
            {
                return null;
            }

            var selected = Subscriptions[cellRange.Row] as NatsSubscription;
            return selected;
        }

        protected void OnCellTaped(object sender, GridInputEventArgs e)
        {
            var natsSubject = GetSelected(e.CellRange);
            if (natsSubject == null)
            {
                return;
            }

            if (e.CellRange.Column == 0)
            {
                natsSubject.Selected = !natsSubject.Selected;
            }
        }

        protected void OnCellDoubleTaped(object sender, GridInputEventArgs e)
        {
            var natsSubject = GetSelected(e.CellRange);
            if (natsSubject == null)
            {
                return;
            }

            if (natsSubject.Subscribed)
            {
                Unsubscribe(natsSubject);
            }
            else
            {
                Subscribe(natsSubject);
            }
        
        }

        private void Subscribe(NatsSubscription natsSubscription)
        {
            NatsService.Subscribe(natsSubscription);
        }

        private void Unsubscribe(NatsSubscription natsSubscription)
        {
            NatsService.Unsubscribe(natsSubscription);
        }
    }
}
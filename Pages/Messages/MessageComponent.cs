using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using nats_ui.Data;
using NLog;

namespace nats_ui.Pages.Messages
{
    public class MessagesComponent : ComponentBase
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        public class MessageModel
        {
            public string Subject { get; set; }
            public string Data { get; set;}
            public string Url { get; set;}
        }

        [Inject]
        private NatsService NatsService { get; set; }

        [Inject]
        private InspectorService Inspector { get; set; }

        [Inject]
        private NavigationManager NavMgr { get; set; }

        protected StandardGridModel<NatsMessage> MessageGrid { get; } = new StandardGridModel<NatsMessage>(); 
        protected MessageCellFactory GridCellFactory { get; } = new MessageCellFactory();

        protected MessageModel Model { get; } = new MessageModel();

        protected override Task OnInitializedAsync()
        {
            NatsService.MessageAdded += OnMessageAdded;
            MessageGrid.SetData(NatsService.Messages);
            MessageGrid.SelectedItemChanged += OnSelectedItemChanged;
            MessageGrid.ItemClicked += OnItemClicked;
            return Task.CompletedTask;
        }

        private void OnItemClicked(string colName, NatsMessage message)
        {
            if (colName == nameof(NatsMessage.Inspect))
            {
                Inspector.Data = message.Data;
                NavMgr.NavigateTo("/inspector");
            }
        }

        private void OnSelectedItemChanged(NatsMessage message)
        {
            Model.Subject = message.Subject;
            Model.Data = message.Data;
            Model.Url = message.Url;

            InvokeAsync(StateHasChanged);
        }

        private void OnMessageAdded(NatsMessage message)
        {
            InvokeAsync(() => MessageGrid.Insert(0, message));
        }

        protected void SaveMessage()
        {
            var message = new NatsMessage
            {
                Url = Model.Url,
                Subject = Model.Url,
                Data = Model.Data
            };

            NatsService.Save(message);
        }
    }
}
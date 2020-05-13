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
        }

        [Inject]
        private NatsService NatsService { get; set; }

        protected StandardGridModel<NatsMessage> MessageGrid { get; } = new StandardGridModel<NatsMessage>(); 
        protected MessageCellFactory GridCellFactory { get; } = new MessageCellFactory();

        protected MessageModel Model { get; } = new MessageModel();

        protected override Task OnInitializedAsync()
        {
            NatsService.MessageReceived += OnMessageReceived;
            MessageGrid.SetData(NatsService.Messages);
            MessageGrid.SelectedItemChanged += OnSelectedItemChanged;
            return Task.CompletedTask;
        }

        private void OnSelectedItemChanged(NatsMessage message)
        {
            Model.Subject = message.Subject;
            Model.Data = message.Data;

            InvokeAsync(StateHasChanged);
        }

        private void OnMessageReceived(NatsMessage message)
        {
            InvokeAsync(() => MessageGrid.Insert(0, message));
        }

        protected void SendMessage()
        {
        
        }
    }
}
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using nats_ui.Data;
using NLog;

namespace nats_ui.Pages.Send
{
    public class SendComponent : ComponentBase
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
        protected SendCellFactory GridCellFactory { get; } = new SendCellFactory();

        protected StandardGridModel<Connection> UrlGrid { get; } = new StandardGridModel<Connection>(); 
        protected UrlCellFactory UrlCellFactory { get; } = new UrlCellFactory();

        protected MessageModel Model { get; } = new MessageModel();

        protected override Task OnInitializedAsync()
        {
            NatsService.MessageSaved += OnMessageSaved;
            MessageGrid.SetData(NatsService.Configuration.SavedMessages);
            MessageGrid.SelectedItemChanged += OnSelectedItemChanged;
            UrlGrid.SetData(NatsService.Connections);
            UrlGrid.CheckAll();
            return Task.CompletedTask;
        }

        private void OnSelectedItemChanged(NatsMessage message)
        {
            Model.Subject = message.Subject;
            Model.Data = message.Data;

            InvokeAsync(StateHasChanged);
        }

        private void OnMessageSaved(NatsMessage message)
        {
            InvokeAsync(() => MessageGrid.Insert(0, message));
        }

        protected void SendMessage()
        {
            foreach ((int i, Connection item) connection in UrlGrid.GetCheckedItems())
            {
                
                var message = new NatsMessage
                {
                    TimeStamp = DateTime.Now,
                    Url = connection.item.Url,
                    Subject = Model.Subject,
                    Data = Model.Data
                };

                NatsService.Send(message);
            }
        }
        protected void RequestMessage()
        {
            foreach ((int i, Connection item) connection in UrlGrid.GetCheckedItems())
            {
                
                var message = new NatsMessage
                {
                    TimeStamp = DateTime.Now,
                    Url = connection.item.Url,
                    Subject = Model.Subject,
                    Data = Model.Data
                };

                NatsService.Request(message);
            }
        }
    }
}
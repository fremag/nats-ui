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
            public string Data { get; set; }
        }

        [Inject]
        private NatsService NatsService { get; set; }

        [Inject]
        private InspectorService Inspector { get; set; }

        [Inject]
        private NavigationManager NavMgr { get; set; }

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
            MessageGrid.ItemClicked += OnItemClicked;
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

        protected void PublishMessage()
        {
            Publish(Model.Subject, Model.Data);
        }

        protected void SaveMessage()
        {
            var message = new NatsMessage
            {
                TimeStamp = DateTime.Now,
                Subject = Model.Subject,
                Data = Model.Data
            };

            NatsService.Save(message);
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

        private void OnItemClicked(string colName, NatsMessage message)
        {
            switch (colName)
            {
                case nameof(NatsMessage.Inspect):
                    Inspector.Data = message.Data;
                    NavMgr.NavigateTo("/inspector");
                    break;
                case nameof(NatsMessage.Run):
                    Publish(message.Subject, message.Data);
                    break;
                case nameof(NatsMessage.Trash):
                    NatsService.Configuration.SavedMessages.Remove(message);
                    NatsService.Save();
                    MessageGrid.Remove(message);
                    break;
            }
        }

        private void Publish(string subject, string data)
        {
            foreach ((int i, Connection item) connection in UrlGrid.GetCheckedItems())
            {
                var msg = new NatsMessage
                {
                    TimeStamp = DateTime.Now,
                    Url = connection.item.Url,
                    Subject = subject,
                    Data = data
                };

                NatsService.Publish(msg);
            }
        }
    }
}
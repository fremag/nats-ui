using System.Collections.Generic;
using System.Threading.Tasks;
using C1.Blazor.Grid;
using C1.DataCollection;
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

        protected C1DataCollection<NatsMessage> Messages { get; private set; }
        protected MessageCellFactory GridCellFactory { get; } = new MessageCellFactory();

        protected MessageModel Model { get; } = new MessageModel();

        protected override Task OnInitializedAsync()
        {
            NatsService.MessageReceived += OnMessageReceived;
            Messages = new C1DataCollection<NatsMessage>(new List<NatsMessage>(NatsService.Messages));
            return Task.CompletedTask;
        }

        private void OnMessageReceived(NatsMessage message)
        {
            InvokeAsync( () => Messages.InsertAsync(0, message));
        }

        protected void SelectedMessageChanged(object sender, GridCellRangeEventArgs e)
        {
            var selected = GetSelected(e.CellRange);
            if (selected == null)
            {
                return;
            }

            Model.Subject = selected.Subject;
            Model.Data = selected.Data;

            InvokeAsync(StateHasChanged);
        }

        private NatsMessage GetSelected(GridCellRange cellRange)
        {
            if (cellRange == null)
            {
                return null;
            }

            var selected = Messages[cellRange.Row] as NatsMessage;
            return selected;
        }

        protected void OnCellTaped(object sender, GridInputEventArgs e)
        {
            var selected = GetSelected(e.CellRange);
            if (selected == null)
            {
                return;
            }

            if (e.CellRange.Column == 0)
            {
                selected.Selected = !selected.Selected;
            }
        }

        protected void OnCellDoubleTaped(object sender, GridInputEventArgs e)
        {
        }

        protected void SendMessage()
        {
        
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using C1.Blazor.Grid;
using C1.DataCollection;
using Microsoft.AspNetCore.Components;
using nats_ui.Data;
using NLog;

namespace nats_ui.Pages
{
    public class NatsConnectionComponent : ComponentBase
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        [Inject]
        private NatsService NatsService { get; set; }

        protected NatsConnectionModel NatsConnection { get; } = new NatsConnectionModel();
        protected C1DataCollection<NatsConnectionModel> connections;

        protected void CreateConnection()
        {
            Logger.Info($"CreateConnection: {NatsConnection}");
            NatsService.Create(NatsConnection.Clone());
        }
        
        protected override async Task OnInitializedAsync()
        {
            NatsService.ConnectionCreated += OnConnectionCreated;
            connections = new C1DataCollection<NatsConnectionModel>(new List<NatsConnectionModel>(NatsService.Configuration.Connections));
        }

        private void OnConnectionCreated(NatsConnectionModel connection)
        {
            connections.InsertAsync(0, connection);
        }

        protected void SelectedConnectionChanged(object sender, GridCellRangeEventArgs e)
        {
            var cellRange = e.CellRange;
            if (cellRange == null)
            {
                return;
            }
            
            var selected = connections[cellRange.Row] as NatsConnectionModel;
            if (selected == null)
            {
                return;
            }
            NatsConnection.Name = selected.Name;
            NatsConnection.Host = selected.Host;
            NatsConnection.Port = selected.Port;
            
            InvokeAsync(StateHasChanged);
        }
    }
}
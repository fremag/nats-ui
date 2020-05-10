using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using C1.Blazor.Core;
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
        public string Status { get; set; }

        protected void CreateConnection()
        {
            Logger.Info($"CreateConnection: {NatsConnection}");
            Status = NatsService.Create(NatsConnection.Clone());
            InvokeAsync(StateHasChanged);
        }

        protected void RemoveConnections()
        {
            Logger.Info("RemoveConnections");
            foreach (var connection in connections.OfType<NatsConnectionModel>().Where(conn => conn.Selected).ToArray())
            {
                Logger.Info($"RemoveConnection: {connection}");
                NatsService.Remove(connection);
            }

            Status = "Connection removed";
            InvokeAsync(StateHasChanged);
        }

        protected override async Task OnInitializedAsync()
        {
            NatsService.ConnectionCreated += OnConnectionCreated;
            NatsService.ConnectionRemoved += OnConnectionRemoved;
            connections = new C1DataCollection<NatsConnectionModel>(new List<NatsConnectionModel>(NatsService.Configuration.Connections));
        }

        private void OnConnectionRemoved(NatsConnectionModel connection)
        {
            for (int i = connections.Count - 1; i >= 0; i--)
            {
                if (connections[i].Equals(connection))
                {
                    connections.RemoveAsync(i);
                    return;
                }
            }
        }

        private void OnConnectionCreated(NatsConnectionModel connection)
        {
            connections.InsertAsync(0, connection);
        }

        protected void SelectedConnectionChanged(object sender, GridCellRangeEventArgs e)
        {
            var selected = GetSelected(e.CellRange);
            if (selected == null)
            {
                return;
            }

            NatsConnection.Name = selected.Name;
            NatsConnection.Host = selected.Host;
            NatsConnection.Port = selected.Port;

            InvokeAsync(StateHasChanged);
        }

        private NatsConnectionModel GetSelected(GridCellRange cellRange)
        {
            if (cellRange == null)
            {
                return null;
            }

            var selected = connections[cellRange.Row] as NatsConnectionModel;
            return selected;
        }

        protected void OnCellTaped(object? sender, GridInputEventArgs e)
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

        public void DumpConnections()
        {
            Logger.Info("DumpConnections");
            foreach (var connection in connections)
            {
                Logger.Info(connection);
            }
        }

        protected ConnectionCellFactory GridCellFactory { get; } = new ConnectionCellFactory();

        protected class ConnectionCellFactory : GridCellFactory
        {
            public override void PrepareCellStyle(GridCellType cellType, GridCellRange range, C1Style style)
            {
                base.PrepareCellStyle(cellType, range, style);
                var selectedColumn = Grid.Columns[nameof(NatsConnectionModel.Selected)];
                if (cellType != GridCellType.Cell)
                {
                    return;
                }

                var value = (bool) Grid[range.Row, selectedColumn.Index];
                style.BackgroundColor = value ? C1Color.Green : C1Color.Gray;
            }
        }
    }
}
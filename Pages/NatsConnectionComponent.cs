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

        public class ConnectionModel
        {
            public string Name { get; set; }
            public string Host { get; set;}
            public int Port { get; set;}
        }

        protected C1DataCollection<Connection> Connections { get; private set; }

        protected string Status { get; set; }
        protected ConnectionModel Model { get; } = new ConnectionModel
        {
            Host = "127.0.0.1", Name = "localhost", Port = 4222
        };

        protected void CreateConnection()
        {
            Logger.Info($"CreateConnection: {null}");
            Status = NatsService.Create(new Connection(Model.Name, Model.Host, Model.Port));
            InvokeAsync(StateHasChanged);
        }

        protected void RemoveConnections()
        {
            Logger.Info("RemoveConnections");
            foreach (var connection in Connections.OfType<Connection>().Where(conn => conn.Selected).ToArray())
            {
                Logger.Info($"RemoveConnection: {connection}");
                NatsService.Remove(connection);
            }

            Status = "Connection removed";
            InvokeAsync(StateHasChanged);
        }

        protected override Task OnInitializedAsync()
        {
            NatsService.ConnectionCreated += OnConnectionCreated;
            NatsService.ConnectionRemoved += OnConnectionRemoved;
            Connections = new C1DataCollection<Connection>(new List<Connection>(NatsService.Configuration.Connections));
            return Task.CompletedTask;
        }

        private void OnConnectionRemoved(Connection connection)
        {
            for (int i = Connections.Count - 1; i >= 0; i--)
            {
                if (Connections[i].Equals(connection))
                {
                    Connections.RemoveAsync(i);
                    return;
                }
            }
        }

        private void OnConnectionCreated(Connection connection)
        {
            Connections.InsertAsync(0, connection);
        }

        protected void SelectedConnectionChanged(object sender, GridCellRangeEventArgs e)
        {
            var selected = GetSelected(e.CellRange);
            if (selected == null)
            {
                return;
            }

            Model.Name = selected.Name;
            Model.Host = selected.Host;
            Model.Port = selected.Port;

            InvokeAsync(StateHasChanged);
        }

        private Connection GetSelected(GridCellRange cellRange)
        {
            if (cellRange == null)
            {
                return null;
            }

            var selected = Connections[cellRange.Row] as Connection;
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
            var connectionModel = GetSelected(e.CellRange);
            if (connectionModel == null)
            {
                return;
            }

            if (connectionModel.Status == ConnectionStatus.Connected)
            {
                Disconnect(connectionModel);
            }
            else
            {
                Connect(connectionModel);
            }
        }

        private void Disconnect(Connection connection)
        {
            Logger.Info($"{nameof(Disconnect)}: {connection}");
            connection.Status = ConnectionStatus.Disconnected;
            InvokeAsync(StateHasChanged);
        }

        private void Connect(Connection connection)
        {
            Logger.Info($"{nameof(Connect)}: {connection}");
            connection.Status = ConnectionStatus.Connected;
            InvokeAsync(StateHasChanged);
        }

        public void DumpConnections()
        {
            Logger.Info("DumpConnections");
            foreach (var connection in Connections)
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
                if (cellType != GridCellType.Cell)
                {
                    return;
                }

                var statusColumn = Grid.Columns[nameof(Connection.Status)];
                var status = (ConnectionStatus) Grid[range.Row, statusColumn.Index];
                if (range.Column == statusColumn.Index && status == ConnectionStatus.Connected )
                {
                    style.BackgroundColor = C1Color.Green;
                    return;
                }

                var selectedColumn = Grid.Columns[nameof(Connection.Selected)];
                var isSelected = (bool) Grid[range.Row, selectedColumn.Index];
                if (isSelected)
                {
                    style.BackgroundColor = C1Color.Gray;
                }
            }
        }
    }
}
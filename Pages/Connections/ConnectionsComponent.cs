using System.Collections.Generic;
using System.Threading.Tasks;
using C1.Blazor.Grid;
using C1.DataCollection;
using Microsoft.AspNetCore.Components;
using nats_ui.Data;
using NLog;

namespace nats_ui.Pages.Connections
{
    public class ConnectionsComponent : ComponentBase
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        public class ConnectionModel
        {
            public string Name { get; set; }
            public string Host { get; set;}
            public int Port { get; set;}
        }

        [Inject]
        private NatsService NatsService { get; set; }

        protected C1DataCollection<Connection> Connections { get; private set; }
        protected ConnectionCellFactory GridCellFactory { get; } = new ConnectionCellFactory();
        protected string Status { get; private set; }
        
        protected ConnectionModel Model { get; } = new ConnectionModel
        {
            Host = "127.0.0.1", Name = "localhost", Port = 4222
        };

        protected void CreateConnection()
        {
            Logger.Info($"CreateConnection: {null}");
            var connection = new Connection(Model.Name, Model.Host, Model.Port);
            Status = NatsService.Create(connection);
            Connections.InsertAsync(0, connection);
        }

        protected void RemoveConnections()
        {
            Logger.Info("RemoveConnections");

            for (int i = Connections.Count - 1; i >= 0; i--)
            {
                var connection = Connections[i] as Connection;
                if (connection != null && !connection.Selected)
                {
                    continue;
                }

                Logger.Info($"RemoveConnection: {connection}");
                NatsService.Remove(connection);
                Connections.RemoveAsync(i);
            }

            Status = "Connection removed";
        }

        protected override Task OnInitializedAsync()
        {
            Connections = new C1DataCollection<Connection>(new List<Connection>(NatsService.Configuration.Connections));
            return Task.CompletedTask;
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
            NatsService.Disconnect(connection);
            InvokeAsync(StateHasChanged);
        }

        private void Connect(Connection connection)
        {
            Logger.Info($"{nameof(Connect)}: {connection}");
            NatsService.Connect(connection);
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
    }
}
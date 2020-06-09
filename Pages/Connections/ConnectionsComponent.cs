using System.Threading.Tasks;
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

        protected StandardGridModel<Connection> Connections { get; } = new StandardGridModel<Connection>();
        protected ConnectionCellFactory GridCellFactory { get; } = new ConnectionCellFactory();
        protected string Status { get; private set; }
        
        protected ConnectionModel Model { get; } = new ConnectionModel
        {
            Host = "127.0.0.1", Name = "localhost", Port = 4222
        };

        protected override Task OnInitializedAsync()
        {
            Connections.SetData(NatsService.Configuration.Connections);
            Connections.SelectedItemChanged += OnSelectedItemChanged;
            Connections.ItemDoubleClicked += OnItemDoubleClicked;
            Connections.ItemClicked += OnItemClicked;
            return Task.CompletedTask;
        }

        private void OnItemClicked(string colName, Connection connection)
        {
            switch (colName)
            {
                case nameof(Connection.Trash):
                    Logger.Info($"RemoveConnection: {connection}");
                    NatsService.Remove(connection);
                    Connections.Remove(connection);
                    break;
                case nameof(Connection.Run):
                    Connect(connection);
                    Connections.Update(connection);
                    break;
                case nameof(Connection.Stop):
                    Disconnect(connection);
                    Connections.Update(connection);
                    break;
            }
        }

        protected void CreateConnection()
        {
            Logger.Info($"CreateConnection: {null}");
            var connection = new Connection(Model.Name, Model.Host, Model.Port);
            if( NatsService.Create(connection, out var msg)) 
            {
                Connections.Insert(0, connection);
            }

            Status = msg;
            InvokeAsync(StateHasChanged);
        }

        private void OnSelectedItemChanged(Connection connection)
        {
            Model.Name = connection.Name;
            Model.Host = connection.Host;
            Model.Port = connection.Port;

            InvokeAsync(StateHasChanged);
        }

        private void OnItemDoubleClicked(string colName, Connection connection)
        {
            Run(connection);
        }

        private void Run(Connection connection)
        {
            if (connection.Status == ConnectionStatus.Connected)
            {
                Disconnect(connection);
            }
            else
            {
                Connect(connection);
            }
            Connections.Update(connection);
        }

        private void Disconnect(Connection connection)
        {
            Logger.Info($"{nameof(Disconnect)}: {connection.Url}");
            NatsService.Disconnect(connection);
        }

        private void Connect(Connection connection)
        {
            Logger.Info($"{nameof(Connect)}: {connection.Url}");
            NatsService.Connect(connection);
        }
    }
}
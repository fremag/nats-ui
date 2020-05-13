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
            return Task.CompletedTask;
        }
        
        protected void CreateConnection()
        {
            Logger.Info($"CreateConnection: {null}");
            var connection = new Connection(Model.Name, Model.Host, Model.Port);
            Status = NatsService.Create(connection);
            Connections.Insert(0, connection);
        }

        protected void RemoveConnections()
        {
            Logger.Info("RemoveConnections");

            foreach(var (index, connection) in Connections.GetSelectedItems()) 
            {
                Logger.Info($"RemoveConnection: {connection}");
                NatsService.Remove(connection);
                Connections.Remove(index);
            }

            Status = "Connection removed";
        }

        private void OnSelectedItemChanged(Connection connection)
        {
            Model.Name = connection.Name;
            Model.Host = connection.Host;
            Model.Port = connection.Port;

            InvokeAsync(StateHasChanged);
        }

        private void OnItemDoubleClicked(int index, Connection connection)
        {
            if (connection.Status == ConnectionStatus.Connected)
            {
                Disconnect(connection);
            }
            else
            {
                Connect(connection);
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
            foreach (Connection connection in Connections)
            {
                Logger.Info(connection);
            }
        }
    }
}
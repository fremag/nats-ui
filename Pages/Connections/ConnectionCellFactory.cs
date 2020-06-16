using C1.Blazor.Core;
using nats_ui.Data;

namespace nats_ui.Pages.Connections
{
    public class ConnectionCellFactory : StandardCellFactory<Connection>
    {
        protected override void PrepareCellStyle(string colName, Connection job, C1Style style)
        {
            if (colName == nameof(Connection.Status) && job.Status == ConnectionStatus.Connected)
            {
                style.Color = C1Color.Green;
            }
        }
    }
}
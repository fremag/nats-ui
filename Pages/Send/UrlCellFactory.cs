using C1.Blazor.Core;
using nats_ui.Data;

namespace nats_ui.Pages.Send
{
    public class UrlCellFactory : StandardCellFactory<Connection>
    {
        protected override void PrepareCellStyle(string colName, Connection item, C1Style cellType)
        {
            
        }
    }
}
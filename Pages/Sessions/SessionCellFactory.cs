using C1.Blazor.Core;
using nats_ui.Data;

namespace nats_ui.Pages.Sessions
{
    public class SessionCellFactory : StandardCellFactory<Session>
    {
        protected override void PrepareCellStyle(string colName, Session item, C1Style cellType)
        {
            
        }
    }
}
using C1.Blazor.Core;
using nats_ui.Data;
using nats_ui.Data.Scripts;

namespace nats_ui.Pages.Scripts
{
    public class ScriptCellFactory : StandardCellFactory<Script>
    {
        protected override void PrepareCellStyle(string colName, Script job, C1Style cellStyle)
        {
        }
    }
}
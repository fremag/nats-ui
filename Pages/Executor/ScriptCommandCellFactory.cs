using C1.Blazor.Core;
using nats_ui.Data;
using nats_ui.Data.Scripts;

namespace nats_ui.Pages.Executor
{
    public class ScriptCommandCellFactory : StandardCellFactory<IScriptCommand>
    {
        protected override void PrepareCellStyle(string colName, IScriptCommand job, C1Style cellStyle)
        {
            
        }
    }
}
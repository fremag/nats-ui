using C1.Blazor.Core;
using nats_ui.Data;
using nats_ui.Data.Scripts;

namespace nats_ui.Pages.Editor
{
    public class EditorCellFactory : StandardCellFactory<ScriptStatement>
    {
        protected override void PrepareCellStyle(string colName, ScriptStatement item, C1Style cellType)
        {
            
        }
   }
}
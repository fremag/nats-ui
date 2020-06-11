using C1.Blazor.Core;
using nats_ui.Data;
using nats_ui.Data.Scripts;

namespace nats_ui.Pages.Editor
{
    public class EditorCellFactory : StandardCellFactory<AbstractScriptCommand>
    {
        protected override void PrepareCellStyle(string colName, AbstractScriptCommand item, C1Style cellType)
        {
            
        }
   }
}
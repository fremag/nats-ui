using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using nats_ui.Data;
using nats_ui.Data.Scripts;
using NLog;

namespace nats_ui.Pages.Scripts
{
    public class ScriptsComponent : ComponentBase
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        [Inject]
        private NatsService NatsService { get; set; }

        [Inject]
        private ScriptService ScriptService { get; set; }

        protected StandardGridModel<Script> ScriptGrid { get; } = new StandardGridModel<Script>(); 
        protected ScriptCellFactory GridCellFactory { get; } = new ScriptCellFactory();

        protected override Task OnInitializedAsync()
        {
            ScriptGrid.SetData(ScriptService.Scripts);
            ScriptGrid.ItemDoubleClicked += OnItemDoubleClicked; 
            return Task.CompletedTask;
        }

        private void OnItemDoubleClicked(int index, Script script)
        {
            ScriptService.SetCurrent(script);
        }
    }
}
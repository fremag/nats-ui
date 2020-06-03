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
        private ScriptService ScriptService { get; set; }

        [Inject]
        private NatsService NatsService { get; set; }

        [Inject]
        private ExecutorService ExecutorService { get; set; }

        [Inject]
        private NavigationManager NavMgr { get; set; }

        protected StandardGridModel<Script> ScriptGrid { get; } = new StandardGridModel<Script>(); 
        protected ScriptCellFactory GridCellFactory { get; } = new ScriptCellFactory();

        protected override Task OnInitializedAsync()
        {
            ScriptGrid.SetData(ScriptService.Scripts);
            ScriptGrid.ItemDoubleClicked += OnItemDoubleClicked; 
            ScriptGrid.ItemClicked += OnItemClicked; 
            return Task.CompletedTask;
        }

        private void OnItemClicked(string colName, Script script)
        {
            switch (colName)
            {
                case nameof(Script.Load):
                    OnItemDoubleClicked(colName, script);
                    break;
                case nameof(Script.Run):
                    ExecutorService.Setup(NatsService, script, ScriptService);
                    NavMgr.NavigateTo("/executor");
                    ExecutorService.Run();
                    break;
            }
        }

        private void OnItemDoubleClicked(string colName, Script script)
        {
            Logger.Info($"Set current script: {script.Name}");
            ScriptService.SetCurrent(script);
            NavMgr.NavigateTo("/editor");
        }
    }
}